using LLama.Common;
using LLama;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using System;
using LLama.Native;

namespace GodotSample
{
    public class Executor : IDisposable
    {
        ChatSession _session;
        InferenceParams _inferenceParams;
        LLamaWeights _model;
        LLamaContext _context;

        // https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf
        // https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/blob/main/Phi-3-mini-4k-instruct-q4.gguf
        string modelPath = @"E:\dev\models\Phi-3-mini-4k-instruct-q4.gguf"; // change it to your own model path.

        private void PreConfiguration()
        {
            NativeLibraryConfig.All.WithLogCallback(delegate (LLamaLogLevel level, string message)
            {
                GD.Print($"{level}: {message}");
            });
        }

        public void Load()
        {

            PreConfiguration();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024, // The longest length of chat as memory.
                GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
            };

            _model = LLamaWeights.LoadFromFile(parameters);
            _context = _model.CreateContext(parameters);

            var executor = new InteractiveExecutor(_context);

            // Add chat histories as prompt to tell AI how to act.
            var chatHistory = new ChatHistory();
            chatHistory.AddMessage(AuthorRole.System, "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.");
            chatHistory.AddMessage(AuthorRole.User, "Hello, Bob.");
            chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");

            _session = new (executor, chatHistory);

            _inferenceParams = new InferenceParams()
            {
                MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
                AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
            };

            GD.Print("The chat session has started.");
        }

        public async Task<string> SendMessage(string message)
        {
            var response = _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, message), _inferenceParams);
            string result = "";

            await foreach ( // Generate the response streamingly.
                var text
                in response)
            {
                result += text;
            }

            return result;
        }


        #region Disposable

        // Flag to indicate if the object has been disposed.
        private bool _disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if( _context != null )
                {
                    _context.Dispose();
                    _model.Dispose();

                    _context = null;
                    _model = null;
                }
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

        ~Executor()
        {
            Dispose(false);
        }

        #endregion
    }
}

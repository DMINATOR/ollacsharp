// https://scisharp.github.io/LLamaSharp/0.12.0/QuickStart/

/*
There are two popular format of model file of LLM now, which are PyTorch format (.pth) and Huggingface format (.bin). LLamaSharp uses GGUF format file, which could be converted from these two formats. To get GGUF file, there are two options:

Search model name + 'gguf' in Huggingface, you will find lots of model files that have already been converted to GGUF format. Please take care of the publishing time of them because some old ones could only work with old version of LLamaSharp.

Convert PyTorch or Huggingface format to GGUF format yourself. Please follow the instructions of this part of llama.cpp readme to convert them with the python scripts.

Generally, we recommend downloading models with quantization rather than fp16, because it significantly reduce the required memory size while only slightly impact on its generation quality.

*/
using LLama.Common;
using LLama;
using LLama.Native;


// https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf
// https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/blob/main/Phi-3-mini-4k-instruct-q4.gguf
string modelPath = @"E:\dev\models\Phi-3-mini-4k-instruct-q4.gguf"; // change it to your own model path.

var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024, // The longest length of chat as memory.
    GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
};


using var model = LLamaWeights.LoadFromFile(parameters);
using var context = model.CreateContext(parameters);


var executor = new InteractiveExecutor(context);

// Add chat histories as prompt to tell AI how to act.
var chatHistory = new ChatHistory();
chatHistory.AddMessage(AuthorRole.System, "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.");
chatHistory.AddMessage(AuthorRole.User, "Hello, Bob.");
chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");

ChatSession session = new(executor, chatHistory);

InferenceParams inferenceParams = new InferenceParams()
{
    MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
    AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
};

Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write("The chat session has started.\nUser: ");
Console.ForegroundColor = ConsoleColor.Green;
string userInput = Console.ReadLine() ?? "";

while (userInput != "exit")
{
    await foreach ( // Generate the response streamingly.
        var text
        in session.ChatAsync(
            new ChatHistory.Message(AuthorRole.User, userInput),
            inferenceParams))
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(text);
    }
    Console.ForegroundColor = ConsoleColor.Green;
    userInput = Console.ReadLine() ?? "";
}
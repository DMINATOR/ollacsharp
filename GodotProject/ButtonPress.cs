using Godot;
using GodotSample;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static LLama.LLamaTemplate;

/*
 * Details:
 * 
 * https://scisharp.github.io/LLamaSharp/0.12.0/QuickStart/
 * 
 */

public partial class ButtonPress : Button
{
    private ExecutorAsyncProxy _executor;
    private TextEdit _textMessage;
    private TextEdit _textHistory;
    private TextEdit _textSystemLog;
    private Label _labelStatus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _textMessage = GetNode<TextEdit>("%TextMessage");
        _textHistory = GetNode<TextEdit>("%TextHistory");
        _labelStatus = GetNode<Label>("%LabelStatus");
        _textSystemLog = GetNode<TextEdit>("%TextSystemLog");

        GD.Print("Loading");

        _executor = new ExecutorAsyncProxy();
        _executor.ResponseReceivedMessageDelegate += MessageReceivedCallbackFromWorkerThread;
        _executor.NativeLLamaMessageReceivedDelegate += SystemLogReceivedCallbackFromWorkerThread;

        GD.Print("Loaded");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// trigger
    private void ButtonPressed()
    {
        GD.Print("Hello world!");

        ProcessMessage(_textMessage.Text);
        _textMessage.Text = ""; // clear
    }

    // This is coming not from UI thread and need to be deferred to UI thread
    private void MessageReceivedCallbackFromWorkerThread(string message)
    {
        CallDeferred(nameof(MessageReceivedCallbackDeferredUIThread), new string( message ));
    }

    private void MessageReceivedCallbackDeferredUIThread(string message)
    {
        _textHistory.Text += $":< '{message}'\r\n";

        _labelStatus.Text = "XX"; // stopwatch.Elapsed.ToString();
    }

    private void SystemLogReceivedCallbackFromWorkerThread(string message)
    {
        CallDeferred(nameof(SystemLogReceivedCallbackDeferredUIThread), new string(message));
    }

    private void SystemLogReceivedCallbackDeferredUIThread(string message)
    {
        _textSystemLog.Text += $"{message}\r\n";
    }

    private void ProcessMessage(string message)
    {
        //var stopwatch = Stopwatch.StartNew();

        _textHistory.Text += $":> '{message}'\r\n";

        _labelStatus.Text = "...";
        _executor.SendMessage(message);

        //_textHistory.Text += $":< '{response}'\r\n";

        //stopwatch.Stop();
        //_labelStatus.Text = stopwatch.Elapsed.ToString();
    }
}

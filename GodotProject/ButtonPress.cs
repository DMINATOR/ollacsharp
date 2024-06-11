using Godot;
using GodotSample;
using System;
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
    private Executor _executor;
    private TextEdit _textMessage;
    private TextEdit _textHistory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _textMessage = GetNode<TextEdit>("%TextMessage");
        _textHistory = GetNode<TextEdit>("%TextHistory");

        GD.Print("Loading");

        _executor = new Executor();
        _executor.Load();

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

    private void ProcessMessage(string message)
    {
        _textHistory.Text += $":> '{message}'\r\n";

        var response = _executor.SendMessage(message).Result;

        _textHistory.Text += $":< '{response}'\r\n";
    }
}

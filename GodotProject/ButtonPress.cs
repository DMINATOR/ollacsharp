using Godot;
using GodotSample;
using System;
using System.Threading.Tasks;

/*
 * Details:
 * 
 * https://scisharp.github.io/LLamaSharp/0.12.0/QuickStart/
 * 
 */

public partial class ButtonPress : Button
{
    private Executor _executor;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
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

        var response = _executor.SendMessage("Hello how are you ?").Result;

        GD.Print($"Response = {response}");
    }
}

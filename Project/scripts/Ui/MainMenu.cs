using Godot;
using System;

public class MainMenu : Menu
{
	
	private AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
	private AudioStreamSample hoverSound = new AudioStreamSample();
	private AudioStreamSample clickSound = new AudioStreamSample();

	public override void _Ready()
	{
		base._Ready();
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		hoverSound = GD.Load<AudioStreamSample>("res://Sound/SFX/hover.wav");
		clickSound = GD.Load<AudioStreamSample>("res://Sound/SFX/click.wav");
	}

	private void _on_StartButton_pressed()
	{
		audioStreamPlayer.Stream = clickSound;
		audioStreamPlayer.Play();
		FadeOut("res://Nodes/sceneTest.tscn");
	}
	
	private void _on_ExitButton_pressed()
	{
		audioStreamPlayer.Stream = clickSound;
		audioStreamPlayer.Play();
		FadeOutExit();
	}

	private void _on_ExitButton_mouse_entered()
	{
		audioStreamPlayer.Stream = hoverSound;
		audioStreamPlayer.Play();
	}


	private void _on_StartButton_mouse_entered()
	{
		audioStreamPlayer.Stream = hoverSound;
		audioStreamPlayer.Play();
	}

}


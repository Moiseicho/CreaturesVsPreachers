using Godot;
using System;

public class Fader : Control
{

	[Signal]
	public delegate void _Retry_after_death();
	[Signal]
	public delegate void _Menu_after_death();
	[Signal]
	public delegate void _Quit_after_death();

	AnimationPlayer ap;

	public override void _Ready()
	{
		ap = (AnimationPlayer)GetNode("AnimationPlayer");
	}

	private void _on_Retry_pressed()
	{
		ap.CurrentAnimation = "TextDisolve";
		Timer timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(EmitRetrySignal));
		AddChild(timer);
		timer.Start();
	}

	private void EmitRetrySignal()
	{
		EmitSignal(nameof(_Retry_after_death));
	}

	private void _on_Menu_pressed()
	{
		ap.CurrentAnimation = "TextDisolve";
		Timer timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(EmitMenuSignal));
		AddChild(timer);
		timer.Start();
	}

	private void EmitMenuSignal()
	{
		EmitSignal(nameof(_Menu_after_death));
	}

	private void _on_Quit_pressed()
	{
		ap.CurrentAnimation = "TextDisolve";
		Timer timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(EmitQuitSignal));
		AddChild(timer);
		timer.Start();
	}

	private void EmitQuitSignal()
	{
		EmitSignal(nameof(_Quit_after_death));
	}

	public void FadeOutDied()
	{
		ap.CurrentAnimation = "FadeOutDied";
	}
}


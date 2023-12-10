using Godot;
using System;

public class GUI : Menu
{
	private ProgressBar healthBar;
	private Player player;
	private Reactor reactor;
	private Control menu;
	private bool menuOpen = false;

	public override void _Ready()
	{
		base._Ready();
		player = (Player)GetNodeOrNull("../Player");
		healthBar = (ProgressBar)GetNode("HUD/Rows/Top row/HealthBar/Percent");
		menu = (Control)GetNode("PauseMenu");
		reactor = (Reactor)GetNodeOrNull("../Reactor");

		player.Connect(nameof(Player._PlayerDied), this, nameof(_on_GameOver));
		reactor.Connect(nameof(Reactor._ReactorDestroyed), this, nameof(_on_GameOver));
	}

	private void _on_GameOver()
	{
		GetTree().Paused = true;
		FadeOutDied();
	}

	public override void _Process(float delta)
	{
		healthBar.Value = player.getHpPercent();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//if the event is the "ui_escape" event depending on menuOpen either open or close the menu
		if (@event.IsActionPressed("ui_escape"))
		{
			if (menuOpen)
			{
				closeMenu();
			}
			else
			{
				openMenu();
			}
		}
	}

	private void openMenu()
	{
		GetTree().Paused = true;
		menu.Visible = true;
		menuOpen = true;
		
	}
	
	private void _on_Resume_pressed()
	{
		closeMenu();
	}

	private void closeMenu()
	{
		GetTree().Paused = false;
		menu.Visible = false;
		menuOpen = false;
	}
	
	private void _on_Menu_pressed()
	{
		closeMenu();
		FadeOut("res://Nodes/ui/Main menu.tscn");
	}
	
	private void _on_Quit_pressed()
	{
		closeMenu();
		FadeOutExit();
	}

	
}
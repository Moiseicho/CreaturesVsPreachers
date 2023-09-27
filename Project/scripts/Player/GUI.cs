using Godot;
using System;

public class GUI : Node
{
	private ProgressBar healthBar;
	private Player Player;

	public override void _Ready()
	{
		Player = GetNodeOrNull<Player>("../Player");
		healthBar = GetNode<ProgressBar>("MarginContainer/Rows/Top row/HealthBar");
	}

	public override void _Process(float delta)
	{
		healthBar.Value = Player.getHpPercent();
	}
}

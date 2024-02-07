using Godot;
using System;

public class VictoryScreen : Menu
{
	public override void _Ready()
	{
		base._Ready();
	}
	
	private void _on_Menu_pressed()
	{
		FadeOut("res://Nodes/ui/Main menu.tscn");
	}

}



using Godot;
using System;

public class MainMenu : Menu
{
	

	private void _on_StartButton_pressed()
	{
		FadeOut("res://Nodes/sceneTest.tscn");
	}
	
	private void _on_ExitButton_pressed()
	{
		FadeOutExit();
	}

}



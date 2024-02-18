using Godot;

public class Menu : Node
{
	protected Fader fader;
	
	public override void _Ready()
	{
		fader = (Fader)GetNode("Fader");

		fader.Connect(nameof(Fader._Retry_after_death), this, nameof(ChangeSceneAfterTimer),
			new Godot.Collections.Array() {"res://Nodes/sceneTest.tscn"});
		fader.Connect(nameof(Fader._Menu_after_death), this, nameof(ChangeSceneAfterTimer), 
			new Godot.Collections.Array() {"res://Nodes/ui/Main menu.tscn"});
		fader.Connect(nameof(Fader._Quit_after_death), this, nameof(ExitGame));
	}

	protected void FadeOut(string scene)
	{
		fader.FadeOut();
		
		Timer timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(ChangeSceneAfterTimer), new Godot.Collections.Array() {scene});
		AddChild(timer);
		timer.Start();
	}

	protected void FadeOutExit()
	{
		fader.FadeOut();
		
		Timer timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(ExitGame));
		AddChild(timer);
		timer.Start();
	}

	protected void FadeOutDied()
	{
		fader.FadeOutDied();
	}

	private void ChangeSceneAfterTimer(string scene)
	{
		GetTree().Paused = false;
		GetTree().ChangeScene(scene);
	}
	
	protected void ExitGame()
	{
		GetTree().Quit();
	}

}

using Godot;
using System;
using System.Collections.Generic;
using IO = System.IO;


public class Spawner : Node
{
	[Export]
	private List<Vector2> spawnPoints;
	
	private List<List<Dictionary<string, int>>> waveNumbers;
	
	private const string folderPath = "res://Nodes/zombles";
	private int zomblesAlive = 0;
	private int subWave = 0;
	private int wave = 0;
	private int timeLeft = 5;
	private Timer timer;
	private Label label;
	private GUI gui;

	private void yippie()
	{
		GD.Print("yippie");
	}

	public override void _Ready()
	{
		gui = GetNode<GUI>("../GUI");
		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", this, nameof(onSecondPassed));
		label = GetNode<Label>("Label");
		label.Text = timeLeft.ToString();

		waveNumbers = new List<List<Dictionary<string, int>>>();
		Godot.File f = new Godot.File();
		f.Open("res://scripts/waves.json", Godot.File.ModeFlags.Read);
		string contents = f.GetAsText();
		f.Close();
		
		Godot.Collections.Dictionary jsonFile = JSON.Parse(contents).Result as Godot.Collections.Dictionary;
		Godot.Collections.Array waves = jsonFile["waves"] as Godot.Collections.Array;
		
		foreach (Godot.Collections.Dictionary wave in waves)
		{
			Godot.Collections.Array subWaves = wave["subWaves"] as Godot.Collections.Array;
			List<Dictionary<string, int>> subWaveList = new List<Dictionary<string, int>>();
			foreach (Godot.Collections.Dictionary subWave in subWaves)
			{
				Dictionary<string, int> zombleNumbers = new Dictionary<string, int>();
				foreach (string zomble in subWave.Keys)
				{
					zombleNumbers[zomble] = Convert.ToInt32(subWave[zomble]);
				}
				subWaveList.Add(zombleNumbers);
			}
			waveNumbers.Add(subWaveList);
		}

		timer.Start();
	}

	public void onSecondPassed(){
		timeLeft--;
		label.Text = timeLeft.ToString();
		if(timeLeft <= 0)
		{
			timer.Stop();
			label.Text = "DANGER!";
			spawn();
			timeLeft = 15;
		}
	}

	public async void spawn()
	{
		Dictionary<string, int> zombleNumber = waveNumbers[wave][subWave];
		foreach (string zomble in zombleNumber.Keys)
		{
			for (int i = 0; i < zombleNumber[zomble]; i++)
			{
				PackedScene zombleScene = GD.Load<PackedScene>(folderPath + "/" + zomble + ".tscn");
				Zomble zombleInstance = zombleScene.Instance() as Zomble;
				zombleInstance.Position = spawnPoints[new Random().Next(0, spawnPoints.Count)];
				zombleInstance.Connect("_ZombleDied", this, nameof(OnZombleDied));
				zomblesAlive++;

				GetParent().AddChild(zombleInstance);

				await ToSignal(GetTree().CreateTimer(0.15f), "timeout");
			}
		}
	}

	public async void OnZombleDied()
	{
		zomblesAlive--;
		if(zomblesAlive <= 0)
		{
			if(subWave < waveNumbers[wave].Count - 1)
			{
				subWave++;
				await ToSignal(GetTree().CreateTimer(2f), "timeout");
				spawn();
			}
			else
			{
				wave++;
				subWave = 0;
				
				if(wave >= waveNumbers.Count)
				{
					gui.victory();
					return;
				}
				
				gui.openUpgradeMenu();
				timer.Start();
				label.Text = timeLeft.ToString();
			}
		}
	}
}

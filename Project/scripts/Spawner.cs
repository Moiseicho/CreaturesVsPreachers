using Godot;
using System;
using System.Collections.Generic;
using IO = System.IO;


public class Spawner : Node
{
	private List<List<Dictionary<string, int>>> waveNumbers;
	private List<PackedScene> zombles;

	[Export]
	private List<Vector2> spawnPoints;

	private int zomblesAlive = 0;

	private int subWave = 0;
	private int wave = 0;

	private Timer timer;
	private Label label;

	private int timeLeft = 60;

	public override void _Ready()
	{

		timer = GetNode<Timer>("Timer");
		label = GetNode<Label>("Label");

		label.Text = timeLeft.ToString();


		string folderPath = "res://Nodes/zombles";
		folderPath = ProjectSettings.GlobalizePath(folderPath);
		string[] files = IO.Directory.GetFiles(folderPath, "*.tscn");
		zombles = new List<PackedScene>();

		foreach (string file in files)
		{
			string fixedFile = file.Replace('\\', '/');
			PackedScene zomble = (PackedScene)ResourceLoader.Load(fixedFile);
			zomble.ResourceName = fixedFile.Substring(fixedFile.LastIndexOf('/') + 1).Replace(".tscn", "");
			zombles.Add(zomble);
			
		}

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
					//convert from string to int
					zombleNumbers[zomble] = Convert.ToInt32(subWave[zomble]);
				}
				subWaveList.Add(zombleNumbers);
			}
			waveNumbers.Add(subWaveList);
		}
		
		timer.Connect("timeout", this, nameof(onSecondPassed));
		timer.Start();
	}

	public void onSecondPassed()
	{
		timeLeft--;
		label.Text = timeLeft.ToString();
		if(timeLeft <= 0)
		{
			timer.Stop();
			label.Text = "DANGER!";
			spawn(wave, subWave);
			timeLeft = 60;
		}
	}

	public async void spawn(int wave, int subWave)
	{
		for (int i = 0; i < zombles.Count; i++)
		{
			if(wave >= waveNumbers.Count)break;
			if(subWave >= waveNumbers[wave].Count)break;
			if(!waveNumbers[wave][subWave].ContainsKey(zombles[i].ResourceName))continue;

			for (int y = 0; y < waveNumbers[wave][subWave][zombles[i].ResourceName]; y++)
			{
				Zomble zombleInstance = zombles[i].Instance() as Zomble;
				zombleInstance.Position = spawnPoints[new Random().Next(0, spawnPoints.Count)];
				zombleInstance.Connect("_ZombleDied", this, nameof(OnZombleDied));
				zomblesAlive++;

				GetParent().AddChild(zombleInstance);

				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
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
				await ToSignal(GetTree().CreateTimer(10f), "timeout");
				spawn(wave, subWave);
			}
			else
			{
				wave++;
				subWave = 0;
				
				if(wave >= waveNumbers.Count)
				{
					//yippie();
					return;
				}
				timer.Start();
				label.Text = timeLeft.ToString();
			}
		}
	}
}

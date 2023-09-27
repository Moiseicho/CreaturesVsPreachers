using Godot;
using System;
using System.Collections.Generic;
using IO = System.IO;

public class Spawner : Node
{
	
	[Export]
	private int waveAmount = 5;

	[Export]
	private List<List<int>> waveNumbers;
	private List<PackedScene> zombles;

	[Export]
	private List<Vector2> spawnPoints;

	public override void _Ready()
	{
		string folderPath = "res://Nodes/zombles";
		folderPath = ProjectSettings.GlobalizePath(folderPath);
		string[] files = IO.Directory.GetFiles(folderPath, "*.tscn");
		zombles = new List<PackedScene>();

		foreach (string file in files)
		{
			string fixedFile = file.Replace('\\', '/');
			zombles.Add((PackedScene)ResourceLoader.Load(fixedFile));
			
		}
	}

	public async void spawn(int wave)
	{
		if (wave >= waveNumbers.Count)return;
		int spawnPointIndex = 0;
		for (int i = 0; i < zombles.Count; i++)
		{
			for (int y = 0; y < waveNumbers[wave][i]; y++)
			{
				Zomble zombleInstance = zombles[i].Instance() as Zomble;
				zombleInstance.Position = spawnPoints[spawnPointIndex];

				Random random = new Random();

				zombleInstance.setSpeed(random.Next(5000, 17000));
				zombleInstance.setRadius(random.Next(0, 1000));
				zombleInstance.setDamage(random.Next(1, 4));

				GetParent().AddChild(zombleInstance);

				spawnPointIndex++;
				if (spawnPointIndex >= spawnPoints.Count)
				{
					spawnPointIndex = 0;
				}
				SceneTreeTimer timer = (SceneTreeTimer)GetTree().CreateTimer(0.1f);
				while (timer.TimeLeft > 0)
				{
					await ToSignal(timer, "timeout");
				}
			}
		}
	}


	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("ui_select"))
		{
			spawn(0);
		}
	}
}

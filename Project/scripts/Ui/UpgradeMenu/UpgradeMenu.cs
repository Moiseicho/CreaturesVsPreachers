using Godot;
using System;
using System.Collections.Generic;

public class UpgradeMenu : TextureRect
{
	private UpgradeOption[] upgradeOptions;
	private TreeNode<UpgradeOption> upgradeTree;

	private Player player;
	[Signal]
	public delegate void UpgradeMenuClose();

	public override void _Ready()
	{
		upgradeOptions = new UpgradeOption[3];
		upgradeOptions[0] = GetNode<UpgradeOption>("UpgradeOptionButton1");
		upgradeOptions[1] = GetNode<UpgradeOption>("UpgradeOptionButton2");
		upgradeOptions[2] = GetNode<UpgradeOption>("UpgradeOptionButton3");

		upgradeOptions[0].TextureNormal = ResourceLoader.Load("res://Sprites/Missing.png") as Texture;
	
		for(int i = 0; i < upgradeOptions.Length; i++)
		{
			upgradeOptions[i].Connect("UpgradeOptionPressed", this, nameof(UpgradeOptionPressed), new Godot.Collections.Array() { i });
		}

		Godot.File f = new Godot.File();

		f.Open("res://scripts/Ui/UpgradeMenu/upgrades.json", Godot.File.ModeFlags.Read);

		string contents = f.GetAsText();
		f.Close();

		Godot.Collections.Dictionary jsonFile = JSON.Parse(contents).Result as Godot.Collections.Dictionary;

		Godot.Collections.Array upgrades = jsonFile["upgrades"] as Godot.Collections.Array;
		
		upgradeTree = new TreeNode<UpgradeOption>(null);
		Stack<Tuple<TreeNode<UpgradeOption>, Godot.Collections.Dictionary>> stack = new Stack<Tuple<TreeNode<UpgradeOption>, Godot.Collections.Dictionary>>();
		foreach(Godot.Collections.Dictionary upgrade in upgrades)
		{
			stack.Push(new Tuple<TreeNode<UpgradeOption>, Godot.Collections.Dictionary>(upgradeTree, upgrade));
		}

		while(stack.Count > 0)
		{
			Tuple<TreeNode<UpgradeOption>, Godot.Collections.Dictionary> tuple = stack.Pop();
			TreeNode<UpgradeOption> parent = tuple.Item1;
			Godot.Collections.Dictionary upgrade = tuple.Item2;

			Godot.Collections.Array subUpgrades = upgrade["subUpgrades"] as Godot.Collections.Array;
			
			StatsUpgrade statsUpgrade = null;
			GiveUpgrade giveUpgrade = null;

			if(upgrade.Contains("stats") && upgrade["stats"] != null)
			{
				Godot.Collections.Dictionary stats = upgrade["stats"] as Godot.Collections.Dictionary;
				float speed = (float)stats["speed"];
				float health = (float)stats["health"];
				statsUpgrade = new StatsUpgrade(speed, health);
			}
			if(upgrade.Contains("itemPath") && upgrade["itemPath"] != null)
			{
				string itemPath = (string)upgrade["itemPath"];
				PackedScene itemScene = ResourceLoader.Load(itemPath) as PackedScene;
				giveUpgrade = new GiveUpgrade(itemScene.Instance() as Giveable);
				giveUpgrade.setImage(ResourceLoader.Load(upgrade["imagePath"] as string) as Texture);
			}else if(upgrade.Contains("ability") && upgrade["ability"] != null)
			{
				Godot.Collections.Dictionary ability = upgrade["ability"] as Godot.Collections.Dictionary;
				if(ability["type"] as string == "throw")
				{
					ThrowAbility tempThrow = new ThrowAbility();
					tempThrow.setBulletScene(ResourceLoader.Load(ability["bulletPath"] as string) as PackedScene);
					tempThrow.Cooldown = (float)ability["cooldown"];
					giveUpgrade = new GiveUpgrade(tempThrow);
					giveUpgrade.setImage(ResourceLoader.Load(upgrade["imagePath"] as string) as Texture);
				}else if(ability["type"] as string == "summon")
				{
					SummonAbility tempSummon = new SummonAbility(ResourceLoader.Load(ability["summonPath"] as string) as PackedScene, (bool)ability["isStatic"]);
					tempSummon.Cooldown = (float)ability["cooldown"];
					giveUpgrade = new GiveUpgrade(tempSummon);
					giveUpgrade.setImage(ResourceLoader.Load(upgrade["imagePath"] as string) as Texture);
				}
			}
			Texture texture = ResourceLoader.Load(upgrade["imagePath"] as string) as Texture;
			UpgradeOption temp = new UpgradeOption(texture, statsUpgrade, giveUpgrade);
			TreeNode<UpgradeOption> thisNode = new TreeNode<UpgradeOption>(temp);
			parent.addChild(thisNode);
			GD.Print("uploaded: " + temp.TextureNormal.ToString());
			foreach(Godot.Collections.Dictionary subUpgrade in subUpgrades)
			{
				stack.Push(new Tuple<TreeNode<UpgradeOption>, Godot.Collections.Dictionary>(thisNode, subUpgrade));
			}
		}
		
	}

	public void Appear(Player player)
	{
		this.player = player;
		bool hasUpgrades = false;

		for(int i = 0; i < 3; i++)
		{
			if(i < upgradeTree.getChildren().Count)
			{
				hasUpgrades = true;
				upgradeOptions[i].clone(upgradeTree.getChildrenData()[i]);
				upgradeOptions[i].Disabled = false;

			}
			else
			{
				upgradeOptions[i].Disabled = true;
			}
			
			upgradeOptions[i].setPlayer(player);
			upgradeOptions[i].Visible = true;
		}
		if(!hasUpgrades)
		{
			EmitSignal(nameof(UpgradeMenuClose));
			return;
		}
		
		this.Visible = true;
	}

	public void UpgradeOptionPressed(int index)
	{
		for(int i = 0; i < 3; i++)
		{
			upgradeOptions[i].Visible = false;
			upgradeOptions[i].Disabled = true;
		}
		upgradeTree = upgradeTree.getChildren()[index];
		EmitSignal(nameof(UpgradeMenuClose));
	}

}

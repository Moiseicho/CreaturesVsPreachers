using Godot;
using System;

public class GUI : Menu
{
	private ProgressBar healthBar;
	private Player player;
	private Reactor reactor;
	private Control menu;
	private bool menuOpen = false;
	private UpgradeMenu upgradeMenu;
	private TextureRect abilityQImage = null;
	private TextureRect abilityEImage = null;
	private TextureProgress abilityQCooldown;
	private TextureProgress abilityECooldown;
	private Label ammoCounter;

	public override void _Ready()
	{
		base._Ready();
		player = (Player)GetNodeOrNull("../Player");
		healthBar = (ProgressBar)GetNode("HUD/Rows/Top row/HealthBar/Percent");
		menu = (Control)GetNode("PauseMenu");
		reactor = (Reactor)GetNodeOrNull("../Reactor");
		upgradeMenu = (UpgradeMenu)GetNode("UpgradeMenu");
		abilityQImage = (TextureRect)GetNode("HUD/Rows/Bottom row/Qsquare/Texture");
		abilityEImage = (TextureRect)GetNode("HUD/Rows/Bottom row/Esquare/Texture");
		abilityQCooldown = (TextureProgress)GetNode("HUD/Rows/Bottom row/Qsquare/Cooldown");
		abilityECooldown = (TextureProgress)GetNode("HUD/Rows/Bottom row/Esquare/Cooldown");
		ammoCounter = (Label)GetNode("HUD/AmmoCounter/Label");

		upgradeMenu.Connect(nameof(UpgradeMenu.UpgradeMenuClose), this, nameof(closeUpgradeMenu));


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
		abilityQCooldown.Value = player.getAbilityQCooldownPercent();
		abilityECooldown.Value = player.getAbilityECooldownPercent();
		abilityQImage.Texture = player.getAbilityQImage();
		abilityEImage.Texture = player.getAbilityEImage();
		int ammo = player.getAmmo();
		if (ammo == -1)
		{
			ammoCounter.Text = "inf.";
		}
		else
		{
			ammoCounter.Text = ammo.ToString() + "/" + player.getAmmoCapacity();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
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

	public void openUpgradeMenu()
	{
		GetTree().Paused = true;
		upgradeMenu.Appear(player);
	}

	public void closeUpgradeMenu()
	{
		upgradeMenu.Visible = false;
		GetTree().Paused = false;
	}

	public void victory()
	{
		FadeOut("res://Nodes/ui/VictoryScreen.tscn");
	}
}

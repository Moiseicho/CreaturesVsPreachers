using Godot;
using System;

public class Reactor : StaticBody2D
{
	
	private float health;

	[Export]
	private float maxHealth = 150;
	private Label label;
	private AnimationPlayer animationPlayer;

	[Signal]
	public delegate void _ReactorDestroyed();

	public override void _Ready()
	{
		health = maxHealth;
		label = (Label)GetNode("Label");
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		animationPlayer.CurrentAnimation = "100%";
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		int healthPercent = (int)Math.Round(100*health/maxHealth);
		label.Text = healthPercent + "%";
		
		if(healthPercent > 50)
		{
			animationPlayer.CurrentAnimation = "100%";
		}else if(healthPercent > 10)
		{
			animationPlayer.CurrentAnimation = "50%";
		}else
		{
			animationPlayer.CurrentAnimation = "10%";
		}
	}

	public void takeDamage(float damage)
	{
		health -= damage;
		if(health <= 0)
		{
			EmitSignal(nameof(_ReactorDestroyed));
		}
	}
}

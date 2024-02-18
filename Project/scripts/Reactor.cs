using Godot;
using System;

public class Reactor : StaticBody2D
{
	[Export]
	private float maxHealth = 150;
	
	private float health;
	private Label label;
	private AnimatedSprite animatedSprite;

	[Signal]
	public delegate void _ReactorDestroyed();

	public override void _Ready()
	{
		health = maxHealth;
		label = (Label)GetNode("Label");
		animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		animatedSprite.Animation = "100%";
	}

	public override void _Process(float delta)
	{
		int healthPercent = (int)Math.Round(100*health/maxHealth);
		label.Text = healthPercent + "%";
		
		if(healthPercent > 50)
		{
			animatedSprite.Animation = "100%";
		}else if(healthPercent > 10)
		{
			animatedSprite.Animation = "50%";
		}else
		{
			animatedSprite.Animation = "10%";
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

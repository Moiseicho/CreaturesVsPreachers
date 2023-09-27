using Godot;
using System;

public class Zomble : KinematicBody2D
{
	[Export]
	private float speed = 0f;
	[Export]
	private float playerTargetRadius;
	[Export]
	private Player player;
	[Export]
	private float damage;
	[Export]
	private float health;
	[Export]
	private float maxHealth = 20;
	
	private AnimationPlayer animationPlayer;
	private Sprite sprite;
	private Timer timer;
	private bool biting = false;
	private Area2D biteBox;
	private bool right = true;
	private Reactor reactor;

	public override void _Ready()
	{
		player = (Player)GetNode("../Player");
		sprite = (Sprite)GetNode("Sprite");
		biteBox = (Area2D)GetNode("BiteBox");
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		reactor = (Reactor)GetNode("../Reactor");
		animationPlayer.CurrentAnimation = "walk";
		timer = new Timer();
		AddChild(timer);
		health = maxHealth;

		timer.Connect("timeout", this, nameof(OnBiteHit));
		animationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
	}

	public void manageLife()
	{
		if(health <= 0)
			QueueFree();
	}

	public override void _Process(float delta)
	{
		manageLife();
		if(biting) return;

		bool playerT = true;
		Vector2 direction = (player.Position - Position);
		if(direction.Length() > playerTargetRadius)
		{
			direction = (reactor.Position - Position);
			playerT = false;
		}
		Vector2 movement = direction.Normalized() * speed * delta;
		if(direction.x < 0)
		{
			if(right)
			{
				biteBox.Position *= new Vector2(-1, 1);
				right = false;
			}
			sprite.FlipH = true;
			
		}else
		{
			if(!right)
			{
				biteBox.Position *= new Vector2(-1, 1);
				right = true;
			}
			sprite.FlipH = false;
		}
		if(!playerT)
		{
			foreach (Area2D area in biteBox.GetOverlappingAreas())
			{
				if (area.IsInGroup("reactorHB"))
				{
					animationPlayer.CurrentAnimation = "bite";
					StartWait(0.4f);
					biting = true;
					return;
				}
			}
		}else if( direction.Length() < 35f)
		{
			animationPlayer.CurrentAnimation = "bite";
			StartWait(0.4f);
			biting = true;
			return;
		}
		animationPlayer.CurrentAnimation = "walk";
		MoveAndSlide(movement);
	}

	private void OnAnimationFinished(string animationName)
	{
		if(animationName == "bite")
		{
			biting = false;
		}
	}

	public void StartWait(float duration)
	{
		timer.WaitTime = duration;
		timer.OneShot = true;
		timer.Start();
	}

	private void OnBiteHit()
	{
		foreach (Area2D area in biteBox.GetOverlappingAreas())
		{
			if (area.IsInGroup("playerHB"))
			{
				player.takeDamage(damage);
				return;
			}
			if (area.IsInGroup("reactorHB"))
			{
				reactor.takeDamage(damage);
			}
		}
	}

	public void setSpeed(int speed){this.speed = speed;}
	public void setRadius(float radius){this.playerTargetRadius = radius;}

	public void setDamage(float damage){this.damage = damage;}
	
	public void takeDamage(float damage, Vector2 direction, float knockback)
	{
		health -= damage;
		MoveAndSlide(direction * (knockback / direction.Length()));
	}

}

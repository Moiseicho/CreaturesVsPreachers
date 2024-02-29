using Godot;
using System;


public class Zomble : KinematicBody2D
{
	
	[Export]
	private bool suicide = false;
	[Export]
	private float biteDelay = 0.4f;
	[Export]
	private float maxHealth = 20;
	[Export]
	private int minSpeed = 0;
	[Export]
	private int maxSpeed = 0;
	[Export]
	private int minDamage = 0;
	[Export]
	private int maxDamage = 0;
	[Export]
	private int minPlayerTargetRadius;
	[Export]
	private int maxPlayerTargetRadius;


	private float tempSpeed = 0f;
	private float speed = 0f;
	private float playerTargetRadius;
	private float damage;
	private float health;
	private Timer slowDownTimer;
	private Timer biteTimer;
	private bool biting = false;
	private bool right = true;
	private bool stuck = false;
	private bool frozen = false;
	private bool died = false;
	private AnimatedSprite animatedSprite;
	private Area2D biteBox;
	private Reactor reactor;
	private Player player;
	private Spawner spawner;

	[Signal]
	public delegate void _ZombleDied();

	private static float MinSpeedCoef = 0.2f;

	public override void _Ready()
	{
		player = (Player)GetNode("../Player");
		biteBox = (Area2D)GetNode("BiteBox");
		animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		reactor = (Reactor)GetNode("../Reactor");
		
		biteTimer = new Timer();
		AddChild(biteTimer);
		biteTimer.Connect("timeout", this, nameof(OnBiteHit));
		slowDownTimer = new Timer();
		AddChild(slowDownTimer);
		slowDownTimer.Connect("timeout", this, nameof(unstuck));

		animatedSprite.Animation = "walk";
		animatedSprite.Play();
		health = maxHealth;

		Random random = new Random();

		speed = random.Next(minSpeed, maxSpeed);
		tempSpeed = speed;
		if(minPlayerTargetRadius == maxPlayerTargetRadius)
		{
			playerTargetRadius = (float)minPlayerTargetRadius / 100;
		}
		else{
			playerTargetRadius = (float)random.Next(minPlayerTargetRadius, maxPlayerTargetRadius) / 100;
		}
		damage = random.Next(minDamage, maxDamage);

		
	}

	public void manageLife()
	{
		if(health <= 0  && !died)
		{
			die();
		}
	}

	public void die()
	{
		if(died) return;
		died = true;
		if(suicide)
		{
			animatedSprite.Animation = "bite";
			StartWait(biteDelay);
			biting = true;
			return;
		}
		
		EmitSignal(nameof(_ZombleDied));
		spawner.decrementZomble();
		QueueFree();
	}

	private void manageFlip(Vector2 direction)
	{
		if(direction.x < 0)
		{
			if(right)
			{
				biteBox.Position *= new Vector2(-1, 1);
				right = false;
			}
			animatedSprite.FlipH = true;
			
		}else
		{
			if(!right)
			{
				biteBox.Position *= new Vector2(-1, 1);
				right = true;
			}
			animatedSprite.FlipH = false;
		}
	}

	public override void _Process(float delta)
	{
		manageLife();
		if(frozen) return;
		if(biting) return;
		if(player == null) return;
		if(reactor == null) return;

		bool playerT = true;
		Vector2 direction = (player.Position - Position);
		Vector2 reactorDirection = (reactor.Position - Position);
		if(direction.Length()/(reactorDirection.Length()) > playerTargetRadius)
		{
			direction = reactorDirection;
			playerT = false;
		}
		Vector2 movement = direction.Normalized() * tempSpeed * delta;
		
		manageFlip(direction);

		if(!playerT)
		{
			foreach (Area2D area in biteBox.GetOverlappingAreas())
			{
				if (area.IsInGroup("reactorHB"))
				{
					animatedSprite.Animation = "bite";
					StartWait(biteDelay);
					biting = true;
					return;
				}
			}
		}else if(direction.Length() < 35f)
		{
			animatedSprite.Animation = "bite";
			StartWait(biteDelay);
			biting = true;
			return;
		}
		MoveAndSlide(movement);
		ManageSpeed(delta);
	}

	private void ManageSpeed(float delta)
	{
		if(stuck) return;
		if(tempSpeed < speed)
		{
			tempSpeed += delta * speed;
		}
		if(tempSpeed > speed)
		{
			tempSpeed = speed;
		}
	}

	public void StartWait(float duration)
	{
		biteTimer.WaitTime = duration;
		biteTimer.OneShot = true;
		biteTimer.Start();
	}

	private void OnBiteHit()
	{
		foreach (Area2D area in biteBox.GetOverlappingAreas())
		{
			if (area.IsInGroup("playerHB") && maxPlayerTargetRadius > 0)
			{
				player.takeDamage(damage);
				return;
			}
			if (area.IsInGroup("reactorHB") && minPlayerTargetRadius < 1000)
			{
				reactor.takeDamage(damage);
				return;
			}
		}
	}

	public void setSpeed(int speed){this.speed = speed;}
	public void setRadius(float radius){this.playerTargetRadius = radius;}

	public void setDamage(float damage){this.damage = damage;}
	
	public void takeDamage(float damage, float knockback)
	{
		if(frozen) return;
		health -= damage;
		if(!stuck)
		{
			tempSpeed = tempSpeed - knockback;
			if(tempSpeed < speed * MinSpeedCoef)
			{
				tempSpeed = speed * MinSpeedCoef;
			}
		}
	}

	public void slowDown(float slowDown, float duration)
	{
		tempSpeed = speed * slowDown;
		slowDownTimer.Stop();
		slowDownTimer.WaitTime = duration;
		slowDownTimer.Start();
		stuck = true;
 	}

	private void unstuck()
	{
		stuck = false;
		slowDownTimer.Stop();
	}

	public void freeze()
	{
		frozen = true;
		animatedSprite.Stop();
	}
	public void unfreeze()
	{
		frozen = false;
		animatedSprite.Play();
	}

	private void _on_AnimatedSprite_animation_finished()
	{
		if(animatedSprite.Animation == "bite")
		{
			if(suicide)
			{
				EmitSignal(nameof(_ZombleDied));
				spawner.decrementZomble();
				QueueFree();
			}
			biting = false;
			animatedSprite.Animation = "walk";
		}
	}

	public void setSpawner(Spawner spawner)
	{
		this.spawner = spawner;
	}

}


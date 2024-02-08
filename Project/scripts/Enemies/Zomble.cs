using Godot;
using System;


public class Zomble : KinematicBody2D
{
	private bool died = false;
	[Export]
	private bool suicide = false;
	[Export]
	private float biteDelay = 0.4f;
	[Export]
	private int minSpeed = 0;
	[Export]
	private int maxSpeed = 0;
	[Export]
	private int minDamage = 0;
	[Export]
	private int maxDamage = 0;
	private float tempSpeed = 0f;
	private float speed = 0f;
	[Export]
	private int minPlayerTargetRadius;
	[Export]
	private int maxPlayerTargetRadius;
	private float playerTargetRadius;
	private Player player;
	private float damage;
	private float health;
	[Export]
	private float maxHealth = 20;
	
	private AnimatedSprite animatedSprite;
	private Timer timer;
	private bool biting = false;
	private Area2D biteBox;
	private bool right = true;
	private Reactor reactor;
	private bool stuck = false;
	private Timer slowDownTimer;
	private bool frozen = false;

	[Signal]
	public delegate void _ZombleDied();

	private const float MinSpeedCoef = 0.2f;

	public override void _Ready()
	{
		player = (Player)GetNode("../Player");
		biteBox = (Area2D)GetNode("BiteBox");
		animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		reactor = (Reactor)GetNode("../Reactor");
		animatedSprite.Animation = "walk";
		timer = new Timer();
		AddChild(timer);
		health = maxHealth;

		animatedSprite.Play();

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

		timer.Connect("timeout", this, nameof(OnBiteHit));

		slowDownTimer = new Timer();
		AddChild(slowDownTimer);
		slowDownTimer.Connect("timeout", this, nameof(unstuck));

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
		died = true;
		if(suicide)
		{
			animatedSprite.Animation = "bite";
			StartWait(biteDelay);
			biting = true;
			return;
		}
		
		EmitSignal(nameof(_ZombleDied));
		QueueFree();
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
	
	public void takeDamage(float damage, float knockback)
	{
		if(frozen) return;
		GD.Print("From Zomble number " + GetInstanceId());
		health -= damage;
		if(!stuck)
		{
			tempSpeed = tempSpeed - knockback;
			if(tempSpeed < speed * MinSpeedCoef)
			{
				tempSpeed = speed * MinSpeedCoef;
			}
		}
		GD.Print("took damage " + damage);
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
				QueueFree();
			}
			biting = false;
			animatedSprite.Animation = "walk";
		}
	}

}


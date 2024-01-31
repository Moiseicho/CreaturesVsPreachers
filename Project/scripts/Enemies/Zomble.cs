using Godot;
using System;


public class Zomble : KinematicBody2D
{
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
	private float playerTargetRadius;
	private Player player;
	private float damage;
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
	private bool stuck = false;
	private Timer slowDownTimer;
	private bool frozen = false;

	[Signal]
	public delegate void _ZombleDied();

	private const float MinSpeedCoef = 0.2f;

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

		Random random = new Random();

		speed = random.Next(minSpeed, maxSpeed);
		tempSpeed = speed;
		playerTargetRadius = random.Next(0, 1000);
		damage = random.Next(minDamage, maxDamage);

		timer.Connect("timeout", this, nameof(OnBiteHit));
		animationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));

		slowDownTimer = new Timer();
		AddChild(slowDownTimer);
		slowDownTimer.Connect("timeout", this, nameof(unstuck));

	}

	public void manageLife()
	{
		if(health <= 0)
		{
			EmitSignal(nameof(_ZombleDied));
			QueueFree();
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
		if(direction.Length() > playerTargetRadius)
		{
			direction = (reactor.Position - Position);
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
	
	public void takeDamage(float damage, float knockback)
	{
		GD.Print("From Zomble number " + GetInstanceId());
		health -= damage;
		if(!stuck)
		{
			tempSpeed = tempSpeed - knockback;
			if(tempSpeed < speed * MinSpeedCoef)
			{
				tempSpeed = speed * MinSpeedCoef;
			}
			GD.Print(tempSpeed);
		}
		GD.Print("took damage " + damage);
	}

	public void slowDown(float slowDown, float duration)
	{
		tempSpeed = speed * slowDown;
		slowDownTimer.Stop();
		slowDownTimer.WaitTime = duration;
		slowDownTimer.Start();
		GD.Print("stuck");
		stuck = true;
 	}

	private void unstuck()
	{
		GD.Print("unstuck");
		stuck = false;
		slowDownTimer.Stop();
	}

	public void freeze()
	{
		frozen = true;
		animationPlayer.Stop();
	}
	public void unfreeze()
	{
		frozen = false;
		animationPlayer.Play();
	}

}

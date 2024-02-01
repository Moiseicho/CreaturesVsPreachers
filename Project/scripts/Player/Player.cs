using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export]
	private float speed;
	private float defaultSpeed;
	private bool right = true;
	private bool dead = false;

	private Ability ability1 = null;
	private float ability1Cooldown;
	private float ability1Timer = 0f;
	private Ability ability2 = null;
	private float ability2Cooldown;
	private float ability2Timer = 0f;
	
	AnimationPlayer ap;
	Sprite sp;
	CollisionShape2D collisionHull;
	Area2D hitbox;


	[Export]
	private float maxHp = 100;
	private float hp;
	private bool abilityEquiped = false;
	private Throwable throwable = null;

	Weapon weapon;
	
	[Signal]
	public delegate void _PlayerDied();

	public override void _Ready()
	{

		defaultSpeed = speed;
		hp = maxHp;

		ap = (AnimationPlayer)GetNode("AnimationPlayer");
		sp = (Sprite)GetNode("Sprite");
		collisionHull = (CollisionShape2D)GetNode("CollisionHull");
		hitbox = (Area2D)GetNode("Hitbox");
		foreach (Node child in GetChildren())
		{
			if(child is Weapon)
			{
				weapon = (Weapon)child;
			}
		}

		/*ThrowAbility temp = new ThrowAbility();
		temp.setBulletScene((PackedScene)ResourceLoader.Load("res://Nodes/bullets/throwables/ToxicGrenade.tscn"));
		temp.Player = this;
		temp.Cooldown = 5f;
		
		ability1 = temp;
		ability1Cooldown = ability1.Cooldown;

		SummonAbility temp2 = new SummonAbility(
			(PackedScene)ResourceLoader.Load("res://Nodes/Summonables/StaticSummonables/IceWall.tscn"),
			true
			);
		temp2.Player = this;
		temp2.Cooldown = 5f;
		
		ability2 = temp2;
		ability2Cooldown = ability2.Cooldown;*/
		
	}

	private void setAnimation(string animation)
	{
		ap.CurrentAnimation = animation;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{	
		if(dead)
		{
			return;
		}
		bool moving = false;
		right = !sp.FlipH;
		Vector2 movement = new Vector2(0, 0);

		if(Input.IsActionPressed("ui_left"))
		{
			if(right)
			{
				hitbox.Scale *= (new Vector2(-1, 1));
				collisionHull.Position *= (new Vector2(-1, 1));
				sp.FlipH = true;
				if(abilityEquiped)
				{
					throwable.Position *= (new Vector2(-1, 1));
				}
			}
			movement += new Vector2(-delta, 0);
			moving = true;
		}
		if(Input.IsActionPressed("ui_right"))
		{
			if(!right)
			{
				hitbox.Scale *= (new Vector2(-1, 1));
				collisionHull.Position *= (new Vector2(-1, 1));
				sp.FlipH = false;
				if(abilityEquiped)
				{
					throwable.Position *= (new Vector2(-1, 1));
				}
			}
			movement += new Vector2(delta, 0);
			moving = true;
		}
		if(Input.IsActionPressed("ui_up"))
		{
			movement += new Vector2(0, -delta);
			moving = true;
		}
		if(Input.IsActionPressed("ui_down"))
		{
			movement += new Vector2(0, delta);
			moving = true;
		}
		if(Input.IsActionPressed("ui_ability1") && ability1 != null && !abilityEquiped && ability1Timer <= 0f)
		{
			ability1.Effect();
			ability1Timer = ability1Cooldown;
		}
		if(Input.IsActionPressed("ui_ability2") && ability2 != null && !abilityEquiped && ability2Timer <= 0f)
		{
			ability2.Effect();
			ability2Timer = ability2Cooldown;
		}
		if(Input.IsActionPressed("ui_shoot") && abilityEquiped)
		{
			throwable.Position = Position + new Vector2(20, 0) * (right ? 1 : -1);
			RemoveChild(throwable);
			GetTree().Root.AddChild(throwable);
			throwable.Throw();
			weapon.enable();
			abilityEquiped = false;
		}



		manageAnimation(moving);
		manageTimers(delta);
		
		if(movement.x != 0 && movement.y != 0)
		{
			movement.x = (float)(movement.x/Math.Sqrt(2));
			movement.y = (float)(movement.y/Math.Sqrt(2));
		}
		MoveAndSlide(movement * speed);
	}

	private void manageTimers(float delta)
	{
		if(ability1Timer > 0f && !abilityEquiped)
		{
			ability1Timer -= delta;
		}
		if(ability2Timer > 0f && !abilityEquiped)
		{
			ability2Timer -= delta;
		}
	}

	private void manageAnimation(bool moving)
	{
		if(moving)
		{
			setAnimation("walk");
		}else
		{
			setAnimation("idle");
		}
	}

	public void takeDamage(float damage)
	{
		hp -= damage;
		if(hp <= 0)
		{
			EmitSignal(nameof(_PlayerDied));
			dead = true;
			//animation to dead
		}
	}

	public int getHpPercent()
	{
		return (int)Math.Round(hp/maxHp*100);
	}

	public bool getOrientation()
	{
		return right;
	}

	public void equipAbility(PackedScene throwableScene)
	{
		weapon.disable();
		throwable = (Throwable)throwableScene.Instance();
		throwable.Position = new Vector2(20, 0) * (right ? 1 : -1);
		AddChild(throwable);
		abilityEquiped = true;
	}

	public void buffStats(float speedBuff, float healthBuff)
	{
		defaultSpeed += speedBuff;
		speed = defaultSpeed;
		maxHp += healthBuff;
		hp += healthBuff;
	}

	public void Give(Weapon newWeapon)
	{
		weapon.QueueFree();
		AddChild(newWeapon);
		weapon = newWeapon;
	}

	public void Give(Ability ability)
	{
		if(ability1 == null)
		{
			ability1 = ability;
			ability1.Player = this;
			ability1Cooldown = ability1.Cooldown;
			//configure UI
		}
		else if(ability2 == null)
		{
			ability2 = ability;
			ability2.Player = this;
			ability2Cooldown = ability2.Cooldown;
			//configure UI
		}
	}
}

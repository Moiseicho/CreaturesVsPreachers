using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export]
	private float speed;
	[Export]
	private float maxHp = 30;

	private float hp;
	private bool abilityEquiped = false;
	private Throwable throwable = null;
	private Ability ability1 = null;
	private float ability1Cooldown;
	private float ability1Timer = 0f;
	private Ability ability2 = null;
	private float ability2Cooldown;
	private float ability2Timer = 0f;
	private float defaultSpeed;
	private bool right = true;
	private bool dead = false;
	AnimatedSprite sprite;
	CollisionShape2D collisionHull;
	Area2D hitbox;
	Weapon weapon;
	
	[Signal]
	public delegate void _PlayerDied();
	[Signal]
	public delegate void _NewAbility();

	public override void _Ready()
	{
		defaultSpeed = speed;
		hp = maxHp;

		sprite = (AnimatedSprite)GetNode("AnimatedSprite");
		collisionHull = (CollisionShape2D)GetNode("CollisionHull");
		hitbox = (Area2D)GetNode("Hitbox");
		foreach (Node child in GetChildren())
		{
			if(child is Weapon)
			{
				weapon = (Weapon)child;
			}
		}	
	}

	private void setAnimation(string animation)
	{
		sprite.Animation = animation;
	}

	private void manageFlip(bool isRight)
	{
		if(right != isRight)
		{
			hitbox.Scale *= (new Vector2(-1, 1));
			collisionHull.Position *= (new Vector2(-1, 1));
			sprite.FlipH = !sprite.FlipH;
			if(abilityEquiped)
			{
				throwable.Position *= (new Vector2(-1, 1));
			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{	
		if(dead)return;

		bool moving = false;
		right = !sprite.FlipH;
		Vector2 movement = new Vector2(0, 0);

		if(Input.IsActionPressed("ui_left"))
		{
			manageFlip(false);
			movement += new Vector2(-delta, 0);
			moving = true;
		}else if(Input.IsActionPressed("ui_right"))
		{
			manageFlip(true);
			movement += new Vector2(delta, 0);
			moving = true;
		}
		if(Input.IsActionPressed("ui_up"))
		{
			movement += new Vector2(0, -delta);
			moving = true;
		}else if(Input.IsActionPressed("ui_down"))
		{
			movement += new Vector2(0, delta);
			moving = true;
		}
		if(Input.IsActionPressed("ui_ability1") && ability1 != null &&
			!abilityEquiped && 
			ability1Timer <= 0f)
		{
			ability1.Effect();
			ability1Timer = ability1Cooldown;
		}else if(Input.IsActionPressed("ui_ability2") && 
			ability2 != null && !abilityEquiped && 
			ability2Timer <= 0f)
		{
			ability2.Effect();
			ability2Timer = ability2Cooldown;
		}else if(Input.IsActionPressed("ui_shoot") && abilityEquiped)
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
			EmitSignal(nameof(_NewAbility));
		}
		else if(ability2 == null)
		{
			ability2 = ability;
			ability2.Player = this;
			ability2Cooldown = ability2.Cooldown;
			EmitSignal(nameof(_NewAbility));
		}
	}

	public float getAbilityQCooldownPercent()
	{
		if(ability1 == null)
		{
			return 100;
		}
		return ability1Timer/ability1Cooldown * 100;
	}

	public float getAbilityECooldownPercent()
	{
		if(ability2 == null)
		{
			return 100;
		}
		return ability2Timer/ability2Cooldown * 100;
	}

	public Texture getAbilityQImage()
	{
		if(ability1 == null)
		{
			return null;
		}
		return ability1.getImage();
	}

	public Texture getAbilityEImage()
	{
		if(ability2 == null)
		{
			return null;
		}
		return ability2.getImage();
	}

	public int getAmmo()
	{
		return weapon.getAmmo();
	}
	public int getAmmoCapacity()
	{
		return weapon.getAmmoCapacity();
	}
}

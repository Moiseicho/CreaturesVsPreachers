using Godot;
using System;

public class Bullet : KinematicBody2D
{
	protected float speed = 10f;
	protected float fizleTime = 0.5f;
	protected float damage = 5;
	protected float knockBack = 5;
	protected int pierce = 0;
	
	protected AnimatedSprite sprite;
	protected Area2D collision;
	[Export]
	protected EffectOnHit effectOnHit;
	[Export]
	protected PackedScene effectOnFizleScene;

	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}
	public int Pierce
	{
		get { return pierce; }
		set { pierce = value; }
	}

	public float FizleTime
	{
		get { return fizleTime; }
		set { fizleTime = value; }
	}

	public float Damage
	{
		get { return damage; }
		set { damage = value; }
	}

	public float KnockBack
	{
		get { return knockBack; }
		set { knockBack = value; }
	}

	public override void _Ready()
	{
		sprite = (AnimatedSprite)GetNode("AnimatedSprite");
		sprite.Playing = true;
		collision = (Area2D)GetNode("Area2D");
		collision.Connect("area_entered", this, nameof(_on_Bullet_area_entered));
	}

	public override void _PhysicsProcess(float delta)
	{
		fizleTime -= delta;
		checkFlip();
		Vector2 velocity = new Vector2(speed, 0).Rotated(Rotation);
		MoveAndCollide(velocity * delta);
		if (fizleTime <= 0)
		{
			Fizle();
		}
	}

	private void checkFlip()
	{
		float fixedDegrees = (360+(RotationDegrees % 360))%360;
		if ((360+(fixedDegrees % 360))%360 > 90 && fixedDegrees % 360 < 270 && !sprite.FlipV)
		{
			sprite.FlipV = true;
		}else if((fixedDegrees % 360 <= 90 || fixedDegrees % 360 >= 270) && sprite.FlipV)
		{
			sprite.FlipV = false;
		}
	}

	protected virtual void Fizle()
	{
		if(effectOnFizleScene != null)
		{
			EffectOnFizle splash = (EffectOnFizle)effectOnFizleScene.Instance();
			splash.Position = GlobalPosition;
			GetTree().Root.CallDeferred("add_child", splash);
		}
		QueueFree();
	}

	protected virtual void _on_Bullet_area_entered(Area2D area)
	{
		if (area.IsInGroup("enemyHitbox"))
		{
			Zomble zomble = (Zomble)area.GetParent();
			zomble.takeDamage(damage, knockBack);
			if(pierce <= 0)Fizle();
			pierce--;
			return;
		}
		if (!area.IsInGroup("enemy") && !area.IsInGroup("playerHB") && !area.IsInGroup("reactorHB") && !area.IsInGroup("ground"))
		{
			Fizle();
		}
	}
	
}

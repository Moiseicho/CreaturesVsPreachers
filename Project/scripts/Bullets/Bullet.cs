using Godot;
using System;

public class Bullet : KinematicBody2D
{
	private float _speed = 10f;
	private float _fizleTime = 0.5f;
	private float _damage = 5;
	private float _knockBack = 5;
	private int _pierce = 0;
	private Area2D collision;

	[Export]
	public float Speed
	{
		get { return _speed; }
		set { _speed = value; }
	}
	[Export]
	public int Pierce
	{
		get { return _pierce; }
		set { _pierce = value; }
	}

	[Export]
	public float FizleTime
	{
		get { return _fizleTime; }
		set { _fizleTime = value; }
	}

	[Export]
	public float Damage
	{
		get { return _damage; }
		set { _damage = value; }
	}

	[Export]
	public float KnockBack
	{
		get { return _knockBack; }
		set { _knockBack = value; }
	}

	public override void _Ready()
	{
		collision = (Area2D)GetNode("Area2D");
		collision.Connect("area_entered", this, nameof(_on_Bullet_area_entered));
	}

	public override void _PhysicsProcess(float delta)
	{
		_fizleTime -= delta;
		Vector2 velocity = new Vector2(_speed, 0).Rotated(Rotation);
		MoveAndCollide(velocity * delta);
		if (_fizleTime <= 0)
		{
			QueueFree();
		}
	}

	private void _on_Bullet_area_entered(Area2D area)
	{
		if (area.IsInGroup("enemyHitbox"))
		{
			Zomble zomble = (Zomble)area.GetParent();
			zomble.takeDamage(_damage, _knockBack);
			if(_pierce <= 0)QueueFree();
			_pierce--;
		}
	}
	
}

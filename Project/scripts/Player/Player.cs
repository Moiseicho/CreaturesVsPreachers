using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export]
	private float speed;
	private float defaultSpeed;
	private bool right = true;
	private bool dead = false;
	
	AnimationPlayer ap;
	Sprite sp;
	CollisionShape2D collisionHull;
	Area2D hitbox;


	[Export]
	private float maxHp = 100;
	private float hp;
	
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
		if(moving)
		{
			setAnimation("walk");
		}else
		{
			setAnimation("idle");
		}
		if(movement.x != 0 && movement.y != 0)
		{
			movement.x = (float)(movement.x/Math.Sqrt(2));
			movement.y = (float)(movement.y/Math.Sqrt(2));
		}
		MoveAndSlide(movement * speed);
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
}

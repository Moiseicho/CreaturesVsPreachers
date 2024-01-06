using Godot;
using System;
using Godot.Collections;

public class Weapon : Sprite
{
	
	[Export]
	private float damagePerBullet = 5;
	[Export]
	private float fireRate = 2f;
	[Export]
	private int ammoCapacity = 12;
	[Export]
	private float reloadTime = 1.5f;
	[Export]
	private float knockback = 5000f;
	[Export]
	private float bulletSpeed = 1000f;
	[Export]
	private Vector2 bulletOffset { get; set;}
	[Export]
	private int pierce = 0;
	
	private float ammo;
	private float fireTimer = 0f;
	private float reloadTimer = 0f;
	
	private bool disabled = false;
	
	AnimationPlayer AP;
	Zomble targetZomble;

	[Export]
	PackedScene bulletScene;
	

	public override void _Ready()
	{
		AP = (AnimationPlayer)GetNode("AnimationPlayer");
		AP.CurrentAnimation = "idle";
		ammo = ammoCapacity;
	}

	public void createBullet()
	{
		Bullet bullet = (Bullet)bulletScene.Instance();
		bullet.Position = GlobalPosition + (bulletOffset * new Vector2(1, FlipV ? -1 : 1)).Rotated(Rotation) ;
		bullet.Damage = damagePerBullet;
		bullet.KnockBack = knockback;
		bullet.FizleTime = 0.5f;
		bullet.Speed = bulletSpeed;
		bullet.Rotation = Rotation;
		bullet.Pierce = pierce;
		GetTree().Root.AddChild(bullet);
	}

	private void shoot()
	{
		if(fireTimer > 0f || reloadTimer > 0f || ammo <= 0) return;
		
		createBullet();

		fireTimer = 1/fireRate;
		AP.CurrentAnimation = "shoot";
		ammo--;
	}
	
	public void manageTimers(float delta)
	{
		if(fireTimer > 0)
		{
			fireTimer -= delta;
			if(fireTimer < 0) 
			{
				fireTimer = 0;
				AP.CurrentAnimation = "idle";
			}
		}
		if(ammo <= 0 && fireTimer <= 0f )
		{
			ammo = ammoCapacity;
			reloadTimer = reloadTime;
			AP.CurrentAnimation = "reload";
		}
		
		if(reloadTimer > 0)
		{
			reloadTimer -= delta;
			if(reloadTimer <= 0)
			{
				reloadTimer = 0;
				AP.CurrentAnimation = "idle";
			}
		}
	}
	
	public Zomble checkZomble()
	{
		Godot.Collections.Array zombles = GetTree().GetNodesInGroup("zomble");
		
		Zomble closestZomble = null;
		float closestDistanceSquared = float.MaxValue;
		
		foreach (Node zombleNode in zombles)
		{
			if (zombleNode is Zomble zomble)
			{
				float distanceSquared = GlobalPosition.DistanceSquaredTo(zomble.GlobalTransform.origin);
				if (distanceSquared < closestDistanceSquared)
				{
					closestZomble = zomble;
					closestDistanceSquared = distanceSquared;
				}
			}
		}
		if(closestDistanceSquared > 75000) return null;
		return closestZomble;
	}
	
	private void checkFlip()
	{
		float fixedDegrees = (360+(RotationDegrees % 360))%360;
		if ((360+(fixedDegrees % 360))%360 > 90 && fixedDegrees % 360 < 270 && !FlipV)
		{
			FlipV = true;
		}else if((fixedDegrees % 360 <= 90 || fixedDegrees % 360 >= 270) && FlipV)
		{
			FlipV = false;
		}
	}
	
	private Vector2 getDirectionToTarget()
	{
		return(targetZomble.GlobalTransform.origin - GlobalTransform.origin).Normalized();
	}

	public void gunlock()
	{
		if(targetZomble != null && reloadTimer <= 0f)
		{
			Vector2 directionToZomble = getDirectionToTarget();
			LookAt(GlobalTransform.origin + directionToZomble);
		}else
		{
			Player player = (Player)GetParent();
			if(player.getOrientation())
			{
				LookAt(GlobalTransform.origin + new Vector2(1, 0));
			}else
			{
				LookAt(GlobalTransform.origin + new Vector2(-1, 0));
			}
		}
		checkFlip();
	}
	
	public override void _Process(float delta)
	{
		if(disabled) return;
		manageTimers(delta);
		targetZomble = checkZomble();
		gunlock();
		if(Input.IsActionPressed("ui_shoot"))shoot();
	}


	public void disable()
	{
		disabled = true;
		Visible = false;
	}

	public void enable()
	{
		disabled = false;
		Visible = true;
		fireTimer = 0.5f;

	}
}

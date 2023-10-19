using Godot;
using System;
using Godot.Collections;

public class weapon : Sprite
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
	private float knockback = 50f;
	
	private float ammo;
	private float fireTimer = 0f;
	private float reloadTimer = 0f;
	
	AnimationPlayer AP;
	Zomble targetZomble;
	
	public override void _Ready()
	{
		AP = (AnimationPlayer)GetNode("AnimationPlayer");
		AP.CurrentAnimation = "idle";
		ammo = ammoCapacity;
	}

	public void shoot()
	{
		if(fireTimer > 0f || reloadTimer > 0f || ammo <= 0) return;
		
		if(targetZomble != null)
			targetZomble.takeDamage(damagePerBullet, getDirectionToTarget(), knockback);
		
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
		if (RotationDegrees % 360 > 90 && RotationDegrees % 360 < 270)
		{
			FlipV = true;
		}else
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
		manageTimers(delta);
		if(Input.IsActionPressed("ui_shoot"))shoot();
		targetZomble = checkZomble();
		gunlock();
	}
}

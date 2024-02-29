using Godot;
using System;
using Godot.Collections;

public class Weapon : AnimatedSprite, Giveable
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
	private Vector2 bulletOffset = new Vector2(0, 0);
	[Export]
	private float bulletTimeOffset = 0f;
	[Export]
	private float fizleTime = 0.5f;
	[Export]
	private int pierce = 0;
	[Export]
	PackedScene bulletScene;
	
	private int ammo;
	private float fireTimer = 0f;
	private float reloadTimer = 0f;
	private Timer bulletOffsetTimer;
	private Texture image;
	private bool disabled = false;
	Zomble targetZomble;
	private Vector2 originalOffset;
	

	public override void _Ready()
	{
		checkFlip();
		this.Animation = "idle";
		ammo = ammoCapacity;
		originalOffset = Offset;
		this.Playing = true;

		bulletOffsetTimer = new Timer();
		bulletOffsetTimer.OneShot = true;
		AddChild(bulletOffsetTimer);
		bulletOffsetTimer.Connect("timeout", this, nameof(createBullet));
	}

	public void createBullet()
	{
		Bullet bullet = (Bullet)bulletScene.Instance();
		bullet.Position = GlobalPosition + (bulletOffset * new Vector2(1, FlipV ? -1 : 1)).Rotated(Rotation);
		bullet.Damage = damagePerBullet;
		bullet.KnockBack = knockback;
		bullet.FizleTime = fizleTime;
		bullet.Speed = bulletSpeed;
		bullet.Pierce = pierce;
		GetTree().Root.AddChild(bullet);
		if(targetZomble != null && !Input.IsActionPressed("ui_gunlock"))
		{
			bullet.LookAt(targetZomble.GetTransform().origin);
		}else
		{
			bullet.Rotation = Rotation;
		}
	}

	private void shoot()
	{
		if(fireTimer > 0f || reloadTimer > 0f || ammo <= 0) return;
		
		if(bulletTimeOffset == 0f)
		{
			createBullet();
		}else{
			
			bulletOffsetTimer.WaitTime = bulletTimeOffset;
			bulletOffsetTimer.Start();
		}

		fireTimer = 1/fireRate;
		this.Animation = "fire";
		if(reloadTime > 0f)ammo--;
	}
	
	public void manageTimers(float delta)
	{
		if(fireTimer > 0)
		{
			fireTimer -= delta;
			if(fireTimer < 0 && reloadTimer <= 0) 
			{
				fireTimer = 0;
				this.Animation = "idle";
			}
		}
		if(ammo <= 0 && fireTimer <= 0f )
		{
			ammo = ammoCapacity;
			reloadTimer = reloadTime;
			this.Animation = "reload";
		}
		
		if(reloadTimer > 0)
		{
			reloadTimer -= delta;
			if(reloadTimer <= 0)
			{
				reloadTimer = 0;
				this.Animation = "idle";
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
		if(closestDistanceSquared > 100000) return null;
		return closestZomble;
	}
	
	private void checkFlip()
	{
		float fixedDegrees = (360+(RotationDegrees % 360))%360;
		if (fixedDegrees > 90 && fixedDegrees < 270 && !FlipV)
		{
			FlipV = true;
			Offset = new Vector2(originalOffset.x, originalOffset.y*-1);
			
		}else if((fixedDegrees <= 90 || fixedDegrees >= 270) && FlipV)
		{
			FlipV = false;
			Offset = originalOffset;
		}
	}

	public void gunlock()
	{
		if(Input.IsActionPressed("ui_gunlock"))
		{
			Vector2 mousePos = GetGlobalMousePosition();
			LookAt(mousePos);
			
		}else if(targetZomble != null && reloadTimer <= 0f)
		{
			LookAt(targetZomble.GetTransform().origin);
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
		if(Input.IsActionPressed("ui_shoot"))
		{
			shoot();
		}else if(Input.IsActionPressed("ui_reload"))
		{
			manualReload();
		}
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

	public void manualReload()
	{
		if(ammo >= ammoCapacity || reloadTimer > 0f || reloadTime == 0) return;
		ammo = ammoCapacity;
		reloadTimer = reloadTime;
		this.Animation = "reload";
	}

	public void setImage(Texture image)
	{
		this.image = image;
	}

	public Texture getImage()
	{
		return image;
	}

	public int getAmmo()
	{
		if(reloadTime == 0)
		{
			return -1;
		}
		return (int)ammo;
	}
	public int getAmmoCapacity()
	{
		return ammoCapacity;
	}
}

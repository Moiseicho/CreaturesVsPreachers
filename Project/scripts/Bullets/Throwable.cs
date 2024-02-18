using Godot;

public class Throwable : Bullet
{

	private bool hold = true;
	private float maxSpeed;
	[Export]
	private float airSlowdown;

	[Export]
	public float Speed {get{return speed;}set{speed = value;}}
	[Export]
	public float FizleTime {get{return fizleTime;}set{fizleTime = value;}}
	[Export]
	public float Damage {get{return damage;}set{damage = value;}}
	[Export]
	public float KnockBack {get{return knockBack;}set{knockBack = value;}}
	[Export]
	public int Pierce {get{return pierce;}set{pierce = value;}}

	public override void _Ready()
	{
		base._Ready();
		sprite.Animation = "hold";
		maxSpeed = speed;
	}

	public override void _PhysicsProcess(float delta)
	{
		if(hold) return;
		base._PhysicsProcess(delta);
		if(speed > 10f)speed -= maxSpeed*delta/fizleTime*airSlowdown;
	}

	public void Throw()
	{
		LookAt(GetGlobalMousePosition());
		hold = false;
		sprite.Animation = "throw";
	}

	protected override void _on_Bullet_area_entered(Area2D area)
	{
		if(hold)return;
		base._on_Bullet_area_entered(area);
		if(area.IsInGroup("reactorHB"))
		{
			Fizle();
		}
	}
}

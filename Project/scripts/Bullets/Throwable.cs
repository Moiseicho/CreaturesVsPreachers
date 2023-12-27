using Godot;

public class Throwable : Bullet
{

	private bool hold = true;
	private AnimationPlayer ap;
	private float maxSpeed;
	[Export]
	private float airSlowdown;

	public override void _Ready()
	{
		base._Ready();
		ap = (AnimationPlayer)GetNode("AnimationPlayer");
		ap.CurrentAnimation = "Hold";
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
		ap.CurrentAnimation = "Throw";
	}

	protected override void _on_Bullet_area_entered(Area2D area)
	{

		//Make something to check if the throwable is outside the stage walls, since their hitboxes don't trigger this function 
		if(hold)return;
		base._on_Bullet_area_entered(area);
		if(area.IsInGroup("reactorHB"))
		{
			Fizle();
		}
	}
}

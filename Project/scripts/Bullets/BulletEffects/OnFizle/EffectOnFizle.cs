using Godot;

public class EffectOnFizle : Area2D
{
	[Export]
	private float Damage = 5;
	[Export]
	private float slow;
	[Export]
	private float effectTime;
	[Export]
	private float tickTime;
	[Export]
	private float entryTime;
	[Export]
	private float fadeTime;
	
	private Timer tickTimer;

	private AnimatedSprite animatedSprite;

	public override void _Ready()
	{
		animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		animatedSprite.Animation = "activation";
		animatedSprite.Frame = 0;
		animatedSprite.Play();

		Timer effectTimer = new Timer();
		effectTimer.WaitTime = effectTime;
		effectTimer.OneShot = true;
		AddChild(effectTimer);
		effectTimer.Connect("timeout", this, nameof(FadeOut));
		
		Timer entryTimer = new Timer();
		entryTimer.WaitTime = entryTime;
		entryTimer.OneShot = true;
		AddChild(entryTimer);
		entryTimer.Connect("timeout", this, nameof(Default));

		tickTimer = new Timer();
		tickTimer.WaitTime = tickTime;
		AddChild(tickTimer);
		tickTimer.Connect("timeout", this, nameof(Tick));

		effectTimer.Start();
		tickTimer.Start();
		entryTimer.Start();
	}

	private void Default()
	{
		animatedSprite.Animation = "default";
	}

	private void FadeOut()
	{
		animatedSprite.Animation = "fade";
		Timer timer = new Timer();
		timer.WaitTime = fadeTime;
		timer.OneShot = true;
		AddChild(timer);
		timer.Connect("timeout", this, nameof(removeEffect));
		timer.Start();
	}
	
	private void removeEffect()
	{
		QueueFree();
	}
	
	private void Tick()
	{
		foreach(Node2D node in GetOverlappingBodies())
		{
			if(node is Zomble)
			{
				Zomble zomble = (Zomble)node;
				zomble.takeDamage(Damage, 0);
				zomble.slowDown(slow, tickTime+0.1f);
			}
		}
	}
	
	private void _on_AcidSplash_body_entered(object body)
	{
		if(body is Zomble)
		{
			Zomble zomble = (Zomble)body;
			zomble.slowDown(slow, tickTime+0.1f);
		}
	}

}




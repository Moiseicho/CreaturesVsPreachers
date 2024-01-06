using Godot;

public class StaticSummon : Summonable
{

	[Export]
	private Vector2 position;
	[Export]
	private bool playerRelative = false;
	private Player player;
	[Export]
	private float entryTime = 0;
	[Export]
	private float effectTime = 0;
	[Export]
	private float fadeTime = 0;
	private AnimatedSprite animatedSprite;
	[Export]
	private bool freezeEnabled = true;
	private Area2D freezeArea;
	private Area2D unfreezeArea;
	private CollisionPolygon2D collisionPolygon2D;

	public override void _Ready()
	{
		animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
		animatedSprite.Animation = "activation";
		animatedSprite.Frame = 0;
		animatedSprite.Play();

		if(freezeEnabled)
		{
			freezeArea = (Area2D)GetNode("FreezeArea");
			collisionPolygon2D = (CollisionPolygon2D)GetNode("CollisionPolygon2D");
			unfreezeArea = (Area2D)GetNode("UnfreezeArea");

			collisionPolygon2D.Disabled = true;
		}
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

		effectTimer.Start();
		entryTimer.Start();
	}

	private void Default()
	{
		if (freezeEnabled)
		{
			foreach (Node2D node in freezeArea.GetOverlappingBodies())
			{
				if (node is Zomble)
				{
					Zomble zomble = (Zomble)node;
					zomble.freeze();
				}
			}
			
			collisionPolygon2D.Disabled = false;
		}
		animatedSprite.Animation = "default";
	}

	public void setPlayer(Player player)
	{
		this.player = player;
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
		if(freezeEnabled)
		{foreach(Node2D node in unfreezeArea.GetOverlappingBodies())
			{
				if (node is Zomble)
				{
					Zomble zomble = (Zomble)node;
					zomble.unfreeze();
				}
			}
		}
		QueueFree();
	}

	public override void Summon()
	{
		Position = position;
		if (playerRelative)
		{
			player.AddChild(this);
		}else
		{
			player.GetTree().Root.AddChild(this);
		}
	}

}

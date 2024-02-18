using Godot;

public class StatsUpgrade : Object
{
	private float speed = 0;
	private float health = 0;
	
	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	public float Health
	{
		get { return health; }
		set { health = value; }
	}

	public StatsUpgrade(float speed, float health)
	{
		this.speed = speed;
		this.health = health;
	}

	public void GiveUpgrade(Player player)
	{
		player.buffStats(speed, health);
	}
}

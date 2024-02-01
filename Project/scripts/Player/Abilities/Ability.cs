using Godot;
using System;

public class Ability : Node
{
	protected float cooldown = 0f;
	protected Player player;

	public Player Player
	{
		get { return player; }
		set { player = value; }
	}
	public float Cooldown
	{
		get { return cooldown; }
		set { cooldown = value; }
	}

	public virtual void Effect()
	{
		// Implementation goes here
	}
}

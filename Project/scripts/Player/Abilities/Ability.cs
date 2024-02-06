using Godot;
using System;

public class Ability : Node, Giveable
{
	protected float cooldown = 0f;
	protected Player player;
	protected Texture image;

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

	public Texture Image
	{
		get { return image; }
		set { image = value; }
	}

	public virtual void Effect(){}

	public void setImage(Texture image)
	{
		this.image = image;
	}

	public Texture getImage()
	{
		return image;
	}
}

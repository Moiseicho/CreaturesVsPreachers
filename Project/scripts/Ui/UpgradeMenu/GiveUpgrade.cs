using Godot;

public class GiveUpgrade : Object
{
	private Giveable item;

	public Giveable Item
	{
		get { return item; }
		set { item = value; }
	}

	public GiveUpgrade(Giveable item)
	{
		this.item = item;
	}

	public void GiveItem(Player player)
	{
		if (item is Ability)
		{
			player.Give((Ability) item);
		}
		else if (item is Weapon)
		{
			player.Give((Weapon) item);
		}
	}

	public void setImage(Texture image)
	{
		item.setImage(image);
	}
}

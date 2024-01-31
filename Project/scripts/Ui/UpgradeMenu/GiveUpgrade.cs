using Godot;

public class GiveUpgrade : Object
{
	private object item;

	public object Item
	{
		get { return item; }
		set { item = value; }
	}

	public GiveUpgrade(object item)
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
}

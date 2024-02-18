using Godot;

public class UpgradeOption : TextureButton
{
	private Player player = null;
	private StatsUpgrade statsUpgrade = null;
	private GiveUpgrade giveUpgrade = null;

	[Signal]
	public delegate void UpgradeOptionPressed();

	public UpgradeOption(Texture image, StatsUpgrade statsUpgrade, GiveUpgrade giveUpgrade)
	{
		this.TextureNormal = image;
		this.TextureDisabled = ResourceLoader.Load("res://Sprites/Missing.png") as Texture;
		this.statsUpgrade = statsUpgrade;
		this.giveUpgrade = giveUpgrade;
	}

	public UpgradeOption()
	{
		this.TextureDisabled = ResourceLoader.Load("res://Sprites/Missing.png") as Texture;
	}

	public void clone(UpgradeOption other)
	{
		this.TextureNormal = other.TextureNormal;
		this.TextureDisabled = other.TextureDisabled;
		this.statsUpgrade = other.statsUpgrade;
		this.giveUpgrade = other.giveUpgrade;
	}

	private void _on_UpgradeOptionButton_pressed()
	{
		if(player == null) return;
		if(statsUpgrade != null) statsUpgrade.GiveUpgrade(player);
		if(giveUpgrade != null) giveUpgrade.GiveItem(player);
		EmitSignal(nameof(UpgradeOptionPressed));
	}

	public void setPlayer(Player player)
	{
		this.player = player;
	}

	public Player getPlayer()
	{
		return player;
	}

}

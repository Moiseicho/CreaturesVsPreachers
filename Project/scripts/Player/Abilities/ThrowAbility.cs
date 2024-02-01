using Godot;

public class ThrowAbility : Ability
{
	[Export]
	private PackedScene bulletScene;

	public void setBulletScene(PackedScene bulletScene)
	{
		this.bulletScene = bulletScene;
	}

	
	public override void Effect()
	{
		player.equipAbility(bulletScene);
	}
}

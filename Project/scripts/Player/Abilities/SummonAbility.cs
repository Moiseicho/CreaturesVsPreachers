using Godot;

public class SummonAbility : Ability
{
	private PackedScene summonableScene;
	private bool isStatic = false;

	public SummonAbility(PackedScene summonableScene, bool isStatic)
	{
		this.summonableScene = summonableScene;
		this.isStatic = isStatic;
	}

	public void setIsStatic(bool isStatic)
	{
		this.isStatic = isStatic;
	}

	public void setSummonableScene(PackedScene summonableScene)
	{
		this.summonableScene = summonableScene;
	}

	public override void Effect()
	{
		if(isStatic)
		{
			GD.Print("Summoning Static: " + summonableScene.ResourceName);
			StaticSummon summonable = (StaticSummon)summonableScene.Instance();
			summonable.setPlayer(player);
			summonable.Summon();
		}
		else
		{
			//KinematicSummon in the future
		}
	}

	

}

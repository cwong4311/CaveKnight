using System.Collections.Generic;

public class DragonStateInfo : IStateInfoMap
{
	private Dictionary<string, (string, double, double)> stateInfoMap = new Dictionary<string, (string, double, double)>()
	{
		{ "AerialLand" , ( "Land" , 1 , 2.35 ) },
		{ "AirLocomotion" , ( "Dragon_Aerial_BlendTree" , 1 , 0.7308437 ) },
		{ "BackLand" , ( "Land" , 1 , 2.34 ) },
		{ "Backstep" , ( "Take Off" , 1 , 1.146667 ) },
		{ "BackstepFireball" , ( "Fly Fireball Shoot" , 1 , 0.825 ) },
		{ "Basic Attack" , ( "Basic Attack" , 1 , 0.75 ) },
		{ "Charging" , ( "Run" , 1.5 , 0 ) },
		{ "Die" , ( "Die" , 1 , 1.5 ) },
		{ "Divebomb" , ( "Fly Glide" , 1 , 1 ) },
		{ "Double Tail Swipe" , ( "Tail Attack" , 1 , 2.413768 ) },
		{ "Enraged_Override" , ( "MouthOpen" , 1 , 0.9 ) },
		{ "Fireball" , ( "Fireball Shoot" , 1 , 0.75 ) },
		{ "FirePillar" , ( "Scream" , 1 , 2.053334 ) },
		{ "Fly Fireball Shoot" , ( "Fly Fireball Shoot" , 1 , 0.825 ) },
		{ "Get Hit" , ( "Get Hit" , 1 , 1.083333 ) },
		{ "LandLocomotion" , ( "Dragon_Ground_BlendTree" , 1 , 1.333333 ) },
		{ "Null_Override" , ( "" , 1 , 1 ) },
		{ "Scream" , ( "Scream" , 1 , 2.083333 ) },
		{ "Tail Attack" , ( "Tail Attack" , 1 , 1.083333 ) },
		{ "Take Off" , ( "Take Off" , 1 , 1.820049 ) },
	};

	public StateInfoSetting? GetStateInfoByName(string stateName)
	{
		if (stateInfoMap.ContainsKey(stateName))
		{
			return new StateInfoSetting()
			{
				Name = stateName,
				Clip = stateInfoMap[stateName].Item1,
				Speed = stateInfoMap[stateName].Item2,
				Duration = stateInfoMap[stateName].Item3
			};
		}
		return null;
	}
}

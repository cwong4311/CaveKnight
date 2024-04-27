using System.Collections.Generic;

public class DragonStateInfo : IStateInfoMap
{
	private Dictionary<string, (string, double, double)> stateInfoMap = new Dictionary<string, (string, double, double)>()
	{
		{ "AirLocomotion" , ( "Blend Tree" , 1 , 1.177778 ) },
		{ "BackLand" , ( "Land" , 1 , 2.6 ) },
		{ "Backstep" , ( "Take Off" , 1 , 2.666667 ) },
		{ "BackstepFireball" , ( "Fly Fireball Shoot" , 1 , 1.1 ) },
		{ "Basic Attack" , ( "Basic Attack" , 1 , 1 ) },
		{ "Defend" , ( "Defend" , 1 , 1.533333 ) },
		{ "Die" , ( "Die" , 1 , 1.5 ) },
		{ "Fireball" , ( "Fireball Shoot" , 1 , 1 ) },
		{ "Fly Fireball Shoot" , ( "Fly Fireball Shoot" , 1 , 1.1 ) },
		{ "Get Hit" , ( "Get Hit" , 1 , 1.333333 ) },
		{ "Land" , ( "Land" , 1 , 2.6 ) },
		{ "LandLocomotion" , ( "Blend Tree" , 1 , 1.333333 ) },
		{ "Scream" , ( "Scream" , 1 , 2.333333 ) },
		{ "Tail Attack" , ( "Tail Attack" , 1 , 1.333333 ) },
		{ "Take Off" , ( "Take Off" , 1 , 2.666667 ) },
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

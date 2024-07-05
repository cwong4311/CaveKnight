using System.Collections.Generic;

public class SkeletonStateInfo : IStateInfoMap
{
	private Dictionary<string, (string, double, double)> stateInfoMap = new Dictionary<string, (string, double, double)>()
	{
		{ "Attack" , ( "Attack" , 1 , 2.516667 ) },
		{ "Death" , ( "Death" , 1 , 2.166667 ) },
		{ "GroundLocomotive" , ( "SkeletonIdle" , 1 , 2.8 ) },
		{ "Hit" , ( "Damage" , 1 , 0.5833334 ) },
		{ "Stagger" , ( "Knockback" , 1 , 2.116667 ) },
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

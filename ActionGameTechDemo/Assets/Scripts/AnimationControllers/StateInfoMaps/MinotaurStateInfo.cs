using System.Collections.Generic;

public class MinotaurStateInfo : IStateInfoMap
{
	private Dictionary<string, (string, double, double)> stateInfoMap = new Dictionary<string, (string, double, double)>()
	{
		{ "Attack" , ( "attack2" , 1 , 1.38065 ) },
		{ "Attack_2" , ( "attack3" , 1 , 1.500807 ) },
		{ "Attack_3" , ( "attack4_kick" , 1 , 1.616606 ) },
		{ "Die" , ( "death" , 1 , 2.333333 ) },
		{ "GroundLocomotive" , ( "MinotaurIdle" , 1 , 1.614583 ) },
		{ "Hurt" , ( "hit_1" , 1 , 0.6884181 ) },
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

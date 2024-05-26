using System.Collections.Generic;

public class OrcAssassinStateInfo : IStateInfoMap
{
	private Dictionary<string, (string, double, double)> stateInfoMap = new Dictionary<string, (string, double, double)>()
	{
		{ "Attack" , ( "atack17" , 1 , 0.78 ) },
		{ "Attack_2" , ( "atack22" , 1 , 1.134421 ) },
		{ "Death" , ( "death4" , 1 , 2.916667 ) },
		{ "Dodge" , ( "sneakwalk" , 1 , 0.4957329 ) },
		{ "GroundLocomotive" , ( "Assassin_Idle" , 1 , 1.902778 ) },
		{ "Hit" , ( "dodge" , 0.6 , 0.574653 ) },
		{ "SpecialAttack" , ( "atack14" , 1.4 , 1.605486 ) },
		{ "SpecialAttack2" , ( "atack20" , 1 , 0.8212562 ) },
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

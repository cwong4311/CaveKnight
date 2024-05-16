using System;

public interface IStateInfoMap
{
    public StateInfoSetting? GetStateInfoByName(string stateName);
}

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class StateInfoMapResolver
{
    public static IStateInfoMap GetStateInfoMap(string animatorName)
    {
        var stateInfoMapName = animatorName + "StateInfo";
        return CreateInstance(stateInfoMapName);
    }

    private static IStateInfoMap CreateInstance(string className)
    {
        // Get all types in the assembly
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        // Filter types that match the given class name and inherit from IBoo interface
        Type targetType = types.FirstOrDefault(t => t.Name == className && typeof(IStateInfoMap).IsAssignableFrom(t));

        // If targetType is found, create an instance and return it
        if (targetType != null)
        {
            return Activator.CreateInstance(targetType) as IStateInfoMap;
        }

        return null;
    }
}

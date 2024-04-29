#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

public class AnimatorStateInfoGenerator : EditorWindow
{
    private AnimatorController selectedController;
    private Dictionary<string, (string, float, float)> stateInfoMap = new Dictionary<string, (string, float, float)>();

    [MenuItem("Window/AnimatorControllerInfoGenerator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AnimatorStateInfoGenerator));
    }

    void OnGUI()
    {
        GUILayout.Label("Select Animator Controller", EditorStyles.boldLabel);
        selectedController = EditorGUILayout.ObjectField(selectedController, typeof(AnimatorController), false) as AnimatorController;

        if (GUILayout.Button("Generate State Info Mapping"))
        {
            GenerateStateInfo(selectedController);
        }
    }

    void GenerateStateInfo(AnimatorController controller)
    {
        if (controller == null)
        {
            Debug.LogError("Please select an Animator Controller!");
            return;
        }

        stateInfoMap.Clear();
        GetAnimatorStateInfo(controller);
        WriteToFile(controller);
    }

    public void GetAnimatorStateInfo(AnimatorController targetAnimator)
    {
        AnimatorControllerLayer[] acLayers = targetAnimator.layers;
        List<AnimatorState> allStates = new List<AnimatorState>();
        foreach (AnimatorControllerLayer i in acLayers)
        {
            ChildAnimatorState[] animStates = i.stateMachine.states;
            foreach (ChildAnimatorState j in animStates)
            {
                var name = j.state.name;
                var clip = j.state.motion.name;
                var speed = j.state.speed;
                var duration = j.state.motion.averageDuration;

                foreach (var transition in j.state.transitions)
                {
                    if (transition.hasExitTime)
                    {
                        duration *= transition.exitTime;
                    }
                }

                if (stateInfoMap.ContainsKey(name) == false)
                {
                    stateInfoMap.Add(name, (clip, speed, duration));
                }
            }
        }
    }

    private void WriteToFile(AnimatorController controller)
    {
        string className = controller.name + "StateInfo";
        string filePath = Application.dataPath + "/Scripts/AnimationControllers/StateInfoMaps/" + className + ".cs";

        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("");
            writer.WriteLine("public class " + className + " : IStateInfoMap");
            writer.WriteLine("{");
            writer.WriteLine("\tprivate Dictionary<string, (string, double, double)> stateInfoMap = new Dictionary<string, (string, double, double)>()");
            writer.WriteLine("\t{");

            if (stateInfoMap != null)
            {
                foreach (var stateInfo in stateInfoMap.OrderBy(e => e.Key))
                {
                    var content = "\"" + stateInfo.Key + "\" , ( \"" + stateInfo.Value.Item1 + "\" , " + stateInfo.Value.Item2 + " , " + stateInfo.Value.Item3 + " )";
                    writer.WriteLine("\t\t{ " + content + " },");
                }
            }

            writer.WriteLine("\t};");

            writer.WriteLine("");

            writer.WriteLine("\tpublic StateInfoSetting? GetStateInfoByName(string stateName)");
            writer.WriteLine("\t{");
                writer.WriteLine("\t\tif (stateInfoMap.ContainsKey(stateName))");
                writer.WriteLine("\t\t{");
                    writer.WriteLine("\t\t\treturn new StateInfoSetting()");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tName = stateName,");
                    writer.WriteLine("\t\t\t\tClip = stateInfoMap[stateName].Item1,");
                    writer.WriteLine("\t\t\t\tSpeed = stateInfoMap[stateName].Item2,");
                    writer.WriteLine("\t\t\t\tDuration = stateInfoMap[stateName].Item3");
                    writer.WriteLine("\t\t\t};");
                writer.WriteLine("\t\t}");
                writer.WriteLine("\t\treturn null;");
            writer.WriteLine("\t}");

            writer.WriteLine("}");
        }

        Debug.Log("Class created successfully at: " + filePath);
    }
}

#endif
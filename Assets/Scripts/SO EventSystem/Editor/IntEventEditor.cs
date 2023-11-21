
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IntEvent), editorForChildClasses: true)]
public class IntEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        IntEvent e = target as IntEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatEvent), editorForChildClasses: true)]
public class FloatEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        FloatEvent e = target as FloatEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     
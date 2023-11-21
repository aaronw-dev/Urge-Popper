
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StringEvent), editorForChildClasses: true)]
public class StringEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        StringEvent e = target as StringEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     
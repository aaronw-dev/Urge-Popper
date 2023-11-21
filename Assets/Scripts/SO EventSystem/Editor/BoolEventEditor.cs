
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolEvent), editorForChildClasses: true)]
public class BoolEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        BoolEvent e = target as BoolEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     
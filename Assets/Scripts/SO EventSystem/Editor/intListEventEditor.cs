
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(intListEvent), editorForChildClasses: true)]
public class intListEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        intListEvent e = target as intListEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     
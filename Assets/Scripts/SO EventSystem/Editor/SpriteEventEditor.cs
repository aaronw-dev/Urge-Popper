
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteEvent), editorForChildClasses: true)]
public class SpriteEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        SpriteEvent e = target as SpriteEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     
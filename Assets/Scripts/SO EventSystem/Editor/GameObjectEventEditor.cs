
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectEvent), editorForChildClasses: true)]
public class GameObjectEventEditor : Editor
{
        
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
        GUI.enabled = Application.isPlaying;
        GameObjectEvent e = target as GameObjectEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(e.testProperty);
        }
    }

                     

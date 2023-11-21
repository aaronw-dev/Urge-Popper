using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AutoNamedAttribute))]
public class AutoNamedAttributeDrawer : PropertyDrawer
{
    private bool _copyName = true;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var scriptableObject = property.serializedObject.targetObject as ScriptableObject;

        if (scriptableObject == null)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            Debug.LogError("Use AutoNamed Attribute only on strings");
            return;
        }

        position.height = 20;

        var toggleRect = new Rect(position.x + position.width - 13, position.y, 15, 20);
        _copyName = EditorGUI.ToggleLeft(toggleRect, "", _copyName);

        var fieldRect = new Rect(position.x, position.y, (position.width - toggleRect.width), position.height);

        if (_copyName)
        {
            if (property.stringValue != scriptableObject.name)
            {
                property.stringValue = scriptableObject.name;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(fieldRect, label, scriptableObject.name);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUI.PropertyField(fieldRect, property, label);
        }
    }
}

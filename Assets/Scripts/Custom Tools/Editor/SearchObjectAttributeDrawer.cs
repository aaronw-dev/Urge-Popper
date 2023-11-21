using NaughtyAttributes.Editor;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CustomPropertyDrawer(typeof(SearchObjectAttribute))]
public class SearchObjectAttributeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        position.width -= 60;

        EditorGUI.ObjectField(position, property, label);

        position.x += position.width;

        position.width = 60;
        if (GUI.Button(position, new GUIContent("Find")))
        {
            //PropertyUtility.GetAttribute <>

            Type t = (attribute as SearchObjectAttribute).searchObjectType;
            string s = (attribute as SearchObjectAttribute).s;
            if (s != string.Empty)
            {
                SearchObjectAttribute searchAtr = PropertyUtility.GetAttribute<SearchObjectAttribute>(property);
                object target = GetTargetObjectWithProperty(property);
                FieldInfo conditionField = ReflectionUtility.GetField(target, searchAtr.s);
                if (conditionField != null &&
                    conditionField.FieldType == typeof(Type))
                {
                    t = (Type)conditionField.GetValue(target);
                }

                PropertyInfo conditionProperty = ReflectionUtility.GetProperty(target, searchAtr.s);
                if (conditionProperty != null &&
                    conditionProperty.PropertyType == typeof(Type))
                {
                    t = (Type)conditionProperty.GetValue(target);
                }

                MethodInfo conditionMethod = ReflectionUtility.GetMethod(target, searchAtr.s);
                if (conditionMethod != null &&
                    conditionMethod.ReturnType == typeof(Type) &&
                    conditionMethod.GetParameters().Length == 0)
                {
                    t = (Type)conditionMethod.Invoke(target, null);
                }
            }


            ObjectSearchProvider OSP = ScriptableObject.CreateInstance<ObjectSearchProvider>();
            OSP.Init(t, property);
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), OSP);
        }
    }
    public static object GetTargetObjectWithProperty(SerializedProperty property)
    {
        string path = property.propertyPath.Replace(".Array.data[", "[");
        object obj = property.serializedObject.targetObject;
        string[] elements = path.Split('.');

        for (int i = 0; i < elements.Length - 1; i++)
        {
            string element = elements[i];
            if (element.Contains("["))
            {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }

        return obj;
    }
    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
        {
            return null;
        }

        Type type = source.GetType();

        while (type != null)
        {
            FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(source);
            }

            PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                return property.GetValue(source, null);
            }

            type = type.BaseType;
        }

        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
        if (enumerable == null)
        {
            return null;
        }

        IEnumerator enumerator = enumerable.GetEnumerator();
        for (int i = 0; i <= index; i++)
        {
            if (!enumerator.MoveNext())
            {
                return null;
            }
        }

        return enumerator.Current;
    }
}
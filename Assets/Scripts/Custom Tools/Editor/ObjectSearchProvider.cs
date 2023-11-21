using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObjectSearchProvider : ScriptableObject, ISearchWindowProvider
{
    Type assetType;

    public SerializedProperty serializedProperty;

    public void Init(Type assetType, SerializedProperty serializedProperty)

    {

        this.assetType = assetType;

        this.serializedProperty = serializedProperty;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> list = new List<SearchTreeEntry>();

        string[] assetGuids = AssetDatabase.FindAssets($"t: {assetType.Name}");
        List<string> paths = new List<string>();
        List<string> Treepaths = new List<string>();
        foreach (string guid in assetGuids)

        {
            paths.Add(AssetDatabase.GUIDToAssetPath(guid));

        }
        paths.Sort((a, b) =>
        {

            string[] splitsl = a.Split('/');
            string[] splits2 = b.Split('/');

            for (int i = 0; i < splitsl.Length; i++)
            {
                if (i > splits2.Length)
                {
                    return 1;
                }
                int value = splitsl[i].CompareTo(splits2[i]);
                if (value != 0)
                {
                    // Make sure that leaves go before nodes
                    if (splitsl.Length != splits2.Length && (i == splitsl.Length - 1 || i == splits2.Length - 1))
                        return splitsl.Length < splits2.Length ? 1 : -1;

                    return value;
                }
            }
            return 0;

        });

        List<int> oldIndexes = new List<int>();
        int lowestNumber = int.MaxValue;
        int lowIndex = 0;
        for (int i = 0; i < paths.Count; i++)
        {
            //Debug.Log(paths[i]);
            string[] lp = paths[i].Split('/');
            //Debug.Log(lp[0]);
            if (lp[0] == "Packages") continue;
            int t = lp.Length;
            int y = i;
            if (t < lowestNumber)
            {
                lowestNumber = t;
                lowIndex = y;
            }
            Treepaths.Add(paths[y]);
            oldIndexes.Add(y);
        }

        //Debug.Log(lowestNumber);
        //Debug.Log(paths[lowIndex]);
        //bool run = true;
        int lN = lowestNumber;
        for (int z = 0; z <= lN - 2; z++)
        {
            string first = paths[lowIndex].Split('/')[z];
            //Debug.Log(first);
            for (int x = 0; x < Treepaths.Count; x++)
            {
                string pt = Treepaths[x];
                string[] ptList = pt.Split('/').ToArray();
                if (z > ptList.Length - 1)
                    continue;
                //Debug.Log(ptList[z]);
                if (first != ptList[z])
                {

                    //run = false;
                    lowestNumber--;
                    if (lowestNumber == 2)
                        break;
                }

            }
        }

        //if (run)
        if (lowestNumber > 2)
            for (int i = 0; i < Treepaths.Count; i++)
            {
                Treepaths[i] = String.Join(@"/", Treepaths[i].Split('/').Skip(lowestNumber - 2));
                //Debug.Log(Treepaths[i]);
            }

        List<string> groups = new List<string>();
        for (int y = 0; y < Treepaths.Count; y++)

        {
            string item = Treepaths[y];
            string[] entryTitle = item.Split('/'); string groupName = "";
            for (int i = 0; i < entryTitle.Length - 1; i++)
            {

                groupName += entryTitle[i];
                if (!groups.Contains(groupName))

                {
                    list.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    groups.Add(groupName);
                }
                groupName += "/";
            }
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(paths[oldIndexes[y]]);

            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last(), EditorGUIUtility.ObjectContent(obj, obj.GetType()).image));


            entry.level = entryTitle.Length;

            entry.userData = obj;

            list.Add(entry);
        }
        return list;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        serializedProperty.objectReferenceValue = (UnityEngine.Object)SearchTreeEntry.userData;
        serializedProperty.serializedObject.ApplyModifiedProperties();
        return true;
    }
}

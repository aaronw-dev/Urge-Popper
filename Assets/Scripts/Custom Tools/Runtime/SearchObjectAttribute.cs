using System;
using UnityEngine;

public class SearchObjectAttribute : PropertyAttribute
{
    public Type searchObjectType;
    public string s = String.Empty;
    public SearchObjectAttribute(Type searchObjectType)
    {
        this.searchObjectType = searchObjectType;
    }

    public SearchObjectAttribute(string s)
    {
        this.s = s;

    }

}
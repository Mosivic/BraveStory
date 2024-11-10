using System;
using System.Linq;

namespace Miros.Core;

public  struct Tag
{
    public readonly string _name;

    public int HashCode {get;set;}
    public string ShortName {get;set;}
    public int[] AncestorHashCodes{get;set;}
    public string[] AncestorNames {get;set;}


    public bool IsValid => !string.IsNullOrEmpty(_name);

    public Tag(string name)
    {
        _name = name;
        HashCode = name.GetHashCode();

        var tags = name.Split('.');

        AncestorNames = new string[tags.Length - 1];
        AncestorHashCodes = new int[tags.Length - 1];
        var i = 0;
        var ancestorTag = "";
        while (i < tags.Length - 1)
        {
            ancestorTag += tags[i];
            AncestorHashCodes[i] = ancestorTag.GetHashCode();
            AncestorNames[i] = ancestorTag;
            ancestorTag += ".";
            i++;
        }

        ShortName = tags.Last();
    }

    public bool IsDescendantOf(Tag other)
    {
        return other.AncestorHashCodes.Contains(HashCode);
    }

    public override bool Equals(object obj)
    {
        return obj is Tag tag && this == tag;
    }



    public static bool operator ==(Tag x, Tag y)
    {
        return x.HashCode == y.HashCode;
    }

    public static bool operator !=(Tag x, Tag y)
    {
        return x.HashCode != y.HashCode;
    }

    public bool HasTag(Tag tag)
    {
        foreach (var ancestorHashCode in AncestorHashCodes)
            if (ancestorHashCode == tag.HashCode)
                return true;

        return this == tag;
    }

    public override int GetHashCode()
    {
        return HashCode;
    }

}
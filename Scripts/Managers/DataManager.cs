using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DataManager : MonoBehaviour
{
    private static Dictionary<string, CharacterData> _characterDataDict = new Dictionary<string, CharacterData>();

    public static void LoadCharacterData()
    {
        string yamlPath = Path.Combine(Application.streamingAssetsPath, "yaml/characters.yaml");
        var characterList = YamlLoader.LoadYamlFile<CharacterData>(yamlPath);

        if (characterList != null)
        {
            _characterDataDict.Clear();
            foreach (var character in characterList)
            {
                _characterDataDict[character.Name] = character;
            }
        }
    }

    public static CharacterData GetCharacterData(string name)
    {
        return _characterDataDict.ContainsKey(name) ? _characterDataDict[name] : null;
    }
} 
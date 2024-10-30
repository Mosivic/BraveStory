using UnityEngine;
using YamlDotNet.Serialization;
using System.IO;
using System.Collections.Generic;

public class YamlLoader
{
    public static List<T> LoadYamlFile<T>(string filePath)
    {
        try
        {
            // 读取YAML文件内容
            string yamlText = File.ReadAllText(filePath);
            
            // 创建YAML反序列化器
            var deserializer = new DeserializerBuilder().Build();
            
            // 将YAML转换为对象列表
            var result = deserializer.Deserialize<List<T>>(yamlText);
            
            return result;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading YAML file {filePath}: {e.Message}");
            return null;
        }
    }
} 
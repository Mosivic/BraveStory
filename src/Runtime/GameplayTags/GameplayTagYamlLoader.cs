using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using System.Linq;

public class GameplayTagYamlLoader
{
    private readonly GameplayTagManager _tagManager;
    private readonly GameplayTagInheritance _inheritance;
    
    public GameplayTagYamlLoader(GameplayTagManager tagManager, GameplayTagInheritance inheritance)
    {
        _tagManager = tagManager;
        _inheritance = inheritance;
    }
    
    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Tag config file not found: {filePath}");
        }
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        var yaml = File.ReadAllText(filePath);
        var config = deserializer.Deserialize<GameplayTagsConfig>(yaml);
        
        foreach (var tagData in config.Tags)
        {
            ProcessTagData(tagData, "");
        }
    }
    
    public void SaveToFile(string filePath)
    {
        var config = new GameplayTagsConfig();
        var rootTags = _tagManager.GetAllRegisteredTags()
            .Where(tag => !tag.ToString().Contains('.'));
            
        foreach (var rootTag in rootTags)
        {
            config.Tags.Add(CreateTagData(rootTag));
        }
        
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        var yaml = serializer.Serialize(config);
        File.WriteAllText(filePath, yaml);
    }
    
    private void ProcessTagData(GameplayTagData tagData, string parentPath)
    {
        var fullPath = string.IsNullOrEmpty(parentPath) ? 
            tagData.Name : $"{parentPath}.{tagData.Name}";
            
        var tag = _tagManager.RequestGameplayTag(fullPath);
        
        // 设置属性
        foreach (var prop in tagData.Properties)
        {
            _inheritance.SetProperty(tag, prop.Key, prop.Value);
        }
        
        // 处理子标签
        foreach (var childData in tagData.Children)
        {
            ProcessTagData(childData, fullPath);
        }
    }
    
    private GameplayTagData CreateTagData(GameplayTag tag)
    {
        var tagData = new GameplayTagData
        {
            Name = tag.ToString().Split('.').Last(),
            Properties = _inheritance.GetAllProperties(tag)
        };
        
        // 添加子标签
        foreach (var childTag in _tagManager.GetDirectChildTags(tag))
        {
            tagData.Children.Add(CreateTagData(childTag));
        }
        
        return tagData;
    }
} 
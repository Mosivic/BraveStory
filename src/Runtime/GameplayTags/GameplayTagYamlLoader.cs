using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;
using Godot;

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
        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Tag config file not found: {filePath}");
            return;
        }
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        var yaml = file.GetAsText();
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
        
        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr($"Cannot create or open file for writing: {filePath}");
            return;
        }
        file.StoreString(yaml);
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
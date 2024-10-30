using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;
using Godot;

public class GameplayTagYamlLoader
{
    private readonly GameplayTagManager _tagManager;
    
    public GameplayTagYamlLoader()
    {
        _tagManager = GameplayTagManager.Instance;
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
        
        // 处理基础路径
        string basePath = config.BasePath ?? "";
        
        foreach (var tagData in config.Tags)
        {
            ProcessTagData(tagData, basePath);
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
        
        // 处理多继承关系，支持完整路径
        foreach (var inheritTag in tagData.Inherits)
        {
            // 如果继承标签包含点号，则视��完整路径
            var parentTagPath = inheritTag.Contains('.') ? 
                inheritTag : $"{parentPath}.{inheritTag}";
                
            var parentTag = _tagManager.RequestGameplayTag(parentTagPath);
            GameplayTagInheritance.Instance.AddInheritance(tag, parentTag);
        }
        
        // 设置属性
        foreach (var prop in tagData.Properties)
        {
            GameplayTagInheritance.Instance.SetProperty(tag, prop.Key, prop.Value);
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
            Properties = GameplayTagInheritance.Instance.GetAllProperties(tag)
        };
        
        // 添加子标签
        foreach (var childTag in _tagManager.GetDirectChildTags(tag))
        {
            tagData.Children.Add(CreateTagData(childTag));
        }
        
        return tagData;
    }
} 
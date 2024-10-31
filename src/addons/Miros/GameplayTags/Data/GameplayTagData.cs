using System.Collections.Generic;

public class GameplayTagData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Inherits { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();
    public List<GameplayTagData> Children { get; set; } = new();
}

public class GameplayTagsConfig
{
    public string BasePath { get; set; }
    public List<GameplayTagData> Tags { get; set; } = new();
} 
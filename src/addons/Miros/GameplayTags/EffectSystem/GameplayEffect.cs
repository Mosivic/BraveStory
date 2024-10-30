public class GameplayEffect
{
    public string Name { get; }
    public GameplayTagContainer RequiredTags { get; } = new();    // 需要的标签
    public GameplayTagContainer IgnoredTags { get; } = new();     // 忽略的标签
    public GameplayTagContainer GrantedTags { get; } = new();     // 赋予的标签
    public float Duration { get; set; }                           // 持续时间
    
    public GameplayEffect(string name)
    {
        Name = name;
    }
}
namespace GPC;

public class BindableProperty<T>(T value)
{
    public T Value { get; set; } = value;
}
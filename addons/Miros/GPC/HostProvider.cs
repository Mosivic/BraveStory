namespace GPC;

public interface HostProvider<H> where H : class
{
    public H Host { get; set; }
}
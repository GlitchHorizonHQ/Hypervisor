namespace Hypervisor.Events
{
    public interface IModEvent
    {
        DateTime OccurredAtUtc { get; }
    }
}
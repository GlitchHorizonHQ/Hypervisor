using System.Runtime.InteropServices;

namespace Hypervisor.Core.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public record EventPayload
    {
        public string HookName { get; init; } = null!;
        public DateTime OccurredAtUtc { get; init; }
        public IReadOnlyDictionary<string, object> Data { get; init; } = null!;
        public bool IsCancelable { get; init; }
        public bool IsCancelled { get; set; }
    }
}

using Hypervisor.Core.Models;

namespace Hypervisor.SDK.Models
{
    public sealed class HVPayload
    {
        public string HookName { get; init; } = string.Empty;
        public string Trigger { get; init; } = string.Empty;
        public Dictionary<string, object> Data { get; init; } = new();
        public string Version { get; init; } = "1.0.0";

        public HVPayload() { }

        public HVPayload(string hookName, string trigger)
        {
            HookName = hookName;
            Trigger = trigger;
        }

        public T? GetValue<T>(string key)
        {
            if (Data.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default;
        }

        public static T Get<T>(object payload, string fieldName, T fallback)
        {
            if (payload is HVPayload p)
            {
                return p.GetValue<T>(fieldName) ?? fallback;
            }

            if (payload is EventPayload ep && ep.Data.TryGetValue(fieldName, out var val))
            {
                try
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                } catch { }
            }

            return fallback;
        }
    }
}

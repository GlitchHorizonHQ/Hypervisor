using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hypervisor.Hooks
{
    public sealed class HookContext
    {
        public string HookName { get; }
        public MethodBase Method { get; }
        public object Instance { get; }
        public object[] Arguments { get; }
        public object ReturnValue { get; }
        public Exception Exception { get; }
        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public HookContext(string hookName, MethodBase method, object instance, object[] arguments)
        {
            HookName = hookName ?? string.Empty;
            Method = method;
            Instance = instance;
            Arguments = arguments ?? Array.Empty<object>();
        }
    }
}

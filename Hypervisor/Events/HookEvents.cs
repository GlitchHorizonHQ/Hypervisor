namespace Hypervisor.Events
{
    public sealed class HookScanCompletedEvent : IModEvent
    {
        public HookScanCompletedEvent(DateTime occurredToUtc, int candiatesFound)
        {
            OccurredAtUtc = occurredToUtc;
            CandidatesFound = candiatesFound;
        }

        public DateTime OccurredAtUtc { get; }
        public int CandidatesFound { get; }
    }

    public sealed class HookInstallCompletedEvent : IModEvent
    {
        public HookInstallCompletedEvent(DateTime occurredAtUtc, int installed, int failed)
        {
            OccurredAtUtc = occurredAtUtc;
            Installed = installed;
            Failed = failed;
        }

        public DateTime OccurredAtUtc { get; }
        public int Installed { get; }
        public int Failed { get; }
    }

    public sealed class HookTriggeredEvent : IModEvent
    {
        public HookTriggeredEvent(DateTime occurredAtUtc, string methodName, int triggerCount)
        {
            OccurredAtUtc = occurredAtUtc;
            MethodName = methodName;
            TriggerCount = triggerCount;
        }

        public DateTime OccurredAtUtc { get; }
        public string MethodName { get; }
        public int TriggerCount { get; }
    }
}

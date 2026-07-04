using MelonLoader;

namespace Hypervisor.Plugins
{
    public static class HVRegistry
    {
        private static readonly Dictionary<string, HVPluginBase> Plugins = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object SyncRoot = new();

        /// <summary>
        /// Returns all currently registered plugins by plugin ID.
        /// </summary>
        public static IReadOnlyDictionary<string, HVPluginBase> RegisteredPlugins => Plugins;

        /// <summary>
        /// Registers a plugin instance with the framework
        /// </summary>
        /// <param name="plugin">Plugin instance to register</param>
        public static void RegisterPlugin(HVPluginBase plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (string.IsNullOrWhiteSpace(plugin.PluginId))
                throw new ArgumentException("PluginId must not be empty", nameof(plugin));

            lock (SyncRoot)
            {
                if (Plugins.TryGetValue(plugin.PluginId, out HVPluginBase existing))
                {
                    if (ReferenceEquals(existing, plugin))
                        return;

                    MelonLogger.Warning($"HVReegistry duplicate PluginId '{plugin.PluginId}' ignored");
                    return;
                }

                Plugins[plugin.PluginId] = plugin;
            }

            MelonLogger.Msg($"HVRegistry registered plugin: {plugin.PluginId} ({plugin.PluginName})");
        }

        /// <summary>
        /// Notifies all compatible plugins that core initialization is complete.
        /// </summary>
        public static void NotifyFrameworkReady()
        {
            HVPluginBase[] snapshot;

            lock (SyncRoot)
            {
                snapshot = new HVPluginBase[Plugins.Count];
                Plugins.Values.CopyTo(snapshot, 0);
            }

            Version currentVersion = GetCurrentFrameworkVersion();

            for (int pluginIndex = 0; pluginIndex < snapshot.Length; pluginIndex++)
            {
                HVPluginBase plugin = snapshot[pluginIndex];

                if (plugin == null)
                    continue;

                if (plugin.RequiredFrameworkVersion > currentVersion)
                {
                    MelonLogger.Warning($"HVRegistry skipped plugin '{plugin.PluginId}' due to required version {plugin.RequiredFrameworkVersion} > {currentVersion}.");
                    continue;
                }

                try
                {
                    plugin.OnFrameworkReady();
                    MelonLogger.Msg($"HVRegistry framework-ready delivered: {plugin.PluginId}");
                }
                catch (Exception exception)
                {
                    MelonLogger.Error($"HVRegistry plugin '{plugin.PluginId}' failed in OnFrameworkReady: {exception.Message}");
                }
            }
        }

        private static Version GetCurrentFrameworkVersion()
        {
            string versionString = ReleaseVersion.Current;

            return Version.TryParse(versionString, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
        }
    }
}

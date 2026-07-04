using MelonLoader;

namespace Hypervisor.Plugins
{
    public abstract class HVPluginBase : MelonMod
    {
        /// <summary>
        /// Gets the plugin's unique identifier
        /// </summary>
        public abstract string PluginId { get; }
        
        /// <summary>
        /// Gets the plugin's human-readable plugin name
        /// </summary>
        public abstract string PluginName { get; }

        /// <summary>
        /// Gets the minimum required framework version
        /// </summary>
        public abstract Version RequiredFrameworkVersion { get; }

        /// <summary>
        /// Called after the framework core has fully initialized and validated plugin compatibility
        /// </summary>
        public abstract void OnFrameworkReady();

        /// <summary>
        /// Registers the plugin with the central framework registry
        /// </summary>
        public override void OnInitializeMelon()
        {
            HVRegistry.RegisterPlugin(this);
        }
    }
}

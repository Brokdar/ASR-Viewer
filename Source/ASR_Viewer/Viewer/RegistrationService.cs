using System;
using System.Collections.Generic;
using Prism.Regions;
using Shared;

namespace Viewer
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegionManager _regionManager;

        private readonly IList<IPlugin> _plugins = new List<IPlugin>();
        public IEnumerable<IPlugin> Plugins => _plugins;

        public event EventHandler<NewRegistrationArgs> NewRegistration; 

        public RegistrationService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Register(IPlugin plugin, Type view)
        {
            _plugins.Add(plugin);
            _regionManager.RegisterViewWithRegion("ModuleRegion", view);
            OnNewRegistration(plugin);
        }

        protected virtual void OnNewRegistration(IPlugin plugin)
        {
            NewRegistration?.Invoke(this, new NewRegistrationArgs {Plugin = plugin});
        }
    }

    public class NewRegistrationArgs : EventArgs
    {
        public IPlugin Plugin { get; set; }
    }
}
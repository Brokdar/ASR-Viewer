using System;
using System.Collections.Generic;
using Prism.Regions;
using Shared;

namespace Viewer
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegionManager _regionManager;

        private readonly List<IPlugin> _plugins = new List<IPlugin>();
        public IEnumerable<IPlugin> Plugins => _plugins;

        public event EventHandler NewRegistration; 

        public RegistrationService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Register(IPlugin plugin, Type view)
        {
            _plugins.Add(plugin);
            _regionManager.RegisterViewWithRegion("ModuleRegion", view);
            OnNewRegistration();
        }

        protected virtual void OnNewRegistration()
        {
            NewRegistration?.Invoke(this, EventArgs.Empty);
        }
    }
}
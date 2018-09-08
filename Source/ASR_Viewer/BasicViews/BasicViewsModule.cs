using BasicViews.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Shared;

namespace BasicViews
{
    public class BasicViewsModule : IPlugin
    {
        private readonly IRegistrationService _registrationService;
        private readonly IUnityContainer _container;

        public string Name { get; }
        public string Symbol { get; }
        public string View { get; }

        public BasicViewsModule(IRegistrationService registrationService, IUnityContainer container)
        {
            _registrationService = registrationService;
            _container = container;

            Name = "Basic";
            Symbol = "\uEA8A";
            View = "Overview";
        }

        public void Initialize()
        {
            _registrationService.Register(this, typeof(Overview));
            _container.RegisterTypeForNavigation<Overview>();
        }
    }
}

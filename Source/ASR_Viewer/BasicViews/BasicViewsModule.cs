using BasicViews.Views;
using Shared;

namespace BasicViews
{
    public class BasicViewsModule : IPlugin
    {
        private readonly IRegistrationService _registrationService;

        public string Name { get; }
        public string Symbol { get; }

        public BasicViewsModule(IRegistrationService registrationService)
        {
            _registrationService = registrationService;

            Name = "Overview";
            Symbol = "\uEA8A";
        }

        public void Initialize()
        {
            _registrationService.Register(this, typeof(Overview));
        }
    }
}

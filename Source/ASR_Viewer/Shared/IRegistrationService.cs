using System;
using System.Collections.Generic;

namespace Shared
{
    public interface IRegistrationService
    {
        IEnumerable<IPlugin> Plugins { get; }

        void Register(IPlugin plugin, Type view);
    }
}
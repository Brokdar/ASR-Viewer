using Prism.Modularity;

namespace Shared
{
    public interface IPlugin : IModule
    {
        string Name { get; }
        string Symbol { get; }
    }
}
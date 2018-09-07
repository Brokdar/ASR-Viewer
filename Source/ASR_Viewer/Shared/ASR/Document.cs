using System.Collections.Generic;
using System.Xml.Linq;

namespace Shared.ASR
{
    public class Document
    {
        public class Information
        {
            public string Name { get; }
            public string Path { get; }

            public Information(string name, string path)
            {
                Name = name;
                Path = path;
            }
        }

        public Information Info { get; }
        private readonly Dictionary<string, Package> _packages = new Dictionary<string, Package>();
        public IEnumerable<Package> Packages => _packages.Values;
        public XElement Root { get; }

        public Document(Information info, XElement root, IEnumerable<Package> packages)
        {
            Info = info;
            Root = root;

            foreach (var package in packages)
            {
                _packages.Add(package.Name, package);
            }
        }

        public Package GetPackage(string name)
        {
            if (_packages.TryGetValue(name, out var package))
            {
                return package;
            }

            throw new KeyNotFoundException($"No AR-Package with {name} found.");
        }
    }
}
using System.Xml.Linq;

namespace Shared.ASR
{
    public class Package
    {
        public string Uuid { get; }
        public string Name { get; }
        public XElement Element { get; }

        public Package(string uuid, string name, XElement element)
        {
            Uuid = uuid;
            Name = name;
            Element = element;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Shared.ASR;

namespace Reader
{
    public class AsrReader
    {
        public Document Read(string pathToFile)
        {
            if (!Path.HasExtension(pathToFile))
                throw new ArgumentException($"{pathToFile} is not a file.");

            if (".arxml" != Path.GetExtension(pathToFile))
                throw new ArgumentException($"{Path.GetFileNameWithoutExtension(pathToFile)} is not of type '.arxml'.", pathToFile);

            var xDoc = XDocument.Load(pathToFile);
            var root = xDoc.Root;
            var info = ExtractDocumentInformation(pathToFile);
            var packages = ExtractArPackages(root);
            var asrDoc = new Document(info, root, packages);

            return asrDoc;
        }

        private static IEnumerable<Package> ExtractArPackages(XElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            var packages = root.Elements().First(element => element.Name.LocalName == "AR-PACKAGES");

            return (from package in packages.Elements()
                let childs = package.Elements().ToArray()
                let uuid = package.FirstAttribute.Value
                let name = childs.First().Value
                let element = childs.Last()
                select new Package(uuid, name, element)).ToList();
        }

        private static Document.Information ExtractDocumentInformation(string path)
        {
            return new Document.Information(Path.GetFileName(path), Path.GetFullPath(path));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using BasicViews.ViewModels;

namespace BasicViews.Services
{
    public static class XSearchService
    {
        public static XElementViewModel FindElementByName(XElementViewModel source, string name)
        {
            if (source.Name == name)
                return source;

            var queue = new Queue<XElementViewModel>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();

                if (element.Name == name)
                    return element;

                foreach (var e in element.Elements)
                {
                    queue.Enqueue(e);
                }
            }

            return null;
        }

        public static XElementViewModel FindElementByUuid(XElementViewModel source, string uuid)
        {
            if (ContainsAttributeOfValue(source, "UUID", uuid))
                return source;

            var queue = new Queue<XElementViewModel>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();

                if (ContainsAttributeOfValue(element, "UUID", uuid))
                    return element;

                foreach (var e in element.Elements)
                {
                    queue.Enqueue(e);
                }
            }

            return null;
        }

        private static bool ContainsAttributeOfValue(XElementViewModel element, string attribute, string value)
        {
            var xAttribute = element.Attribute(attribute);
            return xAttribute != null && xAttribute.Value == value;
        }

        public static XElementViewModel FindElementByValue(XElementViewModel source, string value)
        {
            if (source.Value == value)
                return source;

            var queue = new Queue<XElementViewModel>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();

                if (element.Value == value)
                    return element;

                foreach (var e in element.Elements)
                {
                    queue.Enqueue(e);
                }
            }

            return null;
        }

        public static XElementViewModel FindElementByUri(XElementViewModel source, string uri)
        {
            if (uri.StartsWith("/"))
                uri = uri.TrimStart('/');

            var path = uri.Split('/');

            var node = source;
            for (var i = 0; i < path.Length - 1; i++)
            {
                node = FindElementByValue(node, path[i]);
                if (node == null)
                    return null;
                node = node.Parent;
            }

            node = FindElementByValue(node, path.Last());

            return node?.Parent;
        }

        public static XElementViewModel[] GetPathFromRootTo(XElementViewModel element)
        {
            var path = new List<XElementViewModel> { element };

            while (element.Parent != null)
            {
                path.Add(element.Parent);
                element = element.Parent;
            }

            path.Reverse();

            return path.ToArray();
        }
    }
}
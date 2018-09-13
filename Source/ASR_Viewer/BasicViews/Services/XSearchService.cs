using System.Collections.Generic;
using BasicViews.ViewModels;

namespace BasicViews.Services
{
    public static class XSearchService
    {
        public static XElementViewModel FindElementByName(XElementViewModel source, string name)
        {
            if (source.Name == name)
                return source;

            foreach (var element in source.Elements)
            {
                var result = FindElementByName(element, name);
                if (result != null)
                    return result;
            }

            return null;
        }

        public static XElementViewModel FindElementByAttribute(XElementViewModel source, string attribute, string value)
        {
            var xAttribute = source.Attribute(attribute);
            if (xAttribute != null && xAttribute.Value == value)
                return source;

            foreach (var element in source.Elements)
            {
                var result = FindElementByAttribute(element, attribute, value);
                if (result != null)
                    return result;
            }

            return null;
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
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Prism.Commands;
using Prism.Mvvm;

namespace BasicViews.ViewModels
{
    public class XElementViewModel : BindableBase
    {
        private readonly XElement _element;

        public string Name => _element.Name.LocalName;
        public string Value => _element.HasElements ? null : _element.Value;
        public XElementViewModel Parent { get; }

        public ReadOnlyCollection<XAttribute> Attributes { get; }
        public bool HasAttributes => Attributes.Count > 0;
        public ReadOnlyCollection<XElementViewModel> Elements { get; }
        public bool HasElements => Elements.Count > 0;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if(_isSelected != value)
                    SetProperty(ref _isSelected, value);
            } 
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                    SetProperty(ref _isExpanded, value);
            } 
        }

        private bool _isReference;
        public bool IsReference
        {
            get => _isReference;
            set => SetProperty(ref _isReference, value);
        }

        public XElementViewModel(XElement element, XElementViewModel parent = null)
        {
            _element = element;
            Parent = parent;

            Attributes = new ReadOnlyCollection<XAttribute>(_element.Attributes().ToList());

            Elements = new ReadOnlyCollection<XElementViewModel>(
                    (from child in _element.Elements()
                     select new XElementViewModel(child, this)).ToList()
                );

            if (!element.HasElements && Name.EndsWith("REF"))
            {
                IsReference = true;
            }
        }

        public XElementViewModel Element(string name)
        {
            foreach (var element in Elements)
            {
                if(element.Name == name)
                    return element;
            }

            return null;
        }

        public XAttribute Attribute(string name)
        {
            foreach (var attribute in Attributes)
            {
                if (attribute.Name.LocalName == name)
                    return attribute;
            }

            return null;
        }
    }
}
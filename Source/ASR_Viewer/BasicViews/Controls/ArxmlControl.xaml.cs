using System.Windows;
using System.Xml.Linq;

namespace BasicViews.Controls
{
    /// <summary>
    /// Interaktionslogik für ArxmlControl.xaml
    /// </summary>
    public partial class ArxmlControl
    {
        public ArxmlControl()
        {
            InitializeComponent();
        }

        public XElement ItemSource
        {
            get => (XElement)GetValue(IteItemSourceProperty);
            set => SetValue(IteItemSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for IteItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IteItemSourceProperty =
            DependencyProperty.Register("IteItemSource", typeof(XElement), typeof(ArxmlControl), new PropertyMetadata(0));
    }
}

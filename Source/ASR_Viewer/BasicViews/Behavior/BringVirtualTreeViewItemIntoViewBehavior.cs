using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace BasicViews.Behavior
{
    /// <summary>
    /// Class implements an attached behaviour to bring a selected TreeViewItem in VIRTUALIZED TreeView
    /// into view when selection is driven by the viewmodel (not the user). The System.Windows.Interactivity
    /// library is required for this behavior to compile.
    /// 
    /// Sample Usage:
    /// &lt;i:Interaction.Behaviors>
    ///     &lt;behav:BringVirtualTreeViewItemIntoViewBehavior SelectedItem = "{Binding SelectPathItem}" />
    /// &lt;/ i:Interaction.Behaviors>
    /// 
    /// This behaviour requires a binding to a path like structure of tree view (viewmodel) items.
    /// This implementation requieres an array of objects (object[] SelectedItem) that represents
    /// each tree view item along the path that should be browsed with this behaviour.
    /// 
    /// The <see cref="OnSelectedItemChanged"/> method executes when the bound property has changed.
    /// The behavior browses then along the given path and ensures that all requested items exist
    /// even if we are using a virtual tree.
    /// 
    /// Allows two-way binding of a TreeView's selected item.
    /// Sources:
    /// http://stackoverflow.com/q/183636/46635
    /// http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
    /// </summary>
    internal class BringVirtualTreeViewItemIntoViewBehavior : Behavior<TreeView>
    {
        public object[] SelectedItem
        {
            get => (object[])GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
                typeof(object[]),
                typeof(BringVirtualTreeViewItemIntoViewBehavior),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        /// <summary>
        /// This method is invoked when the value bound at the dependency
        /// property <see cref="SelectedItem"/> has changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Sanity check: Are we looking at the least required data we need?
            if (!(e.NewValue is object[] newNode))
                return;

            if (newNode.Length <= 1)
                return;

            // params look good so lets find the attached tree view (aka ItemsControl)
            var behavior = d as BringVirtualTreeViewItemIntoViewBehavior;
            var tree = behavior?.AssociatedObject;
            var currentParent = (ItemsControl) tree;
            
            // Now loop through each item in the array of bound path items and make sure they exist
            for (var i = 0; i < newNode.Length; i++)
            {
                var node = newNode[i];

                // first try the easy way
                if (!(currentParent.ItemContainerGenerator.ContainerFromItem(node) is TreeViewItem newParent))
                {
                    // if this failed, it's probably because of virtualization, and we will have to do it the hard way.
                    // this code is influenced by TreeViewItem.ExpandRecursive decompiled code, and the MSDN sample at http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
                    // see also the question at http://stackoverflow.com/q/183636/46635
                    currentParent.ApplyTemplate();
                    var itemsPresenter = (ItemsPresenter)currentParent.Template.FindName("ItemsHost", currentParent);
                    if (itemsPresenter != null)
                        itemsPresenter.ApplyTemplate();
                    else
                        currentParent.UpdateLayout();

                    var virtualizingPanel = GetItemsHost(currentParent) as VirtualizingPanel;

                    CallEnsureGenerator(virtualizingPanel);
                    var index = currentParent.Items.IndexOf(node);
                    if (index < 0)
                    {
                        // This is raised when the item in the path array is not part of the tree collection
                        // This can be tricky, because Binding an ObservableDictionary to the treeview will
                        // require that we need an array of KeyValuePairs<K,T>[] here :-(
                        throw new InvalidOperationException("Node '" + node + "' cannot be fount in container");
                    }
                    virtualizingPanel?.BringIndexIntoViewPublic(index);
                    newParent = currentParent.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
                }

                if (newParent == null)
                {
                    throw new InvalidOperationException("Tree view item cannot be found or created for node '" + node + "'");
                }

                if (node == newNode[newNode.Length - 1])
                {
                    newParent.IsSelected = true;

                    var scroller = (ScrollViewer)FindVisualChildElement(tree, typeof(ScrollViewer));
                    scroller.ScrollToBottom();

                    newParent.BringIntoView();
                    break;
                }

                // Make sure nodes (except for last child node) are expanded to make children visible
                if (i < newNode.Length - 1)
                    newParent.IsExpanded = true;

                currentParent = newParent;
            }
        }

        private static FrameworkElement FindVisualChildElement(DependencyObject element, Type childType)
        {
            var count = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < count; i++)
            {
                var dependencyObject = VisualTreeHelper.GetChild(element, i);
                var fe = (FrameworkElement)dependencyObject;

                if (fe.GetType() == childType)
                {
                    return fe;
                }

                var ret = fe.GetType() == typeof(ScrollViewer) ? 
                    FindVisualChildElement((fe as ScrollViewer)?.Content as FrameworkElement, childType) : 
                    FindVisualChildElement(fe, childType);

                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue as object[];
        }

        #region Functions to get internal members using reflection
        // Some functionality we need is hidden in internal members, so we use reflection to get them
        #region ItemsControl.ItemsHost

        private static readonly PropertyInfo ItemsHostPropertyInfo = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Panel GetItemsHost(ItemsControl itemsControl)
        {
            Debug.Assert(itemsControl != null);
            return ItemsHostPropertyInfo.GetValue(itemsControl, null) as Panel;
        }

        #endregion ItemsControl.ItemsHost

        #region Panel.EnsureGenerator
        private static readonly MethodInfo EnsureGeneratorMethodInfo = typeof(Panel).GetMethod("EnsureGenerator", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void CallEnsureGenerator(Panel panel)
        {
            Debug.Assert(panel != null);
            EnsureGeneratorMethodInfo.Invoke(panel, null);
        }
        #endregion Panel.EnsureGenerator
        #endregion Functions to get internal members using reflection
    }
}

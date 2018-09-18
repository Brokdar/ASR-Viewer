using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace BasicViews.Behavior
{
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

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Sanity check: Are we looking at the least required data we need?
            if (!(e.NewValue is object[] newNode))
                return;

            if (newNode.Length <= 1)
                return;

            // params look good so lets find the attached tree view (aka ItemsControl)
            if (!(d is BringVirtualTreeViewItemIntoViewBehavior behavior)) return;

            var tree = behavior.AssociatedObject;
            var currentParent = (ItemsControl) tree;

            // Now loop through each item in the array of bound path items and make sure they exist
            for (var i = 0; i < newNode.Length; i++)
            {
                var node = newNode[i];

                if (!(currentParent.ItemContainerGenerator.ContainerFromItem(node) is TreeViewItem newParent))
                {
                    currentParent.ApplyTemplate();
                    var itemsPresenter = (ItemsPresenter) currentParent.Template.FindName("ItemsHost", currentParent);
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

                    if (GetScrollViewer(tree) is ScrollViewer scroller)
                    {
                        scroller.ScrollToBottom();
                    }

                    newParent.Focus();
                    
                    break;
                }

                // Make sure nodes (except for last child node) are expanded to make children visible
                if (i < newNode.Length - 1)
                    newParent.IsExpanded = true;

                currentParent = newParent;
            }
        }

        private static DependencyObject GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer)
                return o;

            var count = VisualTreeHelper.GetChildrenCount(o);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
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

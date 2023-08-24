using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LLamaStack.WPF.Behaviours
{
    /// <summary>
    /// This will auto scroll a list view to the bottom as items are added.
    /// Automatically suspends if the user scrolls up, and recommences when
    /// the user scrolls to the end.
    /// </summary>
    /// <example>
    ///     <ListView sf:AutoScrollToBottomBehavior="{Binding viewModelAutoScrollFlag}" />
    /// </example>
    public class AutoScrollToBottomBehavior
    {
        /// <summary>
        /// Enumerated type to keep track of the current auto scroll status
        /// </summary>
        public enum StatusType
        {
            NotAutoScrollingToBottom,
            AutoScrollingToBottom,
            AutoScrollingToBottomButSuppressed
        }


        /// <summary>
        /// Gets the automatic scroll to bottom status.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <returns></returns>
        public static StatusType GetAutoScrollToBottomStatus(DependencyObject obj)
        {
            return (StatusType)obj.GetValue(AutoScrollToBottomStatusProperty);
        }


        /// <summary>
        /// Sets the automatic scroll to bottom status.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetAutoScrollToBottomStatus(DependencyObject obj, StatusType value)
        {
            obj.SetValue(AutoScrollToBottomStatusProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoScrollToBottomStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollToBottomStatusProperty =
            DependencyProperty.RegisterAttached(
              "AutoScrollToBottomStatus",
              typeof(StatusType),
              typeof(AutoScrollToBottomBehavior),
              new PropertyMetadata(StatusType.NotAutoScrollingToBottom, (s, e) =>
              {
                  if (s is DependencyObject viewer && e.NewValue is StatusType autoScrollToBottomStatus)
                  {
                      // Set the AutoScrollToBottom property to mirror this one

                      bool? autoScrollToBottom = autoScrollToBottomStatus switch
                      {
                          StatusType.AutoScrollingToBottom => true,
                          StatusType.NotAutoScrollingToBottom => false,
                          StatusType.AutoScrollingToBottomButSuppressed => false,
                          _ => null
                      };

                      if (autoScrollToBottom.HasValue)
                      {
                          SetAutoScrollToBottom(viewer, autoScrollToBottom.Value);
                      }

                      // Only hook/unhook for cases below, not when suspended
                      switch (autoScrollToBottomStatus)
                      {
                          case StatusType.AutoScrollingToBottom:
                              HookViewer(viewer);
                              break;
                          case StatusType.NotAutoScrollingToBottom:
                              UnhookViewer(viewer);
                              break;
                      }
                  }
              }));


        /// <summary>
        /// Gets the automatic scroll to bottom.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetAutoScrollToBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToBottomProperty);
        }


        /// <summary>
        /// Sets the automatic scroll to bottom.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAutoScrollToBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToBottomProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoScrollToBottom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollToBottomProperty =
            DependencyProperty.RegisterAttached(
              "AutoScrollToBottom",
              typeof(bool),
              typeof(AutoScrollToBottomBehavior),
              new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) =>
              {
                  if (s is DependencyObject viewer && e.NewValue is bool autoScrollToBottom)
                  {
                      // Set the AutoScrollToBottomStatus property to mirror this one
                      if (autoScrollToBottom)
                      {
                          SetAutoScrollToBottomStatus(viewer, StatusType.AutoScrollingToBottom);
                      }
                      else if (GetAutoScrollToBottomStatus(viewer) == StatusType.AutoScrollingToBottom)
                      {
                          SetAutoScrollToBottomStatus(viewer, StatusType.NotAutoScrollingToBottom);
                      }

                      // No change if autoScrollToBottom = false && viewer.AutoScrollToBottomStatus = AutoScrollToBottomStatusType.AutoScrollingToBottomButSuppressed;
                  }
              }));


        /// <summary>
        /// Gets the unhook action.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private static Action GetUnhookAction(DependencyObject obj)
        {
            return (Action)obj.GetValue(UnhookActionProperty);
        }


        /// <summary>
        /// Sets the unhook action.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        private static void SetUnhookAction(DependencyObject obj, Action value)
        {
            obj.SetValue(UnhookActionProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty UnhookActionProperty =
            DependencyProperty.RegisterAttached("UnhookAction", typeof(Action), typeof(AutoScrollToBottomBehavior), new PropertyMetadata(null));


        /// <summary>
        /// Handles the Loaded event of the ItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private static void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                itemsControl.Loaded -= ItemsControl_Loaded;
                HookViewer(itemsControl);
            }
        }

        /// <summary>
        /// Hooks the viewer.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        private static void HookViewer(DependencyObject viewer)
        {
            if (viewer is ScrollViewer scrollViewerParent)
            {
                SizeChangedEventHandler contentSizeChangedEventHandler = (s, e) =>
                {
                    if (GetAutoScrollToBottom(viewer))
                        scrollViewerParent.ScrollToBottom();
                };

                EventHandler scrollViewerParentInitialized = (sender, b) =>
                {
                    if (scrollViewerParent.Content is FrameworkElement contentElement)
                        contentElement.SizeChanged += contentSizeChangedEventHandler;

                };
                scrollViewerParent.Initialized += scrollViewerParentInitialized;

                Action unhookAction = () =>
                {
                    if (scrollViewerParent.Content is FrameworkElement contentElement)
                        contentElement.SizeChanged -= contentSizeChangedEventHandler;

                    scrollViewerParent.Initialized -= scrollViewerParentInitialized;
                };

                SetUnhookAction(viewer, unhookAction);
            }
            else if (viewer is ItemsControl itemsControl)
            {
                // If this is triggered the xaml setup then the control won't be loaded yet,
                // and so won't have a visual tree which we need to get the scrollviewer,
                // so defer this hooking until the items control is loaded.
                if (!itemsControl.IsLoaded)
                {
                    itemsControl.Loaded += ItemsControl_Loaded;
                    return;
                }

                if (FindScrollViewer(viewer) is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToBottom();

                    PropertyChangedEventHandler itemPropertyChangedHandler = (s, p) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            if (GetAutoScrollToBottom(viewer))
                                scrollViewer.ScrollToBottom();
                        }));
                    };


                    // Scroll to bottom when the item count changes
                    NotifyCollectionChangedEventHandler itemsCollectionChangedHandler = (s, e) =>
                    {
                        foreach (var item in itemsControl.Items)
                        {
                            if (item is INotifyPropertyChanged oldProperty)
                            {
                                oldProperty.PropertyChanged -= itemPropertyChangedHandler;
                            }
                        }

                        scrollViewer.ScrollToBottom();
                        if (e.NewItems?[0] is INotifyPropertyChanged newProperty)
                        {
                            newProperty.PropertyChanged += itemPropertyChangedHandler;
                        }
                    };
                    ((INotifyCollectionChanged)itemsControl.Items).CollectionChanged += itemsCollectionChangedHandler;


                    ScrollChangedEventHandler scrollChangedEventHandler = (s, e) =>
                    {
                        bool userScrolledToBottom = (e.VerticalOffset + e.ViewportHeight) > (e.ExtentHeight - 10.0);
                        bool userScrolledUp = e.VerticalChange < 0;

                        Debug.WriteLine($"userScrolledToBottom: {userScrolledToBottom}, userScrolledUp: {userScrolledUp}");

                        // Check if auto scrolling should be suppressed
                        if (userScrolledUp)
                        {
                            if (GetAutoScrollToBottomStatus(viewer) == StatusType.AutoScrollingToBottom)
                            {
                                SetAutoScrollToBottomStatus(viewer, StatusType.AutoScrollingToBottomButSuppressed);
                            }
                        }
                        else
                        {
                            if (GetAutoScrollToBottomStatus(viewer) == StatusType.AutoScrollingToBottomButSuppressed)
                            {
                                SetAutoScrollToBottomStatus(viewer, StatusType.AutoScrollingToBottom);
                            }
                        }
                    };

                    scrollViewer.ScrollChanged += scrollChangedEventHandler;

                    Action unhookAction = () =>
                    {
                        ((INotifyCollectionChanged)itemsControl.Items).CollectionChanged -= itemsCollectionChangedHandler;
                        scrollViewer.ScrollChanged -= scrollChangedEventHandler;
                    };

                    SetUnhookAction(viewer, unhookAction);
                }
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the ScrollViewerParent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void ScrollViewerParent_LayoutUpdated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsubscribes the event listeners on the ItemsControl and ScrollViewer
        /// </summary>
        /// <param name="viewer"></param>
        private static void UnhookViewer(DependencyObject viewer)
        {
            var unhookAction = GetUnhookAction(viewer);
            SetUnhookAction(viewer, null);
            unhookAction?.Invoke();
        }

        /// <summary>
        /// A recursive function that drills down a visual tree until a ScrollViewer is found.
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns></returns>
        private static ScrollViewer FindScrollViewer(DependencyObject viewer)
        {
            if (viewer is ScrollViewer scrollViewer)
                return scrollViewer;

            return Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(viewer))
              .Select(i => FindScrollViewer(VisualTreeHelper.GetChild(viewer, i)))
              .Where(child => child != null)
              .FirstOrDefault();
        }
    }
}

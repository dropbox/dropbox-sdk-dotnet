//-----------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace UniversalDemo.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    /// <summary>
    /// Base class for view model objects.
    /// </summary>
    public class ViewModelBase : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the application instance.
        /// </summary>
        public App App
        {
            get
            {
                return (App)Application.Current;
            }
        }

        /// <summary>
        /// Invokes an action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>An asynchronous task.</returns>
        protected async Task InvokeAsync(DispatchedHandler action)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }

        /// <summary>
        /// Notifies observers that a property value has changed.
        /// </summary>
        /// <param name="propName">Name of the property that has changed, if this
        /// is the empty string, then all properties are assumed to have changed.</param>
        protected void NotifyPropertyChanged(string propName = "")
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}

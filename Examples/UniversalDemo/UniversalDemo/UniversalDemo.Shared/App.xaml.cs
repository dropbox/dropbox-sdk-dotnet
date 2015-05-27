//-----------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace UniversalDemo
{
    using System;
    using Dropbox.Api;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Storage;
#if WINDOWS_APP
    using Windows.UI.ApplicationSettings;
#endif
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// The dropbox app key.
        /// </summary>
        /// <remarks>
        /// This can be found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.
        /// </remarks>
        public const string AppKey = "<<Enter your App Key here>>";

        /// <summary>
        /// The redirect URI
        /// </summary>
        public static readonly Uri RedirectUri = new Uri("http://localhost:5000/admin/auth");

        /// <summary>
        /// The dropbox client
        /// </summary>
        private DropboxClient dropboxClient;

#if WINDOWS_PHONE_APP
        /// <summary>
        /// The transitions
        /// </summary>
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <remarks>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </remarks>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            if (!string.IsNullOrEmpty(this.AccessToken))
            {
                this.SetNewDropboxClient();
            }
        }

        /// <summary>
        /// Occurs when [dropbox client changed].
        /// </summary>
        public event EventHandler DropboxClientChanged;

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken
        {
            get
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                object accessToken;
                if (localSettings.Values.TryGetValue("DropboxAccessToken", out accessToken))
                {
                    return accessToken.ToString();
                }

                return string.Empty;
            }

            set
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (string.IsNullOrEmpty(value))
                {
                    localSettings.Values.Remove("DropboxAccessToken");
                    this.DropboxClient = null;
                }
                else
                {
                    localSettings.Values["DropboxAccessToken"] = value;
                    this.SetNewDropboxClient();
                }
            }
        }

        /// <summary>
        /// Gets the dropbox client.
        /// </summary>
        /// <value>
        /// The dropbox client.
        /// </value>
        public DropboxClient DropboxClient
        {
            get
            {
                return this.dropboxClient;
            }

            private set
            {
                using (var old = this.dropboxClient)
                {
                    this.dropboxClient = value;
                }

                var handler = this.DropboxClientChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Raises the <see cref="E:Activated" /> event.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if (args is WebAuthenticationBrokerContinuationEventArgs)
            {
                try
                {
                    DropboxOAuth.ProcessContinuation((WebAuthenticationBrokerContinuationEventArgs)args);
                }
                catch(Exception)
                {
                    // authentication was cancelled or failed, there's no useful way to surface this
                    // information.
                }
            }
       }
#endif

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        /// <exception cref="System.Exception">Failed to create initial page</exception>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Raises the <see cref="E:WindowCreated" /> event.
        /// </summary>
        /// <param name="args">The <see cref="WindowCreatedEventArgs"/> instance containing the event data.</param>
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
#if WINDOWS_APP
            SettingsPane.GetForCurrentView().CommandsRequested += (s, e) =>
                {
                    e.Request.ApplicationCommands.Add(new SettingsCommand(
                        "Dropbox Settings",
                        "Dropbox Settings",
                        _ => { new DropboxSettings().Show(); }));
                };
#endif
            base.OnWindowCreated(args);
        }
 
        /// <summary>
        /// Sets the new dropbox client.
        /// </summary>
        private void SetNewDropboxClient()
        {
            this.DropboxClient = new DropboxClient(
                oauth2AccessToken: this.AccessToken,
                userAgent: "WindowsUniversalAppDemo");
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
   }
}

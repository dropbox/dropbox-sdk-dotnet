namespace UniversalDemo
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// The main application page.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The swipe manager, this manages the swipe manipulation.
        /// </summary>
        private SwipeManager swipeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            var imageSet = new ViewModel.AppImageSet();
            this.DataContext = imageSet;
            this.swipeManager = new SwipeManager(this.PageRoot, imageSet);

            this.Loaded += this.OnLoaded;
            this.SizeChanged += (s, e) => this.OnLoaded(s, e);

            imageSet.App.DropboxClientChanged += this.OnDropboxClientChanged;
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code.
        /// The event data is representative of the pending navigation that will load the
        /// current Page. Usually the most relevant property to examine is Parameter.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.OnDropboxClientChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the dropbox client changes, this allows the UI to reflect changes in 
        /// the connection state of the app.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnDropboxClientChanged(object sender, EventArgs e)
        {
            var app = (App)Application.Current;
            if (string.IsNullOrEmpty(app.AccessToken))
            {
                this.NotConnected.Visibility = Visibility.Visible;
                this.RefreshButton.IsEnabled = false;
            }
            else
            {
                this.NotConnected.Visibility = Visibility.Collapsed;
                this.RefreshButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Called when the app is loaded or when the size changes, this keeps the
        /// previous and next images off screen.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var prevTransform = (TranslateTransform)this.PreviousHolder.RenderTransform;
            prevTransform.X = -this.PageRoot.ActualWidth;

            var nextTransform = (TranslateTransform)this.NextHolder.RenderTransform;
            nextTransform.X = this.PageRoot.ActualWidth;
        }

        /// <summary>
        /// Handles the Click event on the setting app bar button, this displays the 
        /// <see cref="SettingsDialog"/> used to manage the Dropbox connection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SettingsClick(object sender, RoutedEventArgs args)
        {
            var settings = new SettingsDialog();
            var result = await settings.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var app = (App)Application.Current;
                if (string.IsNullOrEmpty(app.AccessToken))
                {
                    DropboxOAuth.AuthorizeAndContinue();
                }
                else
                {
                    app.AccessToken = string.Empty;
                    this.NotConnected.Visibility = Visibility.Visible;
                    this.RefreshButton.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Refreshes the image set
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void RefreshClick(object sender, RoutedEventArgs args)
        {
            this.RefreshButton.IsEnabled = false;
            try
            {
                var imageSet = (ViewModel.AppImageSet)this.DataContext;
                await imageSet.UpdateImageSet();
            }
            finally
            {
                this.RefreshButton.IsEnabled = true;
            }
        }
    }
}

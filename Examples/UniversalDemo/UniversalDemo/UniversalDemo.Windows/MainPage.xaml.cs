namespace UniversalDemo
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

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

            var imageSet = new ViewModel.AppImageSet();
            this.DataContext = imageSet;
            this.swipeManager = new SwipeManager(this.ImageHolder, imageSet);

            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnLoaded;
        }


        /// <summary>
        /// Called when the app is loaded or when the size changes, this keeps the
        /// previous and next images off screen.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var nextTransform = (TranslateTransform)this.NextHolder.RenderTransform;
            nextTransform.X = this.RootGrid.ActualWidth;

            var prevTransform = (TranslateTransform)this.PrevHolder.RenderTransform;
            prevTransform.X = -this.RootGrid.ActualWidth;
        }
    }
}

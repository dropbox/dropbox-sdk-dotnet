//-----------------------------------------------------------------------------
// <copyright file="DropboxSettings.xaml.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace UniversalDemo
{
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// The settings flyout for managing the connection to Dropbox. 
    /// </summary>
    public sealed partial class DropboxSettings : SettingsFlyout
    {
        /// <summary>
        /// The application instance.
        /// </summary>
        private App app = Application.Current as App;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxSettings"/> class.
        /// </summary>
        public DropboxSettings()
        {
            this.InitializeComponent();

            var task = this.CheckConnection();
        }

        /// <summary>
        /// Checks if the app is currently connected.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task CheckConnection()
        {
            if (string.IsNullOrEmpty(this.app.AccessToken))
            {
                this.SetDisconnected();
            }
            else
            {
                await this.SetConnected();
            }
        }

        /// <summary>
        /// Sets the UI state to reflect that the app is connected.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task SetConnected()
        {
            ConnectBtn.Visibility = Visibility.Collapsed;
            DisconnectBtn.Visibility = Visibility.Visible;

            var client = this.app.DropboxClient;
            var account = await client.Users.GetCurrentAccountAsync();

            ConnectText.Text = string.Format(
                "You are currently connected to Dropbox as {0}",
                account.Name.DisplayName);
        }

        /// <summary>
        /// Sets the UI state to reflect that the app is not connected.
        /// </summary>
        private void SetDisconnected()
        {
            ConnectText.Text = "You are not currently connected to Dropbox.";
            ConnectBtn.Visibility = Visibility.Visible;
            DisconnectBtn.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Click event of the Connect button, this initiates authenticating with
        /// Dropbox and authorizing the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await DropboxOAuth.Authorize();
                await this.SetConnected();
            }
            catch (Exception)
            {
                this.SetDisconnected();
            }
        }

        /// <summary>
        /// Handles the Click event of the Disconnect button, this sets the AccessToken to the
        /// empty string, which disconnects the application from Dropbox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            this.app.AccessToken = string.Empty;

            this.SetDisconnected();
        }
    }
}

//-----------------------------------------------------------------------------
// <copyright file="SettingsDialog.xaml.cs" company="Dropbox Inc">
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
    /// This dialog manages the Dropbox connection and presents UI to allow the user
    /// to choose to connect or disconnect
    /// </summary>
    public sealed partial class SettingsDialog : ContentDialog
    {
        /// <summary>
        /// The application instance.
        /// </summary>
        private App app = (App)Application.Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDialog"/> class.
        /// </summary>
        public SettingsDialog()
        {
            this.InitializeComponent();
            var task = this.CheckConnection();
        }

        /// <summary>
        /// Checks the if the app is currently connected and sets the UI to reflect
        /// the connection state.
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
        /// Sets the UI to reflect that the app is currently.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task SetConnected()
        {
            this.PrimaryButtonText = "disconnect";
            var client = this.app.DropboxClient;
            var account = await client.Users.GetCurrentAccountAsync();

            this.ConnectText.Text = string.Format(
                "You are currently connected to Dropbox as {0}",
                account.Name.DisplayName);
        }

        /// <summary>
        /// Sets the UI state to reflect that the app is not connected.
        /// </summary>
        private void SetDisconnected()
        {
            this.PrimaryButtonText = "connect";
            this.ConnectText.Text = "You are not currently connected to Dropbox.";
        }
    }
}

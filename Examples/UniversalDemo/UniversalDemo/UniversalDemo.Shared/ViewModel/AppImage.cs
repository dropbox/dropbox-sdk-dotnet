namespace UniversalDemo.ViewModel
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The view model for an image in the user's Dropbox.
    /// </summary>
    public class AppImage : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppImage"/> class.
        /// </summary>
        /// <param name="name">The file name of the image.</param>
        /// <param name="rev">The revision of the image file.</param>
        public AppImage(string name, string rev)
        {
            this.Name = name;
            this.Rev = rev;
        }

        /// <summary>
        /// Gets the file name of this image.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the current revision of this image.
        /// </summary>
        public string Rev { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this image is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Gets this file as an image.
        /// </summary>
        public ImageSource Image { get; private set; }

        /// <summary>
        /// Fetches the image from Dropbox if necessary.
        /// </summary>
        /// <param name="replaceImage">If set to <c>true</c> then replace an existing image.</param>
        /// <returns>
        /// An asynchronous task.
        /// </returns>
        public async Task Update(bool replaceImage = false)
        {
            if (this.IsLoading ||
                (!replaceImage && this.Image != null))
            {
                return;
            }

            var client = this.App.DropboxClient;
            if (client == null)
            {
                return;
            }

            this.IsLoading = true;
            this.NotifyPropertyChanged("IsLoading");
            try
            {
                using (var download = await client.Files.DownloadAsync("/" + this.Name))
                {
                    var stream = await download.GetContentAsStreamAsync();
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                    this.Image = bitmap;
                    this.NotifyPropertyChanged("Image");

                    if (download.Response.Rev != this.Rev)
                    {
                        this.Rev = download.Response.Rev;
                        this.NotifyPropertyChanged("Rev");
                    }
                }
            }
            finally
            {
                this.IsLoading = false;
                this.NotifyPropertyChanged("IsLoading");
            }
        }

        /// <summary>
        /// Updates the revision of this image. If the revision has changed,
        /// then this will cause an image update.
        /// </summary>
        /// <param name="rev">The rev.</param>
        public void UpdateRev(string rev)
        {
            if (this.Rev == rev)
            {
                return;
            }

            this.Rev = rev;
            this.NotifyPropertyChanged("Rev");

            var task = this.Update(replaceImage: true);
        }
    }
}

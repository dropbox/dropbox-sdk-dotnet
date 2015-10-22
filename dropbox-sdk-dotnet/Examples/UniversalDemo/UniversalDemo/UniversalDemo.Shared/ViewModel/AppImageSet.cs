namespace UniversalDemo.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The view model for the set of images in the user's Dropbox.
    /// </summary>
    public class AppImageSet : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppImageSet"/> class.
        /// </summary>
        public AppImageSet()
        {
            this.App.DropboxClientChanged += this.AppDropboxClientChanged;

            this.Images = new ObservableCollection<AppImage>();

            var task = this.UpdateImageSet();
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        public ObservableCollection<AppImage> Images { get; private set; }

        /// <summary>
        /// Gets the current image.
        /// </summary>
        public AppImage CurrentImage { get; private set; }

        /// <summary>
        /// Gets the next image.
        /// </summary>
        public AppImage NextImage { get; private set; }

        /// <summary>
        /// Gets the previous image.
        /// </summary>
        public AppImage PreviousImage { get; private set; }

        /// <summary>
        /// Moves to the next image.
        /// </summary>
        public void MoveToNextImage()
        {
            this.DoChangeImage(1);
        }

        /// <summary>
        /// Moves to the previous image.
        /// </summary>
        public void MoveToPreviousImage()
        {
            this.DoChangeImage(-1);
        }

        /// <summary>
        /// Updates the image set.
        /// </summary>
        /// <remarks>
        /// This fetches the list of files from Dropbox and, for each file that is an image
        /// ensures that it is in the <see cref="Images"/> property. This method contains
        /// logic to handle removing and updating files that have been deleted or updated
        /// respectively.
        /// </remarks>
        /// <returns>An asynchronous task.</returns>
        public async Task UpdateImageSet()
        {
            var initiallyEmpty = this.Images.Count == 0;

            var client = this.App.DropboxClient;
            if (client == null)
            {
                return;
            }

            var list = await client.Files.ListFolderAsync(string.Empty);
            var previous = this.Images.Select(i => i.Name).ToList();

            foreach (var entry in list.Entries.Where(e => e.IsFile))
            {
                var ext = Path.GetExtension(entry.Name).ToLowerInvariant();

                if (!IsImageExtension(ext))
                {
                    continue;
                }

                var rev = entry.AsFile.Rev;
                var existing = this.Images.FirstOrDefault(i => i.Name == entry.Name);

                if (existing == null)
                {
                    var image = new AppImage(entry.Name, rev);
                    this.Images.Add(image);
                }
                else if (existing.Rev != entry.AsFile.Rev)
                {
                    existing.UpdateRev(rev);
                }

                previous.Remove(entry.Name);
            }

            if (initiallyEmpty && this.Images.Count > 0)
            {
                this.DoChangeImage(0);
            }
            else if (previous.Count > 0)
            {
                foreach (var name in previous)
                {
                    var existing = this.Images.FirstOrDefault(i => i.Name == name);
                    if (existing != null)
                    {
                        this.Images.Remove(existing);
                    }
                }

                this.DoChangeImage(0);
            }
        }

        /// <summary>
        /// Determines whether the specified extension is an image file extension.
        /// </summary>
        /// <param name="ext">The extension to check.</param>
        /// <returns><c>true</c> if <paramref name="ext"/> is the extension for an image file format;
        /// <c>false</c> otherwise.</returns>
        private static bool IsImageExtension(string ext)
        {
            return ext == ".jpg" || ext == ".png" || ext == ".gif" || ext == ".jpeg";
        }

        /// <summary>
        /// Handles the event raised when <see cref="App.DropboxClient"/> changes.
        /// </summary>
        /// <remarks>If the client is <c>null</c>, then this clears all data.</remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AppDropboxClientChanged(object sender, EventArgs args)
        {
            if (this.App.DropboxClient == null)
            {
                this.CurrentImage = null;
                this.PreviousImage = null;
                this.NextImage = null;

                this.Images.Clear();

                this.NotifyPropertyChanged();
            }
            else
            {
                var t = this.UpdateImageSet();
            }
        }

        /// <summary>
        /// Changes the currently selected image, or if no image is currently selected,
        /// selects the first.
        /// </summary>
        /// <param name="increment">The amount to change the current index by.</param>
        private void DoChangeImage(int increment)
        {
            int index = 0;
            if (this.CurrentImage == null)
            {
                if (this.Images.Count == 0)
                {
                    return;
                }
            }
            else
            {
                index = this.Images.IndexOf(this.CurrentImage);
                if (index == -1)
                {
                    index = 0;
                }

                index = (index + this.Images.Count + increment) % this.Images.Count;
            }

            this.CurrentImage = this.Images[index];
            this.NotifyPropertyChanged("CurrentImage");

            this.PreviousImage = this.Images[(index - 1 + this.Images.Count) % this.Images.Count];
            this.NotifyPropertyChanged("PreviousImage");

            this.NextImage = this.Images[(index + 1) % this.Images.Count];
            this.NotifyPropertyChanged("NextImage");

            var task = this.InvokeAsync(async () =>
            {
                await this.CurrentImage.Update();
                await this.NextImage.Update();
                await this.PreviousImage.Update();
            });
        }

     }
}

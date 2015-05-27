//-----------------------------------------------------------------------------
// <copyright file="SwipeManager.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace UniversalDemo
{
    using System;
    using UniversalDemo.ViewModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// Implements horizontal swipe manipulation.
    /// </summary>
    public class SwipeManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwipeManager"/> class.
        /// </summary>
        /// <param name="root">The root object being swiped.</param>
        /// <param name="imageSet">The image set.</param>
        public SwipeManager(FrameworkElement root, AppImageSet imageSet)
        {
            this.Root = root;
            this.ImageSet = imageSet;
           
            this.Root.ManipulationMode = ManipulationModes.TranslateX;
            this.Root.ManipulationDelta += this.OnManipulationDelta;
            this.Root.ManipulationCompleted += this.OnManipulationCompleted;
        }

        /// <summary>
        /// Gets the root object being swiped.
        /// </summary>
        public FrameworkElement Root { get; private set; }

        /// <summary>
        /// Gets the image set.
        /// </summary>
        public AppImageSet ImageSet { get; private set; }

        /// <summary>
        /// Called when the input device changes position during a manipulation.
        /// </summary>
        /// <remarks>This is used to provide feedback by moving the root object
        /// with the manipulation</remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ManipulationDeltaRoutedEventArgs"/>
        /// instance containing the event data.</param>
        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs args)
        {
            var transform = this.Root.RenderTransform as TranslateTransform;
            transform.X = args.Cumulative.Translation.X;
            args.Handled = true;
        }

        /// <summary>
        /// Called when a manipulation is complete.
        /// </summary>
        /// <remarks>
        /// If the manipulation isn't more than half the object width, then the position is reset,
        /// otherwise the direction is picked and the animation switched forward or backwards by a
        /// frame, the underlying objects are updated to complete the illusion.
        /// </remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ManipulationCompletedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs args)
        {
            var anim = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(anim, this.Root);
            Storyboard.SetTargetProperty(anim, "(Border.RenderTransform).(TranslateTransform.X)");

            var absX = Math.Abs(args.Cumulative.Translation.X);
            if (absX < this.Root.ActualWidth / 2)
            {
                // no transition, just animate back to 0
            }
            else if (args.Cumulative.Translation.X > 0)
            {
                // swiping right - move to previous
                anim.From = args.Cumulative.Translation.X - this.Root.ActualWidth;
                this.ImageSet.MoveToPreviousImage();
            }
            else
            {
                // swiping left - move to next
                anim.From = args.Cumulative.Translation.X + this.Root.ActualWidth;
                this.ImageSet.MoveToNextImage();
            }

            args.Handled = true;

            var sb = new Storyboard();
            sb.Children.Add(anim);
            sb.Begin();
        }
    }
}

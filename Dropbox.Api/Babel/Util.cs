//-----------------------------------------------------------------------------
// <copyright file="Util.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Babel
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains utility extension methods.
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// Converts a <see cref="T:System.Threading.Tasks.Task`1"/> to the APM
        /// </summary>
        /// <typeparam name="TResult">The type of the task result.</typeparam>
        /// <param name="task">The task to convert.</param>
        /// <param name="callback">The callback provided to the begin method.</param>
        /// <param name="state">
        /// The state that is passed into the <see cref="IAsyncResult"/>.</param>
        /// <returns>The <see cref="IAsyncResult"/> that will be returned by the 
        /// begin method.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if 
        /// <paramref name="task"/> is null.</exception>
        public static IAsyncResult ToApm<TResult>(
            this Task<TResult> task,
            AsyncCallback callback,
            object state = null)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            var completion = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        completion.TrySetException(t.Exception.InnerException);
                    }
                    else if (t.IsCanceled)
                    {
                        completion.TrySetCanceled();
                    }
                    else
                    {
                        completion.TrySetResult(t.Result);
                    }

                    if (callback != null)
                    {
                        callback(completion.Task);
                    }
                });

            return completion.Task;
        }

        /// <summary>
        /// Converts a <see cref="Task"/> to the APM
        /// </summary>
        /// <param name="task">The task to convert.</param>
        /// <param name="callback">The callback provided to the begin method.</param>
        /// <param name="state">
        /// The state that is passed into the <see cref="IAsyncResult"/>.</param>
        /// <returns>The <see cref="IAsyncResult"/> that will be returned by the 
        /// begin method.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if 
        /// <paramref name="task"/> is null.</exception>
        public static IAsyncResult ToApm(
            this Task task,
            AsyncCallback callback,
            object state = null)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            var completion = new TaskCompletionSource<bool>(state);
            task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        completion.TrySetException(t.Exception.InnerException);
                    }
                    else if (t.IsCanceled)
                    {
                        completion.TrySetCanceled();
                    }
                    else
                    {
                        completion.TrySetResult(true);
                    }

                    if (callback != null)
                    {
                        callback(completion.Task);
                    }
                });

            return completion.Task;
        }
    }
}

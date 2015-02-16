//-------------------------------------------------------------------------------------------
// Copyright © Microsoft Corporation, All Rights Reserved 

// Licensed under MICROSOFT SOFTWARE LICENSE TERMS, 
// MICROSOFT RIGHTS MANAGEMENT SERVICE SDK UI LIBRARIES; 
// You may not use this file except in compliance with the License. 
// See the license for specific language governing permissions and limitations. 
// You may obtain a copy of the license (RMS SDK UI libraries - EULA.DOCX) at the 
// root directory of this project. 

// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS 
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION 
// ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A 
// PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT. 
//-------------------------------------------------------------------------------------------

namespace RMSSampleApp
{
    using RMSSampleApp.Utils;
    using System;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Phone.UI.Input;
    using Windows.Storage;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        private static ContinuationManager continuationManager;

        public static ContinuationManager ContinuationManager { get; private set; }

        public event EventHandler<BackPressedEventArgs> BackPressed;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            ContinuationManager = new ContinuationManager();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
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

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                var pageToGo = typeof(UserEmailPage);
                if (!string.IsNullOrEmpty(AppUtils.GetStoredUserName()))
                {
                    pageToGo = typeof(DefaultPage);
                }

                if (!rootFrame.Navigate(pageToGo, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

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

        /// <summary>
        /// Handles the back button press and navigates through the history of the root frame.
        /// </summary>
        /// <param name="sender">The source of the event. <see cref="HardwareButtons"/></param>
        /// <param name="e">Details about the back button press.</param>
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            var handler = this.BackPressed;
            if (handler != null)
            {
                handler(sender, e);
            }

            if (frame.CanGoBack && !e.Handled)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        private Frame CreateRootFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        protected async override void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);
            Frame rootFrame = CreateRootFrame();

            //  Delete the local copies of unprotected and protected files 
            await AppUtils.CleanupFiles();

            // Ensure this is due to opening protected file
            if (args.Kind == ActivationKind.File && args.Files != null && args.Files[0] != null)
            {
                var file = args.Files[0] as IStorageFile;
                rootFrame.Navigate(typeof(ProtectedTextConsumptionPage), file);
            }

            Window.Current.Activate();
        }

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            //// It is safe to clean up camera files here, becasue they are not in use at this point
            //if ((e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser) ||
            //    (e.PreviousExecutionState == ApplicationExecutionState.Terminated))
            //{
            //    await AppUtils.CleanupFiles();
            //}

            continuationManager = new ContinuationManager();

            Frame rootFrame = CreateRootFrame();

            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                Frame scenarioFrame = Window.Current.Content as Frame;
                ContinuationManager.Continue(continuationEventArgs, scenarioFrame);
            }

            Window.Current.Activate();
        }
    }
}
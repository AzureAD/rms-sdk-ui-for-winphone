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

namespace Microsoft.RightsManagement.UI.RMSCustomControls
{
    using Microsoft.RightsManagement.UI.RMSCustomControls.Models;
    using System;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Phone.UI.Input;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServiceUrlConsentPage : Page
    {
        private string url;
        private AsyncUIOperation<ConsentUIResult> asyncOperation;

        public ServiceUrlConsentPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
        }

        public static IAsyncOperation<ConsentUIResult> ShowServiceUrlConsentPage(string url)
        {
            var context = new ServiceUrlConsentPageContext
            {
                Url = url,
                AsyncOperation = new AsyncUIOperation<ConsentUIResult>()
            };

            var currentPage = Window.Current.Content as Frame;
            var coreWindow = CoreWindow.GetForCurrentThread();
            coreWindow.Dispatcher.RunAsync(
                   CoreDispatcherPriority.Normal,
                   () => currentPage.Navigate(typeof(ServiceUrlConsentPage), context));

            return context.AsyncOperation;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
            ConsentTitleTextBlock.Text = LocalizedStrings.Get("ConsentTitle/Text");
            AcceptButton.Label = LocalizedStrings.Get("AcceptButton/Label");
            DeclineButton.Label = LocalizedStrings.Get("DeclineButton/Label");
            ShowAgainCheckBox.Content = LocalizedStrings.Get("ShowAgainCheckBox/Content");
            ConsentLicenseMessage.Text = LocalizedStrings.Get("ConsentLicenseMessage/Text");
            ConsentTrackingMessage.Text = LocalizedStrings.Get("ConsentTrackingMessage/Text");
            MessageTextBlock.Text = url;
        }

        private async void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            backPressedEventArgs.Handled = true;
            var result = new ConsentUIResult
            {
                Accepted = false,
                ShowAgain = true
            };

            if (this.asyncOperation.Status == AsyncStatus.Started)
            {
                this.asyncOperation.Complete(result);
            }

            await this.GoBack();
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var context = e.Parameter as ServiceUrlConsentPageContext;
            if (context != null)
            {
                url = context.Url;
                asyncOperation = context.AsyncOperation;
                MessageTextBlock.Text = url;
            }
        }

        private async void AcceptButtonClick(object sender, RoutedEventArgs e)
        {
            var result = new ConsentUIResult
                             {
                                 Accepted = true,
                                 ShowAgain =
                                     !(this.ShowAgainCheckBox.IsChecked.HasValue
                                       && this.ShowAgainCheckBox.IsChecked.Value)
                             };

            this.asyncOperation.Complete(result);
            await this.GoBack();
        }

        private async Task GoBack()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (this.Frame.CanGoBack) this.Frame.GoBack(); });
        }

        private async void DeclineButtonClick(object sender, RoutedEventArgs e)
        {
            var result = new ConsentUIResult
                             {
                                 Accepted = false,
                                 ShowAgain =
                                     !(this.ShowAgainCheckBox.IsChecked.HasValue
                                       && this.ShowAgainCheckBox.IsChecked.Value)
                             };

            this.asyncOperation.Complete(result);
            await this.GoBack();
        }
    }
}

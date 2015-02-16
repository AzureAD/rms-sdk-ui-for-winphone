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
    using Microsoft.RightsManagement;
    using RMSSampleApp.Utils;
    using System;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProtectedTextConsumptionPage : Page, IWebAuthenticationContinuable
    {
        private AuthenticationManager authManager;
        private ConsentManager consentManager;
        private IStorageFile file;

        public ProtectedTextConsumptionPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            ViewerControl.IsAutoDismissEnabled = false;
            ViewerControl.IsLightDismissEnabled = false;
            this.authManager = AuthenticationManagerFactory.CreateAuthenticationManager();
            this.consentManager = new ConsentManager(CoreWindow.GetForCurrentThread().Dispatcher);
        }

        /// <summary>
        /// Continue authenticating user
        /// </summary>
        public async void ContinueWebAuthentication(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            await AppUtils.ContinueWebAuthenticationHelperAsync(args, this.authManager);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            await UIEventExceptionHandler.ChainExceptionHandlerToAction(
               async () =>
               {
                   if (e.NavigationMode == NavigationMode.New)
                   {
                       file = e.Parameter as IStorageFile;

                       // Read the content of the protected file

                       IRandomAccessStream encryptedFileStream = await file.OpenAsync(FileAccessMode.Read);

                       // Consent UI will be displayed by ProtectedFileStream.AcquireAsync call, hence set WaitForCosnent 
                       // flag to true. Once user takes an action on Consent, set WaitForCosnent flag to false.
                       localSettings.Values["WaitForCosnent"] = true;

                       GetProtectedFileStreamResult result = await ProtectedFileStream.AcquireAsync(
                           encryptedFileStream,
                           this.authManager.Userid,
                           this.authManager,
                           consentManager,
                           PolicyAcquisitionOptions.None);

                       if (result == null)
                       {
                           await this.GoBack();
                       }
                       else
                       {
                           // set WaitForCosnent flag to false, as consent is shown to the user
                           localSettings.Values["WaitForCosnent"] = false;

                           if (result.Status != GetUserPolicyResultStatus.Success)
                           {
                               throw new Exception(
                                   String.Format(
                                       "Error in reading document. Your policy status is {0}",
                                       result.Status.ToString()));
                           }

                           if (result.Stream.Policy == null)
                           {
                               throw new Exception("Error in reading document");
                           }

                           ViewerControl.Policy = result.Stream.Policy;
                           if (result.Status == GetUserPolicyResultStatus.Success)
                           {
                               // Read and display the protected content
                               await DisplayText(result);
                           }
                       }
                   }
                   else if (e.NavigationMode == NavigationMode.Back)
                   {
                       // Once user takes an action on Consent, ProtectedTextConsumptionPage will be loaded again
                       // but NavigationMode will be NavigationMode.Back in this case.

                       if (localSettings.Values.ContainsKey("WaitForCosnent") &&
                                (bool)localSettings.Values["WaitForCosnent"])
                       {
                           localSettings.Values["WaitForCosnent"] = false;
                       }
                       else
                       {
                           Application.Current.Exit();
                       }
                   }
               });
        }

        private void ProtectionButtonClicked(object sender, RoutedEventArgs e)
        {
            // Open up the PolicyViewer control if not opened
            ViewerControl.IsOpen = !ViewerControl.IsOpen;
        }

        /// <summary>
        /// Read and display the protected content
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task DisplayText(GetProtectedFileStreamResult result)
        {
            using (var reader = new DataReader(result.Stream))
            {
                try
                {
                    uint actualSize = await reader.LoadAsync((uint)result.Stream.Size);
                    TextContent.Text = reader.ReadString(actualSize);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.DetachStream();
                    }
                }
            }
        }

        private async Task GoBack()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (this.Frame.CanGoBack) this.Frame.GoBack(); });
        }
    }
}

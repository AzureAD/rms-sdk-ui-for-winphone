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
    using Microsoft.RightsManagement.UI.RMSCustomControls.Models;
    using RMSSampleApp.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.Foundation;
    using Windows.Phone.UI.Input;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProtectedTextSharePage : Page, IWebAuthenticationContinuable
    {
        private AuthenticationManager authManager;
        private DataTransferManager dataTransferManager;
        private StorageFile protectedFile;
        private ProtectionScheme policy;
        private StorageFile unprotectedTextFile;

        public ProtectedTextSharePage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.authManager = AuthenticationManagerFactory.CreateAuthenticationManager();
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.dataTransferManager_DataRequested);
        }

        public async void ContinueWebAuthentication(
        Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            await AppUtils.ContinueWebAuthenticationHelperAsync(args, this.authManager);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UIEventExceptionHandler.ChainExceptionHandlerToAction(
               () =>
               {
                   ProtectionInfo protectionInfo = e.Parameter as ProtectionInfo;
                   this.policy = protectionInfo.Policy;
                   this.unprotectedTextFile = protectionInfo.UnprotectedTextFile;
               }, false);
        }

        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            try
            {
                request.Data.Properties.Title = "I have securely shared file(s) with you";
                request.Data.Properties.Description = "How to send file?";
                request.Data.Properties.ContentSourceApplicationLink = new Uri("ms-sdk-sharesource:navigate?page=DefaultPage");
                request.Data.SetText("Use RMS Sample App to view the protected file");

                var filesList = new List<StorageFile> { this.protectedFile };
                request.Data.SetStorageItems(filesList, true);
            }
            catch (Exception)
            {
                request.FailWithDisplayText("Failed to share file");
            }
        }

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Read unprotected text into buffer
                var unprotectedTextBuffer = await Windows.Storage.FileIO.ReadBufferAsync(this.unprotectedTextFile);

                // Create a protected(.ptxt) file. For data security, the .txt and .ptxt files should be deleted periodically.
                this.protectedFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    Path.GetFileNameWithoutExtension(this.unprotectedTextFile.Name) + ".ptxt");

                var userPolicy = await UserPolicy.CreateFromTemplateDescriptorAsync(
                    this.policy.Template,
                    this.authManager.Userid,
                    this.authManager,
                    UserPolicyCreationOptions.None, 
                    null);


                using (IRandomAccessStream ptxtStream = await this.protectedFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var protectedFileStream = await ProtectedFileStream.CreateAsync(userPolicy, ptxtStream, ".txt"))
                    {
                        await protectedFileStream.WriteAsync(unprotectedTextBuffer);
                        await protectedFileStream.FlushAsync();
                        await ptxtStream.FlushAsync();
                    }
                }

                // Share protected file with other applications
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ex)
            {
                // No need to exit the app in case of execption
                UIEventExceptionHandler.HandleExceptions(ex, false);
                this.GoBack();
            }
        }

        private async Task GoBack()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (this.Frame.CanGoBack) this.Frame.GoBack(); });
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;
        }

        private async void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            await this.GoBack();
        }
    }
}

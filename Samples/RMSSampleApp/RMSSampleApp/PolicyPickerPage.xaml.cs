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
    using Microsoft.RightsManagement.UI.RMSCustomControls;
    using Microsoft.RightsManagement.UI.RMSCustomControls.Models;
    using RMSSampleApp.Utils;
    using System;
    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PolicyPickerPage : Page, IWebAuthenticationContinuable
    {
        private AuthenticationManager authManager;
        private PolicyPicker picker;
        private ProtectionScheme policy;

        public PolicyPickerPage()
        {
            this.InitializeComponent();
            this.authManager = AuthenticationManagerFactory.CreateAuthenticationManager();
            this.picker = new PolicyPicker();
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await UIEventExceptionHandler.ChainExceptionHandlerToAction(
               async () =>
               {
                   var localSettings = ApplicationData.Current.LocalSettings;
                   if (e.NavigationMode == NavigationMode.New)
                   {
                       try
                       {
                           StorageFile unprotectedTextFile = e.Parameter as StorageFile;

                           
                           picker.Policy = null;

                           // Set WaitForSelectionPick flag to true as PolicyPicker is not launched yet
                           localSettings.Values["WaitForSelectionPick"] = true;

                           // Launch the PolicyPicker control
                           await picker.PickPolicyAsync(authManager.Userid, authManager, typeof(PolicyPickerPage));
                           this.policy = picker.Policy;

                           if (picker.Policy == null)
                           {
                               this.GoBack();
                           }
                           else
                           {
                               // Since user has selected the policy, reset the WaitForSelectionPick flag
                               localSettings.Values["WaitForSelectionPick"] = false;
                               this.Frame.Navigate(typeof(ProtectedTextSharePage), new ProtectionInfo(unprotectedTextFile, this.policy));
                           }
                       }
                       catch (OperationCanceledException)
                       {                        
                           this.GoBack();
                       }
                       catch (Exception ex)
                       {                           
                           UIEventExceptionHandler.HandleExceptions(ex, false);
                           this.GoBack();
                       }
                   }
                   else if (e.NavigationMode == NavigationMode.Back)
                   {
                       if (!(localSettings.Values.ContainsKey("WaitForSelectionPick")
                           && (bool)localSettings.Values["WaitForSelectionPick"]))
                       {
                           localSettings.Values["WaitForSelectionPick"] = false;
                           this.GoBack();
                       }
                   }

               }, false);
        }

        private async void GoBack()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (this.Frame.CanGoBack) this.Frame.GoBack(); });
        }
    }
}

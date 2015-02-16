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

namespace RMSSampleApp.Utils
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.RightsManagement;
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Core;
    using AuthenticationParameters = Microsoft.RightsManagement.AuthenticationParameters;

    internal class AuthenticationManager : IAuthenticationCallback
    {
        private string Authority;

        private string Clientid;

        private string Resource;

        private string Token;

        private AuthenticationResult AuthResult;

        private string Redirect;

        private CoreDispatcher Dispatcher;

        public AuthenticationContext Context { get; private set; }

        public System.Threading.Tasks.TaskCompletionSource<string> CompletionSource { get; set; }

        public string Userid { get; private set; }

        public AuthenticationManager()
        {
            // e.g. this.Clientid = "7608DFAF-F20E-58C6-82C3-08AFEE79D74E";
             this.Clientid = "[Client id for RMSSampleApp that you get from Azure AD portal e.g. 7608DFAF-F20E-58C6-82C3-08AFEE79D74E]";            

            // e.g. this.Redirect = @"https://authorize/";
            this.Redirect = "[Redirect uri for RMSSampleApp that is registered in your Azure AD tenant]";       
            
            this.Token = string.Empty;
            this.Userid = AppUtils.GetStoredUserName();
            this.Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public Windows.Foundation.IAsyncOperation<string> GetTokenAsync(AuthenticationParameters authenticationParameters)
        {
            this.Authority = authenticationParameters.Authority;
            this.Resource = authenticationParameters.Resource;
            this.Userid = authenticationParameters.UserId;
            return this.GetTokenMethod(authenticationParameters).AsAsyncOperation();
        }

        public async Task<string> GetTokenMethod(AuthenticationParameters authenticationParameters)
        {
            await this.GetToken();

            if (string.IsNullOrWhiteSpace(this.Token)) throw new Exception("Authentication ErrorText");

            return this.Token;
        }

        public void OnAuthentication(AuthenticationResult result)
        {
            this.AuthResult = result;
        }

        public async Task GetToken()
        {
            this.Context = await AuthenticationContext.CreateAsync(this.Authority, false);

            // Try to get the token silently.
            var result = await this.Context.AcquireTokenSilentAsync(
                this.Resource,
                this.Clientid);

            if (!this.UpdateAuthenticationResult(result))
            {
                this.CompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.None);
                await
                    this.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () => this.Context.AcquireTokenAndContinue(
                            this.Resource,
                            this.Clientid,
                            new Uri(this.Redirect),
                            null));

                await this.CompletionSource.Task;
            }
        }

        public bool UpdateAuthenticationResult(AuthenticationResult result)
        {
            if (result == null || result.Status != AuthenticationStatus.Success)
            {
                return false;
            }

            if (result.UserInfo != null)
            {
                this.Userid = result.UserInfo.DisplayableId ?? result.UserInfo.UniqueId;
                AppUtils.StoreUserName(this.Userid);
            }

            this.Token = result.AccessToken;
            return true;
        }
    }
}

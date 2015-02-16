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
    using Microsoft.RightsManagement;
    using Microsoft.RightsManagement.UI.RMSCustomControls;
    using Microsoft.RightsManagement.UI.RMSCustomControls.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Core;

    /// <summary>
    /// Implmentation of ICosentCallback interface. Class to handle the consents.
    /// </summary>
    internal class ConsentManager : IConsentCallback
    {
        public string Userid;
        public CoreDispatcher Dispatcher;
        private ConsentUIResult consentResult;

        /// <summary>
        /// Creates an instance of <see cref="ConsentManager"/>
        /// </summary>
        /// <param name="dispatcher">Dispatcher for current UI thread</param>
        public ConsentManager(CoreDispatcher dispatcher)
        {
            this.Userid = AppUtils.GetStoredUserName();
            this.Dispatcher = dispatcher;
        }

        /// <summary>
        /// Consent handler - this method is called from SDK as callback
        /// </summary>
        /// <param name="consents">List of consents to handle</param>
        /// <returns>List of consents with results updated in them</returns>
        public IAsyncOperation<IEnumerable<IConsent>> ConsentsAsync(IEnumerable<IConsent> consents)
        {
            var newList = new List<IConsent>(consents);
            foreach (var consent in newList)
            {
                if (consent.Type == ConsentType.ServiceUrlConsent)
                {
                    var result = this.GetConsentResultHelper(consent.Urls[0].ToString()).Result;
                    consent.Result = new ConsentResult(result.Accepted, result.ShowAgain, this.Userid);
                }
                else
                {
                    var result = this.GetDocumentConsentResultHelper().Result;
                    consent.Result = new ConsentResult(result.Accepted, result.ShowAgain, this.Userid);
                }
            }

            return Task.FromResult(newList as IEnumerable<IConsent>).AsAsyncOperation();
        }

        private async Task<ConsentUIResult> GetDocumentConsentResultHelper()
        {
            var completion =
                new TaskCompletionSource<ConsentUIResult>(TaskCreationOptions.None);
            await this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    var diagManager = new ConsentDialogManager();
                    consentResult = await diagManager.ShowDocumentTrackingConsent();
                    completion.SetResult(consentResult);
                });

            await completion.Task;
            return consentResult;
        }

        private async Task<ConsentUIResult> GetConsentResultHelper(string url)
        {
            var completion =
                new TaskCompletionSource<ConsentUIResult>(TaskCreationOptions.None);
            await this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    var diagManager = new ConsentDialogManager();
                    consentResult = await diagManager.ShowServiceUrlConsent(url);
                    completion.SetResult(consentResult);
                });

            await completion.Task;
            return consentResult;
        }
    }
}

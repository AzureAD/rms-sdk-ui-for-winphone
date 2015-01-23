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

    public class ConsentDialogManager
    {
        public ConsentDialogManager()
        {
            try
            {
                UIUtils.CheckForUIThread();

            }
            catch (Exception e)
            {
                PublicAPIExceptionHandler.HandleAPIException(e);
            }
        }

        public async Task<ConsentUIResult> ShowServiceUrlConsent(string url)
        {
            var result = await ServiceUrlConsentPage.ShowServiceUrlConsentPage(url);
            return result;
        }

        public async Task<ConsentUIResult> ShowDocumentTrackingConsent()
        {
            var result = await DocumentTrackingConsentPage.ShowDocumentTrackingConsentPage();
            return result;
        }
    }
}

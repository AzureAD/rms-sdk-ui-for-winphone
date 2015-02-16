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
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Activation;
    using Windows.Storage;

    internal class AppUtils
    {
        public static async Task ContinueWebAuthenticationHelperAsync(
          WebAuthenticationBrokerContinuationEventArgs args,
          AuthenticationManager authManager)
        {
            var result = await authManager.Context.ContinueAcquireTokenAsync(args);
            var success = authManager.UpdateAuthenticationResult(result);
            if (authManager.CompletionSource == null)
            {
                return;
            }

            if (success)
            {
                authManager.CompletionSource.SetResult(result.AccessToken);
            }
            else
            {
                authManager.CompletionSource.SetException(new Exception(result.ErrorDescription));
            }
        }

        public static void StoreUserName(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid)) return;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["RMSUserEmailId"] = userid;
        }
        
        /// <summary>
        /// Delete the local copies of unprotected and protected files
        /// </summary>
        /// <returns></returns>
        public static async Task CleanupFiles()
        {
            var localFolder =  ApplicationData.Current.LocalFolder;

            var allFiles = await localFolder.GetFilesAsync();

            foreach (var storageFile in allFiles)
            {
                await storageFile.DeleteAsync();
            }           
        }

        /// <summary>
        /// Get the stored user id.
        /// </summary>
        public static string GetStoredUserName()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("RMSUserEmailId"))
            {
                return ApplicationData.Current.LocalSettings.Values["RMSUserEmailId"] as string;
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks if format of the email id/user id is valid
        /// </summary>
        /// <param name="emailId">Email id</param>
        /// <returns>True if the </returns>
        public static bool IsValidEmailAddress(string emailId)
        {
            if (string.IsNullOrWhiteSpace(emailId))
            {
                return false;
            }

            Regex re = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$",
                  RegexOptions.IgnoreCase);
            return re.IsMatch(emailId);
        }
    }
}

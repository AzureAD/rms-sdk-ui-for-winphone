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
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;

    internal static class UIEventExceptionHandler
    {
        internal static async Task<Exception> ChainExceptionHandlerToAction(Func<Task> action, bool terminateApp = true)
        {
            Exception error = null;

            try
            {
                await action();
            }
            catch (Exception ex)
            {
                error = ex;
                HandleExceptions(ex, terminateApp);
            }

            return error;
        }

        internal static Exception ChainExceptionHandlerToAction(Action action, bool terminateApp = true)
        {
            Exception error = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                error = ex;
                HandleExceptions(ex, terminateApp);
            }

            return error;
        }

        public async static Task HandleExceptions(Exception exception, bool terminateApp)
        {
            string errorText = String.Empty;

            if (exception.HResult == ErrorInfomation.USER_NOT_CONSENTED.HResult)
            {
                errorText = "User declined the consent";
            }
            else if (exception.HResult == ErrorInfomation.AUTHENTICATION_FAILED.HResult)
            {
                errorText = "Authentication is canceled or failed";
            }
            else if (exception.HResult == ErrorInfomation.INVALID_PL.HResult)
            {
                errorText = "Invalid License";
            }
            else if (exception.HResult == ErrorInfomation.NEEDS_ONLINE.HResult)
            {
                errorText = "User is not online";
            }
            else if (exception.HResult == ErrorInfomation.REST_SERVICE_DISABLED.HResult)
            {
                errorText = "RMS REST service is disabled";
            }
            else if (exception.HResult == ErrorInfomation.RIGHT_NOT_GRANTED.HResult)
            {
                errorText = "User does not have rights";
            }
            else if (exception.HResult == ErrorInfomation.SERVER_ERROR.HResult)
            {
                errorText = "RMS server error";
            }
            else if (exception.HResult == ErrorInfomation.SERVICE_NOT_AVAILABLE_ERROR.HResult)
            {
                errorText = "RMS service is not available";
            }
            else if (exception.HResult == ErrorInfomation.DEVICE_REJECTED.HResult)
            {
                errorText = "Device rejected error";
            }
            else
            {
                errorText = exception.Message;
            }

            var dialog = new MessageDialog(errorText, "Error");
            UICommandInvokedHandler handler = terminateApp ? new UICommandInvokedHandler(Invoked) : null;
            dialog.Commands.Add(new UICommand("ok",  handler));
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }

        private static void Invoked(IUICommand command)
        {  
            Application.Current.Exit();
        }
    }
   
}

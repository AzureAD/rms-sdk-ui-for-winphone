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
    using System;
    using System.Net.NetworkInformation;
    using System.Text;
    using Windows.ApplicationModel.Core;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    internal static partial class UIUtils
    {
        public static bool IsLightThemeUsed()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Light;
        }

        public static void CheckForUIThread()
        {
            if (!CoreWindow.GetForCurrentThread().Dispatcher.HasThreadAccess)
            {
                throw new RmsWrongThreadException();
            }
        }

        public static void CheckForNetworkConnection()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                throw new RmsCommunicationException();
            }
        }

        /// <summary>
        /// Runs the specified action on the UI thread asynchrounously
        /// </summary>
        /// <param name="action">the action to run</param>
        public static async void RunOnUIThreadAsync(DispatchedHandler action)
        {
            if (CoreWindow.GetForCurrentThread().Dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
            }
        }

        public static String WrapString(String str, int width)
        {
            StringBuilder wrappedBuilder = new StringBuilder();

            while (str.Length > width)
            {
                String str1, str2;

                BreakString(str, width, out str1, out str2);

                wrappedBuilder.Append(str1);

                str = str2;
            }

            // append the last piece
            wrappedBuilder.Append(str);

            return wrappedBuilder.ToString();
        }

        private static void BreakString(String str, int breakIndex, out String line1, out String line2)
        {
            int lineEndIndex = breakIndex;

            // find a space-character from where to break
            while (!Char.IsWhiteSpace(str[lineEndIndex]) && lineEndIndex > 0)
            {
                --lineEndIndex;
            }

            // skip space characters
            while (Char.IsWhiteSpace(str[lineEndIndex]) && lineEndIndex > 0)
            {
                --lineEndIndex;
            }

            // if there is no space to break, then break in the middle of the word
            if (lineEndIndex == 0)
            {
                lineEndIndex = breakIndex;
            }

            line1 = str.Substring(0, lineEndIndex + 1);

            int nextLineStartIndex = lineEndIndex + 1;

            // skip space
            while (nextLineStartIndex < str.Length && Char.IsWhiteSpace(str[nextLineStartIndex]))
            {
                ++nextLineStartIndex;
            }

            // if there is nothing left then it is an empty string
            if (nextLineStartIndex >= str.Length)
            {
                line2 = String.Empty;
            }
            else
            {
                line2 = str.Substring(nextLineStartIndex);

                // add a newline to the end of line1, because it is not the last line
                line1 += "\n";
            }
        }
    }
}
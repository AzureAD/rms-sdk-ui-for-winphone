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
    using RMSSampleApp.Utils;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Phone.UI.Input;
    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextProtectionPage : Page
    {
        public TextProtectionPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void ProtectButton_Click(object sender, RoutedEventArgs e)
        {
            await UIEventExceptionHandler.ChainExceptionHandlerToAction(
               async () =>
               {
                   if (TextContent.Text.Trim() != string.Empty)
                   {
                       // Create a text file and save the text entered by the user in it. We will protect this file 
                       // in ProtectedTextSharePage
                       var localFolder = ApplicationData.Current.LocalFolder;
                       StorageFile unprotectedTextFile = await localFolder.CreateFileAsync("SampleText.txt", CreationCollisionOption.GenerateUniqueName);

                       using (var writeStream = await unprotectedTextFile.OpenStreamForWriteAsync())
                       {
                           using (var sw = new StreamWriter(writeStream))
                           {
                               await sw.WriteLineAsync(TextContent.Text.Trim());
                           }
                       }

                       this.Frame.Navigate(typeof(PolicyPickerPage), unprotectedTextFile);
                   }
                   else
                   {
                       throw new Exception("Please enter valid text");
                   }

               }, false);
        }

        private async void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            await this.GoBack();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;
        }

        private async Task GoBack()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (this.Frame.CanGoBack) this.Frame.GoBack(); });
        }

    }
}

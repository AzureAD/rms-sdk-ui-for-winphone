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
    using Microsoft.RightsManagement.UI.RMSCustomControls.ViewModels;
    using System;
    using System.Collections.Generic;
    using Windows.Foundation;
    using Windows.Phone.UI.Input;
    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PolicyPickerPage : Page
    {
        private PolicyPickerViewModel policyPickerViewModel;

        private int currentSelectedIndex = 0;

        private AsyncUIOperation<ProtectionScheme> asyncOperation;

        private PolicyPickerPageContext context;

        private ProtectionScheme policy;

        public static IAsyncOperation<ProtectionScheme> PickPolicyAsync(
            List<TemplateDescriptor> templates,
            List<PolicyDescriptor> policies)
        {
            var asyncOperation = new AsyncUIOperation<ProtectionScheme>();
            var context = new PolicyPickerPageContext();
            context.TemplatesInfo = templates;
            context.PoliciesInfo = policies;
            var currentPage = Window.Current.Content as Frame;
            var coreWindow = CoreWindow.GetForCurrentThread();
            coreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => currentPage.Navigate(typeof(PolicyPickerPage), context));

            context.AsyncOperation = asyncOperation;
            return asyncOperation;
        }

        public PolicyPickerPage()
        {
            this.InitializeComponent();
            this.PageTitle.Text = LocalizedStrings.Get("PolicyPickerPageTitleLowerCase");
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
        }

        private async void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            backPressedEventArgs.Handled = true;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (this.asyncOperation.Status == AsyncStatus.Started)
                    {
                        this.asyncOperation.Fail(new OperationCanceledException());
                    }
                });
            this.GoBack();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.context = (PolicyPickerPageContext)e.Parameter;
            if (this.context == null)
            {
                this.GoBack();
                return;
            }

            this.asyncOperation = this.context.AsyncOperation;
            this.policyPickerViewModel = new PolicyPickerViewModel();
            this.policyPickerViewModel.Policies = PolicyPickerViewModel.CreateList(this.context.TemplatesInfo, this.context.PoliciesInfo, false);
            PermissionList.ItemsSource = this.policyPickerViewModel.Policies;
            PageTitle.Foreground = policyPickerViewModel.Foreground;
            this.PreSelectSavedSelection();
        }

        private async void PreSelectSavedSelection()
        {
            this.ApplyButton.IsEnabled = false;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.SelectedIndexKey))
            {
                var index = (int)ApplicationData.Current.LocalSettings.Values[Constants.SelectedIndexKey];
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PermissionList.SelectedIndex = index);
                this.ApplyButton.IsEnabled = true;
            }
        }

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ProtectionSchemeViewModel addedItem in e.AddedItems)
            {
                addedItem.Expanded = true;
                addedItem.DescriptionVisibility = Visibility.Visible;
            }

            foreach (ProtectionSchemeViewModel removedItem in e.RemovedItems)
            {
                removedItem.Expanded = false;
                removedItem.DescriptionVisibility = Visibility.Collapsed;
            }

            //if the added item is the custom protection template, reset the selectection to previously selected one. 
            var item = e.AddedItems[0] as ProtectionSchemeViewModel;
            if (item.ProtectionScheme.IsEnabled == true)
            {
                this.currentSelectedIndex = PermissionList.SelectedIndex;
            }
            else
            {
                PermissionList.SelectedIndex = this.currentSelectedIndex;
            }

            ApplicationData.Current.LocalSettings.Values[Constants.SelectedIndexKey] = PermissionList.SelectedIndex;
            this.ApplyButton.IsEnabled = true;
        }

        private async void GoBack()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (this.Frame.CanGoBack) this.Frame.GoBack(); });
        }

        private async void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ProtectionSchemeViewModel)PermissionList.SelectedItem;
            this.policy = selectedItem.ProtectionScheme;
            this.GoBack();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.asyncOperation.Complete(this.policy));
        }

        private async void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.asyncOperation.Fail(new OperationCanceledException()));
            this.GoBack();
        }
    }
}

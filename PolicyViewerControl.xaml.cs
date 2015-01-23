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
    using System.Collections.Generic;
    using System.Threading;
    using Windows.Graphics.Display;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public sealed partial class PolicyViewerControl : UserControl
    {
        private bool isAutoDismissEnabled = false;

        private bool isLightDismissEnabled = true;

        private IEnumerable<UserRights> supportedRights;

        private UserPolicy policy = null;

        //The auto dismiss timeout in milli-seconds.
        private const int AutoDismissTimeout = 5000;

        private Timer timer = null;

        /// <summary>
        /// List of those rights, which are to be displayed in RMS sharing app. We don't need
        /// to show all the rights in EditableDocumentRights
        /// </summary>
        private readonly Dictionary<string, string> rightsToBeDisplayed;

        public PolicyViewerControl()
        {

            this.InitializeComponent();

            // Actual 'Right' is a key, 'Right description' is a value which will be displayed to the user
            rightsToBeDisplayed = new Dictionary<string, string>();
            rightsToBeDisplayed.Add(EditableDocumentRights.Edit, LocalizedStrings.Get("EditRightText"));
            rightsToBeDisplayed.Add(EditableDocumentRights.Extract, LocalizedStrings.Get("ExtractRightText"));
            rightsToBeDisplayed.Add(EditableDocumentRights.Print, LocalizedStrings.Get("PrintRightText"));
            rightsToBeDisplayed.Add(CommonRights.Owner, LocalizedStrings.Get("OwnerRightText"));
            rightsToBeDisplayed.Add(CommonRights.View, LocalizedStrings.Get("ViewRightText"));
        }

        public IEnumerable<UserRights> SupportedRights
        {
            get
            {
                return supportedRights;
            }

            set
            {
                supportedRights = value;
                //Update the UI elements in the viewer popup
                BindPolicyInfo();
            }
        }

        public UserPolicy Policy
        {
            get
            {
                return policy;
            }

            set
            {
                policy = value;

                //update the UI elements in the viewer popup
                BindPolicyInfo();
            }
        }

        public bool IsAutoDismissEnabled
        {
            get
            {
                return isAutoDismissEnabled;
            }

            set
            {
                isAutoDismissEnabled = value;

                //We need to modify the timer only is the viewer popup is currently open.
                if (ViewerPopup.IsOpen)
                {
                    if (isAutoDismissEnabled)
                    {
                        if (timer == null)
                        {
                            timer = new Timer(this.TimerTimedOut, null, AutoDismissTimeout, Timeout.Infinite);
                        }
                    }
                    else
                    {
                        if (timer != null)
                        {
                            timer.Change(0, Timeout.Infinite);
                        }
                    }
                }
            }
        }

        public bool IsLightDismissEnabled
        {
            get
            {
                return isLightDismissEnabled;
            }

            set
            {
                isLightDismissEnabled = value;

                //we need to add/remove the evet handler only is the viewer popup is currently open
                if (ViewerPopup.IsOpen)
                {
                    if (isLightDismissEnabled)
                    {
                        RootGrid.Tapped += RootVisualTap;
                    }
                    else
                    {
                        RootGrid.Tapped -= RootVisualTap;
                    }
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                return ViewerPopup.IsOpen;
            }

            set
            {               
                var currentFrame = Window.Current.Content as Frame;

                if (value)
                {
                    //Modify only if the popup is NOT currently open
                    if (!ViewerPopup.IsOpen && policy != null)
                    {

                        this.ViewerPopup.Width = currentFrame.RenderSize.Width;
                        this.RootGrid.Width = currentFrame.RenderSize.Width;

                        if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Landscape ||
                            DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                        {
                            this.RootGrid.Height = currentFrame.RenderSize.Height;
                        }
                        else
                        {
                            this.RootGrid.Height = Double.NaN;
                        }

                        ViewerPopup.IsOpen = true;

                        if (isLightDismissEnabled)
                        {
                            RootGrid.Tapped += RootVisualTap;
                        }

                        if (isAutoDismissEnabled)
                        {
                            timer = new Timer(this.TimerTimedOut, null, AutoDismissTimeout, Timeout.Infinite);
                        }

                        //Get the viewer into focus
                        this.IsTabStop = true;
                        this.Focus(FocusState.Pointer);
                    }
                }
                else
                {
                    if (ViewerPopup.IsOpen)
                    {
                        ViewerPopup.IsOpen = false;
                        RootGrid.Tapped -= RootVisualTap;

                        if (timer != null)
                        {
                            timer.Change(0, Timeout.Infinite);
                        }
                    }
                }
            }
        }

        private void RootVisualTap(object sender, TappedRoutedEventArgs e)
        {
            if (ViewerPopup.IsOpen)
            {
                IsOpen = false;
            }
        }

        private async void TimerTimedOut(Object obj)
        {
            timer.Dispose();
            timer = null;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsOpen = false);

        }

        private void BindPolicyInfo()
        {
            if (policy == null)
            {
                PolicyCaption.Text = LocalizedStrings.Get("NoProtectionSchemeName");
                PolicyDesc.Text = String.Empty;

                //Granted rights are all the suppported rights. 
                List<string> grantedRights = new List<string>();

                foreach (string rightDescr in this.rightsToBeDisplayed.Values)
                {
                    grantedRights.Add(rightDescr);
                }

                GrantedList.ItemsSource = grantedRights;
                NotGrantedList.ItemsSource = null;
            }
            else
            {
                PolicyCaption.Text = policy.Name;
                PolicyDesc.Text = policy.Description;

                if (policy.AccessCheck(CommonRights.Owner)) //User is the owner and applied some protection.
                {
                    //Granted rights are all the suppported rights. 
                    List<string> grantedRights = new List<string>();

                    foreach (string rightDescr in this.rightsToBeDisplayed.Values)
                    {
                        grantedRights.Add(rightDescr);
                    }

                    GrantedList.ItemsSource = grantedRights;
                    NotGrantedList.ItemsSource = null;
                }
                else //User is the consumer (not owner). 
                {
                    //Intersection of supported rights and granted rights are the actual granted rights. 
                    //The remaining rights in the supported rights are the not grated rights. 

                    List<string> grantedRights = new List<string>();
                    List<string> notGrantedRights = new List<string>();

                    foreach (string right in this.rightsToBeDisplayed.Keys)
                    {
                        if (policy.AccessCheck(right))
                        {
                            grantedRights.Add(this.rightsToBeDisplayed[right]);
                        }
                        else
                        {
                            notGrantedRights.Add(this.rightsToBeDisplayed[right]);
                        }
                    }

                    GrantedList.ItemsSource = grantedRights;
                    NotGrantedList.ItemsSource = notGrantedRights;
                }
            }

            this.UpdateVisibility();
        }

        /// <summary>
        /// This method shows/hides the edit button and other UI elements depending on whether 
        /// the user is the owner of the content and if policy editing is enabled. 
        /// 
        /// Call this method in conjunction with BindPolicyInfo() or whenever the IsPolicyEditingEnabled
        /// is updated.
        /// </summary>
        private void UpdateVisibility()
        {
            if (policy == null) //No Protection : Setting IsOpen = true will cause a no-operation
            {
                ViewerCaption.Text = String.Empty;
                ViewerCaption.Visibility = Visibility.Collapsed;

                PolicyCaption.Visibility = Visibility.Visible;
                PolicyDesc.Visibility = Visibility.Collapsed;

                this.RightsGrid.Visibility = Visibility.Collapsed;

                GrantorInfoGrid.Visibility = Visibility.Collapsed;

            }
            else //There is some protection is place on the file. 
            {
                //The application is READ-ONLY
                {
                    if (policy.AccessCheck(CommonRights.Owner))
                    {
                        //Policy editing is NOT enabled and the user is the owner of the content.
                        ViewerCaption.Text = LocalizedStrings.Get("PermissionsPopupOwnerComment");
                        ViewerCaption.Visibility = Visibility.Visible;

                        PolicyCaption.Visibility = Visibility.Visible;
                        if (string.IsNullOrWhiteSpace(policy.Description))
                        {
                            PolicyDesc.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            // In current implementation, we choose to not show policy description
                            PolicyDesc.Visibility = Visibility.Collapsed;
                        }

                        RightsGrid.Visibility = Visibility.Collapsed;

                        GrantorInfoGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        //Policy editing is NOT enabled and the user is not the owner of the content.
                        ViewerCaption.Text = LocalizedStrings.Get("PermissionsPopupNonOwnerComment");
                        ViewerCaption.Visibility = Visibility.Visible;

                        PolicyCaption.Visibility = Visibility.Visible;
                        PolicyDesc.Visibility = Visibility.Collapsed;

                        this.RightsGrid.Visibility = Visibility.Visible;

                        GrantorDetails.Text = policy.Owner;
                        GrantorInfoGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}

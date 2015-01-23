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

namespace Microsoft.RightsManagement.UI.RMSCustomControls.ViewModels
{
    using Microsoft.RightsManagement;
    using Microsoft.RightsManagement.UI.RMSCustomControls.Models;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Windows.UI;
    using Windows.UI.Xaml.Media;

    public class PolicyPickerViewModel : INotifyPropertyChanged
    {
        private List<ProtectionSchemeViewModel> policies;
        private SolidColorBrush foreground;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public List<ProtectionSchemeViewModel> Policies
        {
            get { return this.policies; }
            set
            {
                this.policies = value;
                NotifyPropertyChanged("Policies");
            }
        }

        public SolidColorBrush Foreground
        {
            get
            {
                return UIUtils.IsLightThemeUsed()
                                           ? new SolidColorBrush(Color.FromArgb(255, 26, 26, 26))
                                           : new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
            }
            set
            {
                this.foreground = value;
                NotifyPropertyChanged("Foreground");
            }
        }


        static public List<ProtectionSchemeViewModel> CreateList(IEnumerable<TemplateDescriptor> templates, IEnumerable<PolicyDescriptor> adHocs, bool noProtection)
        {
            var protectionSchemes = new List<ProtectionSchemeViewModel>();
            templates = templates ?? new List<TemplateDescriptor>();
            adHocs = adHocs ?? new List<PolicyDescriptor>();
            foreach (var template in templates) 
            {
                var schemeViewModel = new ProtectionSchemeViewModel();
                schemeViewModel.ProtectionScheme = ProtectionScheme.CreateProtectionScheme(ProtectionSchemeType.Template, template, null);
                schemeViewModel.ProtectionScheme.IsEnabled = true;
                schemeViewModel.ProtectionScheme.ShowDescription = true;
                protectionSchemes.Add(schemeViewModel);
            }

            foreach (var adhoc in adHocs)
            {
                var schemeViewModel = new ProtectionSchemeViewModel();
                schemeViewModel.ProtectionScheme = ProtectionScheme.CreateProtectionScheme(ProtectionSchemeType.Adhoc, null, adhoc);
                schemeViewModel.ProtectionScheme.IsEnabled = false;
                schemeViewModel.ProtectionScheme.ShowDescription = false;
                protectionSchemes.Add(schemeViewModel);
            }

            if (noProtection) 
            {
                var schemeViewModel = new ProtectionSchemeViewModel();
                schemeViewModel.ProtectionScheme = ProtectionScheme.CreateProtectionScheme(ProtectionSchemeType.NoProtection, null, null);
                schemeViewModel.ProtectionScheme.IsEnabled = false;
                schemeViewModel.ProtectionScheme.ShowDescription = false;
                protectionSchemes.Add(schemeViewModel);
            }

            return protectionSchemes;
        }

    }
}

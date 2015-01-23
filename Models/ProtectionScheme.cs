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

namespace Microsoft.RightsManagement.UI.RMSCustomControls.Models
{
    using System;

    public class ProtectionScheme
    {
        public bool IsEnabled { get; set; }

        public bool ShowDescription { get; set; }

        public ProtectionSchemeType Type { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TemplateDescriptor Template { get; private set; }

        public PolicyDescriptor Policy { get; private set; }

        private ProtectionScheme()
        {
        }

        public static ProtectionScheme CreateProtectionScheme(ProtectionSchemeType type, TemplateDescriptor template, PolicyDescriptor custom)
        {
            var scheme = new ProtectionScheme();
            scheme.Type = type;
            scheme.Template = template;
            scheme.Policy = custom;
            if (type == ProtectionSchemeType.Template && template == null ||
                type == ProtectionSchemeType.Adhoc && custom == null)
            {
                throw new ArgumentException();
            }

            if (type == ProtectionSchemeType.Template)
            {
                scheme.Name = template.Name;
                scheme.Description = template.Description;
            }

            else if (type == ProtectionSchemeType.Adhoc)
            {
                scheme.Name = string.IsNullOrWhiteSpace(custom.Name) ? LocalizedStrings.Get("CustomPolicyText") : custom.Name;
                scheme.Description = string.IsNullOrWhiteSpace(custom.Description) ? string.Empty : template.Description;
            }
            else
            {
                scheme.Name = LocalizedStrings.Get("RemoveProtectionText");
                scheme.Description = string.Empty;
            }

            return scheme;
        }
    }
}

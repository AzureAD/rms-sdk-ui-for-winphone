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
    using Microsoft.RightsManagement.UI.RMSCustomControls.Models;
    using System;
    using System.ComponentModel;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    public class ProtectionSchemeViewModel : INotifyPropertyChanged
    {
        private ProtectionScheme protectionScheme;
        private bool expanded;
        private bool showDescription;
        private bool isEnabled;
        private double pageWidth;
        private Orientation orientation = Orientation.Vertical;
        private double appBarHeight = 150;
        private double descBlockMarginLeft = 60;
        private double descBlockMarginRight = 5;
        private double textBlockMarginLeft = 44;
        private double textBlockMarginRight = 5;
        private Visibility descriptionVisibility;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public ProtectionScheme ProtectionScheme
        {
            get { return protectionScheme; }
            set
            {
                protectionScheme = value;

                descriptionVisibility = protectionScheme.ShowDescription ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ProtectionScheme");
                NotifyPropertyChanged("DescriptionVisibility");
            }
        }

        public bool Expanded
        {
            get
            {
                return this.expanded;
            }

            set
            {
                this.expanded = value;
                NotifyPropertyChanged("ExpandedViewVisibility");
                NotifyPropertyChanged("CollapsedViewVisibility");
            }
        }

        public Visibility ExpandedViewVisibility
        {
            get { return this.expanded ? Visibility.Visible : Visibility.Collapsed; ; }

        }

        public Visibility CollapsedViewVisibility
        {
            get { return this.expanded ? Visibility.Collapsed : Visibility.Visible; }

        }

        public bool ShowDescription
        {
            get
            {
                if (this.isEnabled)
                {
                    return this.showDescription;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this.showDescription = value;
                NotifyPropertyChanged("ShowDescription");
                NotifyPropertyChanged("DescriptionVisibility");
            }
        }

        public Visibility DescriptionVisibility
        {
            get { return descriptionVisibility; }
            set
            {
                descriptionVisibility = value;
                NotifyPropertyChanged("DescriptionVisibility");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value;
                NotifyPropertyChanged("NonSelectedTextForegroundColor");
                NotifyPropertyChanged("DescriptionVisibility");
            }
        }

        public SolidColorBrush NonSelectedTextForegroundColor
        {
            get
            {
                if (UIUtils.IsLightThemeUsed())
                {
                    if (this.isEnabled)
                    {
                        return new SolidColorBrush(Color.FromArgb(255, 77, 77, 77)); //70% Gray
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); //40% Gray
                    }
                }
                else
                {
                    if (this.isEnabled)
                    {
                        return new SolidColorBrush(Color.FromArgb(255, 179, 179, 179)); //30% Gray
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromArgb(255, 103, 103, 103)); //60% Gray
                    }
                }
            }
        }

        /// <summary>
        /// Similar theme that was used in RMS Phone 8.0 app
        /// </summary>
        public SolidColorBrush NonSelectedTextForegroundColorLegacy
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }

        }

        public SolidColorBrush SelectedTextBackgroundColor
        {
            get
            {
                if (UIUtils.IsLightThemeUsed())
                {
                    return new SolidColorBrush(Colors.White);
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
        }

        /// <summary>
        /// Similar theme that was used in RMS Phone 8.0 app
        /// </summary>
        public SolidColorBrush SelectedTextBackgroundColorLegacy
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 77, 77, 77));
            }
        }

        public SolidColorBrush SelectedTextForegroundColor
        {
            get
            {
                if (UIUtils.IsLightThemeUsed())
                {
                    return new SolidColorBrush(Color.FromArgb(255, 77, 77, 77));
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 179, 179, 179));
                }
            }
        }

        public Double PageWidth
        {
            get
            {
                var curFrame = Window.Current.Content as Frame;
                pageWidth = curFrame.RenderSize.Width;
                if (orientation == Orientation.Vertical)
                {
                    return pageWidth;
                }
                else
                {
                    return (pageWidth - appBarHeight);
                }
            }

            set
            {
                pageWidth = value;
                NotifyPropertyChanged("PageWidth");
                NotifyPropertyChanged("TextBlockWidth");
                NotifyPropertyChanged("DescBlockWidth");
            }
        }

        public Orientation PageOrientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
            }
        }

        public Double TextBlockWidth
        {
            get
            {
                return (PageWidth - (textBlockMarginLeft + this.textBlockMarginRight));
            }
        }

        public Double DescBlockWidth
        {
            get
            {
                return (PageWidth - (descBlockMarginLeft + descBlockMarginRight));
            }
        }
    }
}
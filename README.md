UI Library for Microsoft RMS SDK v4.1 for Windows Phone
==================

The UI Library for Microsoft RMS SDK v4.1 for Windows Phone provides the UI, important to your interactive apps development. This library is optional and a developer may choose to build their own UI when using Microsoft RMS SDK v4.1.

##Features

This library provides following UI components:
* **PolicyPicker**: Shows a policy picker screen, where the user can choose RMS template for protection of data or files.
* **PolicyViewer**: Shows the permissions that the user has on a RMS protected data or file.
* **DocumentTrackingConsent and ServiceUrlConsent**: Shows a consent about tracking of the document by sender or IT administrator, that the user can accept/reject.

## Community Help and Support

We leverage [Stack Overflow](http://stackoverflow.com/) to work with the community on supporting Azure Active Directory and its SDKs, including this one! We highly recommend you ask your questions on Stack Overflow (we're all on there!) Also browser existing issues to see if someone has had your question before. 

We recommend you use the "adal" tag so we can see it! Here is the latest Q&A on Stack Overflow for ADAL: [http://stackoverflow.com/questions/tagged/adal](http://stackoverflow.com/questions/tagged/adal)

## Security Reporting

If you find a security issue with our libraries or services please report it to [secure@microsoft.com](mailto:secure@microsoft.com) with as much detail as possible. Your submission may be eligible for a bounty through the [Microsoft Bounty](http://aka.ms/bugbounty) program. Please do not post security issues to GitHub Issues or any other public site. We will contact you shortly upon receiving the information. We encourage you to get notifications of when security incidents occur by visiting [this page](https://technet.microsoft.com/en-us/security/dd252948) and subscribing to Security Advisory Alerts.

##Contributing

All code is licensed under MICROSOFT SOFTWARE LICENSE TERMS, MICROSOFT RIGHTS MANAGEMENT SERVICE SDK UI LIBRARIES. We enthusiastically welcome contributions and feedback. You can clone the repository and start contributing now.


## How to use this library

### Prerequisites
You must have downloaded and/or installed following software

* Git
* Windows OS 8.1
* Visual Studio 2013
* Windows Phone 8.1 development tools
* The Microsoft Rights Management SDK 4.1 package for Windows Phone

You must an Azure Active Directory tenant/domain where your app is registered. To learn more about creating Azure Active Directory
tenant, registering an app in Azure AD, see [here](https://github.com/AzureADSamples/NativeClient-WindowsPhone8.1)

### Setting up development environment

1. Download Microsoft RMS SDK v4.1 for Windows Phone from [here](http://www.microsoft.com/en-us/download/details.aspx?id=45487). 
2. Import UI library project (UILibrary.csproj) under ./rms-sdk-ui-for-winphone/ directory from GitHub to your local machine.
3. Import RMSSampleApp project under ./rms-sdk-ui-for-winphone/ directory from GitHub to your local machine.
4. Open RMSSampleApp.sln file in Visual Studio 2013. Add UILibrary.csproj to the same solution if it is not already added.
5. In Visual Studio Solution Explorer, right click solution file, select Properties. In the Configuration Properties, select
   X86 platform to build for both projects.
6. In both projects, add reference to X86 version of Microsoft.RightsManagement.winmd from previously downloaded Microsoft RMS SDK v4.1.
7. In RMSSampleApp project, add a project reference to UILibrary.
8. Open AuthenticationManager.cs file, in the constructor you will see below code.

			 // e.g. this.Clientid = "7608DFAF-F20E-58C6-82C3-08AFEE79D74E";
             this.Clientid = "[Client id for RMSSampleApp that you get from Azure AD portal e.g. 7608DFAF-F20E-58C6-82C3-08AFEE79D74E]";            

            // e.g. this.Redirect = @"https://authorize/";
            this.Redirect = "[Redirect uri for RMSSampleApp that is registered in your Azure AD tenant]";   
			
   Fill the place holders with appropriate client id and redirect uri. 
			
9. Set RMSSampleApp.csproj as a StartUp project, build the entire solution. Press F5 to run the sample.

## License

Copyright © Microsoft Corporation, All Rights Reserved

Licensed under MICROSOFT SOFTWARE LICENSE TERMS, 
MICROSOFT RIGHTS MANAGEMENT SERVICE SDK UI LIBRARIES;
You may not use this file except in compliance with the License.
See the license for specific language governing permissions and limitations.
You may obtain a copy of the license (RMS SDK UI libraries - EULA.DOCX) at the 
root directory of this project.

THIS CODE IS PROVIDED AS IS BASIS, WITHOUT WARRANTIES OR CONDITIONS
OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.



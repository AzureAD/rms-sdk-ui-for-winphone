UI Library for Microsoft RMS SDK v4.1 for Windows Phone
==================

The UI Library for Microsoft RMS SDK v4.1 for Windows Phone provides the UI, important to your interactive apps development. This library is optional and a developer may choose to build their own UI when using Microsoft RMS SDK v4.1.

##Features

This library provides following UI components:
* **PolicyPicker**: Shows a policy picker screen, where the user can choose RMS template for protection of data or files.
* **PolicyViewer**: Shows the permissions that the user has on a RMS protected data or file.
* **DocumentTrackingConsent and ServiceUrlConsent**: Shows a consent about tracking of the document by sender or IT administrator, that the user can accept/reject.

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


### Setting up development environment

1. Download Microsoft RMS SDK v4.1 for Windows Phone from [here](http://www.microsoft.com/en-us/download/details.aspx?id=45487). 
2. Import UI library project (UILibrary.csproj) under ./rms-sdk-ui-for-winphone/ directory from GitHub.
3. Open the UILibrary.csproj in Visual Studio 2013.
4. In Visual Studio Solution Explorer, right click solution file, select Properties. In the Configuration Properties, select
   X86 platform to build for UILibrary project.
5. In UILibrary project, add reference to X86 version of Microsoft.RightsManagement.winmd
6. Build the UILibrary project.
7. Start using this UILibrary project in your Windows Phone app.

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



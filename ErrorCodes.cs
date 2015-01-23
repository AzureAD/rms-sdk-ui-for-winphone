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
    public static class ProtectionErrorCodes
    {
        /// <summary>
        /// The current user doesn't have a right to open the content.
        /// This is the same error code as IPCERROR_RIGHT_NOT_GRANTED in MSIPC 2.1.
        /// </summary>
        public static int RightNotGranted { get { return unchecked((int)0x80040211); } }

        /// <summary>
        /// The AD RMS client needs to display a UI to complete the requested operation.
        /// This is the same error code as IPCERROR_NEEDS_UI in MSIPC 2.1.
        /// </summary>
        public static int NeedsUI { get { return unchecked((int)0x8004020E); } }

        /// <summary>
        /// Something went wrong while trying to contact the Rights Management Service.  
        /// This is the same error code as IPCERROR_SERVER_ERROR in MSIPC 2.1.
        /// </summary>
        public static int ServerError { get { return unchecked((int)0x80040214); } }

        /// <summary>
        /// Publishing is disabled for the current user.
        /// This is the same error code as IPCERROR_TEMPLATE_NOT_ENABLED_FOR_USER in MSIPC 2.1.
        /// </summary>
        public static int PublishingNotEnabledForUser { get { return unchecked((int)0x8004021A); } }

        internal static int CertificateExpired { get { return unchecked((int)0x800b0101); } }
        internal static int WrongThread { get { return unchecked((int)0x8001010E); } }
        internal static int NetworkDisconnected { get { return unchecked((int)0x800704C6); } }
        internal static int NotSupported { get { return unchecked((int)0x80070032); } }
        internal static int InvalidArgument { get { return unchecked((int)0x80070057); } }
        internal static int OperationCancelled { get { return unchecked((int)0x800704c7); } }
        internal static int OperationFailed { get { return unchecked((int)0x80004005); } }
    }
}
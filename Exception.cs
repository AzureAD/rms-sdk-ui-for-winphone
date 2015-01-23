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

    public class CameraNotFoundException : Exception
    {
        public CameraNotFoundException()
            : this(LocalizedStrings.Get("CameraNotFound"))
        {
        }

        public CameraNotFoundException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Base class for all defined exceptions
    /// </summary>
    internal class ProtectionException : Exception
    {
        protected ProtectionException()
        {
        }

        protected ProtectionException(string message)
            : base(message)
        {
        }

        protected ProtectionException(string message, int errorCode)
            : base(message)
        {
            HResult = (int)errorCode;
        }

        protected ProtectionException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            HResult = errorCode;
        }
    };

    /// <summary>
    /// Base class for all exceptions that are thrown from public API. As in WinRT components it is not allowed
    /// to define custom public exceptions, we define two types of public API exceptions:
    /// 1) Standard exceptions - exceptions for which there is a corresponding built-in .NET exception (e.g., ArgumentException). 
    ///                          For exceptions of this type we throw the built-in .NET exception. In this case we use innerException
    ///                          to pass the internal exception defined here, which may contain debug data for developers.
    /// 2) RMS specific exceptions - exceptions that are specific to RMS (e.g. RightsNotGranted). For exceptions of this type
    ///                              we throw the internal exception and make sure that it has the correct HRESULT that developers
    ///                              can check.
    /// </summary>
    internal class PublicAPIException : ProtectionException
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="exceptionMessage">the exception message</param>
        /// <param name="errorCode">the error code to use</param>
        /// <param name="requestDebugLogs">indicates whether or not we should offer the user to upload the logs</param>
        protected PublicAPIException(string exceptionMessage, int errorCode, bool requestDebugLogs)
            : base(exceptionMessage, errorCode)
        {
            this.RequestDebugLogs = requestDebugLogs;
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="exceptionMessage">the exception message</param>
        /// <param name="errorCode">the error code to use</param>
        /// <param name="requestDebugLogs">indicates whether or not we should offer the user to upload the logs</param>
        /// <param name="innerException">inner exception</param>
        protected PublicAPIException(string exceptionMessage, int errorCode, bool requestDebugLogs, Exception innerException)
            : base(exceptionMessage, errorCode, innerException)
        {
            this.RequestDebugLogs = requestDebugLogs;
        }

        /// <summary>
        /// This returns the built-in .NET exception that corresponds to this exception. If there is no such an exception
        /// it returns this.
        /// </summary>
        /// <returns>The standard exception or this</returns>
        public virtual Exception MapToStandardException()
        {
            return this;
        }

        /// <summary>
        /// Indicates whether we need to offer the user to send us the logs.
        /// </summary>
        public bool RequestDebugLogs { get; private set; }
    }

    /// <summary>
    /// Thrown when the app calls an API that needs a UI thread on a non-UI thread
    /// 
    /// Logs are requested from the user when this exception type is thrown
    /// </summary>
    internal class RmsWrongThreadException : PublicAPIException
    {

        public RmsWrongThreadException()
            : base(LocalizedStrings.Get("ErrorWrongThread"), ProtectionErrorCodes.WrongThread, true)
        {
        }
    }

    /// <summary>
    /// Thrown when an API which requires an internet connection detects that no connection is available
    /// 
    /// Logs are not requested from the user when this exception type is thrown
    /// </summary>
    internal class RmsCommunicationException : PublicAPIException
    {
        public RmsCommunicationException()
            : base(LocalizedStrings.Get("ErrorCommunication"), ProtectionErrorCodes.NetworkDisconnected, false)
        {
        }
    }

    /// <summary>
    /// Thrown when the user has cancelled an operation, such as IPAL authentication. Note that all non-success values
    /// from IPAL are treated as cancellation
    /// 
    /// Logs are not requested from the user when this exception type is thrown
    /// </summary>
    internal class RmsUserCancelledException : PublicAPIException
    {
        public RmsUserCancelledException()
            : base(LocalizedStrings.Get("ErrorUserCancelled"), ProtectionErrorCodes.OperationCancelled, false)
        {
        }

        public override Exception MapToStandardException()
        {
            return new OperationCanceledException(this.Message, this);
        }
    }

    /// <summary>
    /// A generic catch-all for errors encountered internally of the RMS platform. Also meant for the following
    /// conditions:
    /// - STS fails to accept token or issue token for REST service
    /// - Internal consumption related error like: json formatting, pl, encryption, bug, other
    /// - All permanent errors coming from REST service
    /// 
    /// When thrown, our top-level exception handling logic will ask the user to send logs to the service
    /// </summary>
    internal class RmsGeneralException : PublicAPIException
    {
        public RmsGeneralException()
            : base(("ErrorGeneralError"), ProtectionErrorCodes.OperationFailed, true)
        {
        }

        public RmsGeneralException(Exception innerException)
            : base(("ErrorGeneralError"), ProtectionErrorCodes.OperationFailed, true, innerException)
        {
        }
    }

    internal static class PublicAPIExceptionHandler
    {
        public static void HandleAPIException(Exception exception)
        {
            throw GetPublicExceptionForInternalException(exception).MapToStandardException();
        }

        private static PublicAPIException GetPublicExceptionForInternalException(Exception internalException)
        {
            PublicAPIException publicException = null;

            try
            {
                throw internalException;
            }
            catch (PublicAPIException e)
            {
                publicException = e;
            }
            catch (AggregateException e)
            {
                publicException = GetPublicExceptionForInternalException(e.GetBaseException());
            }
            catch (Exception e)
            {
                publicException = new RmsGeneralException(e);
            }

            return publicException;
        }
    }
}
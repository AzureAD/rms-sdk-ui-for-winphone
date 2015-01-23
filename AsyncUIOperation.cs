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
    using Windows.Foundation;

    /// <summary>
    /// Implements an IAsyncOperation for implementation of UI operations that need to be run on a UI thread and wait for user actions (e.g. PolicyPicker.PickPolicyAsync()), 
    /// which means that they can't be implemented using seperate threads with System.Threading.Tasks.Task.
    /// It provides methods like Complete(), Cancel() and Fail() that can be called by the UI operation to complete, cancel or fail the operation.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncUIOperation<TResult> : IAsyncOperation<TResult>
    {
        private TResult result;

        public AsyncUIOperation()
        {
            Status = AsyncStatus.Started;
            ErrorCode = null;
        }

        public void Complete(TResult completionResult)
        {
            this.result = completionResult;
            this.Status = AsyncStatus.Completed;

            if (Completed != null)
            {
                Completed(this, this.Status);
            }
        }

        public void Fail(Exception error)
        {
            this.Status = AsyncStatus.Error;
            this.ErrorCode = error;

            if (Completed != null)
            {
                Completed(this, this.Status);
            }
        }

        public void Cancel()
        {
            this.Status = AsyncStatus.Canceled;

            if (Completed != null)
            {
                Completed(this, this.Status);
            }
        }

        public void Close()
        {
        }

        public Exception ErrorCode { get; private set; }

        public uint Id
        {
            get
            {
                return 1;
            }
        }

        public AsyncStatus Status { get; private set; }

        public TResult GetResults()
        {
            return this.result;
        }

        public AsyncOperationCompletedHandler<TResult> Completed
        {
            get;
            set;
        }
    }
}

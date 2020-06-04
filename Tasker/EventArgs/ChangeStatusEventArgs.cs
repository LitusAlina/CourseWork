using System;
using Tasker.Enums;

namespace Tasker
{
    public class ChangeStatusEventArgs : EventArgs
    {
        public Status Status { get; }

        public ChangeStatusEventArgs(Status status)
        {
            Status = status;
        }
    }
}

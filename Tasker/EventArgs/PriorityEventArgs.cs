using System;
using Tasker.Enums;

namespace Tasker
{
    public class PriorityEventArgs : EventArgs
    {
        public Priority Priority { get; }

        public PriorityEventArgs(Priority priority)
        {
            Priority = Priority;
        }
    }
}

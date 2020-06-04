using System;

namespace Tasker
{
    public class TaskEventArgs : EventArgs
    {
        public Task Task { get; }

        public TaskEventArgs(Task task)
        {
            Task = task;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Tasker;
using Tasker.Enums;

namespace TaskerConsoleApp
{
    public struct DataOfTask
    {
        public string Description { get; set; }
        public int DaysToComplete { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DealWithTask Executor { get; set; }
    }
}

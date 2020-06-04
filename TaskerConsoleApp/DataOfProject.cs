using System;
using System.Collections.Generic;
using System.Text;
using Tasker;

namespace TaskerConsoleApp
{
    public struct DataOfProject
    {
        public string Description { get; set; }
        public int DaysToComplete { get; set; }
        public List<Task> Tasks { get; set; }
        public DealWithProject Executor { get; set; }
    }

}

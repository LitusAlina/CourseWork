using System;

namespace Tasker
{
    public class EmployeeEventArgs : EventArgs
    {
        public Employee Employee { get; }

        public EmployeeEventArgs(Employee employee)
        {
            Employee = employee;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Tasker
{
    public class Employee : DealWithTask
    {
        private string Name { get; }

        public event EventHandler OnNewTaskAdded;

        /* CONSTRUCTORS */
        public Employee(string name) : base()
        {
            this.Name = name;
        }

        public Employee() : base()
        {
            this.Name = "John Doe";
        }
        /* END CONSTRUCTORS */

        public override void AddTask(Task newTask)
        {
            base.AddTask(newTask);

            // Invoking new task handler
            OnNewTaskAdded?.Invoke(this, new TaskEventArgs(newTask));
        }

        public override List<Task> GetTasks() => tasks;

        public override string ToString() => Name; 
    }

}

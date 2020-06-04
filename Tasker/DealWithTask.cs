using System;
using System.Collections.Generic;
using Tasker.Enums;
using Tasker.Exceptions;

namespace Tasker
{
    public abstract class DealWithTask
    {
        protected List<Task> tasks;

        // Removing task from tasks list after completing
        protected void OnTaskCompleted(object sender, EventArgs args)
        {
            if (sender is Task task)
            {
                tasks.RemoveAt(tasks.IndexOf(task));
            }
        }

        /* CONSTRUCTORS */
        public DealWithTask()
        {
            tasks = new List<Task>();
        }

        public DealWithTask(List<Task> tasks)
        {
            this.tasks = tasks;

            foreach (Task task in tasks)
            {
                task.OnTaskCompleted += OnTaskCompleted;
            }
        }
        /* END CONSTRUCTORS */

        public virtual void AddTask(Task newTask)
        {
            tasks.Add(newTask);
            newTask.OnTaskCompleted += OnTaskCompleted;
        }

        public virtual void StartTask(Task task)
        {
            if (!tasks.Contains(task)) throw new AccessDeniedException();
            task.Status = Status.InProgress;
        }

        public virtual void CompleteTask(Task task)
        {
            if (!tasks.Contains(task)) throw new AccessDeniedException();
            task.Status = Status.Completed;
        }

        public virtual List<Task> GetTasks() => tasks;
    }
}


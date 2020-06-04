using System;
using System.Linq;
using System.Collections.Generic;
using Tasker.Enums;

namespace Tasker
{
    public class Project : Deadlines
    {
        private List<Task> tasks;
        private Status _status;
        private DealWithProject _executor;

        public string Description { get; }
        public DealWithProject Executor
        {
            get => _executor;
            set
            {
                _executor = value;
                Executor?.AddProject(this);
            }
        }

        public Status Status
        {
            get => _status;
            set
            {
                _status = value;

                // Invoking event handlers for any project status change
                OnProjectStatusChanged?.Invoke(this, new ChangeStatusEventArgs(_status));

                // Invoking event handlers depending on the new project status
                switch (_status)
                {
                    case Status.InProgress:
                        OnProjectStarted?.Invoke(this, EventArgs.Empty);
                        break;
                    case Status.Completed:
                        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }
        }

        public event EventHandler<ChangeStatusEventArgs> OnProjectStatusChanged;
        public event EventHandler OnProjectStarted;
        public event EventHandler OnProjectCompleted;

        /* CONSTRUCTORS */
        public Project(string description, int daysToComplete, List<Task> tasks, DealWithProject executor) : base(DateTime.Now.AddDays(daysToComplete))
        {
            this.Description = description;
            this.tasks = tasks;
            this.Executor = executor;
            this._status = Status.NotStarted;

            this.OnDeadlineReached += (sender, args) => Status = Status.Overdue;

            // Adding event handlers to determine project status
            foreach (Task task in tasks)
            {
                task.OnTaskStatusChanged += (sender, args) =>
                {
                    if (sender is Task senderTask)
                    {
                        if ((this.Status == Status.NotStarted || this.Status == Status.Completed)
                            && senderTask.Status != Status.NotStarted)
                        {
                            this.Status = Status.InProgress;
                        }

                        if (this.Status == Status.InProgress
                            && tasks.All(el => el.Status == Status.Completed))
                        {
                            this.Status = Status.Completed;
                        }
                    }
                };
            }
        }
        /* END CONSTRUCTORS */

        public List<Task> GetTasks() => tasks;
    }
}

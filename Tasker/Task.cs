using System;
using Tasker.Enums;


namespace Tasker
{
    public class Task : Deadlines
    {
        private Priority _priority;
        private Status _status;
        private DealWithTask _executor;

        public int DaysToComplete { get; }
        public string Description { get; }

        public DealWithTask Executor
        {
            get => _executor;
            set
            {
                _executor = value;
                Executor?.AddTask(this);
            }
        }

        public Status Status
        {
            get => _status;
            set
            {
                _status = value;

                // Invoking event handlers for any task status change
                OnTaskStatusChanged?.Invoke(this, new ChangeStatusEventArgs(_status));

                // Invoking event handlers depending on the new task status
                switch (_status)
                {
                    case Status.InProgress:
                        OnTaskStarted?.Invoke(this, EventArgs.Empty);
                        break;
                    case Status.Completed:
                        OnTaskCompleted?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }
        }

        public Priority Priority
        {
            get => _priority;
            set
            {
                _priority = value;

                // Invoking event handlers for any task priority change
                OnTaskPriorityChange?.Invoke(this, new PriorityEventArgs(_priority));
            }
        }

        public event EventHandler<PriorityEventArgs> OnTaskPriorityChange;
        public event EventHandler<ChangeStatusEventArgs> OnTaskStatusChanged;
        public event EventHandler OnTaskStarted;
        public event EventHandler OnTaskCompleted;

        /* CONSTRUCTORS */
        public Task(string description, int daysToComplete) : base(DateTime.Now.AddDays(daysToComplete))
        {
            this.Description = description;
            this.DaysToComplete = daysToComplete;
            this._priority = Priority.Regular;
            this.Status = Status.NotStarted;
            this.Executor = null;
            this.OnDeadlineReached += (sender, args) => Status = Status.Overdue;
        }

        public Task(string description, int daysToComplete, Priority priority) : base(DateTime.Now.AddDays(daysToComplete))
        {
            this.Description = description;
            this.DaysToComplete = daysToComplete;
            this._priority = priority;
            this.Status = Status.NotStarted;
            this.Executor = null;
            this.OnDeadlineReached += (sender, args) => Status = Status.Overdue;
        }

        public Task(string description, int daysToComplete, DealWithTask executor) : base(DateTime.Now.AddDays(daysToComplete))
        {
            this.Description = description;
            this.DaysToComplete = daysToComplete;
            this._priority = Priority.Regular;
            this.Status = Status.NotStarted;
            this.Executor = executor;
            this.OnDeadlineReached += (sender, args) => Status = Status.Overdue;
        }

        public Task(string description, int daysToComplete, Priority priority, DealWithTask executor) : base(DateTime.Now.AddDays(daysToComplete))
        {
            this.Description = description;
            this.DaysToComplete = daysToComplete;
            this._priority = priority;
            this.Status = Status.NotStarted;
            this.Executor = executor;
            this.OnDeadlineReached += (sender, args) => Status = Status.Overdue;
        }
        /* END CONSTRUCTORS */
    }
}

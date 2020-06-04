using System;
using System.Timers;


namespace Tasker
{
    public abstract class Deadlines
    {
        public DateTime DeadlineDate { get; set; }

        public event EventHandler OnDeadlineReached;

        protected Deadlines(DateTime deadlineDate)
        {
            DeadlineDate = deadlineDate;
            SetDeadlineTimer();
        }

        private void SetDeadlineTimer()
        {
            double interval = TimeSpan.FromHours(24).TotalMilliseconds;

            Timer checkForTime = new Timer(interval);
            checkForTime.Elapsed += CheckDeadline;
            checkForTime.Enabled = true;
        }

        private void CheckDeadline(object sender, ElapsedEventArgs e)
        {
            if (DeadlineDate.Date == DateTime.Now.Date)
            {
                OnDeadlineReached?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

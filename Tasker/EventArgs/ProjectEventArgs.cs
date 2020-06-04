using System;

namespace Tasker
{
    public class ProjectEventArgs : EventArgs
    {
        public Project Project { get; }

        public ProjectEventArgs(Project project)
        {
            Project = project;
        }
    }
}

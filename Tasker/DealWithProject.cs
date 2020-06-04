using System;
using System.Collections.Generic;
using Tasker.Enums;
using Tasker.Exceptions;

namespace Tasker
{
    public abstract class DealWithProject
    {
        protected List<Project> projects;

        // Removing project from projects list after completing
        protected void OnProjectCompleted(object sender, EventArgs args)
        {
            if (sender is Project project)
            {
                projects.RemoveAt(projects.IndexOf(project));
            }
        }

        /*** CONSTRUCTORS ***/
        public DealWithProject()
        {
            projects = new List<Project>();
        }

        public DealWithProject(List<Project> projects)
        {
            this.projects = projects;

            foreach (Project project in projects)
            {
                project.OnProjectCompleted += OnProjectCompleted;
            }
        }
        /*** END CONSTRUCTORS ***/

        public virtual void AddProject(Project newProject)
        {
            projects.Add(newProject);
            newProject.OnProjectCompleted += OnProjectCompleted;
        }

        public void StartProject(Project project)
        {
            if (!projects.Contains(project)) throw new AccessDeniedException();
            project.Status = Status.InProgress;
        }

        public void CompleteProject(Project project)
        {
            if (!projects.Contains(project)) throw new AccessDeniedException();
            project.Status = Status.Completed;
        }

        public virtual List<Project> GetProjects() => projects;
    }
}


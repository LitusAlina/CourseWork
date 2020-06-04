using System;
using System.Collections.Generic;
using Tasker.Exceptions;

namespace Tasker
{
    public class Team : DealWithProject
    {
        private List<Employee> employees;

        public string Name { get; set; }

        public event EventHandler<ProjectEventArgs> OnNewProjectAdded;
        public event EventHandler<EmployeeEventArgs> OnNewWorkerJoined;

        /* CONSTRUCTORS */
        public Team(string name)
        {
            Name = name;
            employees = new List<Employee>();
        }

        public Team(string name, List<Employee> employees)
        {
            Name = name;
            this.employees = employees;
        }
        /* END CONSTRUCTORS */

        public void AddWorker(Employee newEmployee)
        {
            employees.Add(newEmployee);

            // Invoking new worker joined handlers
            OnNewWorkerJoined?.Invoke(this, new EmployeeEventArgs(newEmployee));
        }

        public override void AddProject(Project newProject)
        {
            base.AddProject(newProject);

            List<Task> tasks = newProject.GetTasks();
            DelegateTasksToWorkers(tasks);

            // Invoking new project added handlers
            OnNewProjectAdded?.Invoke(this, new ProjectEventArgs(newProject));
        }

        public List<Employee> GetMembers() => employees;

        public override string ToString() => Name;

        private void DelegateTasksToWorkers(List<Task> tasks)
        {
            if (employees.Count < 1)
            {
                throw new NoExecutorException();
            }

            foreach (Task task in tasks)
            {
                task.Executor = GetWorkerWithMinTasks();
            }
        }

        private Employee GetWorkerWithMinTasks()
        {
            Employee employeeWithMinTasks = employees[0];

            for (int i = 1; i < employees.Count; i++)
            {
                if (GetDaysToCompleteAllTasks(employees[i].GetTasks()) < GetDaysToCompleteAllTasks(employeeWithMinTasks.GetTasks()))
                    employeeWithMinTasks = employees[i];
            }

            return employeeWithMinTasks;
        }

        private static int GetDaysToCompleteAllTasks(List<Task> tasks)
        {
            int days = 0;

            foreach (Task task in tasks)
            {
                days += task.DaysToComplete;
            }

            return days;
        }
    }
}

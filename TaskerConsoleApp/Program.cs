using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasker;
using Tasker.Enums;

namespace TaskerConsoleApp
{
    internal class Program
    {
        private static List<Team> teams = new List<Team>();
        private static List<Project> projects = new List<Project>();

        private static Project currentProject;
        private static Task currentTask;

        private static DataOfProject newProjectData;
        private static List<DataOfTask> newProjectTasks = new List<DataOfTask>();

        /*** EVENT HANDLERS ***/
        private static void ProjectStatusChanged(object sender, EventArgs args)
        {
            if (sender is Project project)
            {
                Console.WriteLine($"\nProject status: \"{project.Description}\" - change to: \"{GetStatusString(project.Status)}\"");
            }
        }

        private static void TaskStatusChanged(object sender, EventArgs args)
        {
            if (sender is Task task)
            {
                Console.WriteLine($"\nTask status: \"{task.Description}\" - change to: \"{GetStatusString(task.Status)}\"");
                if(task.Status == Status.Overdue)
                {
                    Console.WriteLine($"\nProject is overdue");
                }
            }
        }

        private static void DeadlineReached(object sender, EventArgs args)
        {
            if (sender is Project project)
            {
                Console.WriteLine($"\nProject: \"{project.Description}\" is overdue");
            }
            if (sender is Task task)
            {
                Console.WriteLine($"\nTask: \"{task.Description}\" is overdue");
                Console.WriteLine($"\nProject is overdue");
            }
        }

        private static void NewProjectAdded(object sender, ProjectEventArgs args)
        {
            if (sender is Team team)
            {
                Console.WriteLine($"\nTeam: \"{team.Name}\" project was assigned: \"{args.Project.Description}\"");
            }
        }
        /*** END EVENT HANDLERS ***/

        public static void LoadInitialData()
        {
            /*** INITIALIZING DATA ***/
            Team team1 = new Team("Team1", new List<Employee> {
                new Employee("Tyler"),
                new Employee("Tom"),
                new Employee("Tanya")
                });

            Team team2 = new Team("Team2", new List<Employee> {
                new Employee("Alex"),
                new Employee("Audrey"),
                new Employee("Ariana")
                });

            teams.Add(team1);
            teams.Add(team2);

            List<Task> appProjectTasks = new List<Task> {
                new Task("Develop a project implementation plan", 10, Priority.High),
                new Task("Develop an application", 30, Priority.Regular),
                new Task("Develop the design and decoration of the application", 5, Priority.Low),
                };

            List<Task> adappProjectTasks = new List<Task> {
                new Task("Develop a product advertising campaign", 7, Priority.Low),
                new Task("Shoot a commercial", 5, Priority.High),
                new Task("Track application downloads", 10, Priority.Regular),
                };

            Project appProject = new Project("Application development\n", 30, appProjectTasks, team1);
            Project adappProject = new Project("Advertising campaign\n", 20, adappProjectTasks, team2);

            projects.Add(appProject);
            projects.Add(adappProject);
            /*** END INITIALIZING DATA ***/

            /*** SUBSCRIBING TO EVENTS ***/
            foreach (Project project in projects)
            {
                project.OnProjectStatusChanged += ProjectStatusChanged;
                project.OnDeadlineReached += DeadlineReached;

                foreach (Task task in project.GetTasks())
                {
                    task.OnTaskStatusChanged += TaskStatusChanged;
                    task.OnDeadlineReached += DeadlineReached;
                }
            }

            foreach (Team team in teams)
            {
                team.OnNewProjectAdded += NewProjectAdded;
            }
            /*** END SUBSCRIBING TO EVENTS ***/
        } /*** END INITIALIZING DATA METHODS ***/

        /*** DISPLAYING DATA METHODS ***/

        public static void DisplayProjectsWithTasks()
        {
            Console.WriteLine("Current projects:");
            for (int i = 0; i < projects.Count; i++)
            {
                DisplayProjectWithTask(projects[i], i);
            }
        }

        public static void DisplayProjectWithTask(Project project, int index = -1)
        {
            double daysLeft = Math.Ceiling(project.DeadlineDate.Subtract(DateTime.Now).TotalDays);

            if (index >= 0)
            {
                Console.WriteLine($"{index + 1}) {project.Description} (executor: {project.Executor}, {daysLeft} days left");
            }
            else
            {
                Console.WriteLine($"{project.Description} (executor: {project.Executor}, {daysLeft} days left");
            }

            List<Task> tasks = project.GetTasks();
            DisplayTasks(tasks);
        }

        private static void DisplayTasks(List<Task> tasks)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Task task = tasks[i];
                double daysLeft = Math.Ceiling(task.DeadlineDate.Subtract(DateTime.Now).TotalDays);

                ConsoleColor markerColor;
                string marker = GetStatusMarker(task.Status, out markerColor);
                string priority = GetPriorityString(task.Priority);

                Console.WriteLine($"\t\t{marker} ");
                Console.WriteLine($"{i + 1}. {task.Description} ({task.Executor}, {priority} priority, {daysLeft} days left).");
            }
        }

        private static void DisplayTeams(List<Team> teams)
        {
            Console.WriteLine("\nCurrent teams:");
            for (int i = 0; i < teams.Count; i++)
            {
                Team team = teams[i];
                Console.WriteLine($"\t{i + 1}. {team.Name} (number of projects: {team.GetProjects().Count})");
            }
        }
        /*** END DISPLAYING DATA METHODS ***/

        /*** PROMPTING METHODS ***/
        private static void PromptProject()
        {
            Console.Write("\nTo select a project, enter its number(\"n\"- to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                DisplayProjectsWithTasks();
                DisplayProjectMenu();
                return;
            }

            bool parsedSuccessfully = int.TryParse(line, out int projectIndex);

            if (!parsedSuccessfully || projectIndex < 1 || projectIndex > projects.Count)
            {
                Console.WriteLine("Invalid value, try again");
                PromptProject();
            }
            else
            {
                currentProject = projects[projectIndex - 1];
                Console.WriteLine($"Selected project: \"{currentProject.Description}\"");

                DisplayTasks(currentProject.GetTasks());
                PromptTask();
            }
        }

        private static void PromptTask()
        {
            Console.Write("\nTo select a task, enter its number (\"n\" - to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                DisplayProjectsWithTasks();
                DisplayProjectMenu();
                return;
            }

            List<Task> projectTasks = currentProject.GetTasks();

            bool parsedSuccessfully = int.TryParse(line, out int taskIndex);

            if (!parsedSuccessfully || taskIndex < 1 || taskIndex > projectTasks.Count)
            {
                Console.WriteLine("Invalid value, try again");
                PromptTask();
            }
            else
            {
                currentTask = projectTasks[taskIndex - 1];
                Console.WriteLine($"Selected task: \"{currentTask.Description}\"");

                DisplayTaskMenu();
            }
        }

        private static void PromptNewTaskStatus()
        {
            Console.WriteLine("Select a new status: ");
            Console.WriteLine("1. In progress");
            Console.WriteLine("2. Comleted");
            Console.WriteLine("3. Overdue");

            bool selected = false;
            Status newStatus = currentTask.Status;

            while (!selected)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        selected = true;
                        newStatus = Status.InProgress;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        selected = true;
                        newStatus = Status.Completed;
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        selected = true;
                        newStatus = Status.Overdue;
                        break;
                }
            }

            currentTask.Status = newStatus;
            DisplayProjectWithTask(currentProject);
            PromptTask();
        }

        private static void PromptNewTaskPriority()
        {
            Console.WriteLine("\nChoose a new priority: ");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Regular");
            Console.WriteLine("3. High");

            bool selected = false;
            Priority newPriority = currentTask.Priority;

            while (!selected)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        selected = true;
                        newPriority = Priority.Low;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        selected = true;
                        newPriority = Priority.Regular;
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        selected = true;
                        newPriority = Priority.High;
                        break;
                }
            }

            currentTask.Priority = newPriority;
            DisplayProjectWithTask(currentProject);
            PromptTask();
        }

        private static void PromptNewProjectDescription()
        {
            Console.Write("\nEnter a description of the new project(\"n\" - to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                DisplayProjectsWithTasks();
                DisplayProjectMenu();
                return;
            }

            if (!string.IsNullOrEmpty(line))
            {
                newProjectData.Description = line;
                PromptNewProjectDaysToComplete();
            }
            else
            {
                PromptNewProjectDescription();
            }
        }

        private static void PromptNewProjectDaysToComplete()
        {
            Console.Write("Enter the number of days to complete a new project(\"n\" - to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                PromptNewProjectDescription();
                return;
            }

            int days;
            bool parsedSuccessfully = int.TryParse(line, out days);

            if (parsedSuccessfully && days > 0)
            {
                newProjectData.DaysToComplete = days;
                GetTasksForNewProject();
            }
            else
            {
                Console.WriteLine("Invalid value, try again");
                PromptNewProjectDaysToComplete();
            }
        }

        private static void GetTasksForNewProject()
        {
            Console.WriteLine("\nTo create a new task, press 1");
            Console.WriteLine("To continue, press \"d\"");
            Console.WriteLine("To go back, press \"n\"");

            bool selected = false;

            while (!selected)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        selected = true;
                        PromptTaskForNewProjectDescription();
                        break;
                    case ConsoleKey.D:
                        if (newProjectTasks.Count > 0)
                        {
                            selected = true;
                            DisplayTeams(teams);
                            PromptNewProjectExecutor();
                        }
                        else
                        {
                            Console.WriteLine("\nYou cannot create a project without tasks");
                        }
                        break;
                    case ConsoleKey.N:
                        PromptNewProjectDaysToComplete();
                        return;
                }
            }
        }

        private static void PromptTaskForNewProjectDescription()
        {
            Console.Write("\nEnter a description of the new task(\"n\" - to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                GetTasksForNewProject();
                return;
            }

            if (!string.IsNullOrEmpty(line))
            {
                DataOfTask newTask = new DataOfTask();
                newTask.Description = line;

                newProjectTasks.Add(newTask);
                PromptTaskForNewProjectDaysToComplete();
            }
            else
            {
                PromptTaskForNewProjectDescription();
            }
        }

        private static void PromptTaskForNewProjectDaysToComplete()
        {
            Console.Write("Enter the number of days to complete the new task(\"n\" - to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                newProjectTasks.RemoveAt(newProjectTasks.Count - 1);
                PromptTaskForNewProjectDescription();
                return;
            }

            int days;
            bool parsedSuccessfully = int.TryParse(line, out days);

            if (parsedSuccessfully && days > 0)
            {
                DataOfTask lastTask = newProjectTasks[newProjectTasks.Count - 1];
                lastTask.DaysToComplete = days;
                newProjectTasks[newProjectTasks.Count - 1] = lastTask;

                PromptTaskForNewProjectPriority();
            }
            else
            {
                Console.WriteLine("Invalid value, try again");
                PromptTaskForNewProjectDaysToComplete();
            }
        }

        private static void PromptTaskForNewProjectPriority()
        {
            Console.WriteLine("\nChoose the priority of the task: ");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Regular");
            Console.WriteLine("3. High");
            Console.WriteLine("\"n\" - go back");

            bool selected = false;
            Priority priority = newProjectTasks[newProjectTasks.Count - 1].Priority;

            while (!selected)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        selected = true;
                        priority = Priority.Low;
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        selected = true;
                        priority = Priority.Regular;
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        selected = true;
                        priority = Priority.High;
                        break;
                    case ConsoleKey.N:
                        PromptTaskForNewProjectDaysToComplete();
                        return;
                }
            }

            DataOfTask lastTask = newProjectTasks[newProjectTasks.Count - 1];
            lastTask.Priority = priority;
            newProjectTasks[newProjectTasks.Count - 1] = lastTask;

            GetTasksForNewProject();
        }

        private static void PromptNewProjectExecutor()
        {
            Console.Write("Enter the number of the executing team from the list (\"n\" - to go back): ");
            string line = Console.ReadLine();

            if (line == "n")
            {
                GetTasksForNewProject();
                return;
            }

            int index;
            bool parsedSuccessfully = int.TryParse(line, out index);

            if (parsedSuccessfully && index > 0 && index <= teams.Count)
            {
                newProjectData.Executor = teams[index - 1];
                newProjectData.Tasks = newProjectTasks.Select(taskData => new Task(taskData.Description, taskData.DaysToComplete, taskData.Priority)).ToList();

                Project project = new Project(newProjectData.Description, newProjectData.DaysToComplete,
                    newProjectData.Tasks, newProjectData.Executor);

                project.OnProjectStatusChanged += ProjectStatusChanged;
                project.OnDeadlineReached += DeadlineReached;

                foreach (Task task in project.GetTasks())
                {
                    task.OnTaskStatusChanged += TaskStatusChanged;
                    task.OnDeadlineReached += DeadlineReached;
                }

                projects.Add(project);

                newProjectData = new DataOfProject();
                newProjectTasks = new List<DataOfTask>();

                DisplayProjectsWithTasks();
                DisplayProjectMenu();
            }
            else
            {
                Console.WriteLine("Invalid value, try again");
                PromptNewProjectExecutor();
            }
        }
        /*** END PROMPTING METHODS ***/

        /*** DISPLAYING MENUS METHODS ***/
        private static void DisplayProjectMenu()
        {
            Console.WriteLine("\nTo select a project, press 1");
            Console.WriteLine("To create a new project, press 2");

            bool selected = false;

            while (!selected)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        selected = true;
                        PromptProject();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        selected = true;
                        PromptNewProjectDescription();
                        break;
                }
            }
        }

        private static void DisplayTaskMenu()
        {
            Console.WriteLine("\nTo change the status of the task, press 1");
            Console.WriteLine("To change the priority of the task, press 2");
            Console.WriteLine("To go back, press \"n\"");

            bool selected = false;

            while (!selected)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                switch (pressedKey.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        selected = true;
                        Console.WriteLine("\n");
                        PromptNewTaskStatus();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        selected = true;
                        PromptNewTaskPriority();
                        break;
                    case ConsoleKey.N:
                        DisplayProjectWithTask(currentProject);
                        PromptTask();
                        return;
                }
            }
        }

        private static string GetStatusMarker(Status status, out ConsoleColor markerColor)
        {
            string marker = "*";
            markerColor = ConsoleColor.Gray;

            switch (status)
            {
                case Status.NotStarted:
                    marker = "-";
                    
                    break;
                case Status.InProgress:
                    marker = "&";
                    
                    break;
                case Status.Completed:
                    marker = "#";
                   
                    break;
                case Status.Overdue:
                    marker = "!";
                    
                    break;
            }

            return marker;
        }

        private static string GetStatusString(Status status)
        {
            switch (status)
            {
                case Status.NotStarted:
                    return "Not Started";
                case Status.InProgress:
                    return "In Progress";
                case Status.Completed:
                    return "Completed";
                case Status.Overdue:
                    return "Overdue";
                default:
                    return "Unknown";
            }
        }

        private static string GetPriorityString(Priority priority)
        {
            switch (priority)
            {
                case Priority.Low:
                    return "Low ";
                case Priority.Regular:
                    return "Regular ";
                case Priority.High:
                    return "High ";
                default:
                    return "Unknown ";
            }
        }
        /*** END VISUAL HELPERS METHODS ***/

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine();

            LoadInitialData();
            DisplayProjectsWithTasks();
            DisplayProjectMenu();

        }
    }
}

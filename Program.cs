using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32;

class Program {
    static string Ascii => @"  
        ___________                ________                  .____    .__          __   
        \__    ___/___             \______ \   ____          |    |   |__| _______/  |_ 
          |    | /  _ \    ______   |    |  \ /  _ \         |    |   |  |/  ___/\   __\
          |    |(  <_> )  /_____/   |    `   (  <_> )        |    |___|  |\___ \  |  |  
          |____| \____/            /_______  /\____/         |_______ \__/______> |__|  
                    \/                           \/                    \/
                                ";
    static string AppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) , ".cstasks");

    static Program() {
        if (!Directory.Exists(AppPath)) {
            Directory.CreateDirectory(AppPath);
        }
    }
    
    static List<Dictionary<string , object>> Tasks = new();

    static int TaskSelection() {
        Console.Write("\nChoice: ");
        var userInput = Console.ReadLine();

        if (userInput is null) {
            return -1;
        }

        if (int.TryParse(userInput , out int numChoice) && numChoice <= Tasks.Count && numChoice > 0) {
            return numChoice;
        } else {
            CleanUp();
            Console.WriteLine("Unknown Choice!");
            return -1;
        }
    }

    static void CleanUp() {
        Console.Clear();
        Console.WriteLine(Ascii);
    }

    static void SaveTasks() {
        File.WriteAllText(Path.Combine(AppPath, "tasks.csv"), "");
        using (StreamWriter writer = File.AppendText(Path.Combine(AppPath , "tasks.csv"))) {
            foreach (Dictionary<string , object> task in Tasks) {
                writer.WriteLine($"{task["name"]} = {task["state"]}");
            }
        }
    }
    static void AddTask() {
        CleanUp();

        Console.Write("Name: ");
        var taskName = Console.ReadLine();

        if (taskName is null) {
            Console.WriteLine("Name of the task cannot be empty!");
            return;
        }

        bool isUnique = true;

        foreach (Dictionary<string , object> task in Tasks) {
            if (task["name"] is string name && name.Equals(taskName , StringComparison.OrdinalIgnoreCase)) {
                isUnique = false;
                break;
            }
        }

        if (!isUnique) {
            CleanUp();
            Console.WriteLine("Task name must be unique!");
            return;
        }
        
        Tasks.Add(new() {
            { "name" , taskName},
            { "state" , "Unfinished" }
        });

        File.AppendAllText(Path.Combine(AppPath , "tasks.csv"), $"{taskName} = false\n");

        Console.WriteLine("Successfully added the task.");
    } 

    static void RemoveTask() {
        CleanUp();
        
        while (true) {
            int selected;
            while (true) {
                bool anyLeft = false;
                for (int i = 0; i < Tasks.Count; i++) {
                    Console.WriteLine($"{i + 1}. {Tasks[i]["name"]}");
                    anyLeft = true;
                }
                
                if (!anyLeft) {
                    CleanUp();
                    Console.WriteLine("No tasks left!");
                    return;
                }
                
                selected = TaskSelection();

                if (selected <= Tasks.Count && selected > 0) {
                    break;
                } else {
                    CleanUp();
                    Console.WriteLine("Unknown choice!");
                }
            }

            Tasks.RemoveAt(selected - 1);

            SaveTasks();

            Console.WriteLine("Successfully removed task.");
            return;
        }
    }

    static void ListTasks() {
        CleanUp();

        if (Tasks.Count == 0) {
            Console.WriteLine("No tasks!");
            return;
        }

        foreach (Dictionary<string , object> task in Tasks) {
            string state;

            if (task["state"] is false || (task["state"] is string s && s.Equals("unfinished", StringComparison.OrdinalIgnoreCase))) {
                state = "Unfinished";
            } else if (task["state"] is true || (task["state"] is string s1 && s1.Equals("finished", StringComparison.OrdinalIgnoreCase))) {
                state = "Finished";
            } else {
                state = "Unknown";
            }

            Console.WriteLine($"Task: {task["name"]}     State: {state}");
        }
    }

    static void ClearTasks() {
        while (true) {
            CleanUp();
            
            if (Tasks.Count == 0) {
                Console.WriteLine("No tasks to clean!");
                return;
            }

            Console.Write("Are you sure you want to clear ALL tasks? Y/N: ");
            var userInput = Console.ReadLine();

            if (userInput == null) {
                continue;
            } else if (userInput.Equals("y")) {
                Tasks.Clear();
                File.WriteAllText(Path.Combine(AppPath, "tasks.csv"), "");
                Console.WriteLine("Successfully cleared tasks.");
                return;
            } else if (userInput.Equals("n")) {
                CleanUp();
                return;
            }
        }
    }  

    static void ExitApp() {
        Console.Clear();
        Environment.Exit(0);
    }

    static List<Dictionary<string , object>> ReadTasks() {
        List<Dictionary<string, object>> tasks = new();

        // get path of all tasks.
        string tasksPath = Path.Combine(AppPath, "tasks.csv");

        string[] savedTasks;

        try {
            savedTasks = File.ReadAllLines(tasksPath);
        } catch (FileNotFoundException) {
            // Console.WriteLine("Saved tasks not found. Returning empty list...");
            return tasks;
        }

        if (savedTasks.Length == 0) {
            return tasks;
        }

        foreach (string line in savedTasks) {
            string[] data = line.Split('=');
            
            if (data.Length != 2) {
                Console.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            if (!bool.TryParse(data[1] , out bool result)) {
                Console.WriteLine("Failed to parse result on task!");
                result = false;
            }

            tasks.Add(new() {
                { "name" , data[0] },
                { "state" ,  result }
            });
        }

        return tasks;
    }

    static void FinishTask() {
        CleanUp();
        while (true) {
            int selected;
            while (true) {
                bool anyUnfinished = false;
                List<int> unfinished = new();
                for (int i = 0; i < Tasks.Count; i++)
                {
                    // Console.WriteLine($"State: {Tasks[i]["state"]}");
                    if (Tasks[i]["state"] is false || (Tasks[i]["state"] is string s && s.Equals("Unfinished", StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine($"{i + 1}. {Tasks[i]["name"]}");
                        // Console.WriteLine($"State: {Tasks[i]["state"]}");
                        unfinished.Add(i);
                        anyUnfinished = true;
                    }
                }

                if (!anyUnfinished)
                {
                    CleanUp();
                    Console.WriteLine("No unfinished tasks left.");
                    return;
                }

                selected = TaskSelection();

                if (selected <= Tasks.Count && selected > 0 && unfinished.Contains(selected - 1))  {
                    break;
                } else {
                    CleanUp();
                    Console.WriteLine("Unknown choice!");
                }
            }

            Tasks[selected - 1]["state"] = true;

            SaveTasks();

            return;
        }
    }

    static void Main() {
        // getting app path
        Tasks = ReadTasks();
        var options = new Dictionary<string, Action> {
            { "1" , AddTask },
            { "2" , RemoveTask },
            { "3" , FinishTask },
            { "4" , ListTasks },
            { "5" , ClearTasks },
            { "6" , ExitApp }
        };

        CleanUp();

        while (true)
        {
            Console.WriteLine("1. Add  2. Remove 3. Finish Task  4. List  5. Clear  6. Exit\n");
            Console.Write("Choice: ");
            var userChoice = Console.ReadLine();

            if (userChoice is null) {
                continue;
            }

            if (options.TryGetValue(userChoice , out Action? action)) {
                action!();
            } else {
                CleanUp();
                Console.WriteLine("Unknown Choice!");
            }
        }
    }
}
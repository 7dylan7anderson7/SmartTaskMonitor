using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using SmartTaskMonitor.Core.Models;
using System.Linq;

namespace SmartTaskMonitor.Core
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Smart Task Monitor ===");
            var tasks = LoadTasks(); //Call LoadTasks function

            while (true)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1 - View all tasks");
                Console.WriteLine("2 - Filter failed tasks");
                Console.WriteLine("3 - Export tasks to CSV");
                Console.WriteLine("4 - Exit");

                //Gets user input and saves it in a variable
                Console.Write("\nSelect option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        DisplayTasks(tasks); //Displays all task data
                        break;
                    case "2":
                        DisplayTasks(tasks.Where(t => t.Status == "Failed").ToList()); //Displays data on failed tasks
                        break;
                    case "3":
                        ExportToCsv(tasks); //Exports data to .csv file
                        Console.WriteLine("Data has been exported");
                        break;
                    case "4":
                        return; //Exits program
                    default:
                        Console.WriteLine("Invalid option. Try again."); //Case for invalid input
                        break;
                }
            }
        }

        //Function uses TaskLog.cs file to create a list based on the mock data in tasks.json
        static List<TaskLog> LoadTasks()
        {
            var json = File.ReadAllText("Data/tasks.json");
            return JsonSerializer.Deserialize<List<TaskLog>>(json) ?? new List<TaskLog>();
        }

        //Function displays tasks by writing to console
        static void DisplayTasks(List<TaskLog> tasks)
        {
            Console.WriteLine("\nID | Name               | Status  | Duration | Errors | Last Run");
            Console.WriteLine("---------------------------------------------------------------");

            foreach (var t in tasks)
            {
                Console.WriteLine($"{t.Id,2} | {t.Name,-18} | {t.Status,-7} | {t.DurationSeconds,8:F1}s | {t.ErrorCount,6} | {t.LastRun:g}");
            }
        }

        //Function exports data to a .csv file
        static void ExportToCsv(List<TaskLog> tasks)
        {
            string path = "Data/export.csv";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("Id,Name,Status,DurationSeconds,ErrorCount,LastRun");
                foreach (var t in tasks)
                {
                    writer.WriteLine($"{t.Id},{t.Name},{t.Status},{t.DurationSeconds},{t.ErrorCount},{t.LastRun}");
                }
            }
            Console.WriteLine($"Exported to {path}");
        }
    }
}
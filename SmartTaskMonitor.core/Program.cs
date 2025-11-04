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
            var tasks = LoadTasks();

            while (true)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1 - View all tasks");
                Console.WriteLine("2 - Filter failed tasks");
                Console.WriteLine("3 - Export tasks to CSV");
                Console.WriteLine("4 - Exit");

                Console.Write("\nSelect option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        DisplayTasks(tasks);
                        break;
                    case "2":
                        DisplayTasks(tasks.Where(t => t.Status == "Failed").ToList());
                        break;
                    case "3":
                        ExportToCsv(tasks);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static List<TaskLog> LoadTasks()
        {
            var json = File.ReadAllText("Data/tasks.json");
            return JsonSerializer.Deserialize<List<TaskLog>>(json) ?? new List<TaskLog>();
        }

        static void DisplayTasks(List<TaskLog> tasks)
        {
            Console.WriteLine("\nID | Name               | Status  | Duration | Errors | Last Run");
            Console.WriteLine("---------------------------------------------------------------");

            foreach (var t in tasks)
            {
                Console.WriteLine($"{t.Id,2} | {t.Name,-18} | {t.Status,-7} | {t.DurationSeconds,8:F1}s | {t.ErrorCount,6} | {t.LastRun:g}");
            }
        }

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
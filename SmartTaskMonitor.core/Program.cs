using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using SmartTaskMonitor.Core.Models;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartTaskMonitor.Core
{
    class Program
    {
        static async Task Main()
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
                        await DisplayTasksWithPrediction(tasks); //Displays data with AI prediction
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

        //Integration with Python AI prediction model API
        static async Task<double> GetFailurePrediction(TaskLog t)
        {
            using var client = new HttpClient();
            var payload = new
            {
                DurationSeconds = t.DurationSeconds,
                ErrorCount = t.ErrorCount,
                LastRunDaysAgo = (DateTime.Now - t.LastRun).Days
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://127.0.0.1:5001/predict", content);
            var result = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(result);
            return doc.RootElement.GetProperty("failure_probability").GetDouble();
        }

        //Function displays tasks, including AI prediction data, by writing to console
        static async Task DisplayTasksWithPrediction(List<TaskLog> tasks)
        {
            Console.WriteLine("\nID | Name | Status | Predicted Failure Risk");
            Console.WriteLine("---------------------------------------------");

            foreach (var t in tasks)
            {
                double risk = await GetFailurePrediction(t);
                Console.WriteLine($"{t.Id,2} | {t.Name,-15} | {t.Status,-7} | {risk:P0}");
            }
        }
    }
}
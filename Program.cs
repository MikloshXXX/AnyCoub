using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace AnyCoub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(
                "########################################\n"+
                "#                                      #\n"+
                "#          WELCOME TO ANYCOUB!         #\n"+
                "#                                      #\n"+
                "########################################\n");

            FFmpeg.SetExecutablesPath(Directory.GetCurrentDirectory());
            Directory.CreateDirectory("Downloads");

            bool fromList = AskAboutList();

            if (fromList)
            {
                string[] IDs = JsonConvert.DeserializeObject<string[]>(await File.ReadAllTextAsync("CoubList.json"));

                WriteLineWithColor($"\nWork is starting! Total tasks to complete {IDs.Length}", ConsoleColor.Yellow);

                for(int taskNumber = 0; taskNumber < IDs.Length; taskNumber++)
                {
                    try
                    {
                        var status = await CoubService.DownloadCoub(IDs[taskNumber]);

                        if (status == true)
                        {
                            WriteLineWithColor($"Done with {IDs[taskNumber]}. Completed {taskNumber+1}/{IDs.Length} tasks", ConsoleColor.Green);
                        }
                        else
                        {
                            WriteLineWithColor($"Wrong ID - {IDs[taskNumber]}", ConsoleColor.Red);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLineWithColor(ex.Message + "\n", ConsoleColor.Red);
                    }
                }

                return;
            }

            while (true)
            {
                try
                {
                    Console.Write("Please, input the Coub ID to download it: ");
                    var coub = Console.ReadLine();
                    if (coub == "q")
                        return;

                    var status = await CoubService.DownloadCoub(coub);

                    if (status == true)
                    {
                        WriteLineWithColor($"Done with {coub}\n", ConsoleColor.Green);
                    }
                    else
                    {
                        WriteLineWithColor($"Wrong ID - {coub}\n", ConsoleColor.Red);
                    }
                }
                catch (Exception ex)
                {
                    WriteLineWithColor(ex.Message+"\n", ConsoleColor.Red);
                }
            }
        }

        private static bool AskAboutList()
        {
            Console.Write("\nDownload coubs from the JSON list? (y/N): ");

            var key = Console.ReadKey().Key;

            if (key == ConsoleKey.Y)
            {
                Console.WriteLine();
                return true;
            }
            if (key == ConsoleKey.N)
            {
                Console.WriteLine();
                return false;
            }

            return AskAboutList();
        }

        private static void WriteLineWithColor(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}

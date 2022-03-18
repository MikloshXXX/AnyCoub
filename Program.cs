using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

            try
            {
                while (true)
                {
                    Console.Write("Please, input the Coub ID to download it: ");
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var coub = Console.ReadLine();
                    if (coub == "q")
                        return;

                    var status = await CoubService.DownloadCoub(coub);

                    if (status == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Done with {coub}\n");
                        stopwatch.Stop();
                        Console.WriteLine("Elapsed milliseconds: " + stopwatch.ElapsedMilliseconds);
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Something went wrong with {coub}\n");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            catch
            {
                Console.WriteLine("RUNTIME ERROR...");
            }
        }
    }
}

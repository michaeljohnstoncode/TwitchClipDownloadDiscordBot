using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace ClipDownloadDiscordBot.Services
{
    public class UpdateYTDLP
    {
        public void UpdateYoutubeDLP()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            // Initialize a new ProcessStartInfo object
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process using the ProcessStartInfo object
            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                // Execute the command
                using (StreamWriter streamWriter = process.StandardInput)
                {
                    streamWriter.WriteLine($"cd " + currentDirectory);
                    streamWriter.WriteLine($"yt-dlp --update");
                    streamWriter.WriteLine("exit");
                }

                // Read the output
                using (StreamReader streamReader = process.StandardOutput)
                {
                    string output = streamReader.ReadToEnd();
                    Console.WriteLine($"Output from updating YT-DLP: {output}");
                }

                // Read the error output, if any
                using (StreamReader streamReader = process.StandardError)
                {
                    string errorOutput = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        Console.WriteLine("Error from updating YT-DLP: " + errorOutput);
                    }
                }

                process.WaitForExit();
            }
        }
    }
}

// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using OllaCSharpApp;

var ollamaPath = @"E:\dev\ollama\ollama.exe";
var attributes = "run phi3";

void version1()
{
    try
    {
        using(ProcessExecutor executor = new ProcessExecutor(ollamaPath, attributes))
        {
            executor.Start();

            Console.WriteLine("Process started, write exit to quit");
            while(true)
            {
                var line = Console.ReadLine();

                if( line == "exit")
                {
                    break;
                }
                else
                {
                    // send to process
                    executor.WriteToProcess(line);
                }
            }

            executor.Exit();

            Console.WriteLine("Process finished");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

}

async Task version2Async()
{
     // Start the C# Windows application process
        var process = new Process
        {
            StartInfo =
            {
                FileName = ollamaPath,
                Arguments = attributes,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            }
        };

        process.Start();

        // Create tasks to asynchronously read output and error streams
        var outputTask = Task.Run(() => ReadStreamAsync(process.StandardOutput));
        var errorTask = Task.Run(() => ReadStreamAsync(process.StandardError));

        // Communicate with the process
        while (true)
        {
            // Read command from console
            Console.Write("Enter command: ");
            var command = Console.ReadLine();

            // Send command to the process
            process.StandardInput.WriteLine(command);

            // Exit if command is "exit"
            if (command.ToLower() == "exit")
                break;

            // Wait for a moment to receive and display output
            await Task.Delay(100);
        }

        // Wait for output and error reading tasks to complete
        await Task.WhenAll(outputTask, errorTask);

        process.WaitForExit();
}


static async Task ReadStreamAsync(StreamReader reader)
{
    while (!reader.EndOfStream)
    {
        var line = await reader.ReadLineAsync();
        Console.WriteLine(line);
    }
}


async Task version3Async()
{
     // Start the C# Windows application process
        var process = new Process
        {
            StartInfo =
            {
                FileName = ollamaPath,
                Arguments = attributes,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true // Enable event handling
        };

        // Event handler for receiving output data
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };

        // Event handler for receiving error data
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"Error: {e.Data}");
        };

        process.Start();

        // Begin asynchronous reading of output and error streams
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Communicate with the process
        while (true)
        {
            // Read command from console
            Console.Write("Enter command: ");
            var command = Console.ReadLine();

            // Send command to the process
            process.StandardInput.WriteLine(command);

            // Exit if command is "exit"
            if (command.ToLower() == "exit")
                break;
        }

        process.WaitForExit();
}

async Task Version4Async()
{
     // Start the C# Windows application process
        var process = new Process
        {
            StartInfo =
            {
                FileName = ollamaPath,
                Arguments = attributes,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true // Enable event handling
        };

        // Event handler for receiving error data
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"Error: {e.Data}");
        };

        process.Start();

        // Begin asynchronous reading of output and error streams
        var outputTask = Task.Run(async () =>
        {
            while (!process.StandardOutput.EndOfStream)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                    Console.WriteLine(line);
            }
        });

        // Begin asynchronous reading of input stream
        var inputTask = Task.Run(async () =>
        {
            while (true)
            {
                // Read command from console
                Console.Write("Enter command: ");
                var command = Console.ReadLine();

                // Send command to the process
                process.StandardInput.WriteLine(command);

                // Exit if command is "exit"
                if (command.ToLower() == "exit")
                    break;
            }
        });

        // Wait for both tasks to complete
        await Task.WhenAll(outputTask, inputTask);

        process.WaitForExit();
}

void Version5()
{
    var process = new Process
        {
            StartInfo =
            {
                FileName = ollamaPath,
                Arguments = attributes,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true // Enable event handling
        };

        // Event handler for receiving error data
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"Error: {e.Data}");
        };

        process.Start();

        // Read and write streams synchronously
        while (!process.HasExited)
        {
            // Read output
            string output = process.StandardOutput.ReadLine();
            if (!string.IsNullOrEmpty(output))
                Console.WriteLine(output);

            // Read command from console
            Console.Write("Enter command: ");
            string command = Console.ReadLine();

            // Send command to the process
            process.StandardInput.WriteLine(command);

            // Exit if command is "exit"
            if (command.ToLower() == "exit")
                break;
        }

        process.WaitForExit();
}

void Version6()
{
    using (var proc = new System.Diagnostics.Process
    {
        StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = ollamaPath,
            Arguments = attributes,
            UseShellExecute = false,
            RedirectStandardOutput = true, // NOTE: optional
            RedirectStandardInput = true, // NOTE: required
            CreateNoWindow = true, // NOTE: optional
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden // NOTE: optional
        }
    })
    {
        proc.StandardInput.WriteLine("Hello, World!"); // NOTE: this will send "Hello, World!" to the console application's process
        proc.StandardOutput.ReadLine();
    }
}

void Version7()
{
        ProcessStartInfo processStartInfo;
        processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = ollamaPath;
        processStartInfo.Arguments = attributes;
        processStartInfo.CreateNoWindow = true;

        // Redirect IO to allow us to read and write to it.
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardInput = true;
        processStartInfo.UseShellExecute = false;

        Process process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        int counter = 0;
        while (counter < 10)
        {
            // Write to the process's standard input.
            process.StandardInput.WriteLine(counter.ToString());

            // Read from the process's standard output.
            var output = process.StandardOutput.ReadLine();
            Console.WriteLine("Hosted process said: " + output);
            counter++;
        }

        process.Kill();

        Console.WriteLine("Hit any key to exit.");
        Console.ReadKey();
}

Version7();


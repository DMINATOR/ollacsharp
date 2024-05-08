// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using OllaCSharpApp;

var ollamaPath = @"E:\dev\ollama\ollama.exe";
var attributes = "run phi3";

try
{
    /*
    using (Process myProcess = new Process())
    {
        myProcess.StartInfo.UseShellExecute = false;

        // You can start any process, HelloWorld is a do-nothing example.
        myProcess.StartInfo.FileName = ollamaPath;
        myProcess.StartInfo.CreateNoWindow = true;
        myProcess.StartInfo.RedirectStandardOutput = true;  
        
        myProcess.Start();

         // Synchronously read the standard output of the spawned process.
        string output = myProcess.StandardOutput.ReadToEnd();  
        Console.Write(output);
        myProcess.WaitForExit();


        Console.ReadLine();

        Console.WriteLine("Finished");
    }
    */


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

// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using OllaCSharpApp;

var ollamaPath = @"E:\dev\ollama\ollama.exe";

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


    using(ProcessExecutor executor = new ProcessExecutor(ollamaPath))
    {
        executor.Start();

        executor.Exit();

        Console.WriteLine("Finished");
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}

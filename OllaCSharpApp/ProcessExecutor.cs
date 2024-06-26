﻿using System.Diagnostics;

namespace OllaCSharpApp;

/// <summary>
/// Executes a specific process and controls the input and output
/// 
/// References: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.beginoutputreadline?view=net-8.0
/// https://jonathancrozier.com/blog/working-with-processes-using-c-sharp
/// </summary>
public class ProcessExecutor : IDisposable
{
    private string _processPath;
    private string _arguments;

    private Process? _process;

    private StreamWriter? _inputStreamWriter;
    private StreamReader? _outputStreamReader;

    public ProcessExecutor(string processPath, string arguments)
    {
        _processPath = processPath;
        _arguments = arguments;
    }

    public void Dispose()
    {
        if( _process != null )
        {
            _process.Dispose();
            _process = null;
        }
    }

    public void Start()
    {
        try
        {
            _process = new Process();

            _process.StartInfo.Arguments = _arguments;

            // Set UseShellExecute to false for redirection.
            _process.StartInfo.UseShellExecute = false;

            // You can start any process, HelloWorld is a do-nothing example.
            _process.StartInfo.FileName = _processPath;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardOutput = true;  

            // Set our event handler to asynchronously read the sort output.
            //_process.OutputDataReceived += OutputDataReceivedHandler;
            //_process.ErrorDataReceived += ErrorDataReceivedHandler;

            // Redirect standard input as well.  This stream is used synchronously.
            _process.StartInfo.RedirectStandardInput = true;
            
            _process.Start();

            // Use a stream writer to synchronously write the sort input.
            _inputStreamWriter = _process.StandardInput;
            _outputStreamReader = _process.StandardOutput;

            // Start the asynchronous read of the sort output stream.
            //_process.BeginOutputReadLine();
            
            // Read first anything from output before giving control to user
            //ReadFromOutput();
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException($"Failed to start process, {ex.Message}", ex);
        }
    }

    public void Exit()
    {
        try
        {
            _inputStreamWriter!.Close();

            // Wait for the process to write the sorted text lines.
            _process!.WaitForExit();

            _process.Close();
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException($"Failed to exit process, {ex.Message}", ex);
        }
    }

    public void WriteToProcess(string message)
    {
        Console.WriteLine($"[->]{message}");
        //_inputStreamWriter!.WriteLine(message);
        //_inputStreamWriter!.Flush();

        _process.StandardInput.WriteLine(message);

        // Read back the response
        ReadFromOutput();
    }

    private void ReadFromOutput()
    {
        //while( _outputStreamReader.)
        //var result = _outputStreamReader!.ReadToEnd();
        //Console.WriteLine($"[<-]{result}");
        //_process!.WaitForExit();
        _outputStreamReader = _process.StandardOutput;
        while (!_outputStreamReader!.EndOfStream)
        {
            var line = _outputStreamReader.ReadLine();
            Console.WriteLine(line);
        }
/*
        while( true )
        {
            var line = _outputStreamReader!.ReadLine();

            if( line == null )
            {
                break;
            }
        }*/

        //var result = _outputStreamReader!.ReadToEnd();
        //Console.WriteLine($"[<-]{result}");
    }

    private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        // Collect the sort command output.
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            Console.WriteLine($"[->]{outLine.Data}");
        }
    }

    private void ErrorDataReceivedHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        // Collect the sort command output.
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            Console.Error.WriteLine($"[->]{outLine.Data}");
        }
    }
}

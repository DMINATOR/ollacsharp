using System.Diagnostics;

namespace OllaCSharpApp;

/// <summary>
/// Executes a specific process and controls the input and output
/// 
/// References: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.beginoutputreadline?view=net-8.0
/// </summary>
public class ProcessExecutor : IDisposable
{
    private string _processPath;
    private Process? _process;

    private StreamWriter? _inputStreamWriter;

    public ProcessExecutor(string processPath)
    {
        _processPath = processPath;
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

            // Set UseShellExecute to false for redirection.
            _process.StartInfo.UseShellExecute = false;

            // You can start any process, HelloWorld is a do-nothing example.
            _process.StartInfo.FileName = _processPath;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardOutput = true;  

            // Set our event handler to asynchronously read the sort output.
            _process.OutputDataReceived += OutputDataReceivedHandler;

            // Redirect standard input as well.  This stream is used synchronously.
            _process.StartInfo.RedirectStandardInput = true;
            
            _process.Start();

            // Use a stream writer to synchronously write the sort input.
            _inputStreamWriter = _process.StandardInput;

            // Start the asynchronous read of the sort output stream.
            _process.BeginOutputReadLine();
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException($"Failed to start process, {ex.Message}", ex);
        }
      
        Console.ReadLine();

        Console.WriteLine("Finished");
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

    private void WriteToProcess(string message)
    {
        _inputStreamWriter!.Write(message);
    }

    private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        // Collect the sort command output.
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            Console.WriteLine($"[->]{outLine.Data}");
        }
    }
}

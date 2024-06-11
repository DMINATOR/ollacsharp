# Export

This section contains binaries that were exported with Godot for Windows.

The executable can be ran either as console or windows application.

## Notes

To fix the export error:

```text
C:\Program Files\dotnet\sdk\8.0.200\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ConflictResolution.targets(112,5): error NETSDK1152: Found multiple publish output files with the same relative path: 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\avx\llama.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\avx2\llama.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\avx512\llama.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\llama.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\avx\llava_shared.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\avx2\llava_shared.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\avx512\llava_shared.dll, 
C:\Users\kna_i\.nuget\packages\llamasharp.backend.cpu\0.13.0\runtimes\win-x64\native\llava_shared.dll. 
```

the following was added to the Project:

```xml
<ErrorOnDuplicatePublishOutputFiles>false</ErrorOnDuplicatePublishOutputFiles>
```
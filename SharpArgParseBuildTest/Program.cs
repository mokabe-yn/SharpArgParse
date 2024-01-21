using System;
class C
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
        Console.WriteLine("Can I compile single file: SharpArgParse.cs?");
        Console.WriteLine("Target Framework is 4.0 or 8.0");

        string version = System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion();
        Console.WriteLine($"Running framework version: {version}");
    }
}

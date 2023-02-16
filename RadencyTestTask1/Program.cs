using RadencyTestTask1;
using RadencyTestTask1.FileProcessing;

const string dir = "watch2"; 

Console.WriteLine("Hello, World!");
foreach (string arg in Environment.GetCommandLineArgs())  
{  
    Console.WriteLine(arg);  
} 

var fileProcessor = new FileProcessor("./watch2",ProcessingOptions.ReadContinously);
fileProcessor.Run();
using RadencyTestTask1.FileProcessing;
using RadencyTestTask1.Helpers;

var config = ConfigReader.ReadConfig();

var fileProcessor = new FileProcessor(config);
fileProcessor.Run();
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program {
    static void Main(string[] args) {
        // Read the arguments to remove and the file name from the config file
        string[] argumentsToRemove = ReadArgumentsToRemoveFromFile(out string fileName);

        // Remove the arguments to be excluded
        var filteredArgs = FilterArguments(args, argumentsToRemove);

        // Call the other file with the remaining arguments
        CallOtherFile(fileName, filteredArgs);
    }

    static string[] ReadArgumentsToRemoveFromFile(out string fileName) {
        string configFileName = $"{AppDomain.CurrentDomain.FriendlyName}.cfg";
        List<string> argumentsToRemove = new List<string>();
        fileName = "";

        if (File.Exists(configFileName)) {
            string[] lines = File.ReadAllLines(configFileName);
            foreach (var line in lines) {
                if (line.StartsWith("argumentsToRemove=")) {
                    string[] arguments = line.Substring("argumentsToRemove=".Length).Split(',');
                    argumentsToRemove.AddRange(arguments);
                } else if (line.StartsWith("fileName=")) {
                    fileName = line.Substring("fileName=".Length);
                }
            }
        }

        return argumentsToRemove.ToArray();
    }

    static string[] FilterArguments(string[] args, string[] argumentsToRemove) {
        var filteredArgs = new List<string>();

        foreach (var arg in args) {
            if (!argumentsToRemove.Contains(arg)) {
                filteredArgs.Add(arg);
            }
        }

        return filteredArgs.ToArray();
    }

    static void CallOtherFile(string fileName, string[] args) {
        // Build the arguments string
        string arguments = string.Join(" ", args);

        // Start the process
        Process.Start(fileName, arguments);
    }
}

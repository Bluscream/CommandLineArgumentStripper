using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program {
    static void Main(string[] args) {
        // Read the arguments to remove from the config file
        string[] argumentsToRemove = ReadArgumentsToRemoveFromFile();

        // Remove the arguments to be excluded
        var filteredArgs = FilterArguments(args, argumentsToRemove);

        // Call the other file with the remaining arguments
        CallOtherFile(filteredArgs);
    }

    static string[] ReadArgumentsToRemoveFromFile() {
        string configFileName = $"{AppDomain.CurrentDomain.FriendlyName}.cfg";
        List<string> argumentsToRemove = new List<string>();

        if (File.Exists(configFileName)) {
            string[] lines = File.ReadAllLines(configFileName);
            argumentsToRemove.AddRange(lines);
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

    static void CallOtherFile(string[] args) {
        // Replace "mysqld" with the actual file name and path
        string fileName = "mysqld";

        // Build the arguments string
        string arguments = string.Join(" ", args);

        // Start the process
        Process.Start(fileName, arguments);
    }
}

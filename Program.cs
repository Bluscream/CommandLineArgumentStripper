using System;
using System.Diagnostics;

class Program {
    static void Main(string[] args) {
        // Hardcoded list of arguments to remove
        string[] argumentsToRemove = new string[] { "--disable-partition-engine-check" };

        // Remove the arguments to be excluded
        var filteredArgs = FilterArguments(args, argumentsToRemove);

        // Call the other file with the remaining arguments
        CallOtherFile(filteredArgs);
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

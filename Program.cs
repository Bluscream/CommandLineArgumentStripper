using IniParser;
using IniParser.Model;
using System.Diagnostics;
using System.Windows.Forms;

class Program {
    private const string configFilePath = "config.ini";
    private const string sectionName = "General";
    static void Main(string[] _args) {
        var selfName = _args[0];
        var args = (_args.Length > 1) ? new string[_args.Length - 1] : new string[] { };
        if (!File.Exists(configFilePath)) {
            GenerateDefaultConfigFile(configFilePath, args);
            MessageBox.Show($"A default config.ini file has been generated. Please update the file with the appropriate values.\n\n{selfName}\n{string.Join('\n', args)}", "Config file not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(configFilePath);

        string[] argumentsToRemove = data[sectionName]["remove"].Split(',');
        string[] argumentsToAdd = data[sectionName]["add"].Split(',');
        string fileName = data[sectionName]["file"];

        // Remove the arguments to be excluded
        var filteredArgs = FilterArguments(args, argumentsToRemove);

        // Add the arguments to be included
        var finalArgs = AddArguments(filteredArgs, argumentsToAdd);

        // Call the other file with the final arguments
        CallOtherFile(fileName, finalArgs);
    }

    static void GenerateDefaultConfigFile(string filePath, string[] args) {
        IniData data = new IniData();
        data.Sections.AddSection(sectionName);
        data[sectionName].AddKey("remove", string.Join(",", args));
        data[sectionName].AddKey("add", string.Join(",", args));
        data[sectionName].AddKey("file", args[0]);

        var parser = new FileIniDataParser();
        parser.WriteFile(filePath, data);
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

    static string[] AddArguments(string[] args, string[] argumentsToAdd) {
        var finalArgs = new List<string>(args);
        finalArgs.AddRange(argumentsToAdd);
        return finalArgs.ToArray();
    }

    static void CallOtherFile(string fileName, string[] args) {
        // Build the arguments string
        string arguments = string.Join(" ", args);

        // Start the process
        Process.Start(fileName, arguments);
    }
}

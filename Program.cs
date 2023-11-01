using IniParser;
using IniParser.Model;
using IniParser.Parser;
using System.Diagnostics;

internal class Program {
    private static readonly string configFilePath = $"{AppDomain.CurrentDomain.FriendlyName}.ini";
    private static readonly string sectionName = Path.GetFileName(Environment.ProcessPath);
    private static FileIniDataParser iniParser = new();

    private static string[] AddArguments(string[] args, string[] argumentsToAdd) {
        var finalArgs = new List<string>(args);
        finalArgs.AddRange(argumentsToAdd);
        return finalArgs.ToArray();
    }

    private static void CallOtherFile(string fileName, string[] args) {
        // Build the arguments string
        string arguments = string.Join(" ", args);

        // Start the process
        Process.Start(fileName, arguments);
    }

    private static string[] FilterArguments(string[] args, string[] argumentsToRemove) {
        var filteredArgs = new List<string>();

        foreach (var arg in args) {
            if (!argumentsToRemove.Contains(arg)) {
                filteredArgs.Add(arg);
            }
        }

        return filteredArgs.ToArray();
    }
    private static IniData GenerateDefaultSection(IniData data, string[] args) {
        data.Sections.AddSection(sectionName);
        data[sectionName].AddKey("remove", string.Join(",", args));
        data[sectionName].AddKey("add", string.Join(",", args));
        data[sectionName].AddKey("file", sectionName);
        return data;
    }

    private static void GenerateDefaultConfigFile(string[] args) => WriteToConfigFile(GenerateDefaultSection(new IniData(), args));
    private static void WriteToConfigFile(IniData data) => iniParser.WriteFile(configFilePath, data);

    private static void ShowError(string[] args, string message, string title) => MessageBox.Show(
               $"{message}\n\n{sectionName}\n{string.Join('\n', args)}",
                      title,
                             MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
           );

    private static void Main(string[] args) {
        // split args into file and arguments but make it not error if there are no arguments
        // get current running exe name without path but with extension
        //args = args.Skip(1).ToArray();
        if (!File.Exists(configFilePath)) {
            GenerateDefaultConfigFile(args);
            return;
        }

        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(configFilePath);

        if (!data.Sections.ContainsSection(sectionName)) {
            WriteToConfigFile(GenerateDefaultSection(data, args));
            ShowError(args, $"The {sectionName} section was not found in the {configFilePath} file. Please update the file with the appropriate values.", "Config section not found!");
            return;
        }
        string fileName = data[sectionName]["file"];

        // Remove the arguments to be excluded
        if (data[sectionName].ContainsKey("remove")) {
            string[] argumentsToRemove = data[sectionName]["remove"].Split(',');
            args = FilterArguments(args, argumentsToRemove);
        }

        // Add the arguments to be included
        if (data[sectionName].ContainsKey("add")) {
            string[] argumentsToAdd = data[sectionName]["add"].Split(',');
            args = AddArguments(args, argumentsToAdd);
        }

        // Call the other file with the final arguments
        CallOtherFile(fileName, args);
    }
}
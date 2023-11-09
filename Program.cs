using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;

internal class Program {
    private static readonly string configFilePath = $"{AppDomain.CurrentDomain.FriendlyName}.ini";
    private static readonly string sectionName = Path.GetFileName(Environment.ProcessPath);
    private static FileIniDataParser iniParser = new();
    private static Process proxiedProcess;

    private static string[] AddArguments(string[] args, string[] argumentsToAdd) {
        var finalArgs = new List<string>(args);
        finalArgs.AddRange(argumentsToAdd);
        return finalArgs.ToArray();
    }

    private static void RunOtherFile(string fileName, string[] args) {
        string arguments = string.Join(" ", args);
        Process.Start(fileName, arguments);
    }

    private static void ProxyOtherFile(string fileName, string[] args) {
        string arguments = string.Join(" ", args);

        proxiedProcess = new Process();
        proxiedProcess.StartInfo = new() {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        proxiedProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        proxiedProcess.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
        proxiedProcess.Exited += (sender, e) => CloseCurrentProcess();
        proxiedProcess.Start();
        proxiedProcess.BeginOutputReadLine();
        proxiedProcess.BeginErrorReadLine();
        proxiedProcess.WaitForExit();
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
    private static void GenerateDefaultConfigFile(string[] args) => WriteToConfigFile(GenerateDefaultSection(new IniData(), args));

    private static IniData GenerateDefaultSection(IniData data, string[] args) {
        data.Sections.AddSection(sectionName);
        data[sectionName].AddKey("remove", string.Join(",", args));
        data[sectionName].AddKey("add", string.Join(",", args));
        data[sectionName].AddKey("file", sectionName);
        return data;
    }
    private static void Main(string[] args) {
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
        if (data[sectionName].ContainsKey("remove")) {
            string[] argumentsToRemove = data[sectionName]["remove"].Split(',');
            args = FilterArguments(args, argumentsToRemove);
        }
        if (data[sectionName].ContainsKey("add")) {
            string[] argumentsToAdd = data[sectionName]["add"].Split(',');
            args = AddArguments(args, argumentsToAdd);
        }
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => CloseCurrentProcess();
        ProxyOtherFile(fileName, args);
    }

    private static void ShowError(string[] args, string message, string title) => MessageBox.Show(
        $"{message}\n\n{sectionName}\n{string.Join('\n', args)}", title, MessageBoxButtons.OK, MessageBoxIcon.Error);

    private static void WriteToConfigFile(IniData data) => iniParser.WriteFile(configFilePath, data);

    private static void CloseCurrentProcess() {
        if (proxiedProcess != null) {
            if (!proxiedProcess.HasExited) proxiedProcess.Kill();
            Process.GetProcessesByName(proxiedProcess.StartInfo.FileName).ToList().ForEach(p => p.Kill());
        }
        Environment.Exit(0);
    }
}
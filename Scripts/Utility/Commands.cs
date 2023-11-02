using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Godot;

public struct CommandArguments
{
    public readonly Console CallingConsole;
    public readonly SceneTree Tree;
    public readonly string[] Args;

    public CommandArguments(Console callingConsole, SceneTree tree, string[] args)
    {
        this.CallingConsole = callingConsole;
        this.Tree = tree;
        this.Args = args;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string[] Names;

    public CommandAttribute(string[] commandNames)
    {
        Names = commandNames;
    }

    public CommandAttribute(string commandName)
    {
        Names = new[] { commandName };
    }
}

public delegate bool ArgConverter(string input, out object result);

public struct CommandArg
{
    public string Name, Typename;
    public bool HasDefault = false;
    public object Default = null;
    public ArgConverter ArgConverter;

    public CommandArg(string name, string typename, bool hasDefault, ArgConverter argConverter)
    {
        Name = name;
        Typename = typename;
        HasDefault = hasDefault;
        ArgConverter = argConverter;
    }
}

public class Command
{
    public MethodInfo Method;
    public List<CommandArg> Args;
    
    public string Usage { get; private set; }
    
    public void GenerateUsage(string[] names)
    {
        List<string> elements = new();
        elements.Add(string.Join("/", names));
        foreach (var arg in Args)
            elements.Add($"{(arg.HasDefault ? "[" : "<")}{arg.Name}:{arg.Typename}{(arg.HasDefault ? "]" : ">")}");
        Usage = $"{string.Join(" ", elements)}";
    }
}

public class Commands
{
    public static Commands Instance = new();
    public static Dictionary<Type, ArgConverter> ArgConverters;
    
    private Dictionary<string, Command> _commands = new();

    public Commands()
    {
        RegisterArgConverters();
        FindCommands();
    }

    private void RegisterArgConverters()
    {
        ArgConverters = new()
        { { typeof(int), (string input, out object result) =>
        {
            var success = Int32.TryParse(input, out var intResult);
            result = intResult;
            return success;
        } } };
    }
    
    private void FindCommands()
    {
        Stopwatch sw = new();
        sw.Start();
        
        _commands.Clear();
        
        foreach(var type in Assembly.GetExecutingAssembly().GetTypes()) 
        {
            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute is null)
                    continue;
                
                if (!method.IsStatic)
                    throw new Exception("Command functions must be static.");
                
                if (method.ReturnType != typeof(bool))
                    throw new Exception("Command functions must return bool.");
                
                var methodParams = method.GetParameters();
                if (methodParams.Length == 0)
                    throw new Exception("Command function must have at least one argument (of type CommandArgs).");
                if (methodParams[0].ParameterType != typeof(CommandArguments))
                    throw new Exception("The first parameter of a command argument must be of type CommandArgs.");

                List<CommandArg> args = new();
                for (int i = 1; i < methodParams.Length; i++)
                {
                    var param = methodParams[i];
                    if (param.ParameterType != typeof(string) && !ArgConverters.ContainsKey(param.ParameterType))
                        throw new Exception($"Argument \"{param.Name}\" in method {type.Name}.{method.Name} doesn't have a registered converter.");
                    
                    CommandArg arg;
                    arg.Name = param.Name;
                    arg.Typename = param.ParameterType.Name;
                    arg.HasDefault = param.HasDefaultValue;
                    arg.Default = param.DefaultValue;
                    arg.ArgConverter = param.ParameterType != typeof(string) ? ArgConverters[param.ParameterType] : null;
                    args.Add(arg);
                }

                Command cmd = new()
                {
                    Method = method,
                    Args = args
                };
                cmd.GenerateUsage(attribute.Names);
                foreach (string name in attribute.Names)
                    _commands.Add(name, cmd);
            }
        }

        sw.Stop();
        GD.Print($"Took {sw.ElapsedMilliseconds}ms to find commands.");
    }

    [Command(new[] { "quit", "exit" })]
    public static bool Quit(CommandArguments args)
    {
        args.Tree.Quit();
        return true;
    }

    [Command("help")]
    public static bool Help(CommandArguments args, string command = "")
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            if (Commands.Instance._commands.TryGetValue(command, out var cmd))
                args.CallingConsole?.WriteLine(cmd.Usage);
            else
            {
                args.CallingConsole?.WriteError($"Couldn't find command \"{command}\"");
                return false;
            }
        }
        else
        {
            args.CallingConsole?.WriteLine("Commands:");
            foreach (var cmd in Commands.Instance._commands)
                args.CallingConsole?.WriteLine($"    {cmd.Key}: {cmd.Value.Usage}");
        }
        return true;
    }

    [Command("toggle_console")]
    public static bool ToggleConsole(CommandArguments args)
    {
        args.CallingConsole?.ToggleConsole();
        return true;
    }

    [Command("set_vsync_mode")]
    public static bool VsyncMode(CommandArguments args, int mode)
    {
        if (mode < 0 || mode > 3)
            args.CallingConsole.WriteError("Mode should be between 0 and 3, inclusive.");
        else
            DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)mode);
        return true;
    }

    [Command("set_console_font_size")]
    public static bool SetConsoleFontSize(CommandArguments args, int newSize)
    {
        if (newSize < 6)
            args.CallingConsole.WriteError("The console font size can't be less than 6.");
        else if (newSize > 50)
            args.CallingConsole.WriteError("The console font size can't be more than 50.");
        else
        {
            args.CallingConsole.SetFontSize(newSize);
            args.CallingConsole.WriteLine($"Set console font size to {newSize}.");
            return true;
        }
        
        return false;
    }

    [Command("play_game")]
    public static bool PlayGame(CommandArguments args)
    {
        args.Tree.ChangeSceneToFile("res://Scenes/Game.tscn");
        return true;
    }

    [Command("toggle_fps")]
    public static bool ToggleFPS(CommandArguments args)
    {
        var fpsCounter = args.Tree.Root.GetNode<Globals>("Globals").fpsCounter;
        fpsCounter.Visible = !fpsCounter.Visible;
        return true;
    }
    
    public bool Call(string command, string[] args, Console console)
    {
        if (!_commands.ContainsKey(command))
        {
            console.WriteError($"Unknown command: \"{command}\"");
            return false;
        }

        var commandObj = _commands[command];
        CommandArguments argsObject = new(console, console.GetTree(), args);
        List<object> argArray = new();
        argArray.Add(argsObject);
        
        for (int i = 0; i < commandObj.Args.Count; i++)
        {
            if (i >= args.Length)
            {
                if (commandObj.Args[i].HasDefault)
                    argArray.Add(commandObj.Args[i].Default);
                else
                {
                    console.WriteError($"Not enough arguments. Usage: {commandObj.Usage}");
                    return false;
                }
            } else if (commandObj.Args[i].ArgConverter is null) // Should be a string arg
            {
                argArray.Add(args[i]);
                continue;
            }
            else
            {
                bool success = commandObj.Args[i].ArgConverter(args[i], out object result);
                if (success)
                    argArray.Add(result);
                else
                {
                    console.WriteError($"Failed to parse \"{args[i]}\" as {commandObj.Args[i].Typename}.");
                    console.WriteError($"Usage: {commandObj.Usage}");
                    return false;
                }
            }
        }
        
        return (bool) commandObj.Method.Invoke(null, argArray.ToArray())!;
    }

    [Command("clear")]
    public static bool Clear(CommandArguments args)
    {
        args.CallingConsole?.Clear();
        return true;
    }

    [Command("echo")]
    public static bool Echo(CommandArguments args)
    {
        Console.Instance.WriteLine(args.Args.Join(" "));
        return true;
    }

    [Command("print_version")]
    public static bool PrintVersions(CommandArguments args)
    {
        args.CallingConsole?.WriteLine(ProjectSettings.GetSetting("application/config/name").AsString());
        args.CallingConsole?.WriteLine($"On Godot {Engine.GetVersionInfo()["string"]}");
        return true;
    }

    [Command("reload_commands")]
    public static bool ReloadCommands(CommandArguments args)
    {
        Stopwatch sw = new();
        sw.Start();
        Instance.FindCommands();
        sw.Stop();
        args.CallingConsole?.WriteLine($"Took {sw.ElapsedMilliseconds}ms to reload commands.");
        return true;
    }
    
    public bool IsCommandNameValid(string name) => !name.Contains(' ');
    
    public void AddCommand(string name, Command command)
    {
        if (!IsCommandNameValid(name))
        {
            Console.Instance.WriteError($"\"{name}\" is an invalid name for a command.");
            return;
        }

        if (_commands.ContainsKey(name))
        {
            Console.Instance.WriteError($"\"{name}\" is already a command.");
            return;
        }
        
        _commands.Add(name, command);
    }

    public void AddCommand(string[] names, Command command)
    {
        foreach (string name in names)
            AddCommand(name, command);
    }
}

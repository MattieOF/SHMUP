using System;
using System.Linq;
using Godot;

public partial class Console : Window
{
	public static Console Instance;
	
	[Export] public RichTextLabel output;
	[Export] public ConsoleLineEdit inputBox;
	
	private bool _open = false;
	public bool Open => _open;
	
	public override void _Ready()
	{
		if (Instance is not null)
			Instance.QueueFree();
		Instance = this;
		
		CloseRequested += () => SetOpen(false);
		inputBox.TextSubmitted += CommandEntered;
		Visible = false;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("toggle_console") && !Input.IsKeyPressed(Key.Shift))
			SetOpen(!_open);
	}

	public void SetOpen(bool open)
	{
		if (!open)
			inputBox.ReleaseFocus();
		
		_open = open;
		Visible = _open;
		
		if (open)
			inputBox.GrabFocus();
	}
	
	public void ToggleConsole() => SetOpen(!_open);

	public void Clear()
	{
		output.Text = "";
		output.Clear();
	}

	public void WriteLine(string line)
	{
		WriteLine(line, Colors.White);
	}

	public void WriteError(string msg)
	{
		WriteLine(msg, Colors.Red);
		GD.PrintErr(msg);
	}
	
	public void WriteLine(string line, Color color)
	{
		if (output.GetVScrollBar().MaxValue - output.GetVScrollBar().Value - 1 <= output.GetVScrollBar().Page)
			output.ScrollFollowing = true;
		else
			output.ScrollFollowing = false;
		output.Text += $"[color=#cccccc][b][{DateTime.Now:HH:mm:ss.fff}][/b][/color] [color=#{color.ToHtml()}]{line}[/color]\n";
	}

	public void CommandEntered(string command)
	{
		if (string.IsNullOrWhiteSpace(command))
			return;
		
		command = command.Trim();
		string[] tokens = command.Split(" ");
		if (Commands.Instance.GetCommand(tokens[0], out var commandFunc))
			commandFunc(new(this, GetTree(), tokens.Skip(1).ToArray()));
		else
			WriteLine($"Unknown command: \"{command}\"", Colors.Red);

		inputBox.Clear();
	}

	public void SetFontSize(int fontSize)
	{
		output.AddThemeFontSizeOverride("normal_font_size", fontSize);
		output.AddThemeFontSizeOverride("bold_font_size", fontSize);
		output.AddThemeFontSizeOverride("italics_font_size", fontSize);
		output.AddThemeFontSizeOverride("bold_italics_font_size", fontSize);
		output.AddThemeFontSizeOverride("mono_font_size", fontSize);
	}
}

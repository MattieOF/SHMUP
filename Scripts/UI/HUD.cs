using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[ExportCategory("Level Bar")]
	[Export] public ProgressBar LevelBar;
	[Export] public Label CurrentLevel, NextLevel;

	public void SetLevel(int level)
	{
		CurrentLevel.Text = $"LVL {level}";
		NextLevel.Text = $"{level + 1}";
	}

	public void SetLevelProgress(float progress) => LevelBar.Value = progress;
}

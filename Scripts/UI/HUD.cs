using Godot;

public partial class HUD : CanvasLayer
{
	[ExportCategory("Level Bar")]
	[Export] public ProgressBar LevelBar;
	[Export] public Label CurrentLevel, NextLevel;

	public void SetLevel(float level)
	{
		var floored = Mathf.FloorToInt(level);
		SetLevelNumber(floored);
		SetLevelProgress(level - floored);
	}
	
	public void SetLevelNumber(int level)
	{
		CurrentLevel.Text = $"LVL {level}";
		NextLevel.Text = $"{level + 1}";
	}

	public void SetLevelProgress(float progress) => LevelBar.Value = progress;
}

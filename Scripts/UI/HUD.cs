using Godot;

public partial class HUD : CanvasLayer
{
	[ExportCategory("Level Bar")] 
	[Export] public ProgressBar LevelBar;
	[Export] public Label CurrentLevel, NextLevel;

	[ExportCategory("Bridge UI")] 
	[Export] public Panel BridgePanelUI;
	[Export] public HBoxContainer BridgePanelContainer;
	
	[ExportCategory("Other")]
	[Export] public ProgressBar HealthBar;

	private static PackedScene _resourceNotif = GD.Load<PackedScene>("res://Scenes/UI/ResourceNotif.tscn");

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

	public void UpdateHealthBar(float percent)
	{
		HealthBar.Value = percent;
		var fill = HealthBar.GetThemeStylebox("fill");
		(fill as StyleBoxFlat)!.BgColor = Colors.Red.Lerp(Colors.Green, percent);
		HealthBar.AddThemeStyleboxOverride("fill", fill);
	}

	public void HideBridgeUI() => BridgePanelUI.Visible = false;

	public void ShowBridgeUI(BridgeTrigger bridge)
	{
		foreach (var child in BridgePanelContainer.GetChildren())
			child.QueueFree();

		foreach (var resource in bridge.Requirements)
		{
			var ui = _resourceNotif.Instantiate() as ResourceNotif;
			ui!.SetItemAndAmount(resource.Key, resource.Value, false, true, false);
			BridgePanelContainer.AddChild(ui);
		}
		
		BridgePanelUI.Visible = true;
	}
}

using Godot;
using Godot.Collections;

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
	[Export] public VBoxContainer ResourcesContainer;
	[Export] public Array<ItemData> TrackedResources;

	private Dictionary<ItemData, ResourceNotif> _resourceNotifs = new();

	private static PackedScene _resourceNotif = GD.Load<PackedScene>("res://Scenes/UI/ResourceNotif.tscn");

	public override void _Ready()
	{
		var resourceUI = GD.Load<PackedScene>("res://Scenes/UI/ResourceNotif.tscn");
		foreach (var itemType in TrackedResources)
		{
			var ui = resourceUI.Instantiate<ResourceNotif>();
			ui.Initialise(itemType, 0, false, false, false);
			ui.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
			ResourcesContainer.AddChild(ui);
			_resourceNotifs.Add(itemType, ui);
		}
	}

	public void UpdateResourceUI(Player player)
	{
		foreach (var itemType in TrackedResources)
			_resourceNotifs[itemType].SetAmount(player.Inventory.Get(itemType), false);
	}

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
			ui!.Initialise(resource.Key, resource.Value, false, true, false);
			BridgePanelContainer.AddChild(ui);
		}
		
		BridgePanelUI.Visible = true;
	}
}

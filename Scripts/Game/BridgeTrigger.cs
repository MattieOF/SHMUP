using Godot;
using Godot.Collections;

public partial class BridgeTrigger : Area2D
{
	[Export] public TileMap TileMap;
	[Export] public int TilemapLayer;
	[Export] public Dictionary<ItemData, int> Requirements;
	[Export] public AudioStream FailSound, PlaceSound;

	private Player _player;
	private bool _inTrigger;
	
	public override void _Ready()
	{
		BodyEntered += body =>
		{
			_player = body as Player;
			if (_player is null)
				return;
			_inTrigger = true;
			_player.HUD.ShowBridgeUI(this);
		};

		BodyExited += body =>
		{
			if (_player == body)
			{
				_inTrigger = false;
				_player.HUD.HideBridgeUI();
				_player = null;
			}
		};
	}

	public override void _Process(double delta)
	{
		if (_inTrigger)
		{
			if (Input.IsActionJustPressed("build"))
			{
				// Check we have the items
				foreach (var requirement in Requirements)
				{
					if (!_player.Inventory.Has(requirement.Key, requirement.Value))
					{
						this.PlaySoundUI(FailSound);
						return;
					}
				}

				// Use the items
				foreach (var requirement in Requirements)
					_player.Inventory.Remove(requirement.Key, requirement.Value);
				
				TileMap.SetLayerEnabled(TilemapLayer, true);
				this.PlaySoundUI(PlaceSound, GetNode("/root/Game"));
				
				QueueFree();
			}
		}
	}
}

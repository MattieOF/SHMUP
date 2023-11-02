using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
	[Export] public CharacterData Data;
	[Export] public Area2D PickupArea;
	[Export] public HUD HUD;
	[Export] public ItemData[] Gems;
	[Export] public ItemData[] Resources;

	public Inventory Inventory = new();
	
	public int XP { get; private set; }

	private AnimatedSprite2D _sprite;
	private PlayerCamera _camera;
	private Vector2 _movement, _targetCamOffset;
	private bool _movingLastFrame;
	
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		_sprite.SpriteFrames = Data.Sprite;
		_sprite.Animation = "down";
		_sprite.Stop();

		_camera = GetNode<PlayerCamera>("Camera");

		PickupArea.AreaEntered += area => (area as Pickup)?.PickUp(this);
	}

	public override void _Process(double delta)
	{
		// Get input
		int horizontal = (Input.IsActionPressed("right") ? 1 : 0) - (Input.IsActionPressed("left") ? 1 : 0);
		int vertical = (Input.IsActionPressed("down") ? 1 : 0) - (Input.IsActionPressed("up") ? 1 : 0);

		// Calculate movement
		_movement = new Vector2(horizontal, vertical);
		_movement = _movement.Normalized();
		var movementLength = _movement.LengthSquared();
		
		// Update camera look-ahead
		if (movementLength != 0)
			_targetCamOffset = _movement * Globals.Instance.LookAheadDistance;
		_camera.OffsetFromTarget = Utility.MoveToward(_camera.OffsetFromTarget, _targetCamOffset, Globals.Instance.LookAheadSpeed * (float)delta);
		
		// Select animation
		if (movementLength == 0)
		{
			if (_movingLastFrame)
				_sprite.Play(_sprite.Animation.ToString()!.Replace("move_", ""));
			_movingLastFrame = false;
		}
		else if (Mathf.Abs(_movement.X) > Mathf.Abs(_movement.Y))
		{
			_sprite.Play(_movement.X < 0 ? "move_left" : "move_right");
			_movingLastFrame = true;
		}
		else
		{
			_sprite.Play(_movement.Y < 0 ? "move_up" : "move_down");
			_movingLastFrame = true;
		}

		if (Input.IsActionJustPressed("gem_test"))
			(GetNode("/root/Game") as Node2D).SpawnItem(Gems[Utility.RNG.RandiRange(0, Gems.Length - 1)], _camera.GetGlobalMousePosition());
		if (Input.IsActionPressed("res_test"))
			(GetNode("/root/Game") as Node2D).SpawnItem(Resources[Utility.RNG.RandiRange(0, Resources.Length - 1)], _camera.GetGlobalMousePosition());
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Velocity = _movement * Data.MoveSpeed * 50 * (float)delta;
		MoveAndSlide();
	}

	public void AddXP(int xp)
	{
		XP += xp;
		HUD.SetLevel(Data.XPToLevelData.GetLevel(XP));
	}
}

public class Inventory
{
	private Dictionary<ItemData, int> _inv = new();

	public void Add(ItemData itemType, int amount)
	{
		if (_inv.ContainsKey(itemType))
			_inv[itemType] += amount;
		else
			_inv.Add(itemType, amount);
	}

	public void Remove(ItemData itemType, int amount)
	{
		if (!_inv.ContainsKey(itemType))
			return;

		_inv[itemType] = _inv[itemType] - amount;

		if (_inv[itemType] <= 0)
			_inv.Remove(itemType);
	}

	public int Get(ItemData itemType) => _inv.ContainsKey(itemType) ? _inv[itemType] : 0;

	public bool HasAny(ItemData itemType) => _inv.ContainsKey(itemType);

	[Command("print_inv")]
	public static bool PrintInventory(CommandArguments args)
	{
		var player = args.Tree.GetFirstNodeInGroup("Player") as Player;

		if (player is null)
		{
			args.CallingConsole.WriteError($"No player currently exists!");
			return false;
		}

		if (player.Inventory._inv.Count == 0)
		{
			args.CallingConsole.WriteLine($"The players inventory is empty.");
			return true;
		}
        
        args.CallingConsole.WriteLine("Inventory:");	
		foreach (var item in player.Inventory._inv)
		{
			args.CallingConsole.WriteLine($"	{item.Key.Name}: {item.Value}");	
		}
		
        return true;
	}
}

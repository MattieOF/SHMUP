using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public EnemyData Data;
	[Export] public AnimatedSprite2D Sprite;

	public float Health;

	public override void _Ready()
	{
		SetData(Data);
	}

	public void SetData(EnemyData data)
	{
		if (Data == data)
			return;
		
		Data = data;

		if (data is null)
		{
			Console.Instance.WriteError($"Enemy \"{Name}\" has null data!");
			return;
		}
		
		Health = data.MaxHP;
		Sprite.SpriteFrames = data.Sprite;
	}

	public virtual void Die()
	{
		// Roll and spawn loot
		var gemDrops = Data.GemDrop.Roll();
		var itemDrops = Data.ItemDrop.Roll();
		var root = (GetNode("/root/Game") as Node2D)!;
        root.SpawnItems(gemDrops, GlobalPosition, new Vector2(5, 5));
        root.SpawnItems(itemDrops, GlobalPosition, new Vector2(5, 5));
        
		QueueFree();
	}

	[Command("kill_all")]
	public static bool KillAll(CommandArguments args)
	{
		var enemies = args.Tree.GetNodesInGroup("Enemy");
		foreach (var enemy in enemies)
			((Enemy)enemy).Die();

		args.CallingConsole.WriteLine(enemies.Count == 0 ? "No enemies to kill!" : $"Killed {enemies.Count} enemies. You monster.");
		return true;
	}
}

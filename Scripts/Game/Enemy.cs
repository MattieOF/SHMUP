using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public EnemyData Data;
	[Export] public AnimatedSprite2D Sprite;
	[Export] public NavigationAgent2D NavAgent;

	public float Health;

	public override void _Ready()
	{
		SetData(Data);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 dir;
		NavAgent.TargetPosition = GetGlobalMousePosition();
		dir = (NavAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = Velocity.Lerp(dir * Data.MoveSpeed, Data.TurnSpeed * (float)delta);

		if (Mathf.Abs(Velocity.X) < Mathf.Abs(Velocity.Y))
		{
			Sprite.Play("vertical");
			Sprite.FlipH = false;
			Sprite.FlipV = Velocity.Y > 0;
		}
		else
		{
			Sprite.Play("horizontal");
			Sprite.FlipH = Velocity.X > 0;
			Sprite.FlipV = false;
		}
		
		MoveAndSlide();
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
		var scale = Utility.RNG.RandfRange(data.ScaleRange.X, data.ScaleRange.Y);
		Scale = new Vector2(scale, scale);
	}

	public virtual void Die()
	{
		// Roll and spawn loot
		var gemDrops = Data.GemDrop.Roll();
		var itemDrops = Data.ItemDrop.Roll();
		var root = (GetNode("/root/Game") as Node2D)!;
        root.SpawnItems(gemDrops, GlobalPosition, new Vector2(50, 50));
        root.SpawnItems(itemDrops, GlobalPosition, new Vector2(50, 50));
        
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

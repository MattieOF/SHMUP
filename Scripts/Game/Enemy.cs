using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public EnemyData Data;
	[Export] public AnimatedSprite2D Sprite;
	[Export] public NavigationAgent2D NavAgent;
	[Export] public Area2D PlayerDetector;

	public float Health;

	private Player _moveTarget, _target;
	private float _attackCooldown;
	private PackedScene _damageNumberScene = GD.Load<PackedScene>("res://Scenes/UI/DamageNumber.tscn");

	public override void _Ready()
	{
		_moveTarget = GetTree().GetFirstNodeInGroup("Player") as Player;
		
		PlayerDetector.BodyEntered += player => _target = player as Player;
		PlayerDetector.BodyExited += player =>
		{
			if ((player as Player) == _target)
				_target = null;
		};
		
		SetData(Data);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 dir;
		NavAgent.TargetPosition = _moveTarget.Alive ? _moveTarget.GlobalPosition : GetGlobalMousePosition();
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

	public override void _Process(double delta)
	{
		Sprite.Modulate = Utility.MoveToward(Sprite.Modulate, Colors.White, (float)delta * 2);
		
		if (_target is not null)
		{
			if (_attackCooldown > 0)
				_attackCooldown -= (float)delta;
			else
				Attack();
		}
	}

	public virtual void Attack()
	{
		_target.Hurt(Data.BaseDamage);
		_attackCooldown = Data.AttackCooldown;
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
		(GetNode<CollisionShape2D>("AttackRange/AttackShape").Shape as CircleShape2D)!.Radius = data.AttackRadius;
	}

	public virtual void Hurt(float dmg)
	{
		var dmgNumber = _damageNumberScene.Instantiate() as DamageNumber;
		dmgNumber!.Velocity = new Vector2(Utility.RNG.RandfRange(-60, 60), Utility.RNG.RandfRange(-60, -20));
		dmgNumber.AngularVelocity = Utility.RNG.RandfRange(-80, 80);
		dmgNumber.Color = dmg < 0 ? Colors.Green : Colors.Red;
		dmgNumber.Text = (-dmg).ToString("F1"); // Abs so bodged healing via negative dmg doesn't show as negative
		AddChild(dmgNumber);
		dmgNumber.GlobalPosition = GlobalPosition;
		
		this.PlaySound2D(Data.HurtSound, GetNode("/root/Game"));
		
		Health -= dmg;
		Sprite.Modulate = Colors.Red;
		if (Health <= 0)
			Die();
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

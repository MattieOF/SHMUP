using Godot;

public partial class GameManager : Node2D
{
	[Export] public float SpawnCooldown = 0.25f;
	[Export] public Vector2 SpawnCountRange = new(1, 3);
	[Export] public int MaxSpawns = 50;
	[Export] public EnemyData Enemy;
	[Export] public TileMap Map;

	private float _spawnCooldown;
	private Player _player;

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
	}

	public override void _Process(double delta)
	{
		if (!_player.Alive)
			return;
		
		_spawnCooldown -= (float)delta;
		if (_spawnCooldown <= 0)
		{
			int spawned = GetTree().GetNodesInGroup("Enemy").Count;
			var count = Utility.RNG.RandiRange((int)SpawnCountRange.X, (int)SpawnCountRange.Y);
			_spawnCooldown = SpawnCooldown;

			for (int i = 0; i < count; i++)
			{
				if (spawned >= MaxSpawns)
					return;

				var viewRect = _player.Camera.GetViewportRect().Grow(100);
				viewRect.Position = _player.GlobalPosition - (viewRect.Size / 2);
				
				var tries = 5;
				while (tries > 0)
				{
					var pos = viewRect.GetPointAlongPerimeter(Utility.RNG.Randf());
					var tile = Map.GetCellTileData(0, Map.LocalToMap(Map.ToLocal(pos)));
					
					if (tile is not null && tile.GetCustomData("Spawnable").AsBool() && !viewRect.HasPoint(pos))
					{
						this.SpawnEnemy(Enemy, pos);
						break;
					}

					tries--;
				}
				
				spawned++;
			}
		}
	}
}

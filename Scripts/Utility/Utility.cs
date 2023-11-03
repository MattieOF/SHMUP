using Godot;

public static class Utility
{
    public static readonly RandomNumberGenerator RNG = new();
    
    public static AudioStream ChooseRandom(AudioStream[] streams) => streams[RNG.RandiRange(0, streams.Length - 1)];

    public static void PlaySound2D(this Node2D node, AudioStream stream)
    {
        AudioStreamPlayer2D punchPlayer = new AudioStreamPlayer2D();
        node.AddChild(punchPlayer);
        punchPlayer.Stream = stream;
        punchPlayer.Play();
        punchPlayer.Finished += () => punchPlayer.QueueFree();
    }

    public static float EaseInExpo(float value, float expo)
    {
        return value == 0 ? 0 : Mathf.Pow(2, expo * value - expo);
    }

    public static bool CoinFlip(float chance = 0.5f) => RNG.Randf() <= chance;

    public static Vector2 MoveToward(Vector2 start, Vector2 target, float delta)
    {
        start.X = Mathf.MoveToward(start.X, target.X, delta);
        start.Y = Mathf.MoveToward(start.Y, target.Y, delta);
        return start;
    }
    
    public static Vector2 MoveToward(Vector2 start, Vector2 target, Vector2 delta)
    {
        start.X = Mathf.MoveToward(start.X, target.X, delta.X);
        start.Y = Mathf.MoveToward(start.Y, target.Y, delta.Y);
        return start;
    }

    public static Pickup SpawnItem(this Node2D root, ItemData item, Vector2 location)
    {
        var pickup = item.PickupScene.Instantiate();

        if (item.PickupClassOverride is not null)
        {
            var id = pickup.GetInstanceId();
            pickup.SetScript(item.PickupClassOverride);
            pickup = GodotObject.InstanceFromId(id) as Node;
        }

        var asPickup = pickup as Pickup;
        
        root.AddChild(pickup);
        asPickup!.SetItem(item);
        asPickup.GlobalPosition = location;
        return asPickup;
    }

    public static Enemy SpawnEnemy(this Node2D root, EnemyData enemy, Vector2 location)
    {
        var enemyObj = enemy.Scene.Instantiate();

        if (enemy.EnemyClassOverride is not null)
        {
            var id = enemyObj.GetInstanceId();
            enemyObj.SetScript(enemy.EnemyClassOverride);
            enemyObj = GodotObject.InstanceFromId(id) as Node;
        }

        var asEnemy = enemyObj as Enemy;
        
        root.AddChild(enemyObj);
        asEnemy!.SetData(enemy);
        asEnemy.GlobalPosition = location;
        return asEnemy;
    }
}

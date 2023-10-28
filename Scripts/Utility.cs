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
}

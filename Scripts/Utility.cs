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
}

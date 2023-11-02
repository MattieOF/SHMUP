using System;
using Godot;

// Thanks to https://kidscancode.org/godot_recipes/3.x/2d/screen_shake/index.html

[GlobalClass]
public partial class PlayerCamera : Camera2D
{
    [Export] public Vector2 OffsetFromTarget;
    
    [ExportCategory("Camera Shake")] 
    [Export(PropertyHint.Range, "0.01,2,")] public float ShakeDecay = 0.8f;
    [Export] public Vector2 MaxShakeOffset = new(20, 15);
    [Export] public float MaxShakeRotation = 0.1f;
    
    private float _trauma, _traumaExponent = 2;
    private int _noiseY = 0;
    private FastNoiseLite _noise = new();

    public override void _Ready()
    {
        _noise.Seed = (int)Utility.RNG.Randi();
        _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        _noise.Frequency = 0.01f;
        _noise.FractalOctaves = 2;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 shakeOffset = new();
        if (_trauma > 0)
        {
            _noiseY++;
            _trauma = Mathf.Max(_trauma - ShakeDecay * (float)delta, 0);
            var shakeAmount = Mathf.Pow(_trauma, _traumaExponent);
            Rotation = MaxShakeRotation * shakeAmount * _noise.GetNoise2D(_noise.Seed, _noiseY);
            shakeOffset.X = MaxShakeOffset.X * shakeAmount * _noise.GetNoise2D(_noise.Seed * 2, _noiseY);
            shakeOffset.Y = MaxShakeOffset.Y * shakeAmount * _noise.GetNoise2D(_noise.Seed * 3, _noiseY);
        }
        Offset = OffsetFromTarget + shakeOffset;
    }

    public void AddTrauma(float value)
    {
        _trauma = Mathf.Max(_trauma + value, 1);
    }
}

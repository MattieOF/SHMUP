using Godot;

public partial class DamageNumber : Label
{
	public Vector2 Velocity;
	public float AngularVelocity;
	public Color Color = Colors.White;

	public override void _Ready()
	{
		var tween = CreateTween();
		tween.TweenMethod(Callable.From<float>(alpha =>
		{
			Color.A = alpha;
			AddThemeColorOverride("font_color", Color);
		}), Variant.From(1.0f), Variant.From(0.0f), 1.5f);
		tween.TweenCallback(Callable.From(QueueFree)).SetDelay(1.5f);
	}

	public override void _Process(double delta)
	{
		GlobalPosition += (Velocity * (float) delta);
		RotationDegrees += AngularVelocity * (float) delta;
		Velocity -= Velocity * (float) delta * 0.5f;
		AngularVelocity -= AngularVelocity * (float) delta * 0.5f;
	}
}

using Godot;

public partial class ResourceNotif : HBoxContainer
{
	public Vector2 Velocity;
	public float AngularVelocity;
	public Color Color = Colors.White;

	[Export] public TextureRect Icon;
	[Export] public Label Amount, ItemName;
	
	public override void _Ready()
	{
		var tween = CreateTween();
		tween.TweenMethod(Callable.From<float>(alpha =>
		{
			Color.A = alpha;
			var iconModulate = Icon.Modulate;
			iconModulate.A = alpha;
			Icon.Modulate = iconModulate;
			Amount.AddThemeColorOverride("font_color", Color);
			ItemName.AddThemeColorOverride("font_color", Color);
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
	
	public void SetItemAndAmount(ItemData item, int amount)
	{
		Icon.Texture = item.Icon;
		Amount.Text = $"+{amount}";
		ItemName.Text = item.Name;
	}
}

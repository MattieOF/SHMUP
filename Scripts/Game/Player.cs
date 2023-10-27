using Godot;

public partial class Player : AnimatedSprite2D
{
	[Export] public CharacterData Data;
	
	public override void _Ready()
	{
		SpriteFrames = Data.Sprite;
		Animation = "down";
		Stop();
	}

	public override void _Process(double delta)
	{
		// Get input
		int horizontal = (Input.IsActionPressed("right") ? 1 : 0) - (Input.IsActionPressed("left") ? 1 : 0);
		int vertical = (Input.IsActionPressed("down") ? 1 : 0) - (Input.IsActionPressed("up") ? 1 : 0);

		Vector2 movement = new Vector2(horizontal, vertical);
		movement = movement.Normalized();
		movement *= Data.MoveSpeed * (float)delta;
		
		Translate(movement);
		
		// Select animation
		if (movement.LengthSquared() == 0)
			Stop();
		else if (Mathf.Abs(movement.X) > Mathf.Abs(movement.Y))
			Play(movement.X < 0 ? "left" : "right");
		else
			Play(movement.Y < 0 ? "up" : "down");
	}
}

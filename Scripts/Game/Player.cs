using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public CharacterData Data;

	private AnimatedSprite2D _sprite;
	private Vector2 _movement;
	private bool _movingLastFrame = false;
	
	public override void _Ready()
	{
		MotionMode = MotionModeEnum.Floating;
		
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		_sprite.SpriteFrames = Data.Sprite;
		_sprite.Animation = "down";
		_sprite.Stop();
	}

	public override void _Process(double delta)
	{
		// Get input
		int horizontal = (Input.IsActionPressed("right") ? 1 : 0) - (Input.IsActionPressed("left") ? 1 : 0);
		int vertical = (Input.IsActionPressed("down") ? 1 : 0) - (Input.IsActionPressed("up") ? 1 : 0);

		// Calculate movement
		_movement = new Vector2(horizontal, vertical);
		_movement = _movement.Normalized();
		_movement *= Data.MoveSpeed * 50;
		
		// Select animation
		if (_movement.LengthSquared() == 0)
		{
			if (_movingLastFrame)
				_sprite.Play(_sprite.Animation.ToString()!.Replace("move_", ""));
			_movingLastFrame = false;
		}
		else if (Mathf.Abs(_movement.X) > Mathf.Abs(_movement.Y))
		{
			_sprite.Play(_movement.X < 0 ? "move_left" : "move_right");
			_movingLastFrame = true;
		}
		else
		{
			_sprite.Play(_movement.Y < 0 ? "move_up" : "move_down");
			_movingLastFrame = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Velocity = _movement * (float)delta;
		MoveAndSlide();
	}
}

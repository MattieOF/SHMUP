using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public EnemyData Data;
	[Export] public AnimatedSprite2D Sprite;

	public float Health;

	public override void _Ready()
	{
		SetData(Data);
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
	}
}

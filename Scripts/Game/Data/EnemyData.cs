using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string Name = "Enemy";
    [Export] public Vector2 ScaleRange = new(1, 1);
    [Export] public float MaxHP = 100;
    [Export] public SpriteFrames Sprite;
    [Export] public LootTable GemDrop;
    [Export] public LootTable ItemDrop;
    [Export] public Script EnemyClassOverride;
    [Export] public PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Game/Enemy.tscn");
}

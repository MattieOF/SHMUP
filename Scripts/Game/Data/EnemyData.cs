using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string Name = "Enemy";
    [Export] public float MaxHP = 100;
    [Export] public SpriteFrames Sprite;
    [Export] public Dictionary<PackedScene, float> GemDrop;
    [Export] public Dictionary<PackedScene, float> ItemDrop;
    [Export] public Script EnemyClassOverride;
    [Export] public PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Game/Enemy.tscn");
}

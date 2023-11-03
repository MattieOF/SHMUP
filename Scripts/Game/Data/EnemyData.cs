using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string Name = "Enemy";
    [Export] public Vector2 ScaleRange = new(1, 1);
    [Export] public float MaxHP = 100;
    [Export] public float MoveSpeed = 300;
    [Export] public float TurnSpeed = 7;
    [Export] public float AttackCooldown = 1;
    [Export] public float AttackRadius = 90;
    [Export] public float BaseDamage = 10;
    [Export] public SpriteFrames Sprite;
    [Export] public LootTable GemDrop;
    [Export] public LootTable ItemDrop;
    [Export] public Script EnemyClassOverride;
    [Export] public PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Game/Enemy.tscn");
    [Export] public AudioStream HurtSound = GD.Load<AudioStream>("res://Art/Sounds/Attack.wav");
}

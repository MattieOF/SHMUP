using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string Name = "Enemy";
    [Export] public SpriteFrames Sprite;
    [Export] public WeightedItemList GemDrop;
}

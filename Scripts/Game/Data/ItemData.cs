using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string Name = "Item";
    [Export] public SpriteFrames Sprite;
    [Export] public Script PickupClassOverride;
    [Export] public PackedScene PickupScene = GD.Load<PackedScene>("res://Scenes/Game/Gems/Pickup.tscn");
}

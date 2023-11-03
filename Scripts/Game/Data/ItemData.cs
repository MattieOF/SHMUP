using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string Name = "Item";
    [Export] public Texture2D Icon;
    [Export] public SpriteFrames Sprite;
    [Export] public Script PickupClassOverride;
    [Export] public PackedScene PickupScene = GD.Load<PackedScene>("res://Scenes/Game/Gems/Pickup.tscn");
}

public struct ItemStack
{
    public ItemData Data;
    public int Count = 1;

    public ItemStack(ItemData data, int count)
    {
        Data = data;
        Count = count;
    }
}

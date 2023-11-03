using Godot;

[GlobalClass]
public partial class LootElement : Resource
{
    [Export] public ItemData Item;
    [Export] public Vector2 CountRange = new(1, 1);
    [Export] public float Weight;
}

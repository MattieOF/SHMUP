using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WeightedItemList : Resource
{
    [Export] public Dictionary<ItemData, float> Items;
}

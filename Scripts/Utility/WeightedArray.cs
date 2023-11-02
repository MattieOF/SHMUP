using Godot;

[GlobalClass]
public partial class WeightedArray<[MustBeVariant] T> : Resource
{
    [Export] public Godot.Collections.Dictionary<T, float> Items;
    public float Total { get; private set; }
}

[GlobalClass]
public partial class WeightedSceneArray : WeightedArray<PackedScene>
{ }

// [Export] public Godot.Collections.Dictionary<T, float> Items;

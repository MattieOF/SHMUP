using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LootTable : Resource
{
    [Export] public Vector2 RollCountRange = new(1, 1);
    [Export] public Array<LootElement> Elements;

    public IEnumerable<ItemStack> Roll()
    {
        List<ItemStack> result = new();

        float totalWeight = 0;
        foreach (var element in Elements)
            totalWeight += element.Weight;

        int rolls = Utility.RNG.RandiRange((int)RollCountRange.X, (int)RollCountRange.Y);
        for (int i = 0; i < rolls; i++)
        {
            // TODO: Optimise, lots of unnecessary calculation here.
            float roll = Utility.RNG.RandfRange(0, totalWeight);
            float weightAccum = 0;
            foreach (var element in Elements)
            {
                weightAccum += element.Weight;
                if (roll <= weightAccum)
                    result.Add(new ItemStack(element.Item, Utility.RNG.RandiRange((int)element.CountRange.X, (int)element.CountRange.Y)));
            }
        }
        
        return result;
    }
}

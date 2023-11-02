using Godot;
using Godot.Collections;

[GlobalClass]
public partial class XPToLevelData : Resource
{
    [Export] public Array<int> XPNeededForLevel;

    public float GetLevel(int XP)
    {
        for (int i = 0; i < XPNeededForLevel.Count; i++)
        {
            var xp = XP - XPNeededForLevel[i];
            switch (xp)
            {
                case 0:
                    return i + 1;
                case < 0:
                    return i + ((float)XP / XPNeededForLevel[i]);
                default:
                    XP -= XPNeededForLevel[i];
                    break;
            }
        }

        return XPNeededForLevel.Count + ((float)XP / XPNeededForLevel[^1]);
    }
}

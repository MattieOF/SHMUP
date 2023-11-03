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
                    return i + 2;
                case < 0:
                    return i + ((float)XP / XPNeededForLevel[i]) + 1;
                default:
                    XP -= XPNeededForLevel[i];
                    break;
            }
        }

        return 1 + XPNeededForLevel.Count + ((float)XP / XPNeededForLevel[^1]);
    }
}

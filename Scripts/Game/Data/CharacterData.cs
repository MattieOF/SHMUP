using Godot;

[GlobalClass]
public partial class CharacterData : Resource
{
    [Export] public string Name = "Unnamed Character";
    [Export] public SpriteFrames Sprite;
    [Export] public float MoveSpeed = 300;
    [Export] public float BaseHealth = 100;
    [Export] public XPToLevelData XPToLevelData;
}

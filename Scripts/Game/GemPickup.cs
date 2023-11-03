using Godot;

public partial class GemPickup : Pickup
{
    public override void OnPickup()
    {
        this.PlaySound2D(GD.Load<AudioStream>("res://Art/Sounds/Gem.wav"), GetNode("/root/Game"));
        Player.AddXP((Item as GemItemData)!.XPValue * Count);
    }
}

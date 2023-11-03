using Godot;

public partial class ResourcePickup : Pickup
{
    public override void OnPickup()
    {
        var notif = Globals.Instance.ResourceNotif.Instantiate<ResourceNotif>();
        notif.SetItemAndAmount(Item, Count);
        notif.GlobalPosition = GlobalPosition;
        notif.Velocity = new(20 * Utility.RNG.RandfRange(0.5f, 1.5f), -60 * Utility.RNG.RandfRange(0.5f, 1.5f));
        notif.AngularVelocity = Utility.RNG.RandfRange(-10, 10);
        GetNode("/root/Game").AddChild(notif);
        
        Player.Inventory.Add(Item, Count);
    }
}

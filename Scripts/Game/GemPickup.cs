public partial class GemPickup : Pickup
{
    public override void OnPickup() => Player.AddXP((Item as GemItemData)!.XPValue * Count);
}

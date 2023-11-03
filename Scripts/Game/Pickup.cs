using System;
using Godot;

public partial class Pickup : Area2D
{
	public ItemData Item
	{
		get => _item;
		set => SetItem(value);
	}

	public int Count = 1;
	public bool PickedUp { get; private set; }

	private ItemData _item;
	private AnimatedSprite2D _sprite;

	protected Vector2 StartingPos;
	protected Player Player;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	public void SetItem(ItemData item)
	{
		_item = item;
		_sprite.SpriteFrames = item.Sprite;
	}
	
	public void PickUp(Player player)
	{
		if (PickedUp)
			return;
        PickedUp = true;
        Player = player;

        StartingPos = GlobalPosition;
        Tween pickupTween = CreateTween();
        var setPos = new Action<float>(value =>
        {
	        var diff = StartingPos - player.GlobalPosition;
	        GlobalPosition = StartingPos + (diff * value);
        });
        pickupTween.TweenMethod(Callable.From(setPos), Variant.From(0.0f), Variant.From(0.35f), 0.15f).SetEase(Tween.EaseType.Out);
        pickupTween.TweenMethod(Callable.From(setPos), Variant.From(0.35f), Variant.From(-1), 0.3f).SetEase(Tween.EaseType.In);
        pickupTween.TweenCallback(Callable.From(OnPickup));
        pickupTween.TweenCallback(Callable.From(QueueFree));
	}
    
	public virtual void OnPickup() => Console.Instance.WriteWarn($"Pickup \"{Name}\" does not override OnPickup!");
}

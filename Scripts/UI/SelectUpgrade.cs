using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum Upgrades
{
    WeaponSpeed,
    WeaponRange,
    Heal,
    MaxHealth,
    PlayerSpeed,
    Max
}

public partial class SelectUpgrade : CanvasLayer
{
    [Export] public Button[] Buttons = new Button[3];

    private List<Upgrades> _available = new();
    
    public override void _Ready()
    {
        Buttons[0].Pressed += () => Selected(0);
        Buttons[1].Pressed += () => Selected(1);
        Buttons[2].Pressed += () => Selected(2);
        
        var options = Enumerable.Range(0, (int)Upgrades.Max).ToList()!;
        for (int i = 0; i < 3; i++)
        {
            var index = Utility.RNG.RandiRange(0, options.Count - 1);
            _available.Add((Upgrades) options[index]);
            options.RemoveAt(index);
        }

        for (int i = 0; i < 3; i++)
        {
            Buttons[i].Text = Utility.SplitCamelCase(_available[i].ToString());
        }
    }

    public void Selected(int index)
    {
        Upgrades selected = _available[index];
        Player player = (GetTree().GetFirstNodeInGroup("Player") as Player)!;
        
        switch(selected)
        {
            case Upgrades.WeaponSpeed:
                player.GetNode<Scythe>("Scythe").AttackCooldown -= 0.15f;
                break;
            case Upgrades.WeaponRange:
                player.GetNode<Scythe>("Scythe").Range += 20f;
                break;
            case Upgrades.Heal:
                player.Health = player.MaxHealth;
                player.HUD.UpdateHealthBar(1);
                break;
            case Upgrades.MaxHealth:
                player.MaxHealth += 10;
                player.Health += 10;
                player.HUD.UpdateHealthBar(player.Health / player.MaxHealth);
                break;
            case Upgrades.PlayerSpeed:
                player.SpeedMultiplier += 0.1f;
                break;
            case Upgrades.Max:
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GetTree().Paused = false;
        QueueFree();
    }
}

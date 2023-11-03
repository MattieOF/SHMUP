using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Scythe : Sprite2D
{
    [Export] public Player Owner;
    [Export] public float AttackCooldown = 1.5f;
    [Export] public float Range = 150;
    [Export] public float Damage = 20;

    private float _attackCooldown;
    private List<Enemy> _hitEnemies = new();

    public override void _Ready()
    {
        GetNode<Area2D>("Collider").BodyEntered += body =>
        {
            if (body is Enemy enemy)
            {
                if (_hitEnemies.Contains(enemy))
                    return;
                enemy.CallDeferred(Enemy.MethodName.Hurt, Damage);
                _hitEnemies.Add(enemy);
            }
        };
    }

    public override void _Process(double delta)
    {
        if (!Owner.Alive)
            return;
        
        _attackCooldown -= (float)delta;
        if (_attackCooldown <= 0)
        {
            Attack();
        }
    }

    public void Attack()
    {
        _hitEnemies.Clear();
        
        var enemies = GetTree().GetNodesInGroup("Enemy");

        if (enemies.Count == 0)
            return;
        
        var closest = enemies.MinBy(node => (node as Node2D)!.GlobalPosition.DistanceSquaredTo(GlobalPosition));
        var dir = ((closest as Node2D)!.GlobalPosition - GlobalPosition).Normalized();
        Rotation = dir.Angle() - (Mathf.Pi / 2);
        var tween = CreateTween();
        tween.TweenProperty(this, new NodePath(PropertyName.Position), Variant.From(dir * Range), 0.2f);
        tween.TweenProperty(this, new NodePath(PropertyName.Position), Variant.From(Vector2.Zero), 0.2f);

        _attackCooldown = AttackCooldown;
    }
}

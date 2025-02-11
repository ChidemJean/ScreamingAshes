using Godot;
using System;
using ChidemGames.Core.Characters;

namespace ChidemGames.Core
{
    public partial class DamageApplicator : Area3D
    {
        [Export]
        float damage = .2f;

        public bool canDamage = true;

        public override void _Ready()
        {
            // Connect("body_entered", this, nameof(OnBodyEntered));
            BodyEntered += OnBodyEntered;
        }

        public void OnBodyEntered(Node node)
        {
            if (node is Player && canDamage) {
                ((Player) node).TakeDamage(damage);
            }
        }
    }
}
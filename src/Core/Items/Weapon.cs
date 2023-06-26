using Godot;
using System;

namespace ChidemGames.Core.Items
{
    public partial class Weapon : Attackable
    {
        [Export]
        public float damageShot = .25f;

        public override void Attack()
        {
            
        }
    }
}
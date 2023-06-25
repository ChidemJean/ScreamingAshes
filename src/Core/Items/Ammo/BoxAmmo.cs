using Godot;
using System;

namespace ChidemGames.Core.Items.Ammo
{

    public partial class BoxAmmo : Item
    {
        [Export]
        public int bullets = 0;

        public override void _Ready()
        {
            base._Ready();            
        }
    }
}
using Godot;
using System;

namespace ChidemGames.Core.Items.Weapons
{
    public partial class FirearmClip : Item
    {
        [Export]
        public int bullets = 5;

        [Export]
        public int maxBullets = 5;

        [Export]
        FirearmClipType type = FirearmClipType.Pistol;

        public override void _Ready()
        {
            base._Ready();
        }
    }
}
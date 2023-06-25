using Godot;
using System;
using ChidemGames.Core.Items.Weapons;

namespace ChidemGames.Resources
{
    public partial class FirearmClipResource : ItemResource
    {
        [Export]
        public FirearmClipType type = FirearmClipType.Pistol;
    }
}
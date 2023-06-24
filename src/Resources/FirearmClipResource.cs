using Godot;
using System;
using ChidemGames.Core.Items.Weapons;

namespace ChidemGames.Resources
{
    public class FirearmClipResource : ItemResource
    {
        [Export]
        public FirearmClipType type = FirearmClipType.Pistol;
    }
}
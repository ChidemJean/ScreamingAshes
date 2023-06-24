using Godot;
using System;

namespace ChidemGames.Core.Weapons
{

    public class BulletCapsule : RigidBody
    {
        public override async void _Ready()
        {
            await ToSignal(GetTree().CreateTimer(8f), "timeout");
            QueueFree();
        }
    }
}
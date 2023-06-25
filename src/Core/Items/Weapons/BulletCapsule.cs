using Godot;
using System;

namespace ChidemGames.Core.Weapons
{

    public partial class BulletCapsule : RigidBody3D
    {
        public override async void _Ready()
        {
            await ToSignal(GetTree().CreateTimer(8f), "timeout");
            QueueFree();
        }
    }
}
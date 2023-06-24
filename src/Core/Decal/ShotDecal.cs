using Godot;
using System;

namespace ChidemGames.Core.Decal
{
    public class ShotDecal : MeshInstance
    {
        public async override void _Ready()
        {
            await ToSignal(GetTree().CreateTimer(12), "timeout");
            QueueFree();
        }
    }
}
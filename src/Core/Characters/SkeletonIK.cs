using Godot;
using System;

namespace ChidemGames.Core.Characters
{
    [Tool]
    public class SkeletonIK : Godot.SkeletonIK
    {
        public override void _EnterTree()
        {
            Start();
        }

    }
}
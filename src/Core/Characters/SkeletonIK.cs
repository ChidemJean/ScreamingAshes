using Godot;
using System;

namespace ChidemGames.Core.Characters
{
    public partial class SkeletonIK : SkeletonIK3D
    {
        public override void _EnterTree()
        {
            Start();
        }

    }
}
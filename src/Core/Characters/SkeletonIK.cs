using Godot;
using System;

namespace ChidemGames.Core.Characters
{
    [Tool]
    public partial class SkeletonIK : SkeletonIK3D
    {
        public override void _EnterTree()
        {
            Start();
        }

    }
}
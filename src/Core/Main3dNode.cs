using Godot;
using System;

namespace ChidemGames.Core
{
    public partial class Main3dNode : Node3D
    {
        GlobalManager globalManager;
        
        public override void _Ready()
        {
            globalManager = GetNode<GlobalManager>("/root/GlobalManager");
            globalManager.main3dNode = this;
        }
    }
}
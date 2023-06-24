using Godot;
using System;

namespace ChidemGames.Core
{
    public class Main3dNode : Spatial
    {
        GlobalManager globalManager;
        
        public override void _Ready()
        {
            globalManager = GetNode<GlobalManager>("/root/GlobalManager");
            globalManager.main3dNode = this;
        }
    }
}
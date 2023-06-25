using Godot;
using System;

namespace ChidemGames.Debug
{
    public partial class DebugCameraRay : Node3D//ImmediateGeometry
    {
        public override void _Ready()
        {
            // Begin(Mesh.PrimitiveType.Lines);
            // AddVertex(GlobalTranslation);
            // AddVertex(GlobalTranslation + GetParent<Camera>().GlobalTransform.basis.z.Normalized() * 800);
            // End();            
        }
    }
}
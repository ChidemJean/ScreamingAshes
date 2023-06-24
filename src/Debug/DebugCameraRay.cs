using Godot;
using System;

namespace ChidemGames.Debug
{
    public class DebugCameraRay : ImmediateGeometry
    {
        public override void _Ready()
        {
            Begin(Mesh.PrimitiveType.Lines);
            AddVertex(GlobalTranslation);
            AddVertex(GlobalTranslation + GetParent<Camera>().GlobalTransform.basis.z.Normalized() * 800);
            End();            
        }
    }
}
using Godot;
using System;

namespace ChidemGames.Core.Audio
{

    public interface AdaptiveAudioEmitter
    {
        RayCast3D GetTopRay();
        RayCast3D GetLeftRay();
        RayCast3D GetRightRay();
        RayCast3D GetForwardRay();
        RayCast3D GetBackwardRay();
        RayCast3D GetAimRay();
    }
}
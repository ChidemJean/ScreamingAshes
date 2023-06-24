using Godot;
using System;

namespace ChidemGames.Core.Audio
{

    public interface AdaptiveAudioEmitter
    {
        RayCast GetTopRay();
        RayCast GetLeftRay();
        RayCast GetRightRay();
        RayCast GetForwardRay();
        RayCast GetBackwardRay();
        RayCast GetAimRay();
    }
}
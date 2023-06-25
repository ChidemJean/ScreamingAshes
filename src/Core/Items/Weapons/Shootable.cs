using Godot;
using System;

namespace ChidemGames.Core.Items.Weapons
{

    public interface Shootable
    {
        bool Shoot();
        bool Reload();
        void ReloadFinish();
        void PlayReloadSfx(float reloadTime, Node3D clipPivot);
        void ButtBlow();
    }
}
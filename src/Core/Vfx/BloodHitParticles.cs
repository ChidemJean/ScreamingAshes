using Godot;
using System;
using Godot.Collections;

namespace ChidemGames.Core.Vfx
{
    public partial class BloodHitParticles : GpuParticles3D
    {
        Array<GpuParticles3D> particles = new Array<GpuParticles3D>();

        public override void _Ready()
        {
            particles.Add(this);
            foreach (var node in GetChildren()) {
                particles.Add((GpuParticles3D) node);
            }
        }

        public async void Play()
        {
            foreach (var particle in particles)
            {
                particle.Emitting = true;
            }
            await ToSignal(GetTree().CreateTimer(.85f), "timeout");
            QueueFree();
        }
    }
}
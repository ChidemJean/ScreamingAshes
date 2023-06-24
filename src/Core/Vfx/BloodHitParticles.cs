using Godot;
using System;
using Godot.Collections;

namespace ChidemGames.Core.Vfx
{
    public class BloodHitParticles : Particles
    {
        Array<Particles> particles = new Array<Particles>();

        public override void _Ready()
        {
            particles.Add(this);
            foreach (var node in GetChildren()) {
                particles.Add((Particles) node);
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
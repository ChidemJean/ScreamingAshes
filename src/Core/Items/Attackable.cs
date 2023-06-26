using Godot;
using System;

namespace ChidemGames.Core.Items
{
    public abstract partial class Attackable : Item
    {
		[Export]
		float damage = .5f;

		[Export]
		NodePath hitSensorPath;
		Area3D hitSensor;

		public override void _Ready()
		{
			base._Ready();
			hitSensor = GetNode<Area3D>(hitSensorPath);
			hitSensor.BodyEntered += OnBodyEntered;
		}

		public void OnBodyEntered(Node node)
		{
			if (node is BodyPartHittable && player.isAttacking)
			{
				var bodyPart = (BodyPartHittable)node;
				bodyPart.ApplyDamageOnBody(damage, bodyPart.GlobalPosition);
				return;
			}
		}

		public abstract void Attack();
    }
}
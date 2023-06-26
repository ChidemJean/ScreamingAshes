using Godot;
using System;

namespace ChidemGames.Core
{
	public partial class BodyPartHittable : RigidBody3D
	{
		[Export]
		NodePath bodyPath;
		public BodyWithHittableParts body;

		[Export]
		public float damageMultiplier = 1;

		public override void _Ready()
		{
			body = GetNode<BodyWithHittableParts>(bodyPath);
		}

		public void ApplyDamageOnBody(float damage, Vector3 pos)
		{
			body.TakeDamage(damage * damageMultiplier, pos);
		}

	}
}
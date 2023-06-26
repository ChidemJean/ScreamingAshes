using Godot;
using System;

namespace ChidemGames.Core
{
	public interface BodyWithHittableParts
	{
		void TakeDamage(float damage, Vector3 position);
		float GetMaxHealth();
		float GetHealth();
	}
}
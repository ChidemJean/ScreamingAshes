using Godot;
using System;

namespace ChidemGames.Core.Decal
{
	public partial class ShotDecal : Node3D
	{
		public async override void _Ready()
		{
			await ToSignal(GetTree().CreateTimer(12), "timeout");
			QueueFree();
		}
	}
}

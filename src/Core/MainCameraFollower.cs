using Godot;
using System;

namespace ChidemGames.Core
{
   public partial class MainCameraFollower : Camera3D
   {
		GlobalManager globalManager;

      public override void _Ready()
      {
         globalManager = GetNode<GlobalManager>("/root/GlobalManager");
      }

      public override void _Process(double delta)
      {
			GlobalTransform = globalManager.currentPlayer.camera.GlobalTransform;
			Fov = globalManager.currentPlayer.camera.Fov;
      }
   }
}
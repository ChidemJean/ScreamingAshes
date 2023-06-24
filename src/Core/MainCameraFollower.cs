using Godot;
using System;

namespace ChidemGames.Core
{
   public class MainCameraFollower : Camera
   {
		GlobalManager globalManager;

      public override void _Ready()
      {
         globalManager = GetNode<GlobalManager>("/root/GlobalManager");
      }

      public override void _Process(float delta)
      {
			GlobalTransform = globalManager.currentPlayer.camera.GlobalTransform;
			Fov = globalManager.currentPlayer.camera.Fov;
      }
   }
}
using Godot;
using System;
using ChidemGames.Core.Characters;

namespace ChidemGames.Ui
{

   public partial class StaminaBox : HBoxContainer
   {
		[Export]
		NodePath iconPath;
		TextureRect icon;

		[Export]
		NodePath staminaProgressPath;
		Godot.ProgressBar staminaProgress;

		GlobalManager globalManager;

      public override void _Ready()
      {
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			icon = GetNode<TextureRect>(iconPath);
			staminaProgress = GetNode<ProgressBar>(staminaProgressPath);
      }

      public override void _Process(double delta)
      {
         base._Process(delta);
			if (globalManager.currentPlayer != null) {
				staminaProgress.Value = globalManager.currentPlayer.currentStamina / globalManager.currentPlayer.maxStaminaValue;
			}
      }
   }
}
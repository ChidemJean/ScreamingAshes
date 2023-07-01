using Godot;
using System;
using System.Collections.Generic;

namespace ChidemGames.Core.Scenario
{
   public partial class Interactable : Node3D
   {
      [Export]
      NodePath animationPlayerPath;
      AnimationPlayer animationPlayer;

      public bool isOpen = false;

      bool isHighlighted = false;

      [Export]
      NodePath meshesHolderPath;
      Node3D meshesHolder;

		[Export(PropertyHint.Layers3DRender)]
      uint defaultLayer;

      [Export(PropertyHint.Layers3DRender)]
      uint layerHighlight;

      List<MeshInstance3D> meshes = new List<MeshInstance3D>();

		[Export]
		float waitToOut = 0;

		SceneTreeTimer timer;

      public override void _Ready()
      {
         meshesHolder = GetNode<Node3D>(meshesHolderPath);
         if (meshesHolder != null)
         {
            foreach (var child in meshesHolder.GetChildren())
            {
               if (child is MeshInstance3D)
               {
                  meshes.Add((MeshInstance3D)child);
               }
            }
         }
         animationPlayer = GetNode<AnimationPlayer>(animationPlayerPath);
      }

      public void Interact()
      {
         isOpen = !isOpen;
         double curTime = animationPlayer.CurrentAnimationPosition == 1 ? 0 : animationPlayer.CurrentAnimationPosition;
         
         if (isOpen) {
            animationPlayer.Play($"interact_in");
            double animLength = animationPlayer.CurrentAnimationLength;
            double advanceTime = (curTime > 0 && curTime < animLength) ? animLength - curTime : 0;
            animationPlayer.Advance(advanceTime);
         } else {
            animationPlayer.Play($"interact_out");
            double animLength = animationPlayer.CurrentAnimationLength;
            double advanceTime = (curTime > 0 && curTime < animLength) ? animLength - curTime : 0;
            animationPlayer.Advance(advanceTime);
         }
			if (isOpen && waitToOut > 0) {
				WaitToOut();
			}
      }

		public async void WaitToOut()
		{
			timer = GetTree().CreateTimer(waitToOut);
			await ToSignal(timer, "timeout");
			if (isOpen) Interact();
		}

      public bool ChangeHighlight(bool highlight = true)
      {
         if (highlight == isHighlighted)
         {
            return true;
         }

         if (meshes.Count == 0)
         {
            return false;
         }

         isHighlighted = highlight;

         foreach (var mesh in meshes)
         {
            mesh.Layers = highlight ? layerHighlight : defaultLayer;
         }

         return true;
      }
   }
}
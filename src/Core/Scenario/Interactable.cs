using Godot;
using System;
using System.Collections.Generic;

namespace ChidemGames.Core.Scenario
{
   public class Interactable : Spatial
   {
      [Export]
      NodePath animationPlayerPath;
      AnimationPlayer animationPlayer;

      public bool isOpen = false;

      bool isHighlighted = false;

      [Export]
      NodePath meshesHolderPath;
      Spatial meshesHolder;

		[Export(PropertyHint.Layers3dRender)]
      uint defaultLayer;

      [Export(PropertyHint.Layers3dRender)]
      uint layerHighlight;

      List<MeshInstance> meshes = new List<MeshInstance>();

		[Export]
		float waitToOut = 0;

		SceneTreeTimer timer;

      public override void _Ready()
      {
         meshesHolder = GetNode<Spatial>(meshesHolderPath);
         if (meshesHolder != null)
         {
            foreach (var child in meshesHolder.GetChildren())
            {
               if (child is MeshInstance)
               {
                  meshes.Add((MeshInstance)child);
               }
            }
         }
         animationPlayer = GetNode<AnimationPlayer>(animationPlayerPath);
      }

      public void Interact()
      {
         isOpen = !isOpen;
         animationPlayer.Play($"interact_{(isOpen ? "in" : "out")}");
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
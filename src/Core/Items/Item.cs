using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Core.Characters;

namespace ChidemGames.Core.Items
{
   public class Item : RigidBody
   {
      [Export]
      NodePath leftHandPositionPath;
      private Position3D leftHandPosition;

      [Export]
      protected NodePath playerPath;
      public Player player = null;

      [Export]
      NodePath meshesHolderPath;
      Spatial meshesHolder;

      List<MeshInstance> meshes = new List<MeshInstance>();

      [Export(PropertyHint.Layers3dRender)]
      uint defaultLayer;

      [Export(PropertyHint.Layers3dRender)]
      uint layerHighlight;

      bool isHighlighted = false;

      [Export]
      public string itemId = "";

      [Export]
      public bool isTakeable = true;

      [Export]
      public bool isInspectable = true;

      [Export]
      public bool isInteractive = true;

      public Position3D LeftHandPosition
      {
         get
         {
            return this.leftHandPosition;
         }
      }

      protected GlobalManager globalManager;

      public override void _Ready()
      {
         globalManager = GetNode<GlobalManager>("/root/GlobalManager");
         meshesHolder = GetNode<Spatial>(meshesHolderPath);
         if (meshesHolder != null) {
            foreach (var child in meshesHolder.GetChildren()) {
               if (child is MeshInstance) {
                  meshes.Add((MeshInstance) child);
               }
            }
         }

         if (leftHandPositionPath != null)
         {
            leftHandPosition = GetNode<Position3D>(leftHandPositionPath);
         }
      }

      public void ChangePhysics(ModeEnum modeEnum)
      {
         Mode = modeEnum;
      }

      public bool ChangeHighlight(bool highlight = true)
      {
         if (!isInteractive) {
            return false;
         }

         if (highlight == isHighlighted) {
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
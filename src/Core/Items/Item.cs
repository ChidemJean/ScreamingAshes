using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Core.Characters;

namespace ChidemGames.Core.Items
{
   public partial class Item : RigidBody3D
   {
      [Export]
      NodePath leftHandPositionPath;
      private Marker3D leftHandPosition;

      [Export]
      protected NodePath playerPath;
      public Player player = null;

      [Export]
      NodePath meshesHolderPath;
      Node3D meshesHolder;

      List<MeshInstance3D> meshes = new List<MeshInstance3D>();

      [Export(PropertyHint.Layers3DRender)]
      uint defaultLayer;

      [Export(PropertyHint.Layers3DRender)]
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

      public Marker3D LeftHandPosition
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
         meshesHolder = GetNode<Node3D>(meshesHolderPath);
         if (meshesHolder != null) {
            foreach (var child in meshesHolder.GetChildren()) {
               if (child is MeshInstance3D) {
                  meshes.Add((MeshInstance3D) child);
               }
            }
         }

         if (leftHandPositionPath != null)
         {
            leftHandPosition = GetNode<Marker3D>(leftHandPositionPath);
         }
      }

      public void ChangePhysics(bool freeze)
      {
         Freeze = freeze;
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
using Godot;
using System;
using ChidemGames.Core.Items;

namespace ChidemGames.Core.Items.Melee
{
   public partial class Knife : Attackable
   {
      
      public override void Attack()
      {
         // var bodies = hitSensor.GetOverlappingBodies();
         
         // foreach (var node in bodies) 
         // {
         //    GD.Print(node.Name);
         //    if (node is BodyPartHittable)
         //    {
         //       var bodyPart = (BodyPartHittable) node;
         //       bodyPart.ApplyDamageOnBody(damage, bodyPart.GlobalPosition);
         //       return;
         //    }
         // }
      }
   }
}
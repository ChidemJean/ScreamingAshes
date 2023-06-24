using Godot;
using System;

namespace ChidemGames.Tools
{
   public class Fps : Label
   {
      public override void _Process(float delta)
      {
         this.Text = Engine.GetFramesPerSecond().ToString();
      }
   }
}
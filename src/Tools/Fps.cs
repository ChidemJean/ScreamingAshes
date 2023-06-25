using Godot;
using System;

namespace ChidemGames.Tools
{
   public partial class Fps : Label
   {
      public override void _Process(double delta)
      {
         this.Text = Engine.GetFramesPerSecond().ToString();
      }
   }
}
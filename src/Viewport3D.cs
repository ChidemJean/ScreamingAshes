using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Game
{

   public partial class Viewport3D : SubViewport
   {
      [Export] 
		private float scaleFactor = 1.0f;
      
		public float currentScale;

		Vector2 originalSize;

      GlobalEvents globalEvents;

      public async override void _Ready()
      {
         // globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			// originalSize = Size;
         // currentScale = scaleFactor;
         // SizeChanged += OnResize;
         // await ToSignal(GetTree().CreateTimer(.75f), "timeout");
			// UpdateViewport3DSize(scaleFactor);
         // globalEvents.ChangeRenderSize += UpdateViewport3DSize;
      }

      public void OnResize()
      {
			// originalSize = this.Size;
         // UpdateViewport3DSize(this.currentScale);
      }

      public void UpdateViewport3DSize(float value)
      {

         // float scaleFactorClamped = Mathf.Clamp(value, 0.2f, 1f);

         // int newX = Mathf.CeilToInt(GetWindow().Size.X * scaleFactorClamped);
         // int newY = Mathf.CeilToInt(GetWindow().Size.Y * scaleFactorClamped);
         // this.Size = new Vector2I(newX, newY);

         // Rect2 rectMainViewport = GetTree().Root.GetViewport().GetVisibleRect();
         // rectMainViewport.Size = new Vector2(GetWindow().Size.X, GetWindow().Size.Y);
         
         // this.currentScale = scaleFactorClamped;

         // globalEvents.EmitSignal(GameEvent.RenderSizeChanged, this.Size, scaleFactorClamped);
      }
   }
}
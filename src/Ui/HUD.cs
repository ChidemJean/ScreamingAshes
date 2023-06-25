using Godot;
using System;

namespace ChidemGames.Ui
{
   public partial class HUD : Control
   {
      public enum CursorType
      {
         Normal,
         Hand,
			Loading
      }

		public enum CrosshairType
      {
         Normal,
         InteractHand
      }

      [Export]
      NodePath crosshairPath;
      TextureRect crosshair;

      [Export]
      NodePath crosshairHandPath;
      TextureRect crosshairHand;

      [Export]
      NodePath cursorLoadPath;
      TextureProgressBar cursorLoad;

      public CursorType cursorType = CursorType.Normal;
      public CrosshairType crosshairType = CrosshairType.Normal;

		GlobalManager globalManager;

		bool isLoadShow = false;

      public override void _Ready()
      {
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
         crosshair = GetNode<TextureRect>(crosshairPath);
         crosshairHand = GetNode<TextureRect>(crosshairHandPath);
         cursorLoad = GetNode<TextureProgressBar>(cursorLoadPath);

         Input.SetCustomMouseCursor(ResourceLoader.Load("res://assets/ui/cursor/cursor.png"));

			globalManager.ChangeStateFocus(StateFocus.GAME);

			globalManager.hud = this;

      }

		public void ChangeCrosshair(CrosshairType crosshairType)
		{
			switch (crosshairType) {
				case CrosshairType.Normal:
					crosshair.Visible = true;
					crosshairHand.Visible = false;
					return;
				case CrosshairType.InteractHand:
					crosshair.Visible = false;
					crosshairHand.Visible = true;
					return;
			}
		}

		public void ChangeCursor(CursorType cursorType)
		{
			switch (cursorType) {
				case CursorType.Normal:
					Input.SetCustomMouseCursor(ResourceLoader.Load("res://assets/ui/cursor/cursor.png"));
					return;
				case CursorType.Hand:
					Input.SetCustomMouseCursor(ResourceLoader.Load("res://assets/ui/cursor/hand_interact.png"));
					return;
			}
		}

		public async void ShowLoadCursor(float secs, Action callback = null)
		{
			isLoadShow = true;
			cursorLoad.Visible = true;
			cursorLoad.Value = 0;
			Input.MouseMode = Input.MouseModeEnum.Hidden;
			globalManager.isCursorInvisible = true;
			var tween = GetTree().CreateTween();
			tween.TweenProperty(cursorLoad, "value", 100, secs);
			await ToSignal(tween, "finished");
			cursorLoad.Visible = false;
			isLoadShow = false;
			globalManager.isCursorInvisible = false;
			globalManager.ChangeStateFocus(globalManager.stateFocus);
			if (callback != null)
			{
				callback.Invoke();
			}
		}

      public override void _Process(double delta)
      {
         if (!isLoadShow) {
				return;
			}
			cursorLoad.GlobalPosition = GetViewport().GetMousePosition();
      }
   }
}
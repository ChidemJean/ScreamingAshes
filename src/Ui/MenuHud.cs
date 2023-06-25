using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Ui
{
   public partial class MenuHud : Control
   {
      [Export]
      public NodePath holderMenuPath;

      Control holderMenu;

		bool isOpen = false;

		GlobalManager globalManager;
		GlobalEvents globalEvents;

      public override void _Ready()
      {
		globalManager = GetNode<GlobalManager>("/root/GlobalManager");
		globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		holderMenu = GetNode<Control>(holderMenuPath);
		holderMenu.GlobalPosition = new Vector2(GetViewport().GetVisibleRect().Size.X, 0);
		Color modulate = holderMenu.Modulate;
		modulate.A = 0;
		holderMenu.Modulate = modulate;

		// globalEvents.Connect(GameEvent.OpenMenu, this, nameof(OpenMenu));
		// globalEvents.Connect(GameEvent.CloseMenu, this, nameof(CloseMenu));
		globalEvents.OpenMenu += OpenMenu;
		globalEvents.CloseMenu += CloseMenu;
      }

      public override void _Process(double delta)
      {
			if (Input.IsActionJustPressed("escape"))
         {
				if (isOpen) {
					CloseMenu();
				}
				return;
			}
         if (Input.IsActionJustPressed("open_inventory"))
         {
				if (!isOpen) {
					OpenMenu();
				} else {
					CloseMenu();
				}
         }
      }

		public void OpenMenu()
		{
			globalManager.ChangeStateFocus(StateFocus.GAME_MENU);
			var tween = CreateTween();
			tween.TweenProperty(holderMenu, "modulate:a", 1f, .55f);
			tween.Parallel().TweenProperty(holderMenu, "global_position:x", 0, .45f);
			isOpen = true;
			if (globalManager.currentPlayer != null) {
				globalManager.currentPlayer.PlayTakingBackpackAnim();
			}
		}

		public void CloseMenu()
		{
			globalManager.ChangeStateFocus(StateFocus.GAME);
			var tween = CreateTween();
			tween.TweenProperty(holderMenu, "modulate:a", 0f, .2f);
			tween.Parallel().TweenProperty(holderMenu, "global_position:x", GetViewport().GetVisibleRect().Size.X, .35f);
			isOpen = false;
			globalEvents.EmitSignal(GameEvent.OnCloseMenu);
			if (globalManager.currentPlayer != null) {
				globalManager.currentPlayer.PlayKeepBackpackAnim();
			}
		}

   }
}
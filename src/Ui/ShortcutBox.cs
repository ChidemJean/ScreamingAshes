using Godot;
using System;
using ChidemGames.Utils;

namespace ChidemGames.Ui
{

   public partial class ShortcutBox : PanelContainer
   {
		[Export]
		public string inputEventName = "";

		[Export]
		NodePath labelPath;
		Label label;

		Godot.Collections.Array<InputEvent> actionList;

		bool usingJoypad = false;

		[Export]
		StyleBox defaultStyleBox;

		[Export]
		StyleBox selectedStyleBox;

      public override void _Ready()
      {
			label = GetNode<Label>(labelPath);

			if (inputEventName == "") {
				return;
			}

			actionList = InputMap.ActionGetEvents(inputEventName);
         usingJoypad = Input.GetConnectedJoypads().Count > 0;

			UpdateLabel();
      }

		public override void _Input(InputEvent @event)
		{
			if (inputEventName == "") {
				return;
			}
			if (@event.IsActionPressed(inputEventName)) {
				Set("theme_override_styles/panel", selectedStyleBox);
				return;
			}
			if (@event.IsActionReleased(inputEventName)) {
				Set("theme_override_styles/panel", defaultStyleBox);
				return;
			}
		}

		public void UpdateLabel()
		{
			if (inputEventName == "") {
				return;
			}
			foreach (var action in actionList) {
				if (!usingJoypad && (action is InputEventKey)) {
					var eventKey = (InputEventKey) action;
					label.Text = OS.GetKeycodeString(eventKey.Keycode == 0 ? eventKey.PhysicalKeycode : eventKey.Keycode);
					break;
				}
				if (usingJoypad && action is InputEventJoypadMotion) {
					label.Text = "•";
					break;
				}
			}
		}
   }
}
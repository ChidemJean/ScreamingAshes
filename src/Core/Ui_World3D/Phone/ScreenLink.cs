using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Core.Ui_World3D.Phone
{
	public partial class ScreenLink : Control
	{
		[Export]
		string screenKey;

		GlobalEvents globalEvents;

		public override void _Ready()
		{
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			GuiInput += OnGuiInput;
		}

		public void OnGuiInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton) {
				globalEvents.EmitSignal(GameEvent.OpenPhoneScreen, screenKey);
			}
		}
	}
}

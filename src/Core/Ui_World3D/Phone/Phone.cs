using Godot;
using System;
using System.Collections.Generic;

namespace ChidemGames.Core.Ui_World3D.Phone
{
	public partial class Phone : InteractableScreen
	{
		[Export]
		NodePath flashlightPath;
		SpotLight3D flashlight;

		public bool isFlashlightOn = true;

		Dictionary<string, PhoneScreen> screens = new Dictionary<string, PhoneScreen>();

		[Export]
		NodePath phoneUiPath;
		Control phoneUi;

		public override void _Ready()
		{
			base._Ready();
			flashlight = GetNode<SpotLight3D>(flashlightPath);
			phoneUi = GetNode<Control>(phoneUiPath);

			foreach (var node in phoneUi.GetChildren()) {
				if (node is PhoneScreen) {
					var phoneScreen = (PhoneScreen) node;
					if (phoneScreen.key != null) screens.Add(phoneScreen.key, phoneScreen);
				}
			}

			globalEvents.OpenPhoneScreen += OpenPhoneScreenRequest;
		}

		public void OpenPhoneScreenRequest(string key)
		{
			foreach (var screen in screens) {
				screen.Value.IsOn(screen.Value.key == key);
			}
		}

		public void OnCamera(bool on)
		{
			OpenPhoneScreenRequest(on ? "camera" : "home");
		}

		public void FlashlightOn()
		{
			isFlashlightOn = true;
			flashlight.Visible = true;
		}

		public void FlashlightOff()
		{
			isFlashlightOn = false;
			flashlight.Visible = false;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("flashlight"))
			{
				if (isFlashlightOn) {
					FlashlightOff();
				} else {
					FlashlightOn();
				}
			}
			if (Input.IsActionJustPressed("phone_interact")) {
				if (isOn) {
					Off();
				} else {
					On();
				}
			}
		}

		public void Off()
		{
			isOn = false;
			camera.Current = false;
			globalManager.ChangeStateFocus(StateFocus.GAME);
		}

		public void On() 
		{
			isOn = true;
			camera.Current = true;
			globalManager.currentPlayer.SwapFlashlightSlot();
			globalManager.ChangeStateFocus(StateFocus.GAME_MENU);
		}
	}
}
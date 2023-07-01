using Godot;
using System;

namespace ChidemGames.Core
{
	public partial class Phone : Node3D
	{
		[Export]
		NodePath flashlightPath;
		SpotLight3D flashlight;

		public bool isFlashlightOn = true;

		[Export]
		NodePath cameraPath;
		Camera3D camera;

		[Export]
		NodePath cameraPhonePath;
		Camera3D cameraPhone;

		public bool isOnPhone = false;
		public bool isOnCamera = false;

		GlobalManager globalManager;

		[Export]
		NodePath cameraPhonePreviewPath;
		TextureRect cameraPhonePreview;

		[Export]
		NodePath cameraPhonePosPath;
		Marker3D cameraPhonePos;

		public override void _Ready()
		{
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			flashlight = GetNode<SpotLight3D>(flashlightPath);
			camera = GetNode<Camera3D>(cameraPath);
			cameraPhone = GetNode<Camera3D>(cameraPhonePath);
			cameraPhonePos = GetNode<Marker3D>(cameraPhonePosPath);
			cameraPhonePreview = GetNode<TextureRect>(cameraPhonePreviewPath);
			camera.Current = false;
		}

		public void OnCamera(bool isOn)
		{
			isOnCamera = isOn;
			cameraPhonePreview.Visible = isOn;
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
				if (isOnPhone) {
					isOnPhone = false;
					camera.Current = false;
					globalManager.ChangeStateFocus(StateFocus.GAME);
				} else {
					isOnPhone = true;
					camera.Current = true;
					globalManager.ChangeStateFocus(StateFocus.GAME_MENU);
				}
			}
			if (isOnCamera) {
				cameraPhone.GlobalTransform = cameraPhonePos.GlobalTransform;
			}
		}
	}
}
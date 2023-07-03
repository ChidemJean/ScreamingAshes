using Godot;
using System;

namespace ChidemGames.Core.Ui_World3D.Phone
{
	public partial class CameraScreen : PhoneScreen
	{
		
		[Export]
		NodePath cameraPhonePath;
		Camera3D cameraPhone;

		[Export]
		NodePath cameraPhonePreviewPath;
		TextureRect cameraPhonePreview;

		[Export]
		NodePath cameraPhonePosPath;
		Marker3D cameraPhonePos;

		public override void _Ready()
		{
			cameraPhone = GetNode<Camera3D>(cameraPhonePath);
			cameraPhonePos = GetNode<Marker3D>(cameraPhonePosPath);
			cameraPhonePreview = GetNode<TextureRect>(cameraPhonePreviewPath);
		}

		public override void IsOn(bool isOn)
		{
			base.IsOn(isOn);
			cameraPhone.ProcessMode = isOn ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
		}

		public override void _Process(double delta)
		{
			if (isOn) {
				cameraPhone.GlobalTransform = cameraPhonePos.GlobalTransform;
			}
		}
	}
}
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

		public override void _Ready()
		{
			flashlight = GetNode<SpotLight3D>(flashlightPath);
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
		}
	}
}
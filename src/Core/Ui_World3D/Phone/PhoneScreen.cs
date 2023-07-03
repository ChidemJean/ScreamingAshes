using Godot;
using System;

namespace ChidemGames.Core.Ui_World3D.Phone
{
	public partial class PhoneScreen : Control
	{
		[Export]
		public string key;

		public bool isOn = false;

		public override void _Ready()
		{
		}

		public virtual void IsOn(bool isOn)
		{
			Visible = isOn;
			this.isOn = isOn;
		}
	}
}
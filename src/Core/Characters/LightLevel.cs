using Godot;
using System;

namespace ChidemGames.Core.Characters
{
	public partial class LightLevel : SubViewport
	{

		[Export]
		NodePath rootPath;

		Node3D root;

		GlobalManager globalManager;

		public override void _Ready()
		{
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			root = GetNode<Node3D>(rootPath);
		}

		public override void _Process(double delta)
		{
			var player = globalManager.currentPlayer;
			if (player != null && player.lightLevelPos != null) {
				root.GlobalPosition = player.lightLevelPos.GlobalPosition;

				Image img = GetTexture().GetImage();
				//img.Lock();
				float amount = 0;
				float size = img.GetHeight() * img.GetWidth();
				for (int y = 0; y < img.GetHeight(); y++) {
					for (int x = 0; x < img.GetWidth(); x++) {
						Color color = img.GetPixel(x, y);
						amount += (color.R + color.G + color.B) / 3;
					}
				}
				player.lightLevel = amount / size;
			}
		}
	}
}
using Godot;
using System;

namespace ChidemGames.Core.Characters
{
	public class LightLevel : Viewport
	{

		[Export]
		NodePath playerPath;

		Player player;

		[Export]
		NodePath rootPath;

		Spatial root;

		public override void _Ready()
		{
			root = GetNode<Spatial>(rootPath);
			player = GetNode<Player>(playerPath);
		}

		public override void _Process(float delta)
		{
			if (player != null && player.lightLevelPos != null) {
				root.GlobalTranslation = player.lightLevelPos.GlobalTranslation;

				Image img = GetTexture().GetData();
				img.Lock();
				float amount = 0;
				float size = img.GetHeight() * img.GetWidth();
				for (int y = 0; y < img.GetHeight(); y++) {
					for (int x = 0; x < img.GetWidth(); x++) {
						Color color = img.GetPixel(x, y);
						amount += (color.r + color.g + color.b) / 3;
					}
				}
				player.lightLevel = amount / size;
			}
		}
	}
}
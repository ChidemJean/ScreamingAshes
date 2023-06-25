using Godot;
using System;

namespace ChidemGames.Resources
{
   public partial class ItemResource : Resource
   {
		[Export]
		public string itemId;

		[Export]
		public Texture texture;

		[Export]
		public Texture2D textureWhite;

		[Export]
		public int slotsX = 1;

		[Export]
		public int slotsY = 1;

		[Export]
		public int maxStack = 1;

		[Export]
		public string scenePath = "";

		[Export]
		public bool canHandHold = true;

		[Export]
		public int subitems = 0;

   }
}
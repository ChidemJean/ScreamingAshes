using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Ui
{
   public class FastSlot : SlotInventory
   {
		[Export]
		int slotIndex = 0;

      public override void _Ready()
      {
			base._Ready();
      }

		public override void PlaceItem(string itemId)
		{
			base.PlaceItem(itemId);
			globalEvents.EmitSignal(GameEvent.OnFastSlotAttach, slotIndex, itemId);
		}
		
		public override void DetachItem()
		{
			base.DetachItem();
			globalEvents.EmitSignal(GameEvent.OnFastSlotDetach, slotIndex);
		}
   }
}
using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Ui
{
   public partial class FastSlot : SlotInventory
   {
		[Export]
		int slotIndex = 0;

      public override void _Ready()
      {
			base._Ready();
      }

		public override void PlaceItem(string itemId, int itemUniqueId)
		{
			base.PlaceItem(itemId, itemUniqueId);
			globalEvents.EmitSignal(GameEvent.OnFastSlotAttach, slotIndex, itemId, itemUniqueId);
		}
		
		public override void DetachItem()
		{
			base.DetachItem();
			globalEvents.EmitSignal(GameEvent.OnFastSlotDetach, slotIndex);
		}
   }
}
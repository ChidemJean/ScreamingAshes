using Godot;
using System;
using ChidemGames.Ui;

namespace ChidemGames.Events
{
   public partial class GlobalEvents : Node
   {
      
		[Signal]
      public delegate void RenderSizeChangedEventHandler(Vector2 newSize, float scale);

      [Signal]
      public delegate void ChangeRenderSizeEventHandler(float scale);

      [Signal]
      public delegate void ExplosionHappenedEventHandler(Vector3 position);

      [Signal]
      public delegate void CueForceChangeEventHandler(float avg);

      [Signal]
      public delegate void TakeItemEventHandler(string itemId, bool openMenu, int subitems);

      [Signal]
      public delegate void OpenMenuEventHandler();

      [Signal]
      public delegate void CloseMenuEventHandler();

      [Signal]
      public delegate void OnCloseMenuEventHandler();

      [Signal]
      public delegate void OnSlotMouseEnterEventHandler(SlotInventory slotInventory);

      [Signal]
      public delegate void OnSlotMouseLeaveEventHandler(SlotInventory slotInventory);

      [Signal]
      public delegate void DetachItemFromSlotEventHandler(ItemInventory item);

      [Signal]
      public delegate void OnFastSlotAttachEventHandler(int slotIndex, string itemId, int itemUniqueId);

      [Signal]
      public delegate void OnFastSlotDetachEventHandler(int slotIndex);

      [Signal]
      public delegate void InventoryHasBeenUpdateEventHandler();

      [Signal]
      public delegate void OpenPhoneScreenEventHandler(string key);

      [Signal]
      public delegate void PhoneOnHandEventHandler();

   }
}
using Godot;
using System;
using ChidemGames.Ui;

namespace ChidemGames.Events
{
   public class GlobalEvents : Node
   {
      
		[Signal]
      delegate void RenderSizeChanged(Vector2 newSize, float scale);

      [Signal]
      delegate void ChangeRenderSize(float scale);

      [Signal]
      delegate void ExplosionHappened(Vector3 position);

      [Signal]
      delegate void CueForceChange(float avg);

      [Signal]
      delegate void TakeItem(string itemId, bool openMenu);

      [Signal]
      delegate void OpenMenu();

      [Signal]
      delegate void CloseMenu();

      [Signal]
      delegate void OnCloseMenu();

      [Signal]
      delegate void OnSlotMouseEnter(SlotInventory slotInventory);

      [Signal]
      delegate void OnSlotMouseLeave(SlotInventory slotInventory);

      [Signal]
      delegate void DetachItemFromSlot(ItemInventory item);

      [Signal]
      delegate void OnFastSlotAttach(int slotIndex, string itemId);

      [Signal]
      delegate void OnFastSlotDetach(int slotIndex);

   }
}
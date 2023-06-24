using Godot;
using System;

namespace ChidemGames.Ui
{
   public enum SlotInventoryStatus
   {
      Undefined = 0,
      Busy = 1,
      Free = 2,
      HoveredOk = 4,
      HoveredBusy = 5,
      Damaged = 6
   }
}
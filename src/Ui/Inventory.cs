using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Events;
using ChidemGames.Core.Items;
using ChidemGames.Core.Items.Weapons;
using ChidemGames.Resources;

namespace ChidemGames.Ui
{

   public partial class Inventory : Control
   {
      [Export]
      NodePath gridPath;
      GridContainer grid;

      [Export]
      PackedScene slotScene;

      [Export]
      PackedScene itemInvetory;

      GlobalManager globalManager;

      SlotInventory[,] slots;

      [Export]
      int totalRows = 10;

      [Export]
      int totalCols = 10;

      GlobalEvents globalEvents;

      ItemInventory itemDrag = null;

      [Export]
      NodePath itemsHolderPath;
      Control itemsHolder;

      SlotInventory slotHover = null;

      List<FastSlot> fastSlots = new List<FastSlot>();
      List<SlotInventory> slotsHovered = new List<SlotInventory>();
      List<ItemInventory> items = new List<ItemInventory>();

      bool canPlace = true;

      public override void _Ready()
      {
         globalManager = GetNode<GlobalManager>("/root/GlobalManager");
         globalManager.inventory = this;
         globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
         itemsHolder = GetNode<Control>(itemsHolderPath);
         grid = GetNode<GridContainer>(gridPath);

         slots = new SlotInventory[totalRows, totalCols];

         for (int y = 0; y < totalCols; y++)
         {
            for (int x = 0; x < totalRows; x++)
            {
               SlotInventory slot = slotScene.Instantiate<SlotInventory>();
               slot.Pos = new Vector2(x, y);
               grid.AddChild(slot);
               if ((x == 3 && y == 4) || (x == 4 && y == 4) || (x == 3 && y == 5) || (x == 4 && y == 5))
               {
                  slot.IsDamaged = true;
               }
               slots[x, y] = slot;
            }
         }

         foreach (var node in GetNode("FastHandItems").GetChildren())
         {
            fastSlots.Add((FastSlot) node);
         }

         // globalEvents.Connect(GameEvent.TakeItem, this, nameof(OnTakeItem));
         // globalEvents.Connect(GameEvent.OnCloseMenu, this, nameof(OnCloseMenu));
         globalEvents.TakeItem += OnTakeItem;
         globalEvents.OnCloseMenu += OnCloseMenu;

         // globalEvents.Connect(GameEvent.OnSlotMouseEnter, this, nameof(OnSlotMouseEnter));
         // globalEvents.Connect(GameEvent.OnSlotMouseLeave, this, nameof(OnSlotMouseLeave));
         // globalEvents.Connect(GameEvent.DetachItemFromSlot, this, nameof(OnDetachItem));
         globalEvents.OnSlotMouseEnter += OnSlotMouseEnter;
         globalEvents.OnSlotMouseLeave += OnSlotMouseLeave;
         globalEvents.DetachItemFromSlot += OnDetachItem;

      }

      public override void _Input(InputEvent @event)
      {
         if (itemDrag != null)
         {
            if (@event.IsActionPressed("rotate_item_inventory"))
            {
               itemDrag.Rotate();
               itemDrag.UpdateSize(slots[0, 0].Size, GetGridMargin());
            }
            if (@event.IsActionPressed("place_item_inventory"))
            {
               PlaceItem();
            }
         }
      }

      public List<ItemInventory> RequestItem<T>()
      {
         Type type = typeof(T);
         List<ItemInventory> _items = new List<ItemInventory>();

         if (type.Equals(typeof(FirearmClip))) {
            foreach (var item in items) {
               if (item.itemRes is FirearmClipResource && item.GetSubitems() > 0) {
                  _items.Add(item);
               }
            }
         }

         return _items;
      }

      public void RemoveItem(ItemInventory itemInventory)
      {
         itemDrag = itemInventory;
         items.Remove(itemInventory);
         foreach (var slotInventory in GetSlotsForItem(itemInventory))
         {
            slotInventory.DetachItem();
         }
         itemInventory.slotInventoryPivot = null;
         Clean();
         globalEvents.EmitSignal(GameEvent.InventoryHasBeenUpdate);
      }

      public void OnDetachItem(ItemInventory itemInventory)
      {
         itemDrag = itemInventory;
         itemDrag.isFollowingMouse = true;
         itemDrag.isPlaced = false;
         itemDrag.MouseFilter = MouseFilterEnum.Ignore;
         items.Remove(itemDrag);
         foreach (var slotInventory in GetSlotsForItem(itemInventory))
         {
            slotInventory.DetachItem();
         }
         itemDrag.slotInventoryPivot = null;
         globalEvents.EmitSignal(GameEvent.InventoryHasBeenUpdate);
      }

      public void PlaceItem()
      {
         if (slotsHovered.Count > 0 && canPlace)
         {
            items.Add(itemDrag);
            int key = items.IndexOf(itemDrag);
            foreach (SlotInventory slot in slotsHovered)
            {
               slot.PlaceItem(itemDrag.itemId, key);
            }
            itemDrag.Place(slotsHovered, GetGridMargin());
            ClearHoveredSlots();
            itemDrag = null;

            globalEvents.EmitSignal(GameEvent.InventoryHasBeenUpdate);
            // globalManager.hud.ShowLoadCursor(5);
         }
      }

      public int GetSubitemByItemUniqueId(int uniqueId)
      {
         return items[uniqueId].GetSubitems();
      }

      public void UpdateSubitemsForUniqueId(int uniqueId, int subitems = 0)
      {
         items[uniqueId].SetSubitems(subitems);
      }

      public Vector2 GetGridMargin()
      {
         return new Vector2(grid.Get("theme_override_constants/v_separation").ToString().ToFloat(), grid.Get("theme_override_constants/h_separation").ToString().ToFloat());
      }

      public void AddItem(string itemId, int subitems = 0)
      {
         if (itemId != null && itemInvetory != null)
         {
            itemDrag = itemInvetory.Instantiate<ItemInventory>();
            itemsHolder.AddChild(itemDrag);
            itemDrag.InitData(itemId, 1);
            itemDrag.SetSubitems(subitems);
            itemDrag.UpdateUi();
            itemDrag.UpdateSize(slots[0, 0].Size, GetGridMargin());
         
            TryAutomaticPlaceItem();
            if (!canPlace)
            {
               DropItem(itemDrag.itemScenePath, itemDrag.GetSubitems());
            }
         }
      }

      public void OnTakeItem(string itemId, bool openMenu = false, int subitems = -1)
      {
         if (itemId != null && itemInvetory != null && itemDrag == null)
         {
            itemDrag = itemInvetory.Instantiate<ItemInventory>();
            itemsHolder.AddChild(itemDrag);
            itemDrag.InitData(itemId, 1);
            if (subitems > -1) {
               itemDrag.SetSubitems(subitems);
            }
            itemDrag.UpdateUi();
            itemDrag.UpdateSize(slots[0, 0].Size, GetGridMargin());

            bool _openMenu = openMenu;
            if (!openMenu) {
               TryAutomaticPlaceItem();
               _openMenu = !canPlace;
            }

            if (_openMenu)
            {
               globalEvents.EmitSignal(GameEvent.OpenMenu);
               itemDrag.isFollowingMouse = true;
            }
         }
      }

      public void TryAutomaticPlaceItem()
      {
         if (itemDrag.itemRes.category == ItemResourceCategory.Attackable) {
            foreach (var fastSlot in fastSlots) {
               if (fastSlot.Status == SlotInventoryStatus.Free) {
                  ClearHoveredSlots();
                  slotsHovered.Add(fastSlot);
                  canPlace = true;
                  itemDrag.ResetRotation();
                  itemDrag.UpdateSize(fastSlot.Size, Vector2.Zero);
                  PlaceItem();
                  return;
               }
            }
         }
         for (int y = 0; y < slots.GetLength(1); y++)
         {
            for (int x = 0; x < slots.GetLength(0); x++)
            {
               var _slot = slots[x, y];
               OnSlotMouseEnter(_slot);
               if (canPlace)
               {
                  PlaceItem();
                  return;
               }
            }
         }
         // TODO: Notify item added
      }

      public void Clean()
      {
         if (itemDrag != null)
         {
            ClearHoveredSlots();
            itemDrag.isFollowingMouse = false;
            itemDrag.QueueFree();
            itemDrag = null;
         }
      }

      public void OnCloseMenu()
      {
         if (itemDrag != null)
         {
            DropItem(itemDrag.itemScenePath, itemDrag.GetSubitems());
         }
         Clean();
      }

      public void DropItem(string itemScenePath, int subitems = 0)
      {
         var itemScene = ResourceLoader.Load<PackedScene>(itemScenePath);
         Item itemNode = itemScene.Instantiate<Item>();
         globalManager.main3dNode.AddChild(itemNode);
         itemNode.GlobalPosition = globalManager.currentPlayer.GlobalPosition + (globalManager.currentPlayer.GlobalTransform.Basis.Z.Normalized() * 3 + (new Vector3(0, 3f, 0)));
         itemNode.ChangePhysics(false);
         if (itemNode is Weapon) {
            ((Weapon) itemNode).UpdateBullets(subitems);
         }
      }

      public List<SlotInventory> GetSlotsForItem(ItemInventory item)
      {
         List<SlotInventory> _slots = new List<SlotInventory>();

         if (item.slotInventoryPivot != null && item.slotInventoryPivot.type != SlotType.Backpack)
         {
            _slots.Add(item.slotInventoryPivot);
            return _slots;
         }

         int pivotPosX = Mathf.FloorToInt(item.slotPivotPos.X);
         int pivotPosY = Mathf.FloorToInt(item.slotPivotPos.Y);

         for (int x = 0; x < item.slotsX; x++)
         {
            for (int y = 0; y < item.slotsY; y++)
            {
               int xCurrent = pivotPosX + x;
               int yCurrent = pivotPosY + y;

               if (!HasPairIndex(xCurrent, yCurrent))
               {
                  continue;
               }

               SlotInventory slotCheck = slots[xCurrent, yCurrent];
               _slots.Add(slotCheck);
            }
         }

         return _slots;
      }

      public void OnSlotMouseEnter(SlotInventory slotInventory)
      {
         slotHover = slotInventory;

         if (itemDrag == null)
         {
            return;
         }

         ClearHoveredSlots();

         if (slotHover.type == SlotType.Backpack)
         {

            itemDrag.UpdateSize(slots[0, 0].Size, GetGridMargin());

            int pivotPosX = Mathf.FloorToInt(slotHover.Pos.X);
            int pivotPosY = Mathf.FloorToInt(slotHover.Pos.Y);
            canPlace = true;

            for (int x = 0; x < itemDrag.slotsX; x++)
            {
               for (int y = 0; y < itemDrag.slotsY; y++)
               {
                  int xCurrent = pivotPosX + x;
                  int yCurrent = pivotPosY + y;

                  if (!HasPairIndex(xCurrent, yCurrent))
                  {
                     canPlace = false;
                     continue;
                  }

                  SlotInventory slotHovered = slots[xCurrent, yCurrent];
                  slotsHovered.Add(slotHovered);

                  if (!slotHovered.CanUse())
                  {
                     canPlace = false;
                  }
               }
            }

         }
         else
         {
            itemDrag.ResetRotation();
            itemDrag.UpdateSize(slotHover.Size, Vector2.Zero);
            slotsHovered.Add(slotHover);
            if (slotHover.type == SlotType.Fast)
            {
               canPlace = itemDrag.canHandHold && slotHover.CanUse();
            }
            else
            {
               canPlace = slotHover.CanUse();
            }
         }

         foreach (SlotInventory slot in slotsHovered)
         {
            slot.Status = (canPlace) ? SlotInventoryStatus.HoveredOk : SlotInventoryStatus.HoveredBusy;
         }

         itemDrag.ErrorModulate(!canPlace);

      }

      public void OnSlotMouseLeave(SlotInventory slotInventory)
      {
         if (slotInventory == slotHover)
         {
            slotHover = null;
         }
      }

      public void ClearHoveredSlots()
      {
         if (slotsHovered.Count > 0)
         {
            foreach (SlotInventory slot in slotsHovered)
            {
               if (slot.Status == SlotInventoryStatus.HoveredOk || slot.Status == SlotInventoryStatus.HoveredBusy)
               {
                  if (slot.prevStatus != SlotInventoryStatus.Undefined)
                  {
                     slot.Status = slot.prevStatus;
                  }
                  else
                  {
                     slot.Status = SlotInventoryStatus.Free;
                  }
               }
            }
         }
         slotsHovered.Clear();
      }

      public bool HasPairIndex(int rowIndex, int columnIndex)
      {
         if (rowIndex < 0 || rowIndex >= slots.GetLength(0) || columnIndex < 0 || columnIndex >= slots.GetLength(1))
         {
            return false;
         }
         return true;
      }

      public override void _Process(double delta)
      {
         if (Engine.GetFramesDrawn() % 60 == 0) {
         }
      }

   }
}
using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Resources;
using ChidemGames.Events;
using ChidemGames.Extensions;

namespace ChidemGames.Ui
{
   public class ItemInventory : Button
   {
		[Export]
		NodePath thumbPath;
		TextureRect thumb;

		[Export]
		NodePath qtdPath;
		Control qtd;

		[Export]
		NodePath qtdLabelPath;
		Label qtdLabel;

		public bool isFollowingMouse = false;

		public string itemId = "";

		int quantity = 0;
		int subitems = 0;

		GlobalManager globalManager;

		public ItemResource itemRes;

		GlobalEvents globalEvents;

		public int slotsXOriginal = 1;
		public int slotsYOriginal = 1;

		public int slotsX = 1;
		public int slotsY = 1;

		public Vector2 slotPivotPos = Vector2.Inf;

		public bool isPlaced = false;
		public bool canHandHold = false;

		bool clicked = false;

	 	public string itemScenePath = "";

		public SlotInventory slotInventoryPivot;

      public override void _Ready()
      {
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			thumb = GetNode<TextureRect>(thumbPath);
			qtd = GetNode<Control>(qtdPath);
			qtdLabel = GetNode<Label>(qtdLabelPath);

			Connect("gui_input", this, nameof(OnGuiInput));
      }

		public void OnGuiInput(InputEvent @event)
		{
			if (!isPlaced) return;

			if (@event.IsActionPressed("place_item_inventory")) {
				clicked = true;
				return;
			}
			if (@event.IsActionReleased("place_item_inventory")) {
				clicked = false;
				return;
			}
			if (@event is InputEventMouseMotion) {
				var _event = (InputEventMouseMotion) @event;
				Vector2 motion = _event.Relative;
				if (motion.Length() > 0 && clicked) {
					globalEvents.EmitSignal(GameEvent.DetachItemFromSlot, this);
				}
			}
		}

		public void InitData(string itemId, int quantity) 
		{
			this.itemId = itemId;
			this.quantity = quantity;
			itemRes = ResourceLoader.Load<ItemResource>($"{globalManager.itemsResourcePath}{itemId}.tres");
			this.itemScenePath = itemRes.scenePath;
			this.canHandHold = itemRes.canHandHold;
			this.subitems = itemRes.subitems;
			slotsX = itemRes.slotsX;
			slotsY = itemRes.slotsY;
			slotsXOriginal = itemRes.slotsX;
			slotsYOriginal = itemRes.slotsY;
		}

		public void UpdateUi()
		{
			thumb.Texture = itemRes.texture;
			qtd.Visible = subitems > 0;
			qtdLabel.Text = GetSubitems().ToString();
		}

		public int GetSubitems()
		{
			return subitems;
		}

		public void SetSubitems(int qtd)
		{
			subitems = qtd;
		}

		public void UpdateSize(Vector2 sizeSlot, Vector2 gridMargin)
		{
			if (gridMargin == Vector2.Zero) {
				RectMinSize = sizeSlot;
				RectSize = sizeSlot;
				RectPivotOffset = sizeSlot / 2;
				return;
			}
			float marginX = (gridMargin.x * itemRes.slotsX) - gridMargin.x;
			float marginY = (gridMargin.y * itemRes.slotsY) - gridMargin.y;
			Vector2 newSize = (sizeSlot * new Vector2(itemRes.slotsX, itemRes.slotsY)) + new Vector2(marginX, marginY);
			RectMinSize = newSize;
			RectSize = newSize;
			RectPivotOffset = newSize / 2;
		}

      public override void _Process(float delta)
      {
         if (isFollowingMouse) {
				Vector2 mousePosition = GetViewport().GetMousePosition();
				if (slotsXOriginal == slotsX) {
				RectGlobalPosition = mousePosition;
				} else if (slotsXOriginal > slotsYOriginal) {
					float x = (RectSize.y / 2 - RectSize.x / 2);
					float y = (RectSize.x / 2 - RectSize.y / 2);
					RectGlobalPosition = (mousePosition + (new Vector2(x, y)));
				} else {
					float x = (RectSize.y / 2 - RectSize.x / 2);
					float y = (RectSize.x / 2 - RectSize.y / 2);
					RectGlobalPosition = (mousePosition + (new Vector2(x, y)));
				}
			}
      }

		public void ResetRotation()
		{
			slotsX = slotsXOriginal;
			slotsY = slotsYOriginal;
			RectRotation = 0;
		}

		public void Rotate()
		{
			if (slotsXOriginal == slotsYOriginal) {
				return;
			}
			if (slotsX == slotsXOriginal) {
				slotsX = slotsYOriginal;
				slotsY = slotsXOriginal;
				RectRotation = 90;
				return;
			}
			slotsX = slotsXOriginal;
			slotsY = slotsYOriginal;
			RectRotation = 0;
		}

		public void Place(List<SlotInventory> _slots, Vector2 gridMargin)
		{
			MouseFilter = MouseFilterEnum.Pass;
			isFollowingMouse = false;
			SlotInventory slotPivot = _slots[0];
			slotInventoryPivot = slotPivot;
			slotPivotPos = new Vector2(slotPivot.Pos.x, slotPivot.Pos.y);
			
			if (slotsXOriginal == slotsX) {
				RectGlobalPosition = slotPivot.RectGlobalPosition;
			} else if (slotsXOriginal > slotsYOriginal) {
				float x = (RectSize.y / 2 - RectSize.x / 2);
				float y = (RectSize.x / 2 - RectSize.y / 2);
				RectGlobalPosition = (slotPivot.RectGlobalPosition + (new Vector2(x, y)));
			} else {
				float x = (RectSize.y / 2 - RectSize.x / 2);
				float y = (RectSize.x / 2 - RectSize.y / 2);
				RectGlobalPosition = (slotPivot.RectGlobalPosition + (new Vector2(x, y)));
			}
			// } else if (slotsXOriginal > slotsYOriginal) {
			// 	RectGlobalPosition = (slotPivot.RectGlobalPosition + (new Vector2(-(RectSize.x/3f - gridMargin.x * 1.25f), (RectSize.y/2  + gridMargin.y * .5f))));
			// } else {
			// 	RectGlobalPosition = (slotPivot.RectGlobalPosition + (new Vector2(-(RectSize.x/2f - gridMargin.x * .5f), (RectSize.y/3f  + gridMargin.y * 1.25f))));
			// }

			isPlaced = true;
		}

		public void ErrorModulate(bool error)
		{
			if (error) {
				Modulate = new Color(1, .45f, .45f);
			} else {
				Modulate = new Color(1, 1, 1);
			}
		}
   }
}
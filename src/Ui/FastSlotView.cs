using Godot;
using System;
using ChidemGames.Events;
using ChidemGames.Resources;

namespace ChidemGames.Ui
{
   public partial class FastSlotView : HBoxContainer
   {
		[Export]
		NodePath thumbPath;
		public TextureRect thumb;

		[Export]
		StyleBox defaultStyleBox;

		[Export]
		StyleBox selectedStyleBox;

		[Export]
		NodePath panelPath;
		Panel panel;

		GlobalManager globalManager;
		GlobalEvents globalEvents;

		[Export]
		int slotIndex = 0;

		string itemId = "";
		string itemScenePath = "";
		int itemUniqueId = 0;

		[Export]
		string inputActionName = "";

		bool isSelected = false;

      public override void _Ready()
      {
			panel = GetNode<Panel>(panelPath);
			thumb = GetNode<TextureRect>(thumbPath);

			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			// globalEvents.Connect(GameEvent.OnFastSlotAttach, this, nameof(OnFasSlotAttach));
			// globalEvents.Connect(GameEvent.OnFastSlotDetach, this, nameof(OnFasSlotDetach));
			globalEvents.PhoneOnHand += Unselect;
			globalEvents.OnFastSlotAttach += OnFasSlotAttach;
			globalEvents.OnFastSlotDetach += OnFasSlotDetach;
      }

		public override void _Input(InputEvent @event)
		{
			string slotActionName = $"fast_slot_{slotIndex+1}";
			for (int i = 0; i < 3; i++) {
				string actionName = $"fast_slot_{i+1}";
				if (!isSelected && actionName == slotActionName && @event.IsActionPressed(actionName)) {
					if (itemId != "") {
						Select();
						if (this.itemScenePath != null) {
							globalManager.currentPlayer.PlaceItemOnRightHand(this.itemScenePath, this.itemUniqueId);
							globalEvents.EmitSignal(GameEvent.InventoryHasBeenUpdate);
							if (globalManager.currentPlayer.isFlashlightOnHand) {
								globalManager.currentPlayer.phone.Off();
								globalManager.currentPlayer.SwapFlashlightSlot(false);
							}
						}
						return;
					} else {
						globalManager.currentPlayer.RemoveItemRightHand();
						return;
					}
				}
				if (isSelected && actionName != slotActionName && @event.IsActionPressed(actionName)) {
					Unselect();
					return;
				}
			}
		}

		public void OnFasSlotAttach(int slotIndex, string itemId, int itemUniqueId = 0)
		{
			if (this.slotIndex != slotIndex) {
				return;
			}
			this.itemId = itemId;
			var itemRes = ResourceLoader.Load<ItemResource>($"{globalManager.itemsResourcePath}{itemId}.tres");
			itemScenePath = itemRes.scenePath;
			this.itemUniqueId = itemUniqueId;
			thumb.Texture = itemRes.textureWhite;
			var tween = GetTree().CreateTween();
			tween.TweenProperty(thumb, "modulate:a", .9f, .16f);
			if (isSelected) {
				if (this.itemScenePath != null) globalManager.currentPlayer.PlaceItemOnRightHand(this.itemScenePath, this.itemUniqueId);
			}
		}

		public void OnFasSlotDetach(int slotIndex)
		{
			if (this.slotIndex != slotIndex) {
				return;
			}
			this.itemScenePath = "";
			thumb.Texture = null;
			globalManager.currentPlayer.RemoveItemRightHand(this.itemId);
			this.itemId = "";
		}

		public void Select()
		{
			isSelected = true;
			panel.Set("theme_override_styles/panel", selectedStyleBox);
			var tween = GetTree().CreateTween();
			tween.TweenProperty(thumb, "modulate:a", .9f, .16f);
		}

		public void Unselect()
		{
			isSelected = false;
			panel.Set("theme_override_styles/panel", defaultStyleBox);
			var tween = GetTree().CreateTween();
			tween.TweenProperty(thumb, "modulate:a", .08f, .1f);
		}

   }
}
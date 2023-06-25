using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Ui
{
   public partial class SlotInventory : Button
   {
		Vector2 pos = Vector2.Zero;

		public Vector2 Pos {
			get {
				return pos;
			}
			set {
				pos = value;
				// Text = value.x + ", " + value.y;
			}
		}

		protected GlobalEvents globalEvents;

		[Export]
		StyleBox styleBoxDefault;

		[Export]
		StyleBox styleBoxDamaged;

		bool isDamaged = false;

		public bool IsDamaged {
			get {
				return isDamaged;
			}
			set {
				isDamaged = value;
				if (value) {
					Set("custom_styles/normal", styleBoxDamaged);
					Status = SlotInventoryStatus.Damaged;
					Text = "X";
				} else {
					Set("custom_styles/normal", styleBoxDefault);
					Status = SlotInventoryStatus.Free;
					Text = "•";
				}
			}
		}

		[Export]
		SlotInventoryStatus status = SlotInventoryStatus.Free;

		[Export]
		public SlotType type = SlotType.Backpack;

		public SlotInventoryStatus prevStatus = SlotInventoryStatus.Undefined;

		public string itemId = "";

		public SlotInventoryStatus Status {
			get {
				return status;
			}
			set {
				prevStatus = status;
				status = value;
				string textStatus = "";

				switch (value) {
					case SlotInventoryStatus.Busy:
						SelfModulate = new Color(1, 1, 1, 0);
						textStatus = "busy";
						break;
					case SlotInventoryStatus.Damaged:
						SelfModulate = new Color(1, 1, 1);
						textStatus = "damaged";
						break;
					case SlotInventoryStatus.Free:
						SelfModulate = new Color(1, 1, 1);
						textStatus = "free";
						break;
					case SlotInventoryStatus.HoveredOk:
						SelfModulate = new Color(1, 1, 1, isDamaged ? .9f : .25f);
						textStatus = "hover_ok";
						break;
					case SlotInventoryStatus.HoveredBusy:
						SelfModulate = new Color(1, 1, 1, isDamaged ? .9f : .25f);
						textStatus = "hover_busy";
						break;
				}

			}
		}

		public bool CanUse()
		{
			return status == SlotInventoryStatus.Free;
		}

      public override void _Ready()
      {
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			// Connect("mouse_entered", this, nameof(OnMouseEntered));
			// Connect("mouse_exited", this, nameof(OnMouseExited));
			MouseEntered += OnMouseEntered;
			MouseExited += OnMouseExited;
      }

		public void OnMouseEntered()
		{
			globalEvents.EmitSignal(GameEvent.OnSlotMouseEnter, this);
		}

		public void OnMouseExited()
		{
			globalEvents.EmitSignal(GameEvent.OnSlotMouseEnter, this);
		}

		public virtual void PlaceItem(string itemId)
		{
			Status = SlotInventoryStatus.Busy;
			this.itemId = itemId;
		}
		
		public virtual void DetachItem()
		{
			Status = SlotInventoryStatus.Free;
			itemId = "";
		}
   }
}
using Godot;
using System;
using ChidemGames.Core.Items;
using ChidemGames.Events;
using ChidemGames.Core.Scenario;

namespace ChidemGames.Ui
{

   public partial class InteractFeedback : MarginContainer
   {

		[Export]
		NodePath takePath;
		Control take;

		[Export]
		NodePath inspectPath;
		Control inspect;

		[Export]
		NodePath interactPath;
		Control interact;

		GlobalManager globalManager;
		GlobalEvents globalEvents;

      public override void _Ready()
      {
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			take = GetNode<Control>(takePath);
			inspect = GetNode<Control>(inspectPath);
			interact = GetNode<Control>(interactPath);
      }

		public override void _Process(double delta)
		{
			if (globalManager.currentPlayer != null) {
				Item curItem = globalManager.currentPlayer.currentSelectedItem;
				if (curItem != null && curItem.isInteractive) {
					take.Visible = curItem.isTakeable;
					inspect.Visible = curItem.isInspectable;
					return;
				}
				Interactable interactable = globalManager.currentPlayer.currentSelectedInteractable;
				if (interactable != null) {
					interact.Visible = true;
					interact.GetNode<Label>("Label").Text = interactable.isOpen ? "Fechar" : "Abrir";
					return;
				}
			}

			take.Visible = false;
			inspect.Visible = false;
			interact.Visible = false;
		}

      public override void _Input(InputEvent @event)
      {
			if (globalManager.currentPlayer == null) {
				return;
			}
         if (@event.IsActionPressed("interact")) {
				if (globalManager.currentPlayer.currentSelectedItem != null) {
					string itemId = globalManager.currentPlayer.currentSelectedItem.itemId;
					globalManager.currentPlayer.currentSelectedItem.QueueFree();
					globalManager.currentPlayer.currentSelectedItem = null;
					globalEvents.EmitSignal(GameEvent.TakeItem, itemId, false);
				}
				if (globalManager.currentPlayer.currentSelectedInteractable != null) {
					Interactable interactable = globalManager.currentPlayer.currentSelectedInteractable;
					interactable.Interact();
				}
			}
      }
   }
}
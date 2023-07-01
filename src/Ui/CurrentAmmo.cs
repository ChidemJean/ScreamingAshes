using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Core.Items;
using ChidemGames.Core.Items.Weapons;
using ChidemGames.Resources;
using ChidemGames.Events;

namespace ChidemGames.Ui
{
    public partial class CurrentAmmo : Control
    {
        [Export] NodePath currentLabelPath;
        Label currentLabel;

        [Export] NodePath totalLabelPath;
        Label totalLabel;

        GlobalManager globalManager;
        GlobalEvents globalEvents;

        public override void _Ready()
        {
            currentLabel = GetNode<Label>(currentLabelPath);
            totalLabel = GetNode<Label>(totalLabelPath);
            globalManager = GetNode<GlobalManager>("/root/GlobalManager");
            globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			globalEvents.InventoryHasBeenUpdate += UpdateAvailableAmmo;
        }

        public override void _Process(double delta)
        {
            if (globalManager.currentPlayer != null)
            {
                if (globalManager.currentPlayer.itemRightHand != null)
                {
                    var itemRightHand = globalManager.currentPlayer.itemRightHand;
					if (itemRightHand is Weapon) {
						Visible = true;
						if (!globalManager.currentPlayer.isReloading)
						{
							currentLabel.Text = ((Weapon)itemRightHand).GetCurrentBullets().ToString();
						}
						return;
					}
                }
				Visible = false;
            }
        }

        public void UpdateAvailableAmmo()
        {
			if (globalManager.currentPlayer != null)
            {
                if (globalManager.currentPlayer.itemRightHand != null)
                {
                    var itemRightHand = globalManager.currentPlayer.itemRightHand;
                    if (itemRightHand is Weapon)
                    {
						int available = 0;
						List<ItemInventory> items = globalManager.inventory.RequestItem<FirearmClip>();
						if (items != null && items.Count > 0)
						{
							foreach (var itemInv in items)
							{
								FirearmClipResource clipRes = itemInv.itemRes as FirearmClipResource;
								if (((Weapon) itemRightHand).acceptedClipTypes.Contains(clipRes.type))
								{
									available += itemInv.GetSubitems();
								}
							}
						}
                        totalLabel.Text = available.ToString();
                    }
                }
            }
        }
    }
}
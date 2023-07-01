using Godot;
using System;
using ChidemGames.Core.Items.Weapons;

namespace ChidemGames.Core.Items
{
    public partial class Weapon : Attackable
    {
        [Export]
        public float damageShot = .25f;

        [Export]
        public Godot.Collections.Array<FirearmClipType> acceptedClipTypes = new Godot.Collections.Array<FirearmClipType>();

        public override void Attack()
        {

        }

        public virtual int GetCurrentBullets()
        {
            return 0;
        }

        public virtual void UpdateBullets(int bullets = -1)
        {

        }

        public override void UpdateInventorySubitems()
        {
            base.UpdateInventorySubitems();
            globalManager.inventory.UpdateSubitemsForUniqueId(this.itemUniqueId, GetCurrentBullets());
        }
    }
}
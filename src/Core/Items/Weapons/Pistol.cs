using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Core.Audio;
using ChidemGames.Core.Characters;
using ChidemGames.Core.Items;
using ChidemGames.Core.Decal;
using ChidemGames.Ui;
using ChidemGames.Resources;

namespace ChidemGames.Core.Items.Weapons
{
   public partial class Pistol : Weapon, Shootable
   {
      [Export]
      NodePath animPlayerPath;
      AnimationPlayer animPlayer;

      [Export]
      NodePath muzzleFlashParticlesPath;
      GpuParticles3D muzzleFlashParticles;

      [Export]
      NodePath sxfPath;
      SfxOptions sfx;

      [Export]
      NodePath capsulePositionPath;
      Marker3D capsulePosition;

      [Export]
      NodePath slidePath;
      MeshInstance3D slide;

      [Export]
      PackedScene capsule9mm;

      [Export]
      float capsuleDropSpeed = 4f;

      [Export]
      float slideAmount = -0.62f;

      [Export]
      Godot.Collections.Array<FirearmClipType> acceptedClipTypes = new Godot.Collections.Array<FirearmClipType>();

      Node3D game3dRoot;

      Vector3 initilSlidePosition;

      bool isSlideOpen = false;

      [Export]
      PackedScene clip;

      [Export]
      NodePath clipPosPath;

      Marker3D clipPos;

      FirearmClip currentClip = null;

      [Export]
      PackedScene bulletDecal;

      [Export]
      NodePath bulletFromPath;
      Marker3D bulletFrom;

      [Export]
      float bulletRange = 100f;

      int tempBullets = 0;

      [Export(PropertyHint.Layers3DPhysics)]
      uint shotRayMask;

      public override void _Ready()
      {
         base._Ready();
         animPlayer = GetNode<AnimationPlayer>(animPlayerPath);
         muzzleFlashParticles = GetNode<GpuParticles3D>(muzzleFlashParticlesPath);
         sfx = GetNode<SfxOptions>(sxfPath);
         capsulePosition = GetNode<Marker3D>(capsulePositionPath);
         bulletFrom = GetNode<Marker3D>(bulletFromPath);
         clipPos = GetNode<Marker3D>(clipPosPath);
         slide = GetNode<MeshInstance3D>(slidePath);

         if (playerPath != null)
         {
            player = GetNode<Player>(playerPath);
         }
         initilSlidePosition = slide.Position;

         currentClip = clipPos.GetChildOrNull<FirearmClip>(0);
         if (currentClip != null) {
            currentClip.isTakeable = false;
         }
         game3dRoot = globalManager.main3dNode;
      }

      public bool Shoot()
      {
         if (currentClip == null)
         {
            sfx.PlaySfx("empty", player);
            return false;
         }

         var tween = CreateTween();
         tween.TweenProperty(slide, "position:z", initilSlidePosition.Z + slideAmount, .07f);
         isSlideOpen = true;

         if (currentClip.bullets - 1 < 0)
         {
            sfx.PlaySfx("empty", player);
            return false;
         }

         currentClip.bullets--;
         animPlayer.Play("shoot");
         muzzleFlashParticles.Emitting = true;
         sfx.PlaySfx("shoot", player);

         // 
         var resultShootRay = GetBulletRay();

         if (resultShootRay.Count > 0)
         {
            Vector3 shootPosition = (Vector3)resultShootRay["position"];
            Vector3 shootNormal = (Vector3)resultShootRay["normal"];
            Node node = (Node) resultShootRay["collider"];
            if (node is BodyPartHittable) {
               ((BodyPartHittable) node).ApplyDamageOnBody(damageShot, shootPosition);
            }
            var decalNode = bulletDecal.Instantiate<ShotDecal>();
            node.AddChild(decalNode);
            decalNode.GlobalPosition = shootPosition;
         }
         //

         SpawnCapsule();

         if (currentClip.bullets > 0)
         {
            tween.TweenProperty(slide, "position:z", initilSlidePosition.Z, .06f);
            isSlideOpen = false;
         }

         return true;
      }

      public void SpawnCapsule()
      {
         RigidBody3D capsule = capsule9mm.Instantiate<RigidBody3D>();
         globalManager.main3dNode.AddChild(capsule);
         capsule.GlobalPosition = capsulePosition.GlobalPosition;
         capsule.ApplyImpulse(Vector3.Zero, new Vector3(capsuleDropSpeed * 2, capsuleDropSpeed, 0));
      }

      public bool Reload()
      {
         // if (currentClip == null) {
         //    return false;
         // }
         if (currentClip != null && currentClip.bullets == currentClip.maxBullets)
         {
            return false;
         }

         ItemInventory clipItemInv = null;
         List<ItemInventory> items = globalManager.inventory.RequestItem<FirearmClip>();
         if (items != null && items.Count > 0) {
            foreach (var itemInv in items) {
               FirearmClipResource clipRes = itemInv.itemRes as FirearmClipResource;
               if (acceptedClipTypes.Contains(clipRes.type)) {
                  if (clipItemInv == null || itemInv.GetSubitems() > clipItemInv.GetSubitems()) {
                     clipItemInv = itemInv;
                  }
               }
            }
         }

         if (clipItemInv == null) {
            return false;
         }

         tempBullets = clipItemInv.GetSubitems();
         clip = ResourceLoader.Load<PackedScene>(clipItemInv.itemScenePath);
         globalManager.inventory.RemoveItem(clipItemInv);

         if (currentClip != null) {
            globalManager.inventory.AddItem(currentClip.itemId, currentClip.bullets);
            currentClip.QueueFree();
            currentClip = null;
         }

         return true;
      }

      public async void PlayReloadSfx(float reloadTime, Node3D clipPivot)
      {
         var tween = CreateTween();
         tween.TweenProperty(slide, "position:z", initilSlidePosition.Z + slideAmount, .07f);
         CatchClip(reloadTime, clipPivot);
         await ToSignal(GetTree().CreateTimer(reloadTime * .85f), "timeout");
         tween = CreateTween();
         tween.TweenProperty(slide, "position:z", initilSlidePosition.Z, .07f);
         sfx.PlaySfx("cock", player);
      }

      public async void CatchClip(float reloadTime, Node3D clipPivot)
      {
         DropClip();
         await ToSignal(GetTree().CreateTimer(reloadTime * .3f), "timeout");
         Node3D clipNode = SpawnClip(clipPivot);
         await ToSignal(GetTree().CreateTimer(reloadTime * .5f), "timeout");
         clipPivot.RemoveChild(clipNode);
         clipPos.AddChild(clipNode);
      }

      public async void DropClip()
      {
         if (currentClip != null)
         {
            var clipToDrop = currentClip;
            currentClip = null;
            clipToDrop.GetParent().RemoveChild(clipToDrop);
            globalManager.main3dNode.AddChild(clipToDrop);
            var tween = CreateTween();
            tween.TweenProperty(clipToDrop, "global_position:y", clipToDrop.GlobalPosition.Y - 10f, 1.27f);
            await ToSignal(tween, "finished");
            clipToDrop.QueueFree();
         }
      }

      public Godot.Collections.Dictionary GetBulletRay()
      {
         var spaceState = GetWorld3D().DirectSpaceState;
         var rayQuery = PhysicsRayQueryParameters3D.Create(bulletFrom.GlobalPosition, bulletFrom.GlobalPosition + GlobalTransform.Basis.Z.Normalized() * bulletRange, shotRayMask);
         var resultShootRay = spaceState.IntersectRay(rayQuery);
         return resultShootRay;
      }

      public Node3D SpawnClip(Node parent)
      {
         var clipNode = clip.Instantiate<FirearmClip>();
         clipNode.bullets = tempBullets;
         tempBullets = 0;
         currentClip = clipNode;
         if (currentClip != null) {
            currentClip.isTakeable = false;
         }
         parent.AddChild(clipNode);
         return clipNode;
      }

      public void ReloadFinish()
      {
         if (isSlideOpen)
         {
            // var tween = GetTree().CreateTween();
            // tween.TweenProperty(slide, "translation:z", initilSlidePosition.z, .07f);
         }
      }

      public override void Attack()
      {
         base.Attack();
      }

      public void ButtBlow()
      {
      }

      public override void _PhysicsProcess(double delta)
      {
         base._PhysicsProcess(delta);
         if (player == null) {
            return;
         }
			if (player.isAiming) {
				var resultShootRay = GetBulletRay();
				if (resultShootRay.Count > 0)
				{
					// ((MeshInstance3D)laser.GetChild(0)).Visible = true;
					Vector3 shootPosition = (Vector3)resultShootRay["position"];
					// ((MeshInstance3D)laser.GetChild(0)).GlobalPosition = shootPosition;
				} else {
					// ((MeshInstance3D)laser.GetChild(0)).Visible = false;
				}
			} else {
				// ((MeshInstance3D)laser.GetChild(0)).Visible = false;
			}
      }
   }
}
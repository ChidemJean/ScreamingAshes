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
   public class Pistol : Weapon, Shootable
   {
      [Export]
      NodePath animPlayerPath;
      AnimationPlayer animPlayer;

      [Export]
      NodePath muzzleFlashParticlesPath;
      Particles muzzleFlashParticles;

      [Export]
      NodePath sxfPath;
      SfxOptions sfx;

      [Export]
      NodePath capsulePositionPath;
      Position3D capsulePosition;

      [Export]
      NodePath slidePath;
      MeshInstance slide;

      [Export]
      PackedScene capsule9mm;

      [Export]
      float capsuleDropSpeed = 4f;

      [Export]
      float slideAmount = -0.62f;

      [Export]
      Godot.Collections.Array<FirearmClipType> acceptedClipTypes = new Godot.Collections.Array<FirearmClipType>();

      Spatial game3dRoot;

      Vector3 initilSlidePosition;

      bool isSlideOpen = false;

      [Export]
      PackedScene clip;

      [Export]
      NodePath clipPosPath;

      Position3D clipPos;

      FirearmClip currentClip = null;

      [Export]
      PackedScene bulletDecal;

      [Export]
      NodePath bulletFromPath;
      Position3D bulletFrom;

      [Export]
      float bulletRange = 100f;

      [Export]
      NodePath laserPath;

      ImmediateGeometry laser;

      int tempBullets = 0;

      public override void _Ready()
      {
         base._Ready();
         game3dRoot = GetNode<Spatial>("/root/MainScene/ViewportContainer/Viewport/Game");
         animPlayer = GetNode<AnimationPlayer>(animPlayerPath);
         muzzleFlashParticles = GetNode<Particles>(muzzleFlashParticlesPath);
         sfx = GetNode<SfxOptions>(sxfPath);
         capsulePosition = GetNode<Position3D>(capsulePositionPath);
         bulletFrom = GetNode<Position3D>(bulletFromPath);
         clipPos = GetNode<Position3D>(clipPosPath);
         slide = GetNode<MeshInstance>(slidePath);
         laser = GetNode<ImmediateGeometry>(laserPath);
         if (playerPath != null)
         {
            player = GetNode<Player>(playerPath);
         }
         initilSlidePosition = slide.Translation;

         currentClip = clipPos.GetChildOrNull<FirearmClip>(0);
         if (currentClip != null) {
            currentClip.isTakeable = false;
         }

         laser.Begin(Mesh.PrimitiveType.Lines);
         laser.AddVertex(bulletFrom.Translation);
         laser.AddVertex(bulletFrom.Translation + bulletFrom.Transform.basis.z.Normalized() * bulletRange * 500);
         laser.End();
      }

      public bool Shoot()
      {
         if (currentClip == null)
         {
            sfx.PlaySfx("empty", player);
            return false;
         }

         var tween = GetTree().CreateTween();
         tween.TweenProperty(slide, "translation:z", initilSlidePosition.z + slideAmount, .07f);
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

         if (resultShootRay != null && resultShootRay.Contains("position"))
         {
            Vector3 shootPosition = (Vector3)resultShootRay["position"];
            Vector3 shootNormal = (Vector3)resultShootRay["normal"];
            var decalNode = bulletDecal.Instance<ShotDecal>();
            GetNode<Spatial>("/root/MainScene/ViewportContainer/Viewport/Game").AddChild(decalNode);
            decalNode.GlobalTranslation = shootPosition;
         }
         //

         SpawnCapsule();

         if (currentClip.bullets > 0)
         {
            tween.TweenProperty(slide, "translation:z", initilSlidePosition.z, .06f);
            isSlideOpen = false;
         }

         return true;
      }

      public void SpawnCapsule()
      {
         RigidBody capsule = capsule9mm.Instance<RigidBody>();
         game3dRoot.AddChild(capsule);
         capsule.GlobalTranslation = capsulePosition.GlobalTranslation;
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

      public async void PlayReloadSfx(float reloadTime, Spatial clipPivot)
      {
         var tween = GetTree().CreateTween();
         tween.TweenProperty(slide, "translation:z", initilSlidePosition.z + slideAmount, .07f);
         CatchClip(reloadTime, clipPivot);
         await ToSignal(GetTree().CreateTimer(reloadTime * .85f), "timeout");
         tween = GetTree().CreateTween();
         tween.TweenProperty(slide, "translation:z", initilSlidePosition.z, .07f);
         sfx.PlaySfx("cock", player);
      }

      public async void CatchClip(float reloadTime, Spatial clipPivot)
      {
         DropClip();
         await ToSignal(GetTree().CreateTimer(reloadTime * .3f), "timeout");
         Spatial clipNode = SpawnClip(clipPivot);
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
            var tween = GetTree().CreateTween();
            tween.TweenProperty(clipToDrop, "global_translation:y", clipToDrop.GlobalTranslation.y - 10f, 1.27f);
            await ToSignal(tween, "finished");
            clipToDrop.QueueFree();
         }
      }

      public Godot.Collections.Dictionary GetBulletRay()
      {
         var spaceState = GetWorld().DirectSpaceState;
         var resultShootRay = spaceState.IntersectRay(bulletFrom.GlobalTranslation, bulletFrom.GlobalTranslation + GlobalTransform.basis.z.Normalized() * bulletRange);
         return resultShootRay;
      }

      public Spatial SpawnClip(Node parent)
      {
         var clipNode = clip.Instance<FirearmClip>();
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

      public void ButtBlow()
      {
      }

      public override void _PhysicsProcess(float delta)
      {
         base._PhysicsProcess(delta);
         if (player == null) {
            return;
         }
			if (player.isAiming) {
				var resultShootRay = GetBulletRay();
				if (resultShootRay != null && resultShootRay.Contains("position"))
				{
					((MeshInstance)laser.GetChild(0)).Visible = true;
					Vector3 shootPosition = (Vector3)resultShootRay["position"];
					((MeshInstance)laser.GetChild(0)).GlobalTranslation = shootPosition;
				} else {
					((MeshInstance)laser.GetChild(0)).Visible = false;
				}
			} else {
				((MeshInstance)laser.GetChild(0)).Visible = false;
			}
      }
   }
}
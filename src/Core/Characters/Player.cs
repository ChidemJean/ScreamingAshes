using Godot;
using System;
using ChidemGames.Core.Items.Weapons;
using ChidemGames.Core.Items.Melee;
using ChidemGames.Core.Items;
using ChidemGames.Core.Scenario;
using ChidemGames.Core.Audio;
using ChidemGames.Extensions;
using ChidemGames.Core.Vfx;

namespace ChidemGames.Core.Characters
{
   public class Player : KinematicBody, AdaptiveAudioEmitter
   {
      [Export]
      NodePath animationTreePath;

      [Export]
      float speed = 2f;

      [Export]
      float sprintSpeedMultiplier = 2f;

      [Export]
      float crouchSpeedMultiplier = .4f;

      [Export]
      float jumpVelocity = 2f;

      [Export]
      NodePath cameraPath;

      [Export]
      float sensitivity = 5;

      [Export]
      NodePath lookAtPath;

      [Export]
      NodePath skeletonIkRightHandPath;
      Godot.SkeletonIK skeletonIKRightHand;

      [Export]
      NodePath skeletonIkRightHandCodePath;
      Godot.SkeletonIK skeletonIKRightCodeHand;

      [Export]
      NodePath skeletonIkLeftHandPath;
      Godot.SkeletonIK skeletonIKLeftHand;

      [Export]
      NodePath lookAtRightPath;
      Position3D lookAtRight;

      [Export]
      NodePath pivotPath;

      public Position3D pivot;
      Position3D lookAt;

      [Export]
      NodePath lookAtAimPath;
      Position3D lookAtAim;
      Position3D inverseLookAtAim;

      [Export]
      NodePath lookAtHeadPath;
      Position3D lookAtHead;

      [Export]
      NodePath itemSlotRightPath;
      Spatial itemSlotRight;

      [Export]
      NodePath handPivotsAnimPlayerPath;
      AnimationPlayer handPivotsAnimPlayer;

      public Camera camera;
      Spatial head;

      AnimationTree animationTree;

      Vector3 velocity = Vector3.Zero;

      public bool isAiming = false;
      public bool isCrouching = false;

      bool isSprinting = false;

      [Export]
      public bool isReloading = false;

      [Export]
      float reloadTime = 4.033f;

      [Export]
      float buttBlowingTime = 3f;

      [Export]
      float knifeAttackTime = 3f;

      [Export]
      NodePath standShapePath;

      [Export]
      NodePath crouchShapePath;

      CollisionShape standShape;
      CollisionShape crouchShape;

      float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

      Item itemRightHand = null;

      Vector3 initialRotationLookAtHead;

      [Export]
      float verticalDisplacementCrouch = 5f;

      // RAYS
      [Export] NodePath aimRayPath;
      [Export] NodePath topRayPath;
      [Export] NodePath bottomRayPath;
      [Export] NodePath rightRayPath;
      [Export] NodePath leftRayPath;
      [Export] NodePath forwardRayPath;
      [Export] NodePath backwardRayPath;
      RayCast aimRay;
      RayCast topRay;
      RayCast bottomRay;
      RayCast rightRay;
      RayCast leftRay;
      RayCast forwardRay;
      RayCast backwardRay;

      [Export]
      NodePath clipPivotPath;
      Position3D clipPivot;

      [Export] NodePath lightLevelPosPath;
      public Position3D lightLevelPos;
      public float lightLevel = 0f;

      public bool isButtBlowing = false;

      GlobalManager globalManager;
      string currentBlendAim = null;

      [Export]
      NodePath flashLightPath;
      SpotLight flashLight;

      [Export]
      NodePath flashLightSlotPath;
      Spatial flashLightSlot;

      [Export]
      NodePath flashLightHandSlotPath;
      Spatial flashLightHandSlot;

      bool isFlashlightOnHand = false;

      [Export]
      public float maxStaminaValue = 1f;

      [Export]
      public float defaultStaminaVariation = 0.005f;

      public float currentStaminaVariation;

      public float currentStamina;

      [Export]
      public float cooldownRegenerateStamine = 10f;

      float currentCooldownRegenerateStamine = 0;

      [Export(PropertyHint.Layers3dPhysics)]
      uint collisionMaskItemsRay;

      public Item currentSelectedItem = null;
      public Interactable currentSelectedInteractable = null;

      public bool inInventory = false;
      public bool keepingBackpack = false;

      public bool isPlaying = true;

      [Export]
      NodePath backpackPath;
      Spatial backpack;

      [Export]
      NodePath neckBackpackSlotPath;
      Position3D neckBackpackSlot;

      [Export]
      NodePath handBackpackSlotPath;
      Position3D handBackpackSlot;

      SceneTreeTween tweenBackpack;

      [Export]
      public float maxHealth = 1;

      [Export]
      public float health = 1;

      public bool isDead = false;

      [Export]
      NodePath sfxPath;
      SfxOptions sfx;

      [Export]
      PackedScene bloodParticles;

      public override void _Ready()
      {
         globalManager = GetNode<GlobalManager>("/root/GlobalManager");
         globalManager.currentPlayer = this;

         currentStamina = maxStaminaValue;
         currentStaminaVariation = defaultStaminaVariation;

         animationTree = GetNode<AnimationTree>(animationTreePath);
         lookAt = GetNode<Position3D>(lookAtPath);
         lookAtRight = GetNode<Position3D>(lookAtRightPath);
         lookAtAim = GetNode<Position3D>(lookAtAimPath);
         pivot = GetNode<Position3D>(pivotPath);
         lookAtHead = GetNode<Position3D>(lookAtHeadPath);
         clipPivot = GetNode<Position3D>(clipPivotPath);
         camera = GetNode<Camera>(cameraPath);
         head = camera.GetParent<Spatial>();
         itemSlotRight = GetNode<Spatial>(itemSlotRightPath);
         handPivotsAnimPlayer = GetNode<AnimationPlayer>(handPivotsAnimPlayerPath);
         skeletonIKRightHand = GetNode<Godot.SkeletonIK>(skeletonIkRightHandPath);
         skeletonIKLeftHand = GetNode<Godot.SkeletonIK>(skeletonIkLeftHandPath);
         skeletonIKRightCodeHand = GetNode<Godot.SkeletonIK>(skeletonIkRightHandCodePath);
         inverseLookAtAim = lookAtAim.GetNode<Position3D>("InverseLookAtAim");
         sfx = GetNode<SfxOptions>(sfxPath);
         backpack = GetNode<Spatial>(backpackPath);
         neckBackpackSlot = GetNode<Position3D>(neckBackpackSlotPath);
         handBackpackSlot = GetNode<Position3D>(handBackpackSlotPath);

         flashLight = GetNode<SpotLight>(flashLightPath);
         flashLightSlot = GetNode<Spatial>(flashLightSlotPath);
         flashLightHandSlot = GetNode<Spatial>(flashLightHandSlotPath);

         lightLevelPos = GetNode<Position3D>(lightLevelPosPath);
         standShape = GetNode<CollisionShape>(standShapePath);
         crouchShape = GetNode<CollisionShape>(crouchShapePath);

         itemRightHand = itemSlotRight.GetChildOrNull<Item>(0);
         if (itemRightHand != null && itemRightHand.player == null)
         {
            itemRightHand.player = this;
         }

         initialRotationLookAtHead = lookAtHead.Rotation;

         aimRay = GetNode<RayCast>(aimRayPath);
         topRay = GetNode<RayCast>(topRayPath);
         bottomRay = GetNode<RayCast>(bottomRayPath);
         rightRay = GetNode<RayCast>(rightRayPath);
         leftRay = GetNode<RayCast>(leftRayPath);
         forwardRay = GetNode<RayCast>(forwardRayPath);
         backwardRay = GetNode<RayCast>(backwardRayPath);

      }

      public void PlaceItemOnRightHand(string itemScenePath)
      {
         var itemScene = ResourceLoader.Load<PackedScene>(itemScenePath);
         var itemNode = itemScene.Instance<Item>();
         if (itemRightHand != null)
         {
            itemRightHand.QueueFree();
            itemRightHand = null;
         }
         itemSlotRight.AddChild(itemNode);
         itemRightHand = itemNode;
         itemRightHand.player = this;
      }

      public void RemoveItemRightHand(string itemId)
      {
         if (itemRightHand != null)
         {
            if (itemRightHand.itemId == itemId)
            {
               itemRightHand.QueueFree();
               itemRightHand = null;
            }
         }
      }

      public async void ReloadWeapon()
      {
         isReloading = true;
         Shootable weapon = ((Shootable)itemRightHand);
         if (weapon.Reload())
         {
            animationTree.Set("parameters/reload_pistol/active", true);
            weapon.PlayReloadSfx(reloadTime, clipPivot);
            await ToSignal(GetTree().CreateTimer(reloadTime), "timeout");
            weapon.ReloadFinish();
         }
         isReloading = false;
      }

      public async void KnifeAttack()
      {
         isButtBlowing = true;
         animationTree.Set("parameters/knife_attack/active", true);
         await ToSignal(GetTree().CreateTimer(knifeAttackTime), "timeout");
         isButtBlowing = false;
      }

      public async void ButtBlow()
      {
         isButtBlowing = true;
         Shootable weapon = ((Shootable)itemRightHand);
         animationTree.Set("parameters/butt_blow_pistol/active", true);
         await ToSignal(GetTree().CreateTimer(buttBlowingTime), "timeout");
         isButtBlowing = false;
      }

      public Vector3 GetVelocity()
      {
         return this.velocity;
      }

      public override void _Input(InputEvent _event)
      {
         if (_event is InputEventMouseMotion && IsPlaying())
         {
            InputEventMouseMotion motionEvent = (InputEventMouseMotion)_event;
            head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.x - motionEvent.Relative.y * -1 / 1000 * sensitivity, -1, 1f), head.Rotation.y - motionEvent.Relative.x / 1000 * sensitivity, 0);
            // camera.LookAt(head.GlobalTranslation, Vector3.Up);
         }
      }

      public void DisableHandsIK()
      {
         skeletonIKRightCodeHand.Interpolation = 0;
         skeletonIKRightHand.Interpolation = 0;
         if (itemRightHand != null && itemRightHand.LeftHandPosition != null)
         {
            skeletonIKLeftHand.Interpolation = 0;
         }
      }

      public string BlendAimForItem(Item item)
      {
         if (item is Pistol)
         {
            return "blend_pistol_aim";
         }
         if (item is Knife)
         {
            return "blend_knife";
         }

         return "blend_pistol_aim";
      }

      public void SwapFlashlightSlot(bool toHand = true)
      {
         if (toHand)
         {
            if (!isFlashlightOnHand)
            {
               isFlashlightOnHand = true;
               flashLightSlot.RemoveChild(flashLight);
               flashLightHandSlot.AddChild(flashLight);
            }
         }
         else
         {
            if (isFlashlightOnHand)
            {
               isFlashlightOnHand = false;
               flashLightHandSlot.RemoveChild(flashLight);
               flashLightSlot.AddChild(flashLight);
            }
         }
         flashLight.Translation = Vector3.Zero;
         flashLight.RotationDegrees = Vector3.Zero;
      }

      public void UpdateStamina(float variation, float delta)
      {
         if (variation < 0)
         {
            currentCooldownRegenerateStamine = cooldownRegenerateStamine;
         }
         else
         {
            currentCooldownRegenerateStamine -= delta;
            variation += defaultStaminaVariation;
         }
         if (currentCooldownRegenerateStamine > 0 && variation >= 0)
         {
            return;
         }
         currentStamina = Mathf.Clamp(currentStamina + variation, 0, maxStaminaValue);
      }

      public override void _PhysicsProcess(float delta)
      {

         if (!IsOnFloor())
         {
            velocity.y -= gravity * 7f * delta;
         }

         float speedMultiplier = 1.0f;

         if (IsPlaying())
         {
            // ROTATE THE PIVOT OF ALL ---------------------------------
            head.GlobalTranslation = head.GlobalTranslation.LinearInterpolate(pivot.GlobalTranslation, delta * 30f);

            // JUMP ----------------------------------------------------
            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            {
               velocity.y = jumpVelocity;
            }


            // SPRINT --------------------------------------------------
            if (Input.IsActionPressed("sprint") && !isAiming && !isReloading && currentStamina > 0)
            {
               isSprinting = true;
               speedMultiplier = sprintSpeedMultiplier;
               camera.Fov = Mathf.Lerp(camera.Fov, 80f * 1.95f, delta * 4f);
               skeletonIKRightHand.Interpolation = 0;
               SwapFlashlightSlot(false);
            }
            else
            {
               isSprinting = false;
               SwapFlashlightSlot(true);
            }

            UpdateStamina(currentStaminaVariation * (isSprinting ? -1 : 1), delta);

            // CROUCH --------------------------------------------------
            if (Input.IsActionPressed("crouch"))
            {
               isCrouching = true;
               speedMultiplier = crouchSpeedMultiplier;
               if (crouchShape.Disabled)
               {
                  crouchShape.Disabled = false;
                  standShape.Disabled = true;
                  pivot.GlobalTranslation = new Vector3(pivot.GlobalTranslation.x, pivot.GlobalTranslation.y - verticalDisplacementCrouch, pivot.GlobalTranslation.z);
               }
            }
            else
            {
               isCrouching = false;
               if (!crouchShape.Disabled)
               {
                  crouchShape.Disabled = true;
                  standShape.Disabled = false;
                  pivot.GlobalTranslation = new Vector3(pivot.GlobalTranslation.x, pivot.GlobalTranslation.y + verticalDisplacementCrouch, pivot.GlobalTranslation.z);
               }
            }


            // HEAD LOOK AT AIM ----------------------------------------
            var headTargetVec = inverseLookAtAim.GlobalTranslation;
            var newLookAtHeadTransform = lookAtHead.GlobalTransform.LookingAt(headTargetVec, Vector3.Up);
            lookAtHead.GlobalTransform = lookAtHead.GlobalTransform.InterpolateWith(newLookAtHeadTransform, delta * 2f);

            // AIM LOOK AT CAMERA ANGLE --------------------------------
            lookAtAim.GlobalTransform = lookAtAim.GlobalTransform.LookAtWithY(camera.GlobalTransform.basis.z * -1, camera.GlobalTransform.basis.x);

            // HEAD AND AIM --------------------------------------------
            string aimBlendAnim = BlendAimForItem(itemRightHand);
            if (aimBlendAnim != currentBlendAim)
            {
               animationTree.Set($"parameters/{currentBlendAim}/blend_amount", 0);
            }
            currentBlendAim = aimBlendAnim;
            float curAimBlend = (animationTree.Get($"parameters/{currentBlendAim}/blend_amount")).ToString().ToFloat();

            if (Input.IsActionPressed("aim") && IsPlaying())
            {
               if (!isReloading && !isButtBlowing)
               {
                  isAiming = true;
                  camera.Fov = Mathf.Lerp(camera.Fov, 80f * .6f, delta * 3f);
                  skeletonIKRightHand.Interpolation = 0;
                  skeletonIKRightCodeHand.Start();
                  skeletonIKRightCodeHand.Interpolation = Mathf.Lerp(skeletonIKRightCodeHand.Interpolation, 1f, delta * 10f);
                  // skeletonIKRightCodeHand.Target = lookAtAim.GlobalTransform;
                  if (itemRightHand != null && itemRightHand.LeftHandPosition != null)
                  {
                     skeletonIKLeftHand.Target = itemRightHand.LeftHandPosition.GlobalTransform;
                     skeletonIKLeftHand.Interpolation = Mathf.Lerp(skeletonIKLeftHand.Interpolation, 1f, delta * 18.5f);
                  }
                  float blendAimAmount = 1.0f;
                  animationTree.Set($"parameters/{currentBlendAim}/blend_amount", Mathf.Lerp(curAimBlend, blendAimAmount, delta * 6.5f));
               }
            }
            else
            {
               isAiming = false;
               if (isSprinting) lookAtHead.Rotation = initialRotationLookAtHead;
               if (curAimBlend > 0)
               {
                  animationTree.Set($"parameters/{currentBlendAim}/blend_amount", Mathf.Lerp(curAimBlend / 1.7f, 0, delta * 16.5f));
               }
               camera.Fov = Mathf.Lerp(camera.Fov, 80f, delta * 8f);
               if (!isReloading && !isButtBlowing)
               {
                  skeletonIKRightCodeHand.Stop();
                  skeletonIKRightCodeHand.Interpolation = 0;
                  if (!isSprinting)
                  {
                     skeletonIKRightHand.Interpolation = Mathf.Lerp(skeletonIKRightHand.Interpolation, 1, delta * 10f);
                  }
                  if (itemRightHand != null && itemRightHand.LeftHandPosition != null)
                  {
                     skeletonIKLeftHand.Interpolation = Mathf.Lerp(skeletonIKLeftHand.Interpolation, 0, delta * 10f);
                  }
               }
            }

            // FIREARM ----------------------------------------------
            if (itemRightHand is Shootable && !isReloading && !isButtBlowing)
            {
               if (Input.IsActionJustPressed("shot") && isAiming)
               {
                  animationTree.Set("parameters/shot_pistol/active", true);
                  if (((Shootable)itemRightHand).Shoot())
                  {
                     handPivotsAnimPlayer.Play("shoot_pistol_recoil");
                  }
               }
               if (Input.IsActionJustPressed("reload"))
               {
                  DisableHandsIK();
                  ReloadWeapon();
               }
               if (Input.IsActionJustPressed("butt_blow"))
               {
                  DisableHandsIK();
                  ButtBlow();
               }
            }

            // KNIFE -------------------------------------------------------
            if (itemRightHand is Attackable && !isButtBlowing)
            {
               if (Input.IsActionJustPressed("shot"))
               {
                  DisableHandsIK();
                  KnifeAttack();
               }
            }
         }

         // MOTION DIRECTION --------------------------------------------
         Vector3 direction = new Vector3(Input.GetActionStrength("ui_left") - Input.GetActionStrength("ui_right"), 0, Input.GetActionStrength("ui_up") - Input.GetActionStrength("ui_down"));

         if (!IsPlaying())
         {
            direction = Vector3.Zero;
         }

         Vector3 rotationVec = GlobalTranslation.DirectionTo(lookAt.GlobalTranslation);
         if (direction != Vector3.Zero || isAiming)
         {
            float rotationY = Rotation.y;
            Rotation = new Vector3(Rotation.x, Mathf.LerpAngle(rotationY, Mathf.Atan2(rotationVec.x, rotationVec.z), delta * (isAiming ? 20f : 15.5f)), Rotation.z);
            Vector3 rotationVecAim = pivot.GlobalTranslation.DirectionTo(new Vector3(pivot.GlobalTranslation.x, lookAt.GlobalTranslation.y, lookAt.GlobalTranslation.z));
            // float angleToAim = Mathf.LerpAngle(pivot.Rotation.x, Mathf.Atan2(-rotationVecAim.y, rotationVecAim.z), delta * (isAiming ? 14f : 4.5f));
            float angleToAim = Mathf.LerpAngle(pivot.Rotation.x, head.Rotation.x, delta * (isAiming ? 20f : 15.5f));
            pivot.Rotation = new Vector3(head.Rotation.x, pivot.Rotation.y, pivot.Rotation.z);
         }

         // MOTION --------------------------------------------
         if (direction != Vector3.Zero && !isButtBlowing)
         {

            float blendAmount = 1.0f;
            float curBlend = (float)animationTree.Get("parameters/blend/blend_amount");
            animationTree.Set("parameters/blend/blend_amount", Mathf.Lerp(curBlend, blendAmount, delta * 4.5f));
            if (isCrouching)
            {
               float blendMotionAmount = 1.0f;
               float curMotionBlend = (float)animationTree.Get("parameters/motion_blend/blend_amount");
               animationTree.Set("parameters/motion_blend/blend_amount", Mathf.Lerp(curMotionBlend, blendMotionAmount, delta * 4.5f));
               animationTree.Set("parameters/crounch_blend_space/blend_position", new Vector2(velocity.x, velocity.z));
            }
            else
            {
               var blendMotionAmount = 0.0f;
               var curMotionBlend = (float)animationTree.Get("parameters/motion_blend/blend_amount");
               animationTree.Set("parameters/motion_blend/blend_amount", Mathf.Lerp(curMotionBlend, blendMotionAmount, delta * 4.5f));
               var blendSpaceVec = (new Vector2(-direction.x, direction.z)).Normalized() * (isSprinting ? 1.0f : 0.5f);
               var curBlendSpaceVec = (Vector2)animationTree.Get("parameters/motion_blend_space/blend_position");
               animationTree.Set("parameters/motion_blend_space/blend_position", curBlendSpaceVec.LinearInterpolate(blendSpaceVec, delta * 4.8f));
            }

            Vector3 moveVec = GlobalTranslation.Normalized().DirectionTo(direction);
            Vector3 motion = direction.Rotated(Vector3.Up, Mathf.Atan2(rotationVec.x, rotationVec.z)) * speed * speedMultiplier;
            velocity.x = motion.x;
            velocity.z = motion.z;

         }
         else
         {

            var blendAmount = 0.0f;
            var curBlend = (float)animationTree.Get("parameters/blend/blend_amount");
            animationTree.Set("parameters/blend/blend_amount", Mathf.Lerp(curBlend, blendAmount, delta * 3.5f));
            if (isCrouching)
            {
               var blendCrouchAmount = 1.0f;
               var curCrouchBlend = (float)animationTree.Get("parameters/blend_crouch/blend_amount");
               animationTree.Set("parameters/blend_crouch/blend_amount", Mathf.Lerp(curCrouchBlend, blendCrouchAmount, delta * 3.2f));
            }
            else
            {
               var blendCrouchAmount = 0.0f;
               var curCrouchBlend = (float)animationTree.Get("parameters/blend_crouch/blend_amount");
               animationTree.Set("parameters/blend_crouch/blend_amount", Mathf.Lerp(curCrouchBlend, blendCrouchAmount, delta * 3));
               var blendSpaceVec = Vector2.Zero;
               var curBlendSpaceVec = (Vector2)animationTree.Get("parameters/motion_blend_space/blend_position");
               animationTree.Set("parameters/motion_blend_space/blend_position", curBlendSpaceVec.LinearInterpolate(blendSpaceVec, delta * 2.8f));
               animationTree.Set("parameters/crounch_blend_space/blend_position", Vector2.Zero);
            }

            velocity.x = 0f;
            velocity.z = 0f;

         }

         velocity = MoveAndSlide(velocity, Vector3.Up, true, 3);

         RayForInteract();      
      }

      public bool IsPlaying()
      {
         // bool playing = globalManager.stateFocus == StateFocus.GAME;
         return !isDead && isPlaying && globalManager.stateFocus == StateFocus.GAME;
      }

      public async void PlayTakingBackpackAnim()
      {
         isPlaying = false;
         inInventory = true;
         DisableHandsIK();
         // animationTree.Set("parameters/take_backpack/active", true);
         // BackpackToHand();
         // await ToSignal(GetTree().CreateTimer(6), "timeout");
         // animationTree.Set("parameters/idle_backpack/blend_amount", 1);
      }

      public void TakeDamage(float damage)
      {
         health = Mathf.Clamp(health - damage, 0, maxHealth);

         sfx.PlayRandomSfxByCategory("punch", -1, this);

         var particles = bloodParticles.Instance<BloodHitParticles>();
         globalManager.main3dNode.AddChild(particles);
         particles.GlobalTranslation = GlobalTranslation + new Vector3(0,5f,0);
         particles.Play();

         if (health == 0 && !isDead) {
            Die();
         }
         if (!isDead) {
            sfx.PlayRandomSfxByCategory("pain_scream", -1, this);
         }
      }

      public void Die()
      {
         isDead = true;
         isPlaying = false;
         DisableHandsIK();
         sfx.PlayRandomSfxByCategory("scream", -1, this);

         RandomNumberGenerator rng = new RandomNumberGenerator();
         rng.Randomize();

         animationTree.Set("parameters/state/current", rng.RandiRange(1, 2));
      }

      public async void BackpackToHand()
      {
         await ToSignal(GetTree().CreateTimer(2), "timeout");
         neckBackpackSlot.RemoveChild(backpack);
         handBackpackSlot.AddChild(backpack);
         backpack.Translation = Vector3.Zero;
         backpack.RotationDegrees = Vector3.Zero;
      }

      public async void PlayKeepBackpackAnim()
      {
         keepingBackpack = true;
         // var tween = GetTree().CreateTween();
         // tween.TweenProperty(animationTree, "parameters/idle_backpack/blend_amount", 0, 1.5f);
         // await ToSignal(tween, "finished");
         inInventory = false;
         keepingBackpack = false;
         isPlaying = true;
         handBackpackSlot.RemoveChild(backpack);
         neckBackpackSlot.AddChild(backpack);
         backpack.Translation = Vector3.Zero;
         backpack.RotationDegrees = Vector3.Zero;
      }

      public void RayForInteract()
      {
         // RAY ITEMS ------------------------------------------
         if (IsPlaying() && !isSprinting)
         {
            Vector3 from = camera.GlobalTranslation;
            Vector3 to = from + camera.GlobalTransform.basis.z * -1 * 10f;
            var space = GetWorld().DirectSpaceState;
            var itemRay = space.IntersectRay(from, to, null, collisionMaskItemsRay, true, true);
            if (itemRay != null && itemRay.Contains("position"))
            {
               Node item = (Node)itemRay["collider"];
               if (item is Item)
               {
                  if (item == itemRightHand)
                  {
                     return;
                  }
                  Vector3 positionItem = (Vector3)itemRay["position"];
                  Vector3 normalItem = (Vector3)itemRay["normal"];
                  Item _item = (Item)item;
                  if (_item.isTakeable) {
                     _item.ChangeHighlight();
                     ChangeSelectedItem(_item);
                  }
               }
               else if (item is Interactable)
               {
                  Interactable interactable = (Interactable)item;
                  interactable.ChangeHighlight();
                  ChangeSelectedInteractable(interactable);
               }
               if (globalManager.hud != null && (item is Item || item is Interactable)) {
                  globalManager.hud.ChangeCrosshair(Ui.HUD.CrosshairType.InteractHand);
                  return;
               }
            }
            else
            {
               ChangeSelectedItem();
               ChangeSelectedInteractable();
            }
            globalManager.hud.ChangeCrosshair(Ui.HUD.CrosshairType.Normal);
         }
      }

      public void ChangeSelectedItem(Item newItem = null)
      {
         bool cleanItem = newItem == null && currentSelectedItem != null;
         if ((currentSelectedItem != null && currentSelectedItem != newItem) || cleanItem)
         {
            currentSelectedItem.ChangeHighlight(false);
         }
         if (cleanItem)
         {
            currentSelectedItem = null;
            return;
         }

         currentSelectedItem = newItem;
      }

      public void ChangeSelectedInteractable(Interactable newInteractable = null)
      {
         bool cleanInteractable = newInteractable == null && currentSelectedInteractable != null;
         if ((currentSelectedInteractable != null && currentSelectedInteractable != newInteractable) || cleanInteractable)
         {
            currentSelectedInteractable.ChangeHighlight(false);
         }
         if (cleanInteractable)
         {
            currentSelectedInteractable = null;
            return;
         }

         currentSelectedInteractable = newInteractable;
      }

      public RayCast GetTopRay()
      {
         return topRay;
      }

      public RayCast GetLeftRay()
      {
         return leftRay;
      }

      public RayCast GetRightRay()
      {
         return rightRay;
      }

      public RayCast GetForwardRay()
      {
         return forwardRay;
      }

      public RayCast GetBackwardRay()
      {
         return backwardRay;
      }

      public RayCast GetAimRay()
      {
         return aimRay;
      }

      public RayCast GetBottomRay()
      {
         return bottomRay;
      }
   }
}
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
   public partial class Player : CharacterBody3D, AdaptiveAudioEmitter
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
      Godot.SkeletonIK3D skeletonIKRightHand;

      [Export]
      NodePath skeletonIkRightHandCodePath;
      Godot.SkeletonIK3D skeletonIKRightCodeHand;

      [Export]
      NodePath skeletonIkLeftHandPath;
      Godot.SkeletonIK3D skeletonIKLeftHand;

      [Export]
      NodePath lookAtRightPath;
      Marker3D lookAtRight;

      [Export]
      NodePath pivotPath;

      public Marker3D pivot;
      Marker3D lookAt;

      [Export]
      NodePath lookAtAimPath;
      Marker3D lookAtAim;
      Marker3D inverseLookAtAim;

      [Export]
      NodePath lookAtHeadPath;
      Marker3D lookAtHead;

      [Export]
      NodePath itemSlotRightPath;
      Node3D itemSlotRight;

      [Export]
      NodePath handPivotsAnimPlayerPath;
      AnimationPlayer handPivotsAnimPlayer;

      public Camera3D camera;
      Node3D head;

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

      CollisionShape3D standShape;
      CollisionShape3D crouchShape;

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
      RayCast3D aimRay;
      RayCast3D topRay;
      RayCast3D bottomRay;
      RayCast3D rightRay;
      RayCast3D leftRay;
      RayCast3D forwardRay;
      RayCast3D backwardRay;

      [Export]
      NodePath clipPivotPath;
      Marker3D clipPivot;

      [Export] NodePath lightLevelPosPath;
      public Marker3D lightLevelPos;
      public float lightLevel = 0f;

      public bool isButtBlowing = false;

      GlobalManager globalManager;
      string currentBlendAim = null;

      [Export]
      NodePath flashLightPath;
      SpotLight3D flashLight;

      [Export]
      NodePath flashLightSlotPath;
      Node3D flashLightSlot;

      [Export]
      NodePath flashLightHandSlotPath;
      Node3D flashLightHandSlot;

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

      [Export(PropertyHint.Layers3DPhysics)]
      uint collisionMaskItemsRay;

      public Item currentSelectedItem = null;
      public Interactable currentSelectedInteractable = null;

      public bool inInventory = false;
      public bool keepingBackpack = false;

      public bool isPlaying = true;

      [Export]
      NodePath backpackPath;
      Node3D backpack;

      [Export]
      NodePath neckBackpackSlotPath;
      Marker3D neckBackpackSlot;

      [Export]
      NodePath handBackpackSlotPath;
      Marker3D handBackpackSlot;

      Tween tweenBackpack;

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

      String[] states = new String[] { "default", "die_forward", "die_backward" };

      public override void _Ready()
      {
         globalManager = GetNode<GlobalManager>("/root/GlobalManager");
         globalManager.currentPlayer = this;

         currentStamina = maxStaminaValue;
         currentStaminaVariation = defaultStaminaVariation;

         animationTree = GetNode<AnimationTree>(animationTreePath);
         lookAt = GetNode<Marker3D>(lookAtPath);
         lookAtRight = GetNode<Marker3D>(lookAtRightPath);
         lookAtAim = GetNode<Marker3D>(lookAtAimPath);
         pivot = GetNode<Marker3D>(pivotPath);
         lookAtHead = GetNode<Marker3D>(lookAtHeadPath);
         clipPivot = GetNode<Marker3D>(clipPivotPath);
         camera = GetNode<Camera3D>(cameraPath);
         head = camera.GetParent<Node3D>();
         itemSlotRight = GetNode<Node3D>(itemSlotRightPath);
         handPivotsAnimPlayer = GetNode<AnimationPlayer>(handPivotsAnimPlayerPath);
         skeletonIKRightHand = GetNode<Godot.SkeletonIK3D>(skeletonIkRightHandPath);
         skeletonIKLeftHand = GetNode<Godot.SkeletonIK3D>(skeletonIkLeftHandPath);
         skeletonIKRightCodeHand = GetNode<Godot.SkeletonIK3D>(skeletonIkRightHandCodePath);
         inverseLookAtAim = lookAtAim.GetNode<Marker3D>("InverseLookAtAim");
         sfx = GetNode<SfxOptions>(sfxPath);
         backpack = GetNode<Node3D>(backpackPath);
         neckBackpackSlot = GetNode<Marker3D>(neckBackpackSlotPath);
         handBackpackSlot = GetNode<Marker3D>(handBackpackSlotPath);

         flashLight = GetNode<SpotLight3D>(flashLightPath);
         flashLightSlot = GetNode<Node3D>(flashLightSlotPath);
         flashLightHandSlot = GetNode<Node3D>(flashLightHandSlotPath);

         lightLevelPos = GetNode<Marker3D>(lightLevelPosPath);
         standShape = GetNode<CollisionShape3D>(standShapePath);
         crouchShape = GetNode<CollisionShape3D>(crouchShapePath);

         itemRightHand = itemSlotRight.GetChildOrNull<Item>(0);
         if (itemRightHand != null && itemRightHand.player == null)
         {
            itemRightHand.player = this;
         }

         initialRotationLookAtHead = lookAtHead.Rotation;

         aimRay = GetNode<RayCast3D>(aimRayPath);
         topRay = GetNode<RayCast3D>(topRayPath);
         bottomRay = GetNode<RayCast3D>(bottomRayPath);
         rightRay = GetNode<RayCast3D>(rightRayPath);
         leftRay = GetNode<RayCast3D>(leftRayPath);
         forwardRay = GetNode<RayCast3D>(forwardRayPath);
         backwardRay = GetNode<RayCast3D>(backwardRayPath);

      }

      public void PlaceItemOnRightHand(string itemScenePath)
      {
         var itemScene = ResourceLoader.Load<PackedScene>(itemScenePath);
         var itemNode = itemScene.Instantiate<Item>();
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
            animationTree.Set("parameters/reload_pistol/request", (int) AnimationNodeOneShot.OneShotRequest.Fire);
            weapon.PlayReloadSfx(reloadTime, clipPivot);
            await ToSignal(GetTree().CreateTimer(reloadTime), "timeout");
            weapon.ReloadFinish();
         }
         isReloading = false;
      }

      public async void KnifeAttack()
      {
         isButtBlowing = true;
         animationTree.Set("parameters/knife_attack/request", (int) AnimationNodeOneShot.OneShotRequest.Fire);
         await ToSignal(GetTree().CreateTimer(knifeAttackTime), "timeout");
         isButtBlowing = false;
      }

      public async void ButtBlow()
      {
         isButtBlowing = true;
         Shootable weapon = ((Shootable)itemRightHand);
         animationTree.Set("parameters/butt_blow_pistol/request", (int) AnimationNodeOneShot.OneShotRequest.Fire);
         await ToSignal(GetTree().CreateTimer(buttBlowingTime), "timeout");
         isButtBlowing = false;
      }

      public Vector3 GetVelocity()
      {
         return this.velocity;
      }

      public override void _Input(InputEvent _event)
      {
         if (_event is InputEventMouseMotion)
         {
            InputEventMouseMotion motionEvent = (InputEventMouseMotion)_event;
            head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.X - motionEvent.Relative.Y * -1 / 1000 * sensitivity, -1, 1f), head.Rotation.Y - motionEvent.Relative.X / 1000 * sensitivity, 0);
            // camera.LookAt(head.GlobalPosition, Vector3.Up);
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
         flashLight.Position = Vector3.Zero;
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

      public override void _PhysicsProcess(double _delta)
      {
         float delta = (float) _delta;

         if (!IsOnFloor())
         {
            velocity.Y -= gravity * 7f * delta;
         }

         float speedMultiplier = 1.0f;

         if (IsPlaying())
         {
            // ROTATE THE PIVOT OF ALL ---------------------------------
            head.GlobalPosition = head.GlobalPosition.Lerp(pivot.GlobalPosition, delta * 30f);

            // JUMP ----------------------------------------------------
            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            {
               velocity.Y = jumpVelocity;
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
                  pivot.GlobalPosition = new Vector3(pivot.GlobalPosition.X, pivot.GlobalPosition.Y - verticalDisplacementCrouch, pivot.GlobalPosition.Z);
               }
            }
            else
            {
               isCrouching = false;
               if (!crouchShape.Disabled)
               {
                  crouchShape.Disabled = true;
                  standShape.Disabled = false;
                  pivot.GlobalPosition = new Vector3(pivot.GlobalPosition.X, pivot.GlobalPosition.Y + verticalDisplacementCrouch, pivot.GlobalPosition.Z);
               }
            }


            // HEAD LOOK AT AIM ----------------------------------------
            var headTargetVec = inverseLookAtAim.GlobalPosition;
            var newLookAtHeadTransform = lookAtHead.GlobalTransform.LookingAt(headTargetVec, Vector3.Up);
            lookAtHead.GlobalTransform = lookAtHead.GlobalTransform.InterpolateWith(newLookAtHeadTransform, delta * 2f);

            // AIM LOOK AT CAMERA ANGLE --------------------------------
            lookAtAim.GlobalTransform = lookAtAim.GlobalTransform.LookAtWithY(camera.GlobalTransform.Basis.Z * -1, camera.GlobalTransform.Basis.X);

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
                  skeletonIKRightCodeHand.Target = lookAtAim.GlobalTransform;
                  if (itemRightHand != null && itemRightHand.LeftHandPosition != null)
                  {
                     skeletonIKLeftHand.Target = itemRightHand.LeftHandPosition.GlobalTransform;
                     skeletonIKLeftHand.Interpolation = Mathf.Lerp(skeletonIKLeftHand.Interpolation, 1f, delta * 18.5f);
                  }
                  float blendAimAmount = 1.0f;
                  // animationTree.Set($"parameters/{currentBlendAim}/blend_amount", Mathf.Lerp(curAimBlend, blendAimAmount, delta * 6.5f));
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
                  animationTree.Set("parameters/shot_pistol/request", (int) AnimationNodeOneShot.OneShotRequest.Fire);
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

         Vector3 rotationVec = GlobalPosition.DirectionTo(lookAt.GlobalPosition);
         if (direction != Vector3.Zero || isAiming)
         {
            float rotationY = Rotation.Y;
            Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(rotationY, Mathf.Atan2(rotationVec.X, rotationVec.Z), delta * (isAiming ? 20f : 15.5f)), Rotation.Z);
            Vector3 rotationVecAim = pivot.GlobalPosition.DirectionTo(new Vector3(pivot.GlobalPosition.X, lookAt.GlobalPosition.Y, lookAt.GlobalPosition.Z));
            // float angleToAim = Mathf.LerpAngle(pivot.Rotation.x, Mathf.Atan2(-rotationVecAim.y, rotationVecAim.z), delta * (isAiming ? 14f : 4.5f));
            float angleToAim = Mathf.LerpAngle(pivot.Rotation.X, head.Rotation.X, delta * (isAiming ? 20f : 15.5f));
            pivot.Rotation = new Vector3(head.Rotation.X, pivot.Rotation.Y, pivot.Rotation.Z);
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
               animationTree.Set("parameters/crounch_blend_space/blend_position", new Vector2(velocity.X, velocity.Z));
            }
            else
            {
               var blendMotionAmount = 0.0f;
               var curMotionBlend = (float)animationTree.Get("parameters/motion_blend/blend_amount");
               animationTree.Set("parameters/motion_blend/blend_amount", Mathf.Lerp(curMotionBlend, blendMotionAmount, delta * 4.5f));
               var blendSpaceVec = (new Vector2(-direction.X, direction.Z)).Normalized() * (isSprinting ? 1.0f : 0.5f);
               var curBlendSpaceVec = (Vector2)animationTree.Get("parameters/motion_blend_space/blend_position");
               animationTree.Set("parameters/motion_blend_space/blend_position", curBlendSpaceVec.Lerp(blendSpaceVec, delta * 4.8f));
            }

            Vector3 moveVec = GlobalPosition.Normalized().DirectionTo(direction);
            Vector3 motion = direction.Rotated(Vector3.Up, Mathf.Atan2(rotationVec.X, rotationVec.Z)) * speed * speedMultiplier;
            velocity.X = motion.X;
            velocity.Z = motion.Z;

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
               animationTree.Set("parameters/motion_blend_space/blend_position", curBlendSpaceVec.Lerp(blendSpaceVec, delta * 2.8f));
               animationTree.Set("parameters/crounch_blend_space/blend_position", Vector2.Zero);
            }

            velocity.X = 0f;
            velocity.Z = 0f;

         }

         Velocity = velocity;
         MoveAndSlide();

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

         var particles = bloodParticles.Instantiate<BloodHitParticles>();
         globalManager.main3dNode.AddChild(particles);
         particles.GlobalPosition = GlobalPosition + new Vector3(0,5f,0);
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

         animationTree.Set("parameters/state/transition_request", states[rng.RandiRange(1, 2)]);
      }

      public async void BackpackToHand()
      {
         await ToSignal(GetTree().CreateTimer(2), "timeout");
         neckBackpackSlot.RemoveChild(backpack);
         handBackpackSlot.AddChild(backpack);
         backpack.Position = Vector3.Zero;
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
         backpack.Position = Vector3.Zero;
         backpack.RotationDegrees = Vector3.Zero;
      }

      public void RayForInteract()
      {
         // RAY ITEMS ------------------------------------------
         if (IsPlaying() && !isSprinting)
         {
            Vector3 from = camera.GlobalPosition;
            Vector3 to = from + camera.GlobalTransform.Basis.Z * -1 * 10f;
            var space = GetWorld3D().DirectSpaceState;
            var rayQuery = PhysicsRayQueryParameters3D.Create(from, to, collisionMaskItemsRay);
            var itemRay = space.IntersectRay(rayQuery);
            if (itemRay.Count > 0)
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

      public RayCast3D GetTopRay()
      {
         return topRay;
      }

      public RayCast3D GetLeftRay()
      {
         return leftRay;
      }

      public RayCast3D GetRightRay()
      {
         return rightRay;
      }

      public RayCast3D GetForwardRay()
      {
         return forwardRay;
      }

      public RayCast3D GetBackwardRay()
      {
         return backwardRay;
      }

      public RayCast3D GetAimRay()
      {
         return aimRay;
      }

      public RayCast3D GetBottomRay()
      {
         return bottomRay;
      }
   }
}
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using ChidemGames.Core.Enemies.AI;
using ChidemGames.Core.Audio;
using ChidemGames.Events;
using ChidemGames.Core.Characters;

namespace ChidemGames.Core.Enemies
{
   public class DefaultEnemy : KinematicBody, AdaptiveAudioEmitter
   {

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
		float patrolSpeed = 2f;

		[Export]
		float chasingSpeed = 3.6f;

		[Export]
		float huntingSpeed = 4.6f;

      [Export]
      public EnemyState state;

      [Export]
      NodePath animationTreePath;
      public AnimationTree animationTree;

      [Export]
      NodePath sfxPath;
      SfxOptions sfx;

		[Export]
		NodePath navigationAgentPath;
		NavigationAgent navigationAgent;

		Player player = null;

		GlobalManager globalManager;
		GlobalEvents globalEvents;

		List<Position3D> waypoints = new List<Position3D>();
		public int waypointIdx;
		
		RandomNumberGenerator rng = new RandomNumberGenerator();

		Vector3 lastTargetPos;

		[Export]
		float patrolTime = 4f;

		[Export]
		NodePath closeSightPath;
		Area closeSight;

		[Export]
		NodePath farSightPath;
		Area farSight;

		public bool playerInCloseSigth = false;
		public bool playerInFarSigth = false;
		public bool playerInCloseHearing = false;
		public bool playerInFarHearing = false;

		public Vector3 velocity;
		public Vector3 direction;

		float motionTimeScale = 1;

		[Export]
		NodePath headPath;
		Area head;

		[Export(PropertyHint.Layers3dPhysics)]
		uint playerMask;

		[Export]
		float closeLightDetectionLevel = 0;

		[Export]
		float farLightDetectionLevel = 0;

		[Export]
		float farCrouchedLightDetectionLevel = 0;

		[Export]
		NodePath viewportDebugPath;
		Viewport viewportDebug;

		[Export]
		NodePath labelDebugPath;
		Label labelDebug;

		[Export]
		NodePath meshDebugPath;
		MeshInstance meshDebug;

		bool playHuntingSfx = false;
		bool playIdleSfx = false;

		[Export]
		float rangeToAttack = 4f;

		[Export]
		float timeAttack = 2f;

		[Export]
		float attackDamage = .2f;

		bool isAttacking = false;

		float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

		[Export]
      public float maxHealth = 1;

      [Export]
      public float health = 1;

      public bool isDead = false;

      public override void _Ready()
      {
			rng.Randomize();

			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

			navigationAgent = GetNode<NavigationAgent>(navigationAgentPath);
			animationTree = GetNode<AnimationTree>(animationTreePath);
			sfx = GetNode<SfxOptions>(sfxPath);
			closeSight = GetNode<Area>(closeSightPath);
			farSight = GetNode<Area>(farSightPath);
			head = GetNode<Area>(headPath);

			closeSight.Connect("body_entered", this, nameof(OnCloseSightEntered));
			closeSight.Connect("body_exited", this, nameof(OnCloseSightExited));
			farSight.Connect("body_entered", this, nameof(OnFarSightEntered));
			farSight.Connect("body_exited", this, nameof(OnFarSightExited));

			aimRay = GetNode<RayCast>(aimRayPath);
         topRay = GetNode<RayCast>(topRayPath);
         bottomRay = GetNode<RayCast>(bottomRayPath);
         rightRay = GetNode<RayCast>(rightRayPath);
         leftRay = GetNode<RayCast>(leftRayPath);
         forwardRay = GetNode<RayCast>(forwardRayPath);
         backwardRay = GetNode<RayCast>(backwardRayPath);

			var _waypoints = GetTree().GetNodesInGroup("DefaultEnemyWaypoints");
			foreach (var _waypoint in _waypoints) {
				waypoints.Add((Position3D) _waypoint);
			}

			if (waypoints.Count > 0) {
				navigationAgent.SetTargetLocation(waypoints[rng.RandiRange(0, waypoints.Count - 1)].GlobalTranslation);
			}

			viewportDebug = GetNode<Viewport>(viewportDebugPath);
			labelDebug = GetNode<Label>(labelDebugPath);
			meshDebug = GetNode<MeshInstance>(meshDebugPath);
      }

      public override void _PhysicsProcess(float delta)
      {
			if (!IsOnFloor())
         {
            velocity.y -= gravity * 7f * delta;
         }

			if (isDead) return;

			float curSpeed = 0f;

         switch (state)
         {
            case EnemyState.Idle:
					break;

            case EnemyState.Patrol:
					if (navigationAgent.IsNavigationFinished()) {
						PatrolTimer();
						state = EnemyState.Waiting;
						navigationAgent.SetTargetLocation(waypoints[rng.RandiRange(0, waypoints.Count - 1)].GlobalTranslation);
						return;
					}
					if (Engine.GetFramesDrawn() % 60 == 0) CheckPlayer();
					curSpeed = patrolSpeed;
					motionTimeScale = 1;
					break;

            case EnemyState.Chasing:
					if (navigationAgent.IsNavigationFinished()) {
						PatrolTimer();
						state = EnemyState.Waiting;
					}
					navigationAgent.SetTargetLocation(globalManager.currentPlayer.GlobalTranslation);
					curSpeed = chasingSpeed;
					motionTimeScale = 2;
					break;

            case EnemyState.Hunting:
					if (Engine.GetFramesDrawn() % 20 == 0) navigationAgent.SetTargetLocation(globalManager.currentPlayer.GlobalTranslation);
					curSpeed = huntingSpeed;
					motionTimeScale = 3;
					if (!playHuntingSfx) {
						PlayHuntingSfx();
					}
					Attack();
					break;

            case EnemyState.Waiting:
					curSpeed = 0;
					CheckPlayer();
               break;
         }

			MoveTowards(curSpeed, delta);

			animationTree.Set("parameters/space_motion/blend_position", 1);
			animationTree.Set("parameters/time_scale_motion/scale", motionTimeScale);

			float curMotionBlend = (animationTree.Get($"parameters/motion/blend_amount")).ToString().ToFloat();
			if (velocity.Length() > 0.2f) {
				animationTree.Set($"parameters/motion/blend_amount", Mathf.Lerp(curMotionBlend, 1, delta * 6.5f));
			} else {
				animationTree.Set($"parameters/motion/blend_amount", Mathf.Lerp(curMotionBlend, 0, delta * 4.5f));
			}

			// DEBUG ------------------------------------------------
			labelDebug.Text = $"{GetStateName()}\nPlayer Close Sight: {playerInCloseSigth}\nPlayer Far Sight: {playerInFarSigth}";
			SpatialMaterial material = meshDebug.MaterialOverride as SpatialMaterial;
			material.AlbedoTexture = viewportDebug.GetTexture();
      }

		public float DistanceToPlayer()
		{
			return GlobalTranslation.DistanceTo(globalManager.currentPlayer.GlobalTranslation);
		}

		public async void Attack()
		{
			if (isAttacking) return;

			if (DistanceToPlayer() <= rangeToAttack) {
				isAttacking = true;
				animationTree.Set("parameters/long_attack/active", true);
				await ToSignal(GetTree().CreateTimer(timeAttack), "timeout");
				if (DistanceToPlayer() <= rangeToAttack) {
					globalManager.currentPlayer.TakeDamage(attackDamage);
				}
				isAttacking = false;
				if (globalManager.currentPlayer.isDead) {
					state = EnemyState.Patrol;
				}
			}
		}

		public async void PlayHuntingSfx()
		{
			playHuntingSfx = true;
			sfx.PlayRandomSfxByCategory("screams", -1, this);
			await ToSignal(GetTree().CreateTimer(8f), "timeout");
			playHuntingSfx = false;
		}

		public string GetStateName()
		{
			switch (state)
         {
            case EnemyState.Idle:
					return "Idle";

            case EnemyState.Patrol:
					return "Patrol";

            case EnemyState.Chasing:
					return "Chasing";

            case EnemyState.Hunting:
					return "Hunting";

            case EnemyState.Waiting:
					return "Waiting";
         }
			return "";
		}

		public void CheckPlayer()
		{
			var spaceState = GetWorld().DirectSpaceState;
			var result = spaceState.IntersectRay(head.GlobalTranslation, globalManager.currentPlayer.GlobalTranslation + new Vector3(0,2f,0), null, playerMask);
			bool isPlayerBehindWall = false;

			if (result.Count > 0) {
				Node node = (Node) result["collider"];
				if (node is Player) {
					Player targetPlayer = (Player) node;
					if (playerInCloseSigth) {
						if (targetPlayer.lightLevel > closeLightDetectionLevel) {
							state = EnemyState.Chasing;
						}
					}
					if (playerInFarSigth) {
						if (!targetPlayer.isCrouching && targetPlayer.lightLevel > farLightDetectionLevel) {
							state = EnemyState.Hunting;
							navigationAgent.SetTargetLocation(globalManager.currentPlayer.GlobalTranslation);
						}
						if (targetPlayer.isCrouching && targetPlayer.lightLevel > farCrouchedLightDetectionLevel) {
							state = EnemyState.Hunting;
							navigationAgent.SetTargetLocation(globalManager.currentPlayer.GlobalTranslation);
						}
					}
				}
			}
		}

		public async void PatrolTimer()
		{
			await ToSignal(GetTree().CreateTimer(patrolTime), "timeout");
			state = EnemyState.Patrol;
			waypointIdx += 1;
			if (waypointIdx > waypoints.Count - 1) {
				waypointIdx = 0;
			}
			navigationAgent.SetTargetLocation(waypoints[waypointIdx].GlobalTranslation);
		}

		public void MoveTowards(float speed, float delta)
		{
			if (speed > 0) {
				var targetPos = navigationAgent.GetNextLocation();
				direction = GlobalTranslation.DirectionTo(targetPos);
				Vector3 lookingPos = lastTargetPos.LinearInterpolate(targetPos, 1.25f);
				LookAt(new Vector3(lookingPos.x, GlobalTranslation.y, lookingPos.z), Vector3.Up);
				lastTargetPos = lookingPos;
			}
			if (state == EnemyState.Hunting && DistanceToPlayer() <= rangeToAttack) {
				return;
			}
			velocity = MoveAndSlide(direction * speed);
			if (playerInFarHearing) {
				CheckPlayer();
			}
		}

		public void TakeDamage(float damage)
      {
         health = Mathf.Clamp(health - damage, 0, maxHealth);

         sfx.PlayRandomSfxByCategory("punch", -1, this);

         if (health == 0 && !isDead) {
            Die();
         }
      }

      public void Die()
      {
         isDead = true;
         sfx.PlayRandomSfxByCategory("screams", -1, this);

         RandomNumberGenerator rng = new RandomNumberGenerator();
         rng.Randomize();

         animationTree.Set("parameters/state/current", rng.RandiRange(1, 2));
      }

		public void OnCloseSightEntered(Node node) 
		{
			if (node is Player) {
				playerInCloseSigth = true;
			}
		}
		public void OnCloseSightExited(Node node) 
		{
			if (node is Player) {
				playerInCloseSigth = false;
			}
		}
		public void OnFarSightEntered(Node node) 
		{
			if (node is Player) {
				playerInFarSigth = true;
			}
		}
		public void OnFarSightExited(Node node) 
		{
			if (node is Player) {
				playerInFarSigth = false;
			}
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
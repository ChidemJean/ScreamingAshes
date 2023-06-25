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
   public partial class DefaultEnemy : CharacterBody3D, AdaptiveAudioEmitter
   {

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
		NavigationAgent3D navigationAgent;

		Player player = null;

		GlobalManager globalManager;
		GlobalEvents globalEvents;

		List<Marker3D> waypoints = new List<Marker3D>();
		public int waypointIdx;
		
		RandomNumberGenerator rng = new RandomNumberGenerator();

		Vector3 lastTargetPos;

		[Export]
		float patrolTime = 4f;

		[Export]
		NodePath closeSightPath;
		Area3D closeSight;

		[Export]
		NodePath farSightPath;
		Area3D farSight;

		public bool playerInCloseSigth = false;
		public bool playerInFarSigth = false;
		public bool playerInCloseHearing = false;
		public bool playerInFarHearing = false;

		public Vector3 velocity;
		public Vector3 direction;

		float motionTimeScale = 1;

		[Export]
		NodePath headPath;
		Area3D head;

		[Export(PropertyHint.Layers3DPhysics)]
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
		MeshInstance3D meshDebug;

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

			navigationAgent = GetNode<NavigationAgent3D>(navigationAgentPath);
			animationTree = GetNode<AnimationTree>(animationTreePath);
			sfx = GetNode<SfxOptions>(sfxPath);
			closeSight = GetNode<Area3D>(closeSightPath);
			farSight = GetNode<Area3D>(farSightPath);
			head = GetNode<Area3D>(headPath);

			// closeSight.Connect("body_entered", this, nameof(OnCloseSightEntered));
			// closeSight.Connect("body_exited", this, nameof(OnCloseSightExited));
			// farSight.Connect("body_entered", this, nameof(OnFarSightEntered));
			// farSight.Connect("body_exited", this, nameof(OnFarSightExited));

			closeSight.BodyEntered += OnCloseSightEntered;
			closeSight.BodyExited += OnCloseSightExited;
			farSight.BodyExited += OnFarSightEntered;
			farSight.BodyExited += OnFarSightExited;

			aimRay = GetNode<RayCast3D>(aimRayPath);
         topRay = GetNode<RayCast3D>(topRayPath);
         bottomRay = GetNode<RayCast3D>(bottomRayPath);
         rightRay = GetNode<RayCast3D>(rightRayPath);
         leftRay = GetNode<RayCast3D>(leftRayPath);
         forwardRay = GetNode<RayCast3D>(forwardRayPath);
         backwardRay = GetNode<RayCast3D>(backwardRayPath);

			var _waypoints = GetTree().GetNodesInGroup("DefaultEnemyWaypoints");
			foreach (var _waypoint in _waypoints) {
				waypoints.Add((Marker3D) _waypoint);
			}

			if (waypoints.Count > 0) {
				navigationAgent.TargetPosition = waypoints[rng.RandiRange(0, waypoints.Count - 1)].GlobalPosition;
			}

			viewportDebug = GetNode<Viewport>(viewportDebugPath);
			labelDebug = GetNode<Label>(labelDebugPath);
			meshDebug = GetNode<MeshInstance3D>(meshDebugPath);
      }

      public override void _PhysicsProcess(double _delta)
      {
		float delta = (float) _delta;

		if (!IsOnFloor())
         {
            velocity.Y -= gravity * 7f * delta;
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
						navigationAgent.TargetPosition = waypoints[rng.RandiRange(0, waypoints.Count - 1)].GlobalPosition;
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
					navigationAgent.TargetPosition = globalManager.currentPlayer.GlobalPosition;
					curSpeed = chasingSpeed;
					motionTimeScale = 2;
					break;

            case EnemyState.Hunting:
					if (Engine.GetFramesDrawn() % 20 == 0) navigationAgent.TargetPosition = globalManager.currentPlayer.GlobalPosition;
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
			var material = meshDebug.MaterialOverride as BaseMaterial3D;
			material.AlbedoTexture = viewportDebug.GetTexture();
      }

		public float DistanceToPlayer()
		{
			return GlobalPosition.DistanceTo(globalManager.currentPlayer.GlobalPosition);
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
			var spaceState = GetWorld3D().DirectSpaceState;
			var rayQuery = PhysicsRayQueryParameters3D.Create(head.GlobalPosition, globalManager.currentPlayer.GlobalPosition + new Vector3(0,2f,0), playerMask);
			var result = spaceState.IntersectRay(rayQuery);
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
							navigationAgent.TargetPosition = globalManager.currentPlayer.GlobalPosition;
						}
						if (targetPlayer.isCrouching && targetPlayer.lightLevel > farCrouchedLightDetectionLevel) {
							state = EnemyState.Hunting;
							navigationAgent.TargetPosition = globalManager.currentPlayer.GlobalPosition;
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
			navigationAgent.TargetPosition = waypoints[waypointIdx].GlobalPosition;
		}

		public void MoveTowards(float speed, float delta)
		{
			if (speed > 0) {
				var targetPos = navigationAgent.GetNextPathPosition();
				direction = GlobalPosition.DirectionTo(targetPos);
				Vector3 lookingPos = lastTargetPos.Lerp(targetPos, 1.25f);
				LookAt(new Vector3(lookingPos.X, GlobalPosition.Y, lookingPos.Z), Vector3.Up);
				lastTargetPos = lookingPos;
			}
			if (state == EnemyState.Hunting && DistanceToPlayer() <= rangeToAttack) {
				return;
			}
			velocity = direction * speed;
			Velocity = velocity;
			MoveAndSlide();
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
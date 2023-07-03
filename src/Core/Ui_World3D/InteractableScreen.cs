using Godot;
using System;
using ChidemGames.Events;

namespace ChidemGames.Core.Ui_World3D
{
	public partial class InteractableScreen : Node3D
	{

		[Export]
		NodePath cameraPath;
		protected Camera3D camera;

		public bool isOn = false;

		protected GlobalManager globalManager;
		protected GlobalEvents globalEvents;

		Vector2 meshSize;
		[Export]
		NodePath viewportPath;
		SubViewport viewport;
		[Export]
		NodePath panelFacePath;
		MeshInstance3D panelFace;
		[Export]
		NodePath touchAreaPath;
		RigidBody3D touchArea;

		bool isMouseHeld = false;
		bool isMouseInside = false;
		Vector3 lastMousePosition3D;
		Vector2 lastMousePosition2D;

		[Export(PropertyHint.Layers3DPhysics)]
		uint rayMask;

		[Export]
		float displayYScale = 1;

		public override void _Ready()
		{
			globalManager = GetNode<GlobalManager>("/root/GlobalManager");
			globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
			camera = GetNode<Camera3D>(cameraPath);
			camera.Current = false;

			viewport = GetNode<SubViewport>(viewportPath);
			panelFace = GetNode<MeshInstance3D>(panelFacePath);
			touchArea = GetNode<RigidBody3D>(touchAreaPath);
			touchArea.MouseEntered += OnMouseEntered;

			// SetPanelSize(displayYScale);
			meshSize = (Vector2) panelFace.Mesh.Get("size");
		}

		public void OnMouseEntered()
		{
			isMouseInside = true;
		}

        public override void _Input(InputEvent @event)
        {
			if (!isOn) return;

			if ((@event is InputEventMouseButton || @event is InputEventMouseMotion)) {
				HandleMouse(@event);
			} else {
				viewport.PushInput(@event);
			}
        }

        public void HandleMouse(InputEvent @event)
		{
			if (@event is InputEventMouseButton) {
				isMouseHeld = @event.IsPressed();
			}
			Vector3? mousePos3D = FindMouse(((InputEventMouse)@event).GlobalPosition);

			isMouseInside = mousePos3D != null;

			if (isMouseInside) {
				mousePos3D = touchArea.GlobalTransform.AffineInverse() * mousePos3D;
				lastMousePosition3D = (Vector3) mousePos3D;
			} else {
				mousePos3D = lastMousePosition3D;
				if (mousePos3D == null) {
					mousePos3D = Vector3.Zero;
				}
			}

			var mousePos2D = new Vector2(((Vector3)mousePos3D).X, -((Vector3)mousePos3D).Z);

			mousePos2D.X += meshSize.X / 2;
			mousePos2D.Y += meshSize.Y / 2;

			mousePos2D.X = mousePos2D.X / meshSize.X;
			mousePos2D.Y = mousePos2D.Y / meshSize.Y;

			mousePos2D.X = mousePos2D.X * viewport.Size.X;
			mousePos2D.Y = mousePos2D.Y * viewport.Size.Y;

			mousePos2D.Y = viewport.Size.Y - mousePos2D.Y;

			((InputEventMouse) @event).Position = mousePos2D;
			((InputEventMouse) @event).GlobalPosition = mousePos2D;

			if (@event is InputEventMouseMotion) {
				if (lastMousePosition2D == null) {
					((InputEventMouseMotion) @event).Relative = Vector2.Zero;
				} else {
					((InputEventMouseMotion) @event).Relative = mousePos2D - lastMousePosition2D;
				}
				lastMousePosition2D = mousePos2D;
			}
			viewport.PushInput(@event);
		}

		public Vector3? FindMouse(Vector2 globalPos)
		{
			float rayLength = 1;
			Vector3 from = camera.ProjectRayOrigin(globalPos);
			Vector3 to = from + camera.ProjectRayNormal(globalPos) * rayLength;
			var rayQuery = PhysicsRayQueryParameters3D.Create(from, to, rayMask);
			var result = GetWorld3D().DirectSpaceState.IntersectRay(rayQuery);

			if (result.Count > 0) {
				return (Vector3?) result["position"];
			}

			return null;
		}

		public void SetPanelSize(float yScale)
		{
			var touchAreaCollisionShape = touchArea.GetNode<CollisionShape3D>("Shape");

			float viewportWidth = viewport.Size.X;
			float viewportHeight = viewport.Size.Y;
			float xScale = 0;

			var diff = viewportWidth / (viewportWidth + viewportHeight);
			xScale = yScale / diff;

			panelFace.Mesh.Set("size", new Vector2(xScale, yScale));

			touchAreaCollisionShape.Shape.Set("size", new Vector3(xScale/2, yScale/2, 0.05f));
		}
	}
}
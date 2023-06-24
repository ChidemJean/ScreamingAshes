using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Core.Characters;

namespace ChidemGames.Debug
{
    public struct RayData {
        public RayCast ray;
        public Label label;
        public RayData(RayCast ray, Label label) {
            this.ray = ray;
            this.label = label;
        }
    }

    public class DebugPlayer : MarginContainer
    {
        [Export] NodePath upLabelPath;
        [Export] NodePath downLabelPath;
        [Export] NodePath rightLabelPath;
        [Export] NodePath leftLabelPath;
        [Export] NodePath forwardLabelPath;
        [Export] NodePath backwardLabelPath;
        [Export] NodePath aimLabelPath;
        [Export] NodePath ambientLabelPath;
        [Export] NodePath busLabelPath;
        [Export] NodePath footstepRightLabelPath;
        [Export] NodePath footstepLeftLabelPath;
        [Export] NodePath lightLevelLabelPath;
        [Export] NodePath lightLevelDebugPath;
        [Export] NodePath itemHighlightPath;

        Label upLabel;
        Label downLabel;
        Label rightLabel;
        Label leftLabel;
        Label forwardLabel;
        Label backwardLabel;
        Label aimLabel;
        Label ambientLabel;
        Label busLabel;
        Label footstepRightLabel;
        Label footstepLeftLabel;
        Label lightLevelLabel;
        Label itemHighlight;
        ViewportContainer lightLevelDebug;

        [Export] NodePath playerPath;
        Player player;

        [Export]
        Color positiveColor;

        [Export]
        Color negativeColor;

        List<RayData> raysData = new List<RayData>();
        
        public override void _Ready()
        {
            player = GetNode<Player>(playerPath);
            upLabel = GetNode<Label>(upLabelPath);
            downLabel = GetNode<Label>(downLabelPath);
            rightLabel = GetNode<Label>(rightLabelPath);
            leftLabel = GetNode<Label>(leftLabelPath);
            forwardLabel = GetNode<Label>(forwardLabelPath);
            backwardLabel = GetNode<Label>(backwardLabelPath);
            aimLabel = GetNode<Label>(aimLabelPath);
            ambientLabel = GetNode<Label>(ambientLabelPath);
            busLabel = GetNode<Label>(busLabelPath);
            footstepRightLabel = GetNode<Label>(footstepRightLabelPath);
            footstepLeftLabel = GetNode<Label>(footstepLeftLabelPath);
            lightLevelLabel = GetNode<Label>(lightLevelLabelPath);
            itemHighlight = GetNode<Label>(itemHighlightPath);
            lightLevelDebug = GetNode<ViewportContainer>(lightLevelDebugPath);

            raysData.Add(new RayData(player.GetTopRay(), upLabel));
            raysData.Add(new RayData(player.GetBottomRay(), downLabel));
            raysData.Add(new RayData(player.GetRightRay(), rightLabel));
            raysData.Add(new RayData(player.GetLeftRay(), leftLabel));
            raysData.Add(new RayData(player.GetForwardRay(), forwardLabel));
            raysData.Add(new RayData(player.GetBackwardRay(), backwardLabel));
            raysData.Add(new RayData(player.GetAimRay(), aimLabel));
        }

        public override void _PhysicsProcess(float delta)
        {
            foreach (RayData rayData in raysData) {
                if (rayData.ray.IsColliding()) {
                    Vector3 cpVec = rayData.ray.GetCollisionPoint();
                    string cp = $"({cpVec.x.ToString("0.00")}, {cpVec.y.ToString("0.00")}, {cpVec.z.ToString("0.00")})";
                    rayData.label.Text = $"{cp}";
                    rayData.label.Set("custom_colors/font_color", positiveColor);
                } else {
                    rayData.label.Text = "False";
                    rayData.label.Set("custom_colors/font_color", negativeColor);
                }
            }

            if (player.currentSelectedItem != null){
                itemHighlight.Text = player.currentSelectedItem.itemId;
                itemHighlight.Set("custom_colors/font_color", positiveColor);
            } else {
                itemHighlight.Text = "false";
                itemHighlight.Set("custom_colors/font_color", negativeColor);
            }

            lightLevelLabel.Text = player.lightLevel.ToString();
        }

        public void UpdateLastAmbient(string msg)
        {
            ambientLabel.Text = msg;
        }

        public void UpdateLastBus(string bus)
        {
            busLabel.Text = bus;
        }

        public void UpdateLastRightFootstep(string bus)
        {
            footstepRightLabel.Text = bus;
        }

        public void UpdateLastLeftFootstep(string bus)
        {
            footstepLeftLabel.Text = bus;
        }

    }
}
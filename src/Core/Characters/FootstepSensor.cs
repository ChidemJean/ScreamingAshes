using Godot;
using System;
using ChidemGames.Core.Audio;
using ChidemGames.Debug;

namespace ChidemGames.Core.Characters
{
    public class FootstepSensor : Area
    {
        [Export]
        NodePath sfxPath;
        SfxOptions sfx;

        [Export]
        NodePath playerPath;
        Player player;

        DebugPlayer debugPlayer;

        public enum Foot { Right, Left }

        [Export]
        Foot foot;

        public override void _Ready()
        {
            debugPlayer = GetNode<DebugPlayer>("/root/MainScene/DebugPlayer");
            player = GetNode<Player>(playerPath);
            sfx = GetNode<SfxOptions>(sfxPath);
            Connect("body_entered", this, nameof(OnBodyEntered));
        }

        public void OnBodyEntered(Node node)
        {
            string category = "dirt";
            if (node.IsInGroup(NodeGroups.ConcreteBuildings)) {
                category = "concrete";
            }
            if (node is KinematicBody && (player.GetBottomRay().IsColliding() || !player.IsOnFloor())) {
                return;
            }

            if (foot == Foot.Left) {
                debugPlayer.UpdateLastLeftFootstep(category);
            } else {
                debugPlayer.UpdateLastRightFootstep(category);
            }

            float velLength = player.GetVelocity().Length();
            sfx.PlayRandomSfxByCategory(category, category == "dirt" ? (velLength / 5f) : (velLength / 3f), player);
        }
    }
}
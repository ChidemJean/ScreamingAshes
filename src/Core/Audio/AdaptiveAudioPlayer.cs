using Godot;
using System.Collections.Generic;
using System;
using ChidemGames.Debug;

namespace ChidemGames.Core.Audio
{

    public partial class AdaptiveAudioPlayer : AudioStreamPlayer3D
    {
        
        [Export]
        int adaptiveRange = 25;

        [Export]
        bool debug = true;

        [Export]
        float smallThreshold = 15;

        [Export]
        float mediumThreshold = 25;

        [Export]
        float bigThreshold = 50;

        [Export]
        float tunnelThreshold = 5;

        public AdaptiveAudioEmitter emitter = null;

        [Export]
        public bool autoPlay = true;

        List<RayCast3D> rays = new List<RayCast3D>();

        string type = "generic";

        DebugPlayer debugPlayer;

        public override void _Ready()
        {
            debugPlayer = GetNode<DebugPlayer>("/root/MainScene/DebugPlayer");

            if (emitter == null) return;

            rays.Add(emitter.GetTopRay());
            rays.Add(emitter.GetBackwardRay());
            rays.Add(emitter.GetForwardRay());
            rays.Add(emitter.GetLeftRay());
            rays.Add(emitter.GetRightRay());

            if (autoPlay) {
                PlaySfx();
            }
        }

        public void PlaySfx()
        {
            if (emitter == null) return;
            Adapt();
            Play();
        }

        public void Adapt()
        {
            if (!emitter.GetTopRay().IsColliding()) {
                
                if (
                    emitter.GetRightRay().IsColliding() && emitter.GetLeftRay().IsColliding() &&
                    !emitter.GetForwardRay().IsColliding() && !emitter.GetBackwardRay().IsColliding()
                ) 
                {
                    type = "open_2";
                } else if (
                    emitter.GetForwardRay().IsColliding() && emitter.GetBackwardRay().IsColliding() &&
                    !emitter.GetRightRay().IsColliding() && !emitter.GetLeftRay().IsColliding()
                ) 
                {
                    type = "open_2";
                } else if (
                    emitter.GetRightRay().IsColliding() && emitter.GetLeftRay().IsColliding() &&
                    emitter.GetForwardRay().IsColliding() && emitter.GetBackwardRay().IsColliding()
                ) 
                {
                    type = "generic";
                } else {
                    type = "open";
                }

            } else if (emitter.GetTopRay().IsColliding()) {

                float roomSize = 0;
                float xWidth = 0;
                float zWidth = 0;

                foreach (RayCast3D ray in rays) {
                    if (ray.IsColliding()) {
                        Vector3 cp = ray.GetCollisionPoint();
                        roomSize += cp.DistanceTo(GlobalPosition);

                        if (ray.Name == "Right" || ray.Name == "Left") {
                            //cp = ray.GetCollisionPoint();
                            zWidth += cp.DistanceTo(GlobalPosition);
                        } else if (ray.Name == "Forward" || ray.Name == "Backward") {
                            //cp = ray.GetCollisionPoint();
                            xWidth += cp.DistanceTo(GlobalPosition);
                        }
                    }
                }

                if (debug) {
                    debugPlayer.UpdateLastAmbient("Room Size: " + roomSize.ToString("0.00") + "\nZ Width: " + zWidth.ToString("0.00") + "\nX Width: " + xWidth.ToString("0.00"));
                }

                type = "generic";

                if (roomSize <= smallThreshold) {
                    type = "generic";
                } else if (roomSize > smallThreshold && roomSize <= mediumThreshold) {
                    type = "room_small";
                } else if (roomSize > mediumThreshold && roomSize <= bigThreshold) {
                    type = "room_medium";
                } else if (roomSize > bigThreshold) {
                    type = "room_big";
                }

                string tunnelType = CheckIfTunnel(zWidth, xWidth);
                if (tunnelType != null) {
                    type = tunnelType;
                }
            }

            if (emitter.GetAimRay().IsColliding()) {
                if (type.Contains("_")) {
                    type = "damped_" + type.Split("_")[0];
                }
                type = "damped_open";
            }

            Bus = type;

            if (debug) {
                debugPlayer.UpdateLastBus(Bus);
            }
        }

        public string CheckIfTunnel(float zWidth, float xWidth)
        {
            // CHECK IF TUNNEL
            if (xWidth > zWidth * tunnelThreshold || zWidth > xWidth * tunnelThreshold || xWidth == 0 || zWidth == 0) {
                if (type.Contains("_")) {
                    string ambient = type.Split("_")[0];
                    string size = type.Split("_")[1];
                    if (ambient == "room") {
                        return "tunnel_" + size;
                    }
                }
                return "tunnel_small";
            }

            return null;
        }
    }
}
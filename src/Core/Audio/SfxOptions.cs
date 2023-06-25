using Godot;
using System;
using Godot.Collections;

namespace ChidemGames.Core.Audio
{

   public partial class SfxOptions : Node3D
   {
      [Export]
      Dictionary<string, AudioStream> streams = new Dictionary<string, AudioStream>();

		[Export]
      Array<AudioStream> streamsRandom = new Array<AudioStream>();

		[Export]
      Dictionary<string, StreamsCategory> streamsCategory = new Dictionary<string, StreamsCategory>();

		RandomNumberGenerator rng = new RandomNumberGenerator();

		[Export]
		float unitSize = 5f;

		[Export]
		string bus = "master";

		[Export]
		bool adaptive = true;

      public override void _Ready()
      {
      }

      public void PlaySfx(string streamKey, AdaptiveAudioEmitter emitter = null)
      {
			AudioStream stream = streams[streamKey];
			SpawnNewPlayer(stream, -1, emitter);
      }

		public void PlayRandomSfx(float customUnitSize = -1, AdaptiveAudioEmitter emitter = null)
		{
			rng.Randomize();
			int idx = rng.RandiRange(0, streamsRandom.Count - 1);
			AudioStream stream = streamsRandom[idx];
			SpawnNewPlayer(stream, customUnitSize, emitter);
		}

		public void PlayRandomSfxByCategory(string category, float customUnitSize = -1, AdaptiveAudioEmitter emitter = null)
		{
			StreamsCategory _streams = streamsCategory[category];
			rng.Randomize();
			int idx = rng.RandiRange(0, _streams.streamsRandom.Count - 1);
			AudioStream stream = _streams.streamsRandom[idx];
			SpawnNewPlayer(stream, customUnitSize, emitter);
		}

		private void SpawnNewPlayer(AudioStream stream, float customUnitSize = -1, AdaptiveAudioEmitter emitter = null)
		{
			AudioStreamPlayer3D streamPlayer3D;

			if (adaptive) {
				streamPlayer3D = new AdaptiveAudioPlayer();
				((AdaptiveAudioPlayer) streamPlayer3D).emitter = emitter;
				((AdaptiveAudioPlayer) streamPlayer3D).autoPlay = true;
			} else {
				streamPlayer3D = new AudioStreamPlayer3D();
			}

			streamPlayer3D.UnitSize = customUnitSize == -1 ? unitSize : customUnitSize;
         streamPlayer3D.Stream = stream;
			AddChild(streamPlayer3D);
			if (!adaptive) {
				streamPlayer3D.Bus = this.bus;
         	streamPlayer3D.Play();
			}
			streamPlayer3D.Connect("finished", this, nameof(OnStreamPlayerFinished), new Godot.Collections.Array() { streamPlayer3D });
		}

		public void OnStreamPlayerFinished(object streamPlayer) 
		{
			((Node) streamPlayer).QueueFree();
		}

   }
}
using Godot;
using Godot.Collections;
using System;

namespace ChidemGames.Core.Audio
{
    public partial class StreamsCategory : Resource
    {
        [Export]
        public Array<AudioStream> streamsRandom = new Array<AudioStream>();
    }
}
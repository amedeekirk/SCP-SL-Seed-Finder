using System;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using Handlers = Exiled.Events.Handlers;

namespace NukeRoomRadiation
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Singleton;
        private EventHandlers handler;
        public static bool ScanInProgress = false;
        public static bool Force = false;
        public override void OnEnabled()
        {
            Singleton = this;
            handler = new EventHandlers(this);

            Handlers.Map.Generated += handler.OnGenerated;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handlers.Server.RoundStarted -= handler.OnGenerated;

            handler = null;
            Singleton = null;

            base.OnDisabled();
        }

        public override string Name => "Seed Finder";
        public override string Author => "Amede";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(8, 8, 0);
        public override PluginPriority Priority => PluginPriority.High;
    }
}
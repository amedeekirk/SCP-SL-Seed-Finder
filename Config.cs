using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using Exiled.API.Enums;

namespace NukeRoomRadiation
{
    public class Config : IConfig
    {
        [Description("Determines whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not to show debug logs.")]
        public bool Debug { get; set; } = true;

        [Description("AHP lost per second.")]
        public int RadiationDamage { get; set; } = 10;

        [Description("Delay in seconds before radiation damage starts.")]
        public int RadiationDelay { get; set; } = 60;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Buildmode
{
    public static class Extensions
    {
        private static ConditionalWeakTable<TSPlayer, PluginPlayerData> players = new ConditionalWeakTable<TSPlayer, PluginPlayerData>();
        public static PluginPlayerData GeBMData(this TSPlayer tsplayer)
        {
            var playerData = players.GetOrCreateValue(tsplayer);
            playerData.Player = tsplayer;
            return playerData;
        }
        /// <summary>
        /// Wether or not the Player is currently using BuildMode.
        /// </summary>
        /// <param name="tsplayer"></param>
        /// <returns></returns>
        public static bool IsBuildModeOn(this TSPlayer tsplayer)
        {
            var playerData = players.GetOrCreateValue(tsplayer);
            return playerData.BuildModeOn;
        }
        /// <summary>
        /// Toggles the state of BuildMode of the player.
        /// </summary>
        /// <param name="tsplayer"></param>
        /// <returns></returns>
        public static bool ToggleBuildMode(this TSPlayer tsplayer)
        {
            var playerData = players.GetOrCreateValue(tsplayer);
            playerData.BuildModeOn = !playerData.BuildModeOn;
            return playerData.BuildModeOn;
        }
    }

    /// <summary>
    /// Contains plugin related additional player data.
    /// </summary>
    public class PluginPlayerData
    {
        internal TSPlayer Player { get; set; }
        /// <summary>
        /// Indicator if buildmode is toggled on the player.
        /// </summary>
        public bool BuildModeOn { get; set; }
    }
}

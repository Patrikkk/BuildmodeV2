using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Buildmode
{
    public static class PluginUtils
    {
        /// <summary>
        /// A list of Buffs that are useful for building.
        /// </summary>
        public static readonly List<int> BuildModeBuffs = new List<int>() 
        {
            BuffID.Swiftness,
            BuffID.Shine,
            BuffID.NightOwl,
            BuffID.Panic,
            BuffID.Mining,
            BuffID.Builder,
            BuffID.Lifeforce
        };

    }
}

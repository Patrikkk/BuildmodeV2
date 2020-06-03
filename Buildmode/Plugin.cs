using Buildmode.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace Buildmode
{
    [ApiVersion(2,1)]
    public class Plugin : TerrariaPlugin
    {
        #region Plugin Info
        public override string Author => "Patrikk";
        public override string Description => "Sends fake world data to the player, as well as provides other functions to assist productive building.";
        public override string Name => "BuildMode";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        public Plugin(Main game) : base(game)
        {
        }

        #region Initialize
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetSendBytes.Register(this, OnSendBytes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetSendBytes.Deregister(this, OnSendBytes);
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Hooks
        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("buildmode.use", BuildModeCmd, "buildmode"));
            Commands.ChatCommands.Add(new Command("buildmode.check", BMCheck, "bmcheck"));
            GetPacketHandler.InitGetPacketHandler();
        }

        /// <summary>
        /// Modifies outgoing packets to send fake data to the player.
        /// </summary>
        /// <param name="args"></param>
        private void OnSendBytes(SendBytesEventArgs args)
        {
            var player = TShock.Players[args.Socket.Id];

            if (player == null || !player.IsBuildModeOn())
                return;

            PacketTypes packetType = (PacketTypes)args.Buffer[2];
            switch (packetType)
            {
                /// Invoked whenever the server sends WorldInfo data.
                /// Sending crafted data to the player such as: 
                /// Setting time to noon, stopping rain, wind and sandstorm,
                /// as well as modifying world surface and rock layer height
                /// to display a surface background to the player.
                case PacketTypes.WorldInfo:
                    {
                        WorldInfoPacket packet = new WorldInfoPacket();
                        packet.time = 27000;
                        packet.BitsByte1 = new BitsByte(true);  // isDayTime = true
                        packet.worldSurface = (short)Main.maxTilesY;
                        packet.rockLayer = (short)Main.maxTilesY;
                        packet.windSpeedTarget = 0;
                        packet.maxRaining = 0;
                        packet.IntendedSeverity = 0;
                        packet.Write(new MemoryStream(args.Buffer, args.Offset, args.Count));

                        break;
                    }
                /// Invoked whenever time is modified on the server.
                /// Sending crafted data to the player such as:
                /// Setting time to noon.
                case PacketTypes.TimeSet:
                    {
                        TimeSetPacket packet = new TimeSetPacket();
                        packet.dayTime = true;
                        packet.time = 27000;
                        packet.Write(new MemoryStream(args.Buffer, args.Offset, args.Count));

                        break;
                    }
                /// Invoked whenever Server sends updated data of an NPC.
                /// Sending crafted data to the player such as:
                /// If NPC is not friendly, change its position to make them invisible for the player.
                case PacketTypes.NpcUpdate:
                    {
                        NPCUpdatePacket packet = new NPCUpdatePacket();
                        packet.Read(new MemoryStream(args.Buffer, args.Offset, args.Count));
                        if (!Main.npc[packet.npcIndex].active || Main.npc[packet.npcIndex].friendly)
                            return;
                        packet.position = new Microsoft.Xna.Framework.Vector2();
                        packet.velocity = new Microsoft.Xna.Framework.Vector2();
                        packet.Write(new MemoryStream(args.Buffer, args.Offset, args.Count));
                        break;
                    }
                /// Invoked whenever Server sends updated data of a Projectile.
                /// Sending crafted data to the player such as:
                /// If projectile is hostile and it is not the player's projectile, change its position to make it invisible for the player.
                case PacketTypes.ProjectileNew:
                    {
                        ProjectileNewPacket projectilePacket = new ProjectileNewPacket();
                        projectilePacket.Read(new MemoryStream(args.Buffer, args.Offset, args.Count));

                        if (Main.projectile.First(e => e.identity == projectilePacket.Index && e.owner == projectilePacket.Owner).friendly)
                            return;
                        projectilePacket.Type = 0;
                        projectilePacket.Write(new MemoryStream(args.Buffer, args.Offset, args.Count));
                        break;
                    }
            }
        }
        #endregion

        private void BuildModeCmd(CommandArgs args)
        {
            if (args.TPlayer.hostile)
            {
                args.Player.SendErrorMessage("You cannot enable Buildmode with PvP active!");
                return;
            }

            args.Player.ToggleBuildMode();
            args.Player.SendSuccessMessage((args.Player.IsBuildModeOn() ? "En" : "Dis") + "abled build mode.");

            /// Send a WorldInfo packet to update time/background immediately.
            NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

            /// Process all existing NPCs to make them disappear for the player.
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].friendly)
                    args.Player.SendData(PacketTypes.NpcUpdate, "", i);
            }

            /// Process all existing projectiles to make them disappear for the player.
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && !Main.projectile[i].friendly)
                    args.Player.SendData(PacketTypes.ProjectileNew, "", i);
            }
        }

        private void BMCheck(CommandArgs args)
        {
            var username = String.Join(" ", args.Parameters);

            var matchedPlayers = TSPlayer.FindByNameOrID(username);
            if (matchedPlayers.Count < 1)
                args.Player.SendErrorMessage("No players matched that name!");
            else if (matchedPlayers.Count > 1)
                args.Player.SendMultipleMatchError(matchedPlayers.Select(p => p.Name));
            else
            {
                if (matchedPlayers[0].IsBuildModeOn())
                {
                    args.Player.SendInfoMessage($"{matchedPlayers[0].Name} has Buildmode enabled!");
                }
                else
                    args.Player.SendInfoMessage($"{matchedPlayers[0].Name} does not have Buildmode enabled.");
            }
        }
    }
}

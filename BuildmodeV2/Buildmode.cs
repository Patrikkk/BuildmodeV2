using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using Terraria.DataStructures;
using TerrariaApi.Server;
using TShockAPI;

namespace BuildmodeV2
{
    [ApiVersion(2,1)]
    public class Buildmode : TerrariaPlugin
    {
        public override string Author => "Zaicon";
        public override string Description => "Contains various commands to assist productive building.";
        public override string Name => "BuildmodeV2";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public Buildmode(Main game) : base(game)
        {
            Order = 10;
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.NetSendBytes.Register(this, OnSendBytes);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
                ServerApi.Hooks.NetSendBytes.Deregister(this, OnSendBytes);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            }
            base.Dispose(disposing);
        }

        private Timer timer;

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("buildmode.use", BuildModeCmd, "buildmode"));
            Commands.ChatCommands.Add(new Command("buildmode.check", BMCheck, "bmcheck"));

            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += OnElapsed;
            timer.Start();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < TShock.Players.Length; i++)
            {
                if (TShock.Players[i] == null || !TShock.Players[i].Active || !TShock.Players[i].IsLoggedIn)
                    continue;

                if (TShock.Players[i].GetData<bool>("buildmode"))
                {
                    Player plr = Main.player[i];
                    TSPlayer tsplr = TShock.Players[i];

                    if (plr.hostile)
                    {
                        tsplr.SendErrorMessage("You cannot use Buildmode when PvP is active!");
                        TShock.Players[i].SetData("buildmode", false);
                        return;
                    }

                    NetMessage.SendData(7, i);
                    if (plr.statLife < plr.statLifeMax2 && !plr.dead)
                    {
                        tsplr.Heal(plr.statLifeMax2 - plr.statLife);
                        plr.statLife = plr.statLifeMax2;
                    }
                    tsplr.SetBuff(3, Int16.MaxValue); // Swiftness
                    tsplr.SetBuff(11, Int16.MaxValue); // Shine
                    tsplr.SetBuff(12, Int16.MaxValue); // Night owl
                    tsplr.SetBuff(63, Int16.MaxValue); // Panic
                    tsplr.SetBuff(104, Int16.MaxValue); // Mining
                    tsplr.SetBuff(107, Int16.MaxValue); // Builder
                    tsplr.SetBuff(113, Int16.MaxValue); // Lifeforce
                }
            }
        }

        private void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];

            if (player == null || !player.Active || !player.RealPlayer)
                return;

            player.SetData("buildmode", false);
        }

        private void OnSendBytes(SendBytesEventArgs args)
        {
            var player = TShock.Players[args.Socket.Id];

            if (player == null || !player.ContainsData("buildmode") || !player.GetData<bool>("buildmode"))
                return;

            switch (args.Buffer[2])
            {
                case 7: //WorldInfo
                    using (var writer = new BinaryWriter(new MemoryStream(args.Buffer, args.Offset, args.Count)))
                    {
                        writer.BaseStream.Position += 3;
                        writer.Write(27000); //Time
                        writer.Write(new BitsByte(true)); // isDayTime = true
                        writer.BaseStream.Position += 9;
                        writer.Write((short)Main.maxTilesY); //worldSurface
                        writer.Write((short)Main.maxTilesY); //rockLayer
                        writer.BaseStream.Position += 4;
                        writer.Write(Main.worldName); //worldName
                        writer.BaseStream.Position += 73;
                        writer.Write((Single)0); //Rain
                        writer.BaseStream.Position += 14;
                        writer.Write((Single)0); //Sandstorm
                    }
                    break;
                case 18:
                    using (var writer = new BinaryWriter(new MemoryStream(args.Buffer, args.Offset, args.Count)))
                    {
                        writer.BaseStream.Position += 3;
                        writer.Write(true); // isDayTime
                        writer.Write(27000); // Time
                    }
                    break;
                case 23:
                    int npcIndex;
                    using (var reader = new BinaryReader(new MemoryStream(args.Buffer, args.Offset, args.Count)))
                    {
                        reader.ReadBytes(3);
                        npcIndex = reader.ReadInt16();
                    }
                    if (Main.npc[npcIndex].friendly)
                        return;
                    using (var writer = new BinaryWriter(new MemoryStream(args.Buffer, args.Offset, args.Count)))
                    {
                        //Hide all NPCs at 0,0
                        writer.BaseStream.Position += 5;
                        writer.Write((Single)0); //PosX
                        writer.Write((Single)0); //PosY
                    }
                    break;
                case 27:
                    short projectileID;
                    byte projectileOwner;
                    using (var reader = new BinaryReader(new MemoryStream(args.Buffer, args.Offset, args.Count)))
                    {
                        reader.ReadBytes(3);
                        projectileID = reader.ReadInt16();
                        reader.ReadBytes(22);
                        projectileOwner = reader.ReadByte();
                    }
                    if (Main.projectile.First(e => e.identity == projectileID && e.owner == projectileOwner).friendly)
                        return;
                    using (var writer = new BinaryWriter(new MemoryStream(args.Buffer, args.Offset, args.Count)))
                    {
                        writer.BaseStream.Position += 3;
                        writer.BaseStream.Position += 25;
                        writer.Write((Int16)0); //Type
                    }
                    break;
            }
        }

        private void OnGetData(GetDataEventArgs args)
        {
            var player = TShock.Players[args.Msg.whoAmI];

            if (args.Handled || player == null || !player.ContainsData("buildmode") || !player.GetData<bool>("buildmode"))
                return;

            switch (args.MsgID)
            {
                case PacketTypes.Teleport:
                    if (player.TPlayer.FindBuffIndex(88) > -1)
                        player.Heal(100);
                    break;
                case PacketTypes.PaintTile:
                case PacketTypes.PaintWall:
                    int count = 0;
                    int type = args.Msg.readBuffer[args.Index + 8];

                    Item lastItem = null;
                    foreach (Item i in player.TPlayer.inventory)
                    {
                        if (i.paint == type)
                        {
                            lastItem = i;
                            count += i.stack;
                        }
                    }
                    if (count <= 5 && lastItem != null)
                        player.GiveItemCheck(lastItem.type, lastItem.Name, lastItem.stack);
                    break;
                case PacketTypes.Tile:
                    count = 0;
                    type = args.Msg.readBuffer[args.Index];
                    if ((type == 1 || type == 3) && player.TPlayer.inventory[player.TPlayer.selectedItem].type != 213)
                    {
                        int tile = args.Msg.readBuffer[args.Index + 9];
                        if (player.SelectedItem.tileWand > 0)
                            tile = player.SelectedItem.tileWand;
                        lastItem = null;
                        foreach (Item i in player.TPlayer.inventory)
                        {
                            if ((type == 1 && i.createTile == tile) || (type == 3 && i.createWall == tile))
                            {
                                lastItem = i;
                                count += i.stack;
                            }
                        }
                        if (count <= 5 && lastItem != null)
                            player.GiveItemCheck(lastItem.type, lastItem.Name, lastItem.stack);
                    }
                    else if (type == 5 || type == 10 || type == 12 || type == 16)
                    {
                        foreach (Item i in player.TPlayer.inventory)
                        {
                            if (i.type == 530)
                                count += i.stack;
                        }
                        if (count <= 5)
                            player.GiveItemCheck(530, "Wire", 1000 - count);
                    }
                    else if (type == 8)
                    {
                        foreach (Item i in player.TPlayer.inventory)
                        {
                            if (i.type == 849)
                                count += i.stack;
                        }
                        if (count <= 5)
                            player.GiveItemCheck(849, "Actuator", 1000 - count);
                    }
                    break;
                case PacketTypes.PlayerHurtV2:
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {
                        reader.ReadByte();
                        PlayerDeathReason.FromReader(reader);

                        short damage = reader.ReadInt16();
                        player.Heal(damage);
                        player.TPlayer.statLife = player.TPlayer.statLifeMax2;
                    }
                        break;
            }
        }

        private void BuildModeCmd(CommandArgs args)
        {
            if (args.TPlayer.hostile)
            {
                args.Player.SendErrorMessage("You cannot enable Buildmode with PvP active!");
                return;
            }

            args.Player.SetData("buildmode", !args.Player.GetData<bool>("buildmode"));

            args.Player.SendSuccessMessage((args.Player.GetData<bool>("buildmode") ? "En" : "Dis") + "abled build mode.");
            // Time
            NetMessage.SendData(7, args.Player.Index);
            // NPCs
            for (int i = 0; i < 200; i++)
            {
                if (!Main.npc[i].friendly)
                    args.Player.SendData(PacketTypes.NpcUpdate, "", i);
            }
            // Projectiles
            for (int i = 0; i < 1000; i++)
            {
                if (!Main.projectile[i].friendly)
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
                if (matchedPlayers[0].GetData<bool>("buildmode"))
                {
                    args.Player.SendInfoMessage($"{matchedPlayers[0].Name} has Buildmode enabled!");
                }
                else
                    args.Player.SendInfoMessage($"{matchedPlayers[0].Name} does not have Buildmode enabled.");
            }
        }
    }
}

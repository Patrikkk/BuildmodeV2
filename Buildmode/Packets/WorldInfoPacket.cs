using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Social;

namespace Buildmode.Packets
{
    public class WorldInfoPacket : IPacket
    {
        public void Read(MemoryStream Stream)
        {
            using (var reader = new BinaryReader(Stream))
            {
                reader.BaseStream.Position += 3; //Skipping length and packettype bytes.
                time = reader.ReadInt32();
                BitsByte1 = reader.ReadByte();
                moonPhase = reader.ReadByte();

                maxTilesX = reader.ReadInt16();
                maxTilesY = reader.ReadInt16();
                spawnTileX = reader.ReadInt16();
                spawnTileY = reader.ReadInt16();

                worldSurface = reader.ReadInt16();
                rockLayer = reader.ReadInt16();

                worldID = reader.ReadInt32();
                worldName = reader.ReadString();
                GameMode = reader.ReadByte();
                UniqueId = new Guid(reader.ReadBytes(16));
                WorldGeneratorVersion = reader.ReadUInt64();
                moonType = reader.ReadByte();

                treeBG1 = reader.ReadByte();
                treeBG2 = reader.ReadByte();
                treeBG3 = reader.ReadByte();
                treeBG4 = reader.ReadByte();
                corruptBG = reader.ReadByte();
                jungleBG = reader.ReadByte();
                snowBG = reader.ReadByte();
                hallowBG = reader.ReadByte();
                crimsonBG = reader.ReadByte();
                desertBG = reader.ReadByte();
                oceanBG = reader.ReadByte();
                mushroomBG = reader.ReadByte();
                underworldBG = reader.ReadByte();

                iceBackStyle = reader.ReadByte();
                jungleBackStyle = reader.ReadByte();
                hellBackStyle = reader.ReadByte();

                windSpeedTarget = reader.ReadSingle();
                numClouds = reader.ReadByte();
                for (int num173 = 0; num173 < 3; num173++)
                {
                    treeX[num173] = reader.ReadInt32();
                }
                for (int num174 = 0; num174 < 4; num174++)
                {
                    treeStyle[num174] = reader.ReadByte();
                }
                for (int num175 = 0; num175 < 3; num175++)
                {
                    caveBackX[num175] = reader.ReadInt32();
                }
                for (int num176 = 0; num176 < 4; num176++)
                {
                    caveBackStyle[num176] = reader.ReadByte();
                }

                TreeTops.SyncReceive(reader);

                maxRaining = reader.ReadSingle();

                BitsByte2 = reader.ReadByte();
                BitsByte3 = reader.ReadByte();
                BitsByte4 = reader.ReadByte();
                BitsByte5 = reader.ReadByte();
                BitsByte6 = reader.ReadByte();
                BitsByte7 = reader.ReadByte();
                BitsByte8 = reader.ReadByte();

                Copper = reader.ReadInt16();
                Iron = reader.ReadInt16();
                Silver = reader.ReadInt16();
                Gold = reader.ReadInt16();
                Cobalt = reader.ReadInt16();
                Mythril = reader.ReadInt16();
                Adamantite = reader.ReadInt16();

                invasionType = reader.ReadSByte();
                LobbyId = reader.ReadUInt64();
                IntendedSeverity = reader.ReadSingle();
            }
        }

        public void Write(MemoryStream Stream)
        {
            using (var writer = new BinaryWriter(Stream))
            {
                writer.BaseStream.Position = 0L;
                long startPos = writer.BaseStream.Position;
                writer.BaseStream.Position += 2L;
                writer.Write((byte)PacketTypes.WorldInfo);


                writer.Write(time);
                writer.Write(BitsByte1);
                writer.Write(moonPhase);

                writer.Write(maxTilesX);
                writer.Write(maxTilesY);
                writer.Write(spawnTileX);
                writer.Write(spawnTileY);

                writer.Write(worldSurface);
                writer.Write(rockLayer);

                writer.Write(worldID);
                writer.Write(worldName);

                writer.Write(GameMode);
                writer.Write(UniqueId.ToByteArray());
                writer.Write(WorldGeneratorVersion);

                writer.Write(moonType);

                writer.Write(treeBG1);
                writer.Write(treeBG2);
                writer.Write(treeBG3);
                writer.Write(treeBG4);
                writer.Write(corruptBG);
                writer.Write(jungleBG);
                writer.Write(snowBG);
                writer.Write(hallowBG);
                writer.Write(crimsonBG);
                writer.Write(desertBG);
                writer.Write(oceanBG);
                writer.Write(mushroomBG);
                writer.Write(underworldBG);

                writer.Write(iceBackStyle);
                writer.Write(jungleBackStyle);
                writer.Write(hellBackStyle);

                writer.Write(windSpeedTarget);
                writer.Write(numClouds);
                for (int i = 0; i < 3; i++)
                {
                    writer.Write(treeX[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    writer.Write((byte)treeStyle[i]);
                }
                for (int i = 0; i < 3; i++)
                {
                    writer.Write(caveBackX[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    writer.Write((byte)caveBackStyle[i]);
                }
                TreeTops.SyncSend(writer);
                if (!Main.raining)
                {
                    maxRaining = 0f;
                }
                writer.Write(maxRaining);

                writer.Write(BitsByte2);
                writer.Write(BitsByte3);
                writer.Write(BitsByte4);
                writer.Write(BitsByte5);
                writer.Write(BitsByte6);
                writer.Write(BitsByte7);
                writer.Write(BitsByte8);

                writer.Write(Copper);
                writer.Write(Iron);
                writer.Write(Silver);
                writer.Write(Gold);
                writer.Write(Cobalt);
                writer.Write(Mythril);
                writer.Write(Adamantite);

                writer.Write(invasionType);
                writer.Write(LobbyId);
                writer.Write(IntendedSeverity);

                int length = (int)writer.BaseStream.Position;
                writer.BaseStream.Position = startPos;
                writer.Write((short)length);
                writer.BaseStream.Position = length;

            }
        }

        public int time { get; set; } = (int)Main.time;
        public BitsByte BitsByte1 { get; set; } = new BitsByte(Main.dayTime, Main.bloodMoon, Main.eclipse);
        public byte moonPhase { get; set; } = (byte)Main.moonPhase;
        public short maxTilesX { get; set; } = (short)Main.maxTilesX;
        public short maxTilesY { get; set; } = (short)Main.maxTilesY;
        public short spawnTileX { get; set; } = (short)Main.spawnTileX;
        public short spawnTileY { get; set; } = (short)Main.spawnTileY;
        public short worldSurface { get; set; } = (short)Main.worldSurface;
        public short rockLayer { get; set; } = (short)Main.rockLayer;
        public int worldID { get; set; } = Main.worldID;
        internal string worldName { get; set; } = Main.worldName;
        public byte GameMode { get; set; } = (byte)Main.GameMode;
        public Guid UniqueId { get; set; } = Main.ActiveWorldFileData.UniqueId;
        public ulong WorldGeneratorVersion { get; set; } = Main.ActiveWorldFileData.WorldGeneratorVersion;
        public byte moonType { get; set; } = (byte)Main.moonType;
        public byte treeBG1 { get; set; } = (byte)WorldGen.treeBG1;
        public byte treeBG2 { get; set; } = (byte)WorldGen.treeBG2;
        public byte treeBG3 { get; set; } = (byte)WorldGen.treeBG3;
        public byte treeBG4 { get; set; } = (byte)WorldGen.treeBG4;
        public byte corruptBG { get; set; } = (byte)WorldGen.corruptBG;
        public byte jungleBG { get; set; } = (byte)WorldGen.jungleBG;
        public byte snowBG { get; set; } = (byte)WorldGen.snowBG;
        public byte hallowBG { get; set; } = (byte)WorldGen.hallowBG;
        public byte crimsonBG { get; set; } = (byte)WorldGen.crimsonBG;
        public byte desertBG { get; set; } = (byte)WorldGen.desertBG;
        public byte oceanBG { get; set; } = (byte)WorldGen.oceanBG;
        public byte mushroomBG { get; set; } = (byte)WorldGen.mushroomBG;
        public byte underworldBG { get; set; } = (byte)WorldGen.underworldBG;
        public byte iceBackStyle { get; set; } = (byte)Main.iceBackStyle;
        public byte jungleBackStyle { get; set; } = (byte)Main.jungleBackStyle;
        public byte hellBackStyle { get; set; } = (byte)Main.hellBackStyle;
        public float windSpeedTarget { get; set; } = Main.windSpeedTarget;
        public byte numClouds { get; set; } = (byte)Main.numClouds;
        public int[] treeX { get; set; } = Main.treeX;
        public int[] treeStyle { get; set; } = Main.treeStyle;
        public int[] caveBackX { get; set; } = Main.caveBackX;
        public int[] caveBackStyle { get; set; } = Main.caveBackStyle;
        public TreeTopsInfo TreeTops { get; set; } = WorldGen.TreeTops;
        public float maxRaining { get; set; } = Main.maxRaining;
        public BitsByte BitsByte2 { get; set; } = new BitsByte(WorldGen.shadowOrbSmashed, NPC.downedBoss1, NPC.downedBoss2, NPC.downedBoss3,
                                                                Main.hardMode, NPC.downedClown, false, NPC.downedPlantBoss);
        public BitsByte BitsByte3 { get; set; } = new BitsByte(NPC.downedMechBoss1, NPC.downedMechBoss2, NPC.downedMechBoss3, NPC.downedMechBossAny,
                                                                Main.cloudBGActive >= 1f, WorldGen.crimson, Main.pumpkinMoon, Main.snowMoon);
        public BitsByte BitsByte4 { get; set; } = new BitsByte(false, Main.fastForwardTime, Main.slimeRain, NPC.downedSlimeKing, NPC.downedQueenBee,
                                                                NPC.downedFishron, NPC.downedMartians, NPC.downedAncientCultist);
        public BitsByte BitsByte5 { get; set; } = new BitsByte(NPC.downedMoonlord, NPC.downedHalloweenKing, NPC.downedHalloweenTree, NPC.downedChristmasIceQueen, NPC.downedChristmasSantank,
                                                                NPC.downedChristmasTree, NPC.downedGolemBoss, BirthdayParty.PartyIsUp);
        public BitsByte BitsByte6 { get; set; } = new BitsByte(NPC.downedPirates, NPC.downedFrost, NPC.downedGoblins, Sandstorm.Happening, DD2Event.Ongoing,
                                                                DD2Event.DownedInvasionT1, DD2Event.DownedInvasionT2, DD2Event.DownedInvasionT3);
        public BitsByte BitsByte7 { get; set; } = new BitsByte(NPC.combatBookWasUsed, LanternNight.LanternsUp, NPC.downedTowerSolar, NPC.downedTowerVortex, NPC.downedTowerNebula,
                                                                NPC.downedTowerStardust, Main.forceHalloweenForToday, Main.forceXMasForToday);
        public BitsByte BitsByte8 { get; set; } = new BitsByte(NPC.boughtCat, NPC.boughtDog, NPC.boughtBunny, NPC.freeCake, Main.drunkWorld,
                                                                NPC.downedEmpressOfLight, NPC.downedQueenSlime, Main.getGoodWorld);
        public short Copper { get; set; } = (short)WorldGen.SavedOreTiers.Copper;
        public short Iron { get; set; } = (short)WorldGen.SavedOreTiers.Iron;
        public short Silver { get; set; } = (short)WorldGen.SavedOreTiers.Silver;
        public short Gold { get; set; } = (short)WorldGen.SavedOreTiers.Gold;
        public short Cobalt { get; set; } = (short)WorldGen.SavedOreTiers.Cobalt;
        public short Mythril { get; set; } = (short)WorldGen.SavedOreTiers.Mythril;
        public short Adamantite { get; set; } = (short)WorldGen.SavedOreTiers.Adamantite;
        public sbyte invasionType { get; set; } = (sbyte)Main.invasionType;
        public ulong LobbyId { get; set; } = Main.LobbyId;
        public float IntendedSeverity { get; set; } = Sandstorm.IntendedSeverity;
    }
}

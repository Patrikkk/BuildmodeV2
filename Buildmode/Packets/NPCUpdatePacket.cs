using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Buildmode.Packets
{
    public class NPCUpdatePacket : IPacket
    {
        public void Write(MemoryStream Stream)
        {
            using (var writer = new BinaryWriter(Stream))
            {
                writer.BaseStream.Position = 0L;
                long startPos = writer.BaseStream.Position;
                writer.BaseStream.Position += 2L;
                writer.Write((byte)PacketTypes.NpcUpdate);

                NPC nPC2 = Main.npc[npcIndex];
                writer.Write(npcIndex);
                writer.WriteVector2(position);
                writer.WriteVector2(velocity);
                writer.BaseStream.Position = writer.BaseStream.Length;
                /*writer.Write((ushort)nPC2.target);
                int num19 = nPC2.life;
                if (!nPC2.active)
                {
                    num19 = 0;
                }
                if (!nPC2.active || nPC2.life <= 0)
                {
                    nPC2.netSkip = 0;
                }
                short value3 = (short)nPC2.netID;
                bool[] array = new bool[4];
                BitsByte bb21 = (byte)0;
                bb21[0] = (nPC2.direction > 0);
                bb21[1] = (nPC2.directionY > 0);
                bb21[2] = (array[0] = (nPC2.ai[0] != 0f));
                bb21[3] = (array[1] = (nPC2.ai[1] != 0f));
                bb21[4] = (array[2] = (nPC2.ai[2] != 0f));
                bb21[5] = (array[3] = (nPC2.ai[3] != 0f));
                bb21[6] = (nPC2.spriteDirection > 0);
                bb21[7] = (num19 == nPC2.lifeMax);
                writer.Write(bb21);
                BitsByte bb22 = (byte)0;
                bb22[0] = (nPC2.statsAreScaledForThisManyPlayers > 1);
                bb22[1] = nPC2.SpawnedFromStatue;
                bb22[2] = (nPC2.strengthMultiplier != 1f);
                writer.Write(bb22);
                for (int num20 = 0; num20 < NPC.maxAI; num20++)
                {
                    if (array[num20])
                    {
                        writer.Write(nPC2.ai[num20]);
                    }
                }
                writer.Write(value3);
                if (bb22[0])
                {
                    writer.Write((byte)nPC2.statsAreScaledForThisManyPlayers);
                }
                if (bb22[2])
                {
                    writer.Write(nPC2.strengthMultiplier);
                }
                if (!bb21[7])
                {
                    byte b4 = 1;
                    if (nPC2.lifeMax > 32767)
                    {
                        b4 = 4;
                    }
                    else if (nPC2.lifeMax > 127)
                    {
                        b4 = 2;
                    }
                    writer.Write(b4);
                    switch (b4)
                    {
                        case 2:
                            writer.Write((short)num19);
                            break;
                        case 4:
                            writer.Write(num19);
                            break;
                        default:
                            writer.Write((sbyte)num19);
                            break;
                    }
                }
                if (nPC2.type >= 0 && nPC2.type < 663 && Main.npcCatchable[nPC2.type])
                {
                    writer.Write((byte)nPC2.releaseOwner);
                }
                */

                int length = (int)writer.BaseStream.Position;
                writer.BaseStream.Position = startPos;
                writer.Write((short)length);
                writer.BaseStream.Position = length;
            }
        }

        public void Read(MemoryStream Stream)
        {
            using (var reader = new BinaryReader(Stream))
            {
                reader.BaseStream.Position += 3; //Skipping length and packettype bytes.
                npcIndex = reader.ReadInt16();
                position = reader.ReadVector2();
                velocity = reader.ReadVector2();
               /* int num53 = reader.ReadUInt16();
                if (num53 == 65535)
                {
                    num53 = 0;
                }
                BitsByte bitsByte6 = reader.ReadByte();
                BitsByte bitsByte7 = reader.ReadByte();
                float[] array = new float[NPC.maxAI];
                for (int num54 = 0; num54 < NPC.maxAI; num54++)
                {
                    if (bitsByte6[num54 + 2])
                    {
                        array[num54] = reader.ReadSingle();
                    }
                    else
                    {
                        array[num54] = 0f;
                    }
                }
                int num55 = reader.ReadInt16();
                int? playerCountForMultiplayerDifficultyOverride = 1;
                if (bitsByte7[0])
                {
                    playerCountForMultiplayerDifficultyOverride = reader.ReadByte();
                }
                float value5 = 1f;
                if (bitsByte7[2])
                {
                    value5 = reader.ReadSingle();
                }
                int num56 = 0;
                if (!bitsByte6[7])
                {
                    switch (reader.ReadByte())
                    {
                        case 2:
                            num56 = reader.ReadInt16();
                            break;
                        case 4:
                            num56 = reader.ReadInt32();
                            break;
                        default:
                            num56 = reader.ReadSByte();
                            break;
                    }
                }
                int num57 = -1;
                NPC nPC2 = Main.npc[num52];
                if (nPC2.active && Main.multiplayerNPCSmoothingRange > 0 && Vector2.DistanceSquared(nPC2.position, vector4) < 640000f)
                {
                    nPC2.netOffset += nPC2.position - vector4;
                }
                if (!nPC2.active || nPC2.netID != num55)
                {
                    nPC2.netOffset *= 0f;
                    if (nPC2.active)
                    {
                        num57 = nPC2.type;
                    }
                    nPC2.active = true;
			        NPCSpawnParams spawnparams = new NPCSpawnParams
                    {
                        playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride,
                        strengthMultiplierOverride = value5
                    };
                    nPC2.SetDefaults(num55, spawnparams);
                }
                nPC2.position = vector4;
                nPC2.velocity = velocity;
                nPC2.target = num53;
                nPC2.direction = (bitsByte6[0] ? 1 : (-1));
                nPC2.directionY = (bitsByte6[1] ? 1 : (-1));
                nPC2.spriteDirection = (bitsByte6[6] ? 1 : (-1));
                if (bitsByte6[7])
                {
                    num56 = (nPC2.life = nPC2.lifeMax);
                }
                else
                {
                    nPC2.life = num56;
                }
                if (num56 <= 0)
                {
                    nPC2.active = false;
                }
                nPC2.SpawnedFromStatue = bitsByte7[0];
                if (nPC2.SpawnedFromStatue)
                {
                    nPC2.value = 0f;
                }
                for (int num58 = 0; num58 < NPC.maxAI; num58++)
                {
                    nPC2.ai[num58] = array[num58];
                }
                if (num57 > -1 && num57 != nPC2.type)
                {
                    nPC2.TransformVisuals(num57, nPC2.type);
                }
                if (num55 == 262)
                {
                    NPC.plantBoss = num52;
                }
                if (num55 == 245)
                {
                    NPC.golemBoss = num52;
                }
                if (nPC2.type >= 0 && nPC2.type < 663 && Main.npcCatchable[nPC2.type])
                {
                    nPC2.releaseOwner = reader.ReadByte();
                }*/
            }

        }

        public short npcIndex { get; set; }
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        /*public ushort target { get; set; }
        public BitsByte BitsByte1 { get; set; }
        public BitsByte BitsByte2 { get; set; }
        public float AI { get; set; }
        public short netID { get; set; }
        public byte PlayerCountDiffOverride { get; set; }
        public float StrengthMultiplier { get; set; }
        public byte LifeBytes { get; set; }
        public int life { get; set; }
        public byte ReleaseOwner { get; set; }*/

    }
}

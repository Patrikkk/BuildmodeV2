using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Buildmode.Packets
{
    public class TimeSetPacket : IPacket
    {
        public void Write(MemoryStream Stream)
        {
            using (var writer = new BinaryWriter(Stream))
            {
                writer.BaseStream.Position = 0L;
                long startPos = writer.BaseStream.Position;
                writer.BaseStream.Position += 2L;
                writer.Write((byte)PacketTypes.TimeSet);

                writer.Write((byte)(dayTime ? 1 : 0));
                writer.Write((int)time);
                writer.Write(sunModY);
                writer.Write(moonModY);

                int length = (int)writer.BaseStream.Position;
                writer.BaseStream.Position = startPos;
                writer.Write((short)length);
                writer.BaseStream.Position = length;
            }
        }

        public void Read(MemoryStream Stream)
        {
        }

        public bool dayTime { get; set; } = Main.dayTime;
        public int time { get; set; } = (int)Main.time;
        public short sunModY { get; set; } = Main.sunModY;
        public short moonModY { get; set; } = Main.moonModY;
    }
}

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
                writer.BaseStream.Position += 3;
                
                writer.Write((byte)(dayTime ? 1 : 0));
                writer.Write((int)time);
                writer.Write(sunModY);
                writer.Write(moonModY);
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

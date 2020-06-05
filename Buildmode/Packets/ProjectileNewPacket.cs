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
    public class ProjectileNewPacket : IPacket
    {
        public void Read(MemoryStream Stream)
        {
            using (var reader = new BinaryReader(Stream))
            {
                reader.BaseStream.Position += 3; //Skipping length and packettype bytes.
                Index = reader.ReadInt16();
                Vector2 position2 = reader.ReadVector2();
                Vector2 velocity3 = reader.ReadVector2();
                Owner = reader.ReadByte();
                Type = reader.ReadInt16();
            }
        }

        public void Write(MemoryStream Stream)
        {
            using (var writer = new BinaryWriter(Stream))
            {
                writer.BaseStream.Position += 2;

                writer.Write(Index);
                writer.WriteVector2(Position);
                writer.WriteVector2(Velocity);
                writer.Write(Owner);
                writer.Write(Type);
            }
        }

        public short Index { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public byte Owner { get; set; }
        public short Type { get; set; }
    }
}

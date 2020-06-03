using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildmode.Packets
{
    public interface IPacket
    {
        void Read(MemoryStream Stream);
        void Write(MemoryStream Stream);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TomB.Util;
using Xunit;

namespace Testing
{
    public class StreamTest
    {
        [Fact]
        public void Test1()
        {
            var buffer = new byte[8];
            var ms1 = new MemoryStream(buffer);
            ByteUtil.WriteI32BE(ms1, 0x11223344);
            ByteUtil.WriteI32LE(ms1, 0x11223344);

            Assert.Equal(buffer, new byte[] { 0x11, 0x22, 0x33, 0x44, 0x44, 0x33, 0x22, 0x11 });


            var ms2 = new MemoryStream(buffer);

            Assert.True(ByteUtil.ReadI32BE(ms2) == 0x11223344);
            Assert.True(ByteUtil.ReadI32LE(ms2) == 0x11223344);

        }
    }
}

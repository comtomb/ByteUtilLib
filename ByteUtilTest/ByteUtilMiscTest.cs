using TomB.Util;
using System;
using Xunit;

namespace Testing
{
    public class ByteUtilTest
    {
        [Fact]
        public void TestHexToBytes()
        {
            string h1 = "0F1E2D3C4B5A69788796A5B4C3D2E1F0";
            byte[] exp = new byte[] { 0x0F,0x1E, 0x2D, 0x3C, 0x4B, 0x5A, 0x69, 0x78, 0x87, 0x96, 0xA5, 0xB4, 0xC3, 0xD2, 0xE1, 0xF0 };
            byte[] com = ByteUtil.HexToByteArray(h1);
            Assert.Equal(exp, com);
        }
        [Fact]
        public void TestBytesToHex()
        {
            byte[] src = new byte[] { 0x22,0x0F, 0x1E, 0x2D, 0x3C, 0x4B, 0x5A, 0x69, 0x78, 0x87, 0x96, 0xA5, 0xB4, 0xC3, 0xD2, 0xE1, 0xF0 ,0xFF,0x33};
            string exp = "0F*1E*2D*3C*4B*5A*69*78*#87*96*A5*B4*C3*D2*E1*F0*#FF";
            string comp = ByteUtil.BytesToHexString(src, 1, 17, '*', '#');
            Assert.Equal(exp, comp);
        }
        [Fact]
        public void TestRevert()
        {
            byte[] src = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15  };

            byte[] rev1 = ByteUtil.RevertToArray(src, 2, 6);
            byte[] rev1exp = new byte[] { 8,7, 6, 5, 4, 3};
            Assert.Equal(rev1exp, rev1);

            ByteUtil.RevertInline(src, 2, 3);
            byte[] rev2exp = new byte[] { 1, 2, 5, 4, 3, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Assert.Equal(rev2exp, src);

            ByteUtil.RevertInline(src, 8, 4);
            byte[] rev3exp = new byte[] { 1, 2, 5, 4, 3, 6, 7,  8, 12,11, 10, 9,  13, 14, 15 };
            Assert.Equal(rev3exp, src);

        }


    }
}

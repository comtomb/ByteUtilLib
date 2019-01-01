using TomB.ByteUtilNetLib;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Testing
{
    public class PutGetIntTest
    {
        [Fact]
        public void TestPutGet16()
        {
            byte[] tmp = new byte[3];
            ByteUtil.PutI16LE(tmp, 1, 258);
            Assert.Equal(new byte[] { 0, 2, 1 }, tmp);
            Assert.True(ByteUtil.GetI16LE(tmp, 1) == 258);

            ByteUtil.PutI16LE(tmp, 1, -300);
            Assert.Equal(new byte[] { 0, 0xd4, 0xfe }, tmp);
            Assert.True(ByteUtil.GetI16LE(tmp, 1) == -300);
            Assert.True(ByteUtil.GetU16LE(tmp, 1) == 0xfed4);

            ByteUtil.PutI16BE(tmp, 1, 258);
            Assert.Equal(new byte[] { 0, 1, 2 }, tmp);
            Assert.True(ByteUtil.GetI16BE(tmp, 1) == 258);

            ByteUtil.PutI16BE(tmp, 1, -300);
            Assert.Equal(new byte[] { 0, 0xfe, 0xd4 }, tmp);
            Assert.True(ByteUtil.GetI16BE(tmp, 1) == -300);
            Assert.True(ByteUtil.GetU16BE(tmp, 1) == 0xfed4);
        }
        [Fact]
        public void TestPutGet32()
        {
            byte[] tmp = new byte[5];
            ByteUtil.PutI32LE(tmp, 1, 0x77665544);
            Assert.Equal(new byte[] { 0, 0x44, 0x55, 0x66, 0x77 }, tmp);
            Assert.True(ByteUtil.GetI32LE(tmp, 1) == 0x77665544);
            Assert.True(ByteUtil.GetU32LE(tmp, 1) == 0x77665544);

            ByteUtil.PutI32LE(tmp, 1, -0x77665544);
            Assert.Equal(new byte[] { 0, 0xbc, 0xaa, 0x99, 0x88 }, tmp);
            Assert.True(ByteUtil.GetI32LE(tmp, 1) == -0x77665544);
            Assert.True(ByteUtil.GetU32LE(tmp, 1) == 0x8899aabc);

            ByteUtil.PutI32BE(tmp, 1, -0x77665544);
            Assert.Equal(new byte[] { 0, 0x88, 0x99, 0xaa, 0xbc }, tmp);
            Assert.True(ByteUtil.GetI32BE(tmp, 1) == -0x77665544);
            Assert.True(ByteUtil.GetU32BE(tmp, 1) == 0x8899aabc);

        }
        [Fact]
        public void TestPutGet64()
        {
            var tmp = new byte[9];
            ByteUtil.PutI64LE(tmp, 1, 0x5566778899AABBCCL);
            Assert.Equal(new byte[] { 0, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55 }, tmp);
            Assert.True(ByteUtil.GetI64LE(tmp, 1) == 0x5566778899AABBCCL);
            Assert.True(ByteUtil.GetU64LE(tmp, 1) == 0x5566778899AABBCCL);

            ByteUtil.PutI64BE(tmp, 1, 0x5566778899AABBCCL);
            Assert.Equal(new byte[] { 0, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc }, tmp);
            Assert.True(ByteUtil.GetI64BE(tmp, 1) == 0x5566778899AABBCCL);
            Assert.True(ByteUtil.GetU64BE(tmp, 1) == 0x5566778899AABBCCL);

        }

    }
}

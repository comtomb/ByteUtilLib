// MIT License
//
// Copyright (c) 2018 tomb
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 

using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TomB.Util
{
	/// <summary>
	/// Utilities to operate on Byte-Arrays
	/// </summary>
    public  static class ByteUtil
    {
    	/// <summary>
    	/// return some build information
    	/// 	Mode: DEBUG/RELEASE
    	/// 	Target Framework
    	/// 	CPU
    	/// </summary>
    	/// <returns>hello_mode_tfm_cpu</returns>
        public static string GetHello()                                      
        {
            string tfm = "";
            string cpu = "";
            string mode = "";
#if DEBUG
            mode+="DEBUG";
#endif
#if RELEASE
            mode += "RELEASE";
#endif

#if TFM_NET451
            tfm+="NET451";
#endif
#if TFM_NET462
            tfm+="NET462";
#endif
#if TFM_NET472
            tfm+="NET472";
#endif
#if TFM_NETCORE21
            tfm += "NETCORE21";
#endif
#if TFM_NETSTANDARD20
            tfm +="NETSTANDARD20";
#endif
#if CPU_ANY
            cpu += "ANYCPU";
#endif
#if CPU_X86
            cpu+="x86";
#endif
#if CPU_X64
            cpu+="x64";
#endif

            return "hello_" +tfm+"_"+cpu+"_"+mode;
        }

		/// <summary>
		/// converts a string of HEX-Digits to <code>byte[]</code>
		/// </summary>
		/// <param name="src">string of hex digits</param>
		/// <returns></returns>
        public static byte[] HexToByteArray( String src)
        {
            if (src == null)
                throw new ArgumentNullException();
            int len = src.Length;
            if ((len & 1) != 0)
                throw new ArgumentException();
            byte[] ret = new byte[len >> 1];
            HexToBytes(ret, 0, src);
            return ret;
        }
        /// <summary>
        /// converts a string of HEX-Digits to byte[]
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="destOfs">offset in destination</param>
        /// <param name="src">string of hex-digits to be parsed. </param>
        /// <returns>number of bytes</returns>
        public static int HexToBytes( byte[] dest,int destOfs,String src )
        {
            if (dest == null || src == null)
                throw new ArgumentNullException();
            int len = src.Length;
            if (destOfs < 0 || (len&1)!=0)
                throw new ArgumentException();
            for(int i=0;i<len;i+=2)
            {
                byte x;
                // TODO explicit HEX-digit parsing
                if (!Byte.TryParse(src.Substring(i, 2), NumberStyles.HexNumber, null, out x))
                    throw new ArgumentException();
                dest[destOfs + i >> 1] = x;
            }
            return len >> 1;
        }
        /// <summary>
        /// convert a byte array to human readable string in hex representation. 
        /// </summary>
        /// <param name="src">source array</param>
        /// <param name="interChar">character to inserted after each byte</param>
        /// <param name="interOctet">character to be inserted after each 8th byte</param>
        /// <returns></returns>
        public static String BytesToHexString( byte[] src, char interChar = '\0', char interOctet = '\0')
        {
            if (src == null)
                throw new ArgumentNullException();
            return BytesToHexString(src, 0, src.Length, interChar, interOctet);
        }
        /// <summary>
        /// convert a byte array to human readable string in hex representation.
        /// </summary>
        /// <param name="src">source array</param>
        /// <param name="srcOfs">start offset in src</param>
        /// <param name="srcLen">number of bytes to be converted</param>
        /// <param name="interChar">character to inserted after each byte</param>
        /// <param name="interOctet">character to be inserted after each 8th byte</param>
        /// <returns>Hex-string</returns>
        public static String BytesToHexString(byte[] src, int srcOfs, int srcLen,char interChar='\0',char interOctet='\0')
        {
            var sb = new StringBuilder();
            BytesToHex(sb, src, srcOfs, srcLen, interChar, interOctet);
            return sb.ToString();
        }
        /// <summary>
        /// convert a byte array to human readable string in hex representation. Append result to an existing <code>StringBuilder</code>
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="src">source</param>
        /// <param name="srcOfs">start offset in src</param>
        /// <param name="srcLen">number of bytes to be converted</param>
        /// <param name="interChar">character to inserted after each byte</param>
        /// <param name="interOctet">character to be inserted after each 8th byte</param>
        public static void BytesToHex(StringBuilder dest,byte[] src, int srcOfs , int srcLen , char interChar = '\0', char interOctet = '\0')
        {
            if (dest == null || src == null)
                throw new ArgumentNullException();
            if (srcOfs < 0 || srcLen < 0 || srcOfs + srcLen > src.Length)
                throw new ArgumentException();
            for (int i = 0; i < srcLen; i++)
            {
                if (i > 0 && interChar != '\0')
                    dest.Append(interChar);
                if (i > 0 && (i % 8) == 0 && interOctet != '\0')
                    dest.Append(interOctet);

                dest.Append(src[srcOfs + i].ToString("X2"));
            }
        }
        
        /// <summary>
		/// revert the content of a byte array: {0,1,2,3,4,5,6} --> {6,5,4,3,2,1,0}
		/// </summary>
		/// <param name="src">array to be reverted</param>
        public static void RevertInline( byte[] src)
        {
            if (src == null)
                throw new ArgumentNullException();
            RevertInline(src, 0, src.Length);
        }
        /// <summary>
        /// revert a part og the content of a byte array.
        /// e.g {0,1,2,3,4,5,6} --> {0,1,4,3,2,5,6
        /// </summary>
        /// <param name="src">array where a part needs to be reverted</param>
        /// <param name="srcOfs">start offset</param>
        /// <param name="srcLen">number of bytes to be reverted</param>
        public static void RevertInline( byte[] src,int srcOfs,int srcLen )
        {
            if (src == null)
                throw new ArgumentNullException();
            if (srcOfs < 0 || srcLen < 0 || srcOfs + srcLen > src.Length)
                throw new ArgumentException();
            int end = srcOfs + srcLen - 1;
            int start = srcOfs;
            for(int i=0;i<srcLen/2;i++)
            {
                var h = src[end];
                src[end] = src[start];
                src[start] = h;
                start++;
                end--;
            }
        }
        /// <summary>
        /// extract a part of an array and store the bytes in reverted order in another array
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="destOfs">destination offset</param>
        /// <param name="src">array where a part needs to be reverted</param>
        /// <param name="srcOfs">start offset</param>
        /// <param name="srcLen">number of bytes to be reverted</param>
        public static void Revert(byte[] dest,int destOfs,byte[] src,int srcOfs,int srcLen )
        {
            if (dest == null || src == null)
                throw new ArgumentNullException();
            if (srcOfs < 0 || destOfs < 0 || srcLen<0)
                throw new ArgumentException();
            if (srcOfs + srcLen > src.Length || destOfs + srcLen > dest.Length)
                throw new ArgumentException();
            for (int i = 0; i < srcLen; i++)
                dest[destOfs + srcLen - i - 1] = src[srcOfs + i];
        }
        /// <summary>
        /// extract a part of an array and return the bytes in reverted order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="srcOfs">start</param>
        /// <param name="srcLen">length</param>
        /// <returns></returns>
        public static byte[] RevertToArray(byte[] src, int srcOfs, int srcLen)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (srcOfs < 0 || srcLen < 0 || srcOfs + srcLen > src.Length)
                throw new ArgumentException();
            byte[] r = new byte[srcLen];
            Revert(r, 0, src, srcOfs, srcLen);
            return r;
        }
        /// <summary>
        /// store <code>v</code> as a Signed Int 16 in Little Endian Order
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="ofs"></param>
        /// <param name="v"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutI16LE(byte[] dest, int ofs, short v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > dest.Length)
                throw new ArgumentException();
            dest[ofs  ] = (byte)(v & 0xff);
            dest[ofs+1] = (byte)((v>>8) & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Signed Int 16 in Big Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutI16BE(byte[] dest, int ofs, short v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > dest.Length)
                throw new ArgumentException();
            dest[ofs    ] = (byte)((v >> 8) & 0xff);
            dest[ofs + 1] = (byte)(v & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Unsigned Int 16 in Little Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutU16LE(byte[] dest, int ofs, ushort v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)(v & 0xff);
            dest[ofs + 1] = (byte)((v >> 8) & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Unsigned Int 16 in Big Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutU16BE(byte[] dest, int ofs, ushort v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)((v >> 8) & 0xff);
            dest[ofs + 1] = (byte)(v & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Signed Int 32 in Little Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutI32LE( byte[] dest,int ofs, int v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > dest.Length)
                throw new ArgumentException();
            dest[ofs    ] = (byte)(v & 0xff);
            dest[ofs + 1] = (byte)((v >> 8) & 0xff);
            dest[ofs + 2] = (byte)((v >> 16) & 0xff);
            dest[ofs + 3] = (byte)((v >> 24) & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Signed Int 32 in Big Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutI32BE(byte[] dest, int ofs, int v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > dest.Length)
                throw new ArgumentException();
            dest[ofs    ] = (byte)((v >> 24) & 0xff);
            dest[ofs + 1] = (byte)((v >> 16) & 0xff);
            dest[ofs + 2] = (byte)((v >> 8)  & 0xff);
            dest[ofs + 3] = (byte)( v        & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as an Unsigned Int 32 in Little Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutU32LE(byte[] dest, int ofs, uint v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)(v & 0xff);
            dest[ofs + 1] = (byte)((v >> 8) & 0xff);
            dest[ofs + 2] = (byte)((v >> 16) & 0xff);
            dest[ofs + 3] = (byte)((v >> 24) & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as an Unsigned Int 32 in Big Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutU32BE(byte[] dest, int ofs, uint v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)((v >> 24) & 0xff);
            dest[ofs + 1] = (byte)((v >> 16) & 0xff);
            dest[ofs + 2] = (byte)((v >> 8) & 0xff);
            dest[ofs + 3] = (byte)(v & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Signed Int 64 in Little Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutI64LE(byte[] dest, int ofs, long v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)(v & 0xff);
            dest[ofs + 1] = (byte)((v >> 8) & 0xff);
            dest[ofs + 2] = (byte)((v >> 16) & 0xff);
            dest[ofs + 3] = (byte)((v >> 24) & 0xff);
            dest[ofs + 4] = (byte)((v >> 32) & 0xff);
            dest[ofs + 5] = (byte)((v >> 40) & 0xff);
            dest[ofs + 6] = (byte)((v >> 48) & 0xff);
            dest[ofs + 7] = (byte)((v >> 56) & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as a Signed Int 64 in Big Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutI64BE(byte[] dest, int ofs, long v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)((v >> 56) & 0xff);
            dest[ofs + 1] = (byte)((v >> 48) & 0xff);
            dest[ofs + 2] = (byte)((v >> 40) & 0xff);
            dest[ofs + 3] = (byte)((v >> 32) & 0xff);
            dest[ofs + 4] = (byte)((v >> 24) & 0xff);
            dest[ofs + 5] = (byte)((v >> 16) & 0xff);
            dest[ofs + 6] = (byte)((v >> 8) & 0xff);
            dest[ofs + 7] = (byte)(v & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as an Unsigned Int 64 in Little Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutU64LE(byte[] dest, int ofs, ulong v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)(v & 0xff);
            dest[ofs + 1] = (byte)((v >> 8) & 0xff);
            dest[ofs + 2] = (byte)((v >> 16) & 0xff);
            dest[ofs + 3] = (byte)((v >> 24) & 0xff);
            dest[ofs + 4] = (byte)((v >> 32) & 0xff);
            dest[ofs + 5] = (byte)((v >> 40) & 0xff);
            dest[ofs + 6] = (byte)((v >> 48) & 0xff);
            dest[ofs + 7] = (byte)((v >> 56) & 0xff);
        }
        /// <summary>
        /// store <code>v</code> as an Unsigned Int 64 in Big Endian Order
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="v">value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutU64BE(byte[] dest, int ofs, ulong v)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > dest.Length)
                throw new ArgumentException();
            dest[ofs] = (byte)((v >> 56) & 0xff);
            dest[ofs + 1] = (byte)((v >> 48) & 0xff);
            dest[ofs + 2] = (byte)((v >> 40) & 0xff);
            dest[ofs + 3] = (byte)((v >> 32) & 0xff);
            dest[ofs + 4] = (byte)((v >> 24) & 0xff);
            dest[ofs + 5] = (byte)((v >> 16) & 0xff);
            dest[ofs + 6] = (byte)((v >> 8) & 0xff);
            dest[ofs + 7] = (byte)(v & 0xff);
        }
        /// <summary>
        /// get a signed Int 16 value from a byte array. Little Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetI16LE(byte[] src,int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > src.Length)
                throw new ArgumentException();
            short v = 0;
            v |= ((short)src[ofs  ]);
            v |= (short)(((int)src[ofs+1])<<8);
            return v;
        }
        /// <summary>
        /// get a signed Int 16 value from a byte array. Big Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetI16BE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > src.Length)
                throw new ArgumentException();
            short v = 0;
            v |= (short)(((int)src[ofs]) << 8);
            v |= ((short)src[ofs+1]);
            return v;
        }
        /// <summary>
        /// get an unsigned Int 16 value from a byte array. Little Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetU16LE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > src.Length)
                throw new ArgumentException();
            ushort v = 0;
            v |= ((ushort)src[ofs]);
            v |= (ushort)(((int)src[ofs + 1]) << 8);
            return v;
        }
        /// <summary>
        /// get an unsigned Int 16 value from a byte array. Big Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetU16BE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 2 > src.Length)
                throw new ArgumentException();
            ushort v = 0;
            v |= (ushort)(((int)src[ofs]) << 8);
            v |= ((ushort)src[ofs + 1]);
            return v;
        }
        /// <summary>
        /// get a signed Int 32 value from a byte array. Little Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetI32LE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > src.Length)
                throw new ArgumentException();
            int v = 0;
            v |= (((int)src[ofs])         );
            v |= (((int)src[ofs + 1]) << 8);
            v |= (((int)src[ofs + 2]) << 16);
            v |= (((int)src[ofs + 3]) << 24);
            return v;
        }
        /// <summary>
        /// get a signed Int 32 value from a byte array. Big Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetI32BE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > src.Length)
                throw new ArgumentException();
            int v = 0;
            v |= (((int)src[ofs])     <<24);
            v |= (((int)src[ofs + 1]) <<16);
            v |= (((int)src[ofs + 2]) <<8);
            v |= (((int)src[ofs + 3]));
            return v;
        }
        /// <summary>
        /// get an unsigned Int 32 value from a byte array. Little Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetU32LE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > src.Length)
                throw new ArgumentException();
            uint v = 0;
            v |= (((uint)src[ofs]));
            v |= (((uint)src[ofs + 1]) << 8);
            v |= (((uint)src[ofs + 2]) << 16);
            v |= (((uint)src[ofs + 3]) << 24);
            return v;
        }
        /// <summary>
        /// get an unsigned Int 32 value from a byte array. Big Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetU32BE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 4 > src.Length)
                throw new ArgumentException();
            uint v = 0;
            v |= (((uint)src[ofs]) << 24);
            v |= (((uint)src[ofs + 1]) << 16);
            v |= (((uint)src[ofs + 2]) << 8);
            v |= (((uint)src[ofs + 3]));
            return v;
        }
        /// <summary>
        /// get a signed Int 64 value from a byte array. Little Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetI64LE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > src.Length)
                throw new ArgumentException();
            long v = 0;
            v |= (((long)src[ofs]));
            v |= (((long)src[ofs + 1]) << 8);
            v |= (((long)src[ofs + 2]) << 16);
            v |= (((long)src[ofs + 3]) << 24);
            v |= (((long)src[ofs + 4]) << 32);
            v |= (((long)src[ofs + 5]) << 40);
            v |= (((long)src[ofs + 6]) << 48);
            v |= (((long)src[ofs + 7]) << 56);
            return v;
        }
        /// <summary>
        /// get a signed Int 64 value from a byte array. Big Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetI64BE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > src.Length)
                throw new ArgumentException();
            long v = 0;
            v |= (((long)src[ofs    ]  << 56));
            v |= (((long)src[ofs + 1]) << 48);
            v |= (((long)src[ofs + 2]) << 40);
            v |= (((long)src[ofs + 3]) << 32);
            v |= (((long)src[ofs + 4]) << 24);
            v |= (((long)src[ofs + 5]) << 16);
            v |= (((long)src[ofs + 6]) << 8);
            v |= (((long)src[ofs + 7]) );
            return v;
        }
        /// <summary>
        /// get a unsigned Int 64 value from a byte array. Little Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetU64LE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > src.Length)
                throw new ArgumentException();
            ulong v = 0;
            v |= (((ulong)src[ofs]));
            v |= (((ulong)src[ofs + 1]) << 8);
            v |= (((ulong)src[ofs + 2]) << 16);
            v |= (((ulong)src[ofs + 3]) << 24);
            v |= (((ulong)src[ofs + 4]) << 32);
            v |= (((ulong)src[ofs + 5]) << 40);
            v |= (((ulong)src[ofs + 6]) << 48);
            v |= (((ulong)src[ofs + 7]) << 56);
            return v;
        }
        /// <summary>
        /// get an usigned Int 64 value from a byte array. Big Endian order
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <returns>retrieved value</returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetU64BE(byte[] src, int ofs)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + 8 > src.Length)
                throw new ArgumentException();
            ulong v = 0;
            v |= (((ulong)src[ofs] << 56));
            v |= (((ulong)src[ofs + 1]) << 48);
            v |= (((ulong)src[ofs + 2]) << 40);
            v |= (((ulong)src[ofs + 3]) << 32);
            v |= (((ulong)src[ofs + 4]) << 24);
            v |= (((ulong)src[ofs + 5]) << 16);
            v |= (((ulong)src[ofs + 6]) << 8);
            v |= (((ulong)src[ofs + 7]));
            return v;
        }
        /// <summary>
        /// extract a part of an array and return it as a new array
        /// </summary>
        /// <param name="src">source</param>
        /// <param name="ofs">offset</param>
        /// <param name="len">length</param>
        /// <returns>byte[len]</returns>
        public static byte[] ExtractToBytes(byte[] src,int ofs,int len)
        {
            if (src == null)
                throw new ArgumentNullException();
            if (ofs < 0 || ofs + len > src.Length)
                throw new ArgumentException();
            var h = new byte[len];
            Array.Copy(src, ofs, h, 0, len);
            return h;
        }
        /// <summary>
        /// return a byte[] of Length <code>len</code> with random values 
        /// </summary>
        /// <param name="len">length of requested array</param>
        /// <param name="rnd">rnd to be used. can be <code>null</code> </param>
        /// <returns></returns>
        public static byte[] RandomArray(int len,Random rnd=null)
        {
        	if(len<=0)
        		throw new ArgumentException();
        	var arr=new byte[len];
            if (rnd == null)
                rnd = new Random();
            rnd.NextBytes(arr);
        	return arr;
        }
        /// <summary>
        /// fill a part of an array with random values
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="ofs">offset</param>
        /// <param name="len">length</param>
        /// <param name="rnd">rnd to be used. can be <code>null</code> </param>
        public static void Randomize(byte[] dest, int ofs,int len,Random rnd=null)
        {
        	if(dest==null)
        		throw new ArgumentNullException();
            if (ofs < 0 || ofs + len > dest.Length)
                throw new ArgumentException();   
            if(rnd==null)
            	rnd=new Random();
            for (int i = 0; i < len; i++)
                dest[ofs + i] = (byte)rnd.Next(255);
        }
        /// <summary>
        /// fill an array with random values
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="rnd">rnd to be used. can be <code>null</code> </param>
        public static void Randomize(byte[] dest,Random rnd=null)
        {
        	if(dest==null)
        		throw new ArgumentNullException();
        	Randomize(dest,0,dest.Length,rnd);
        }
        /// <summary>
        /// concatenate arrays to a single array
        /// </summary>
        /// <param name="arrs"></param>
        /// <returns></returns>
        public static byte[] Concat( params byte[][] arrs)
        {
            int total = 0;
            for (int i = 0; i < arrs.Length; i++)
                total += arrs[i].Length;
            var ret = new byte[total];
            int p = 0;
            for(int i=0;i<arrs.Length;i++)
            {
                arrs[i].CopyTo(ret, p);
                p += arrs[i].Length;
            }

            return ret;
        }
        /// <summary>
        /// concatenate arrays to a single array
        /// </summary>
        /// <param name="dest">array to receive the concatenated arrays</param>
        /// <param name="ofs"></param>
        /// <param name="arrs"></param>
        /// <returns></returns>
        public static int Concat(byte[] dest,int ofs,params byte[][] arrs)
        {
            if (dest == null)
                throw new ArgumentNullException();
            if (ofs < 0)
                throw new ArgumentException();
            int total = 0;
            for (int i = 0; i < arrs.Length; i++)
                total += arrs[i].Length;
            if (dest.Length - ofs < total)
                throw new ArgumentException();
            int p = ofs;
            for (int i = 0; i < arrs.Length; i++)
            {
                arrs[i].CopyTo(dest, p);
                p += arrs[i].Length;
            }

            return total;

        }
        #region Stream Read Ops
        /// <summary>
        /// async read of single byte
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte> ReadByteAsync(Stream stream )
        {
            var b = await ReadBytesAsync(stream, 1);
            return b[0];
        }

        /// <summary>
        /// Read a byte array from a stream
        /// </summary>
        /// <param name="stream">input</param>
        /// <param name="len">length</param>
        /// <returns></returns>
        public static byte[] ReadBytes(Stream stream, int len)
        {
            var buffer = new byte[len];
            int p = 0;
            while(p<len)
            {
                int got = stream.Read(buffer, p, len - p);
                if (got <= 0)
                    throw new IOException();
                p += got;
            }
            return buffer;
        }
        /// <summary>
        /// Async Read a byte array from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadBytesAsync(Stream stream,int len)
        {
            var buffer = new byte[len];
            int p = 0;
            while (p < len)
            {
                int got = await stream.ReadAsync(buffer, p, len - p);
                if (got <= 0)
                    throw new IOException();
                p += got;
            }
            return buffer;

        }
        /// <summary>
        /// Read a single byte from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte ReadByte(Stream stream)
        {
            int b = stream.ReadByte();
            if (b < 0)
                throw new IOException();
            return (byte)b;
        }
        /// <summary>
        /// Read I32BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static int ReadI32BE(Stream stream )
        {
            return GetI32BE(ReadBytes(stream, 4), 0);
        }
        /// <summary>
        /// async Read I32 from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<int> ReadI32BEAsync(Stream stream)
        {
            return GetI32BE(await ReadBytesAsync(stream, 4), 0);
        }

        /// <summary>
        /// Read I32LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static int ReadI32LE(Stream stream)
        {
            return GetI32LE(ReadBytes(stream, 4), 0);
        }
        /// <summary>
        /// async Read I32LE from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<int> ReadI32LEAsync(Stream stream)
        {
            return GetI32LE(await ReadBytesAsync(stream, 4), 0);
        }
        /// <summary>
        /// Read U32BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static uint ReadU32BE(Stream stream)
        {
            return GetU32BE(ReadBytes(stream, 4), 0);
        }
        /// <summary>
        /// async Read U32BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<uint> ReadU32BEAsync(Stream stream)
        {
            return GetU32BE(await ReadBytesAsync(stream, 4), 0);
        }
        /// <summary>
        /// Read U32LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static uint ReadU32LE(Stream stream)
        {
            return GetU32LE(ReadBytes(stream, 4), 0);
        }
        /// <summary>
        /// async Read U32LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<uint> ReadU32LEAsync(Stream stream)
        {
            return GetU32LE(await ReadBytesAsync(stream, 4), 0);
        }

        /// <summary>
        /// Read I64BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static long ReadI64BE(Stream stream)
        {
            return GetI64BE(ReadBytes(stream, 8), 0);
        }
        /// <summary>
        /// async Read I64BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<long> ReadI64BEAsync(Stream stream)
        {
            return GetI64BE(await ReadBytesAsync(stream, 8), 0);
        }
        /// <summary>
        /// Read I64LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static long ReadI64LE(Stream stream)
        {
            return GetI64LE(ReadBytes(stream, 8), 0);
        }
        /// <summary>
        /// async Read I64LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<long> ReadI64LEAsync(Stream stream)
        {
            return GetI64LE(await ReadBytesAsync(stream, 8), 0);
        }
        /// <summary>
        /// Read U64BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static ulong ReadU64BE(Stream stream)
        {
            return GetU64BE(ReadBytes(stream, 8), 0);
        }
        /// <summary>
        /// async Read U64BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<ulong> ReadU64BEAsync(Stream stream)
        {
            return GetU64BE(await ReadBytesAsync(stream, 8), 0);
        }
        /// <summary>
        /// async Read U64LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<ulong> ReadU64LEAsync(Stream stream)
        {
            return GetU64LE(await ReadBytesAsync(stream, 8), 0);
        }
        /// <summary>
        /// Read U64LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<ulong> ReadU64LE(Stream stream)
        {
            return GetU64LE(await ReadBytesAsync(stream, 8), 0);
        }


        /// <summary>
        /// Read I16BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static short ReadI16BE(Stream stream)
        {
            return GetI16BE(ReadBytes(stream, 2), 0);
        }
        /// <summary>
        /// async Read I16BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<short> ReadI16BEAsync(Stream stream)
        {
            return GetI16BE(await ReadBytesAsync(stream, 2), 0);
        }
        /// <summary>
        /// Read I16LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<short> ReadI16LEAsync(Stream stream)
        {
            return GetI16LE(await ReadBytesAsync(stream, 2), 0);
        }
        /// <summary>
        /// Read U16BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static ushort ReadU16BE(Stream stream)
        {
            return GetU16BE(ReadBytes(stream, 2), 0);
        }
        /// <summary>
        /// async Read U16BE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<ushort> ReadU16BEAsync(Stream stream)
        {
            return GetU16BE(await ReadBytesAsync(stream, 2), 0);
        }
        /// <summary>
        /// Read U16LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static ushort ReadU16LE(Stream stream)
        {
            return GetU16LE(ReadBytes(stream, 2), 0);
        }
        /// <summary>
        /// Read U16LE from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>value</returns>
        public static async Task<ushort> ReadU16LEAsync(Stream stream)
        {
            return GetU16LE(await ReadBytesAsync(stream, 2), 0);
        }



        #endregion
        #region Stream Write Ops
        /// <summary>
        /// Write an array to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="arr"></param>
        public static void WriteBytes(Stream stream,byte[] arr )
        {
            stream.Write(arr, 0, arr.Length );
        }
        /// <summary>
        /// async write an array to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static async Task<int> WriteBytesAsync(Stream stream,byte[] arr)
        {
            await stream.WriteAsync(arr, 0, arr.Length);
            return 0;
        }
        /// <summary>
        /// Write a byte to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v"></param>
        public static void WriteByte(Stream stream,byte v)
        {
            stream.WriteByte(v);
        }
        /// <summary>
        /// Write an I32BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteI32BE(Stream stream,int v)
        {
            var h = new byte[4];
            PutI32BE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an I32BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteI32BEAsync(Stream stream, int v)
        {
            var h = new byte[4];
            PutI32BE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an I32LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteI32LE(Stream stream, int v)
        {
            var h = new byte[4];
            PutI32LE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an I32LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteI32LEAsync(Stream stream, int v)
        {
            var h = new byte[4];
            PutI32LE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an U32BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteU32BE(Stream stream, uint v)
        {
            var h = new byte[4];
            PutU32BE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an U32BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteU32BEAsync(Stream stream, uint v)
        {
            var h = new byte[4];
            PutU32BE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an U32LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteU32LE(Stream stream, uint v)
        {
            var h = new byte[4];
            PutU32LE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an U32LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteU32LEAsync(Stream stream, uint v)
        {
            var h = new byte[4];
            PutU32LE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }

        /// <summary>
        /// Write an I16BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteI16BE(Stream stream, short v)
        {
            var h = new byte[2];
            PutI16BE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an I16BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteI16BEAsync(Stream stream, short v)
        {
            var h = new byte[2];
            PutI16BE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an I16LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteI16LE(Stream stream, short v)
        {
            var h = new byte[2];
            PutI16LE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an I16LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteI16LEAsync(Stream stream, short v)
        {
            var h = new byte[2];
            PutI16LE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an U16BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteU16BE(Stream stream, ushort v)
        {
            var h = new byte[2];
            PutU16BE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an U16BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteU16BEAsync(Stream stream, ushort v)
        {
            var h = new byte[2];
            PutU16BE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an U16LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteU16LE(Stream stream, ushort v)
        {
            var h = new byte[2];
            PutU16LE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an U16LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteU16LEAsync(Stream stream, ushort v)
        {
            var h = new byte[2];
            PutU16LE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }

        /// <summary>
        /// Write an I64BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteI64BE(Stream stream, long v)
        {
            var h = new byte[8];
            PutI64BE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an I64BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteI64BEAsync(Stream stream, long v)
        {
            var h = new byte[8];
            PutI64BE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an I64LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteI64LE(Stream stream, long v)
        {
            var h = new byte[8];
            PutI64LE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an I64LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteI64LEAsync(Stream stream, long v)
        {
            var h = new byte[8];
            PutI64LE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an U64BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteU64BE(Stream stream, ulong v)
        {
            var h = new byte[8];
            PutU64BE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an U64BE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteU64BEAsync(Stream stream, ulong v)
        {
            var h = new byte[8];
            PutU64BE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }
        /// <summary>
        /// Write an U64LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static void WriteU64LE(Stream stream, ulong v)
        {
            var h = new byte[8];
            PutU64LE(h, 0, v);
            WriteBytes(stream, h);
        }
        /// <summary>
        /// async Write an U64LE to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="v">value to be written</param>
        public static Task WriteU64LEAsync(Stream stream, ulong v)
        {
            var h = new byte[8];
            PutU64LE(h, 0, v);
            return WriteBytesAsync(stream, h);
        }



        #endregion
    }
}

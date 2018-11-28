using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteUtilNetLib
{
    public sealed class ByteUtil
    {
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
                if (!Byte.TryParse(src.Substring(i, 2), NumberStyles.HexNumber, null, out x))
                    throw new ArgumentException();
                dest[destOfs + i >> 1] = x;
            }
            return len >> 1;
        }
        public static String BytesToHexString( byte[] src, char interChar = '\0', char interOctet = '\0')
        {
            if (src == null)
                throw new ArgumentNullException();
            return BytesToHexString(src, 0, src.Length, interChar, interOctet);
        }
        public static String BytesToHexString(byte[] src, int srcOfs, int srcLen,char interChar='\0',char interOctet='\0')
        {
            StringBuilder sb = new StringBuilder();
            BytesToHex(sb, src, srcOfs, srcLen, interChar, interOctet);
            return sb.ToString();
        }
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

        public static void RevertInline( byte[] src)
        {
            if (src == null)
                throw new ArgumentNullException();
            RevertInline(src, 0, src.Length);
        }
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
    }
}

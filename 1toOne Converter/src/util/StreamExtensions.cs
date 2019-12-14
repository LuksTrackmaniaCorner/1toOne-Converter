using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace _1toOne_Converter.Streams
{
    //Low effort implementations of some simple and convenient stream operations
    public static class StreamExtensions
    {
        public static int Peek(this Stream stream, byte[] array, int offset, int count, int peekDistance = 0)
        {
            long pos = stream.Position;
            stream.Position += peekDistance;
            int result = stream.Read(array, offset, count); //Could cause exceptions
            stream.Position = pos; //Reset position
            return result;
        }

        public static byte[] SimpleRead(this Stream stream, int count)
        {
            byte[] result = new byte[count];
            if(stream.Read(result, 0, count) != count) {
                throw new Exception("SimpleRead couldn't read " + count + " bytes");
            }
            return result;
        }

        public static byte[] SimplePeek(this Stream stream, int count, int peekDistance = 0)
        {
            long pos = stream.Position;
            stream.Position += peekDistance;

            byte[] result = new byte[count];
            int n = stream.Read(result, 0, count);

            stream.Position = pos;

            if(n != count) {
                throw new Exception("SimplePeek couldn't read " + count + " bytes");
            }
            return result;
        }

        public static void SimpleWrite(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public static byte ReadAByte(this Stream stream)
        {
            return stream.SimpleRead(1)[0];
        }

        public static void WriteByte(this Stream stream, byte b)
        {
            byte[] srcdata = { b };
            stream.SimpleWrite(srcdata);
        }

        public static ushort ReadUshort(this Stream stream)
        {
            return BitConverter.ToUInt16(stream.SimpleRead(2), 0);
        }

        public static uint ReadUInt(this Stream stream)
        {
            return BitConverter.ToUInt32(stream.SimpleRead(4), 0);
        }

        public static uint PeekUInt(this Stream stream)
        {
            return BitConverter.ToUInt32(stream.SimplePeek(4), 0);
        }

        public static void WriteUInt(this Stream stream, uint ui)
        {
            var srcdata = BitConverter.GetBytes(ui);
            stream.SimpleWrite(srcdata);
        }

        public static float ReadFloat(this Stream stream)
        {
            return BitConverter.ToSingle(stream.SimpleRead(4), 0);
        }

        public static void WriteFloat(this Stream stream, float f)
        {
            var srcdata = BitConverter.GetBytes(f);
            stream.SimpleWrite(srcdata);
        }
    }
}

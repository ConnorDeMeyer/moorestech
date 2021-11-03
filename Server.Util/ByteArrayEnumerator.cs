﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Util
{
    public class ByteArrayEnumerator
    {
        private readonly List<byte> _payload;
        private int index = 0;
        public ByteArrayEnumerator(byte[] payload)
        {
            _payload = payload.ToList();
        }

        public byte MoveNextToGetByte()
        {
            return _payload[index++];
        }
        public int MoveNextToGetInt()
        {
            var b = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                b.Add(_payload[index++]);
            }
            if (BitConverter.IsLittleEndian) b.Reverse();
            return BitConverter.ToInt32(b.ToArray(),0);
        }

        public short MoveNextToGetShort()
        {
            var b = new List<byte>();
            for (int i = 0; i < 2; i++)
            {
                b.Add(_payload[index++]);
            }
            if (BitConverter.IsLittleEndian) b.Reverse();
            return BitConverter.ToInt16(b.ToArray(),0);
        }
        public float MoveNextToGetFloat()
        {
            var b = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                b.Add(_payload[index++]);
            }
            if (BitConverter.IsLittleEndian) b.Reverse();
            return BitConverter.ToSingle(b.ToArray(),0);
        }
        /// <summary>
        /// バイト数を指定してそのバイト数の文字列を取得します
        /// </summary>
        /// <param name="byteNum">バイト数 指定しないor0の時最後まで取得する</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string MoveNextToGetString(int byteNum)
        {
            if (byteNum < 0)
            {
                throw new ArgumentOutOfRangeException($"指定バイト数:{byteNum} バイト数は0以上にしてください");
            }
            var b = new List<byte>();
            if (byteNum == 0)
            {
                while (index < b.Count)
                {
                    b.Add(_payload[index++]);
                }
            }
            else
            {
                for (int i = 0; i < byteNum; i++)
                {
                    b.Add(_payload[index++]);
                }
            }
            return Encoding.UTF8.GetString(b.ToArray());
        }
    }
}
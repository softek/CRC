﻿using System;

namespace CrcTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Softek.CRC.Crc16 test = new Softek.CRC.Crc16();
            Console.WriteLine($"Polynomial: {test.Polynomial:x4} Endianness: {test.Endianness}");

            test.FlipEndianness();
            Console.WriteLine($"Flipped Polynomial: {test.Polynomial:x4} Endianness: {test.Endianness}");

            test.FlipEndianness();
            Console.WriteLine($"Flipped back Polynomial: {test.Polynomial:x4} Endianness: {test.Endianness}");

            Console.WriteLine($"Initial       Crc16 value: {test}  Byte Count:{test.ByteCount:N0}");

            // testing AddByte
            test.AddByte(0x80);
            Console.WriteLine($"AddByte test  Crc16 value: {test}  Byte Count:{test.ByteCount:N0}");
            test.AddByte(0x00);
            Console.WriteLine($"AddByte test  Crc16 value: {test}  Byte Count:{test.ByteCount:N0}");

            // testing reset
            test.Reset();
            Console.WriteLine($"Reset test    Crc16 value: {test}  Byte Count:{test.ByteCount:N0}");

            // testing AddBytes
            test.AddBytes(new byte[2] { 0x80, 0x00 });
            Console.WriteLine($"AddBytes test Crc16 value: {test}  Byte Count:{test.ByteCount:N0}");

            Console.ReadKey();
        }
    }
}

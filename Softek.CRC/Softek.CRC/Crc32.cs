using System;
using System.Collections.Generic;
using System.Text;

namespace Softek.CRC
{
    public class Crc32
    {
        private UInt32[] _lookup;
        private UInt32 _polynomial = 0xEDB88320; // ISO 3309 standard 32 bit CRC
        private Endianness _endianness = Endianness.Little;
        private UInt32 _seed = 0;

        public Crc32()
        {
            // initialize the object with the CRC-16 polynomial
            Reset();
        }

        public Crc32(UInt32 polynomial)
        {
            _polynomial = polynomial;

            Reset();
        }

        public Crc32(UInt32 polynomial, Endianness endianness)
        {
            _polynomial = polynomial;
            _endianness = endianness;

            Reset();
        }

        public Crc32(UInt32 polynomial, Endianness endianness, UInt32 seed)
        {
            _polynomial = polynomial;
            _endianness = endianness;
            _seed = seed;

            Reset();
        }

        public Crc32(Crc32Type type)
        {
            _polynomial = GetPolynomialByType(type);

            Reset();
        }

        public Crc32(Crc32Type type, Endianness endianness)
        {
            _polynomial = GetPolynomialByType(type);

            if (endianness == Endianness.Big)
            {
                _polynomial = FlipPolynomial(_polynomial);
                _endianness = endianness;
            }

            Reset();
        }

        public Crc32(Crc32Type type, Endianness endianness, UInt32 seed)
        {
            _polynomial = GetPolynomialByType(type);

            if (endianness == Endianness.Big)
            {
                _polynomial = FlipPolynomial(_polynomial);
                _endianness = endianness;
            }

            _seed = seed;

            Reset();
        }

        private UInt32 GetPolynomialByType(Crc32Type type)
        {
            switch (type)
            {
                case Crc32Type.Castagnoli:
                    return 0x82F63B78;
                case Crc32Type.ISO_3309:
                    return 0xEDB88320;
                case Crc32Type.Koopman:
                    return 0xEB31D82E;
                case Crc32Type.Koopman_2:
                    return 0x992C1A4C;
                case Crc32Type.Q:
                    return 0xD5828281;
                default:
                    throw new ArgumentOutOfRangeException("type", "The type parameter must be a valid Crc32Type.");
            }
        }

        public UInt32 Polynomial
        {
            get { return _polynomial; }
            set
            {
                if (_polynomial != value)
                {
                    _polynomial = value;
                    Reset();
                }
            }
        }

        public UInt32 Seed
        {
            get { return _seed; }
            set
            {
                if (_seed != value)
                {
                    _seed = value;
                    Reset();
                }
            }
        }

        public Endianness Endianness
        {
            get
            {
                return _endianness;
            }
            set
            {
                if (_endianness != value)
                {
                    _endianness = value;
                    Reset();
                }
            }
        }

        /// <summary>
        /// This function will switch the endianness property as well as converting the polynomial into the flipped endianness form.
        /// </summary>
        public void FlipEndianness()
        {
            switch (_endianness)
            {
                case Endianness.Big:
                    {
                        _endianness = Endianness.Little;
                    }
                    break;
                case Endianness.Little:
                    {
                        _endianness = Endianness.Big;
                    }
                    break;
            }

            // flip the polynomial
            _polynomial = FlipPolynomial(_polynomial);

            Reset();
        }

        private UInt32 FlipPolynomial(UInt32 polynomial)
        {
            UInt32 returnPolynomial = 0;

            for (int i = 0; i < 32; i++)
            {
                if ((polynomial & (UInt32)(1 << i)) == (UInt32)(1 << i))
                    returnPolynomial += (UInt32)(1 << (31 - i));
            }

            return returnPolynomial;
        }

        public UInt32 Crc { get; private set; }

        public long ByteCount { get; private set; }

        public void Reset()
        {
            Crc = Seed;
            ByteCount = 0;
            GenerateLookupTable();
        }

        private void GenerateLookupTable()
        {
            _lookup = new UInt32[256];

            for (int i = 0; i < 256; i++)
            {
                switch (Endianness)
                {
                    case Endianness.Big:
                        {
                            _lookup[i] = (UInt32)(i << 24);

                            for (int j = 0; j < 8; j++)
                            {
                                if ((_lookup[i] & 0x80000000) == 0x80000000)
                                    _lookup[i] = (UInt32)(((_lookup[i] << 1)) ^ Polynomial);
                                else
                                    _lookup[i] <<= 1;
                            }
                        }
                        break;
                    case Endianness.Little:
                        {
                            _lookup[i] = (UInt32)i;

                            for (int j = 0; j < 8; j++)
                            {
                                if ((_lookup[i] & 1) == 1)
                                    _lookup[i] = (UInt32)((_lookup[i] >> 1) ^ Polynomial);
                                else
                                    _lookup[i] >>= 1;
                            }
                        }
                        break;
                }
            }
        }

        public void AddByte(byte data)
        {
            switch (Endianness)
            {
                case Endianness.Big:
                    {
                        Crc = (UInt32)((Crc << 8) ^ _lookup[(Crc >> 24) ^ data]);
                    }
                    break;
                case Endianness.Little:
                    {
                        Crc = (UInt32)((Crc >> 8) ^ _lookup[(Crc ^ data) & 0x000000FF]);
                    }
                    break;
            }
            ByteCount++;
        }

        public void AddBytes(byte[] data)
        {
            foreach (byte b in data)
            {
                AddByte(b);
            }
        }

        public override string ToString()
        {
            return Crc.ToString("x8");
        }
    }
}

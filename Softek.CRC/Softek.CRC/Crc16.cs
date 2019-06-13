using System;
using System.Collections.Generic;
using System.Text;

namespace Softek.CRC
{
    public class Crc16
    {
        private ushort[] _lookup;
        private ushort _polynomial = 0xA001;
        private Endianness _endianness = Endianness.Little;
        private ushort _seed = 0;

        public Crc16()
        {
            // initialize the object with the CRC-16 polynomial
            Reset();
        }

        public Crc16(ushort polynomial)
        {
            _polynomial = polynomial;

            Reset();
        }

        public Crc16(ushort polynomial, Endianness endianness)
        {
            _polynomial = polynomial;
            _endianness = endianness;

            Reset();
        }

        public Crc16(ushort polynomial, Endianness endianness, ushort seed)
        {
            _polynomial = polynomial;
            _endianness = endianness;
            _seed = seed;

            Reset();
        }

        public Crc16(Crc16Type type)
        {
            _polynomial = GetPolynomialByType(type);

            Reset();
        }

        public Crc16(Crc16Type type, Endianness endianness)
        {
            _polynomial = GetPolynomialByType(type);

            if (endianness == Endianness.Big)
            {
                _polynomial = FlipPolynomial(_polynomial);
                _endianness = endianness;
            }

            Reset();
        }

        public Crc16(Crc16Type type, Endianness endianness, ushort seed)
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

        private ushort GetPolynomialByType(Crc16Type type)
        {
            switch (type)
            {
                case Crc16Type.ARINC:
                    return 0xD405;
                case Crc16Type.CCITT:
                    return 0x8404;
                case Crc16Type.CDMA2000:
                    return 0xE613;
                case Crc16Type.Chakravarty:
                    return 0xA8F4;
                case Crc16Type.DECT:
                    return 0x91A0;
                case Crc16Type.DNP:
                    return 0xA6BC;
                case Crc16Type.IBM:
                    return 0xA001;
                case Crc16Type.OpenSafety_A:
                    return 0xAC9A;
                case Crc16Type.OpenSafety_B:
                    return 0xDAAE;
                case Crc16Type.Profibus:
                    return 0xF3B8;
                case Crc16Type.T10:
                    return 0xEDD1;
                default:
                    throw new ArgumentOutOfRangeException("type", "The type parameter must be a valid Crc16Type.");
            }
        }

        public ushort Polynomial
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

        public ushort Seed
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
            switch(_endianness)
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

        private ushort FlipPolynomial(ushort polynomial)
        {
            ushort returnPolynomial = 0;

            for(int i = 0; i < 16; i++)
            {
                if ((polynomial & (1 << i)) == (1 << i))
                    returnPolynomial += (ushort)(1 << (15 - i));
            }

            return returnPolynomial;
        }

        public ushort Crc { get; private set; }

        public long ByteCount { get; private set; }

        public void Reset()
        {
            Crc = Seed;
            ByteCount = 0;
            GenerateLookupTable();
        }

        private void GenerateLookupTable()
        {
            _lookup = new ushort[256];

            for (int i = 0; i < 256; i++)
            {
                switch (Endianness)
                {
                    case Endianness.Big:
                        {
                            _lookup[i] = (ushort)(i << 8);

                            for (int j = 0; j < 8; j++)
                            {
                                if ((_lookup[i] & 0x8000) == 0x8000)
                                    _lookup[i] = (ushort)(((_lookup[i] << 1)) ^ Polynomial);
                                else
                                    _lookup[i] <<= 1;
                            }
                        }
                        break;
                    case Endianness.Little:
                        {
                            _lookup[i] = (ushort)i;

                            for (int j = 0; j < 8; j++)
                            {
                                if ((_lookup[i] & 1) == 1)
                                    _lookup[i] = (ushort)((_lookup[i] >> 1) ^ Polynomial);
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
                        Crc = (ushort)((Crc << 8) ^ _lookup[(Crc >> 8) ^ data]);
                    }
                    break;
                case Endianness.Little:
                    {
                        Crc = (ushort)((Crc >> 8) ^ _lookup[(Crc ^ data) & 0x00FF]);
                    }
                    break;
            }
            ByteCount++;
        }

        public void AddBytes(byte[] data)
        {
            foreach(byte b in data)
            {
                AddByte(b);
            }
        }

        public override string ToString()
        {
            return Crc.ToString("x4");
        }
    }
}

using System;

namespace Softek.CRC
{
    public class Crc8
    {
        private byte[] _lookup;
        private byte _polynomial = 0xE0; // CCITT 8-bit polynomial
        private Endianness _endianness = Endianness.Little;
        private byte _seed = 0;

        public Crc8()
        {
            Reset();
        }

        public Crc8(byte polynomial)
        {
            _polynomial = polynomial;

            Reset();
        }

        public Crc8(byte polynomial, Endianness endianness)
        {
            _polynomial = polynomial;
            _endianness = endianness;

            Reset();
        }

        public Crc8(byte polynomial, Endianness endianness, byte seed)
        {
            _polynomial = polynomial;
            _endianness = endianness;
            _seed = seed;

            Reset();
        }

        public Crc8(Crc8Type type)
        {
            _polynomial = GetPolynomialByType(type);

            Reset();
        }

        public Crc8(Crc8Type type, Endianness endianness)
        {
            _polynomial = GetPolynomialByType(type);

            if (endianness == Endianness.Big)
            {
                _polynomial = FlipPolynomial(_polynomial);
                _endianness = endianness;
            }

            Reset();
        }

        public Crc8(Crc8Type type, Endianness endianness, byte seed)
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

        private byte GetPolynomialByType(Crc8Type type)
        {
            switch (type)
            {
                case Crc8Type.AUTOSAR:
                    return 0xF4;
                case Crc8Type.Bluetooth:
                    return 0xE5;
                case Crc8Type.CCITT:
                    return 0xE0;
                case Crc8Type.Dallas:
                    return 0x8C;
                case Crc8Type.DARC:
                    return 0x9C;
                case Crc8Type.DVB:
                    return 0xAB;
                case Crc8Type.GSM_B:
                    return 0x92;
                case Crc8Type.Maxim:
                    return 0x8C;
                case Crc8Type.SAE_J1850:
                    return 0xB8;
                case Crc8Type.WCDMA:
                    return 0xD9;
                default:
                    throw new ArgumentOutOfRangeException("type", "The type parameter must be a valid Crc8Type.");
            }
        }

        public byte Polynomial
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

        public byte Seed
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

        private byte FlipPolynomial(byte polynomial)
        {
            byte returnPolynomial = 0;

            for (int i = 0; i < 8; i++)
            {
                if ((polynomial & (1 << i)) == (1 << i))
                    returnPolynomial += (byte)(1 << (7 - i));
            }

            return returnPolynomial;
        }

        public byte Crc { get; private set; }

        public long ByteCount { get; private set; }

        public void Reset()
        {
            Crc = Seed;
            ByteCount = 0;
            GenerateLookupTable();
        }

        private void GenerateLookupTable()
        {
            _lookup = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                switch (Endianness)
                {
                    case Endianness.Big:
                        {
                            _lookup[i] = (byte)i;

                            for (int j = 0; j < 8; j++)
                            {
                                if ((_lookup[i] & 0x80) == 0x80)
                                    _lookup[i] = (byte)(((_lookup[i] << 1)) ^ Polynomial);
                                else
                                    _lookup[i] <<= 1;
                            }
                        }
                        break;
                    case Endianness.Little:
                        {
                            _lookup[i] = (byte)i;

                            for (int j = 0; j < 8; j++)
                            {
                                if ((_lookup[i] & 1) == 1)
                                    _lookup[i] = (byte)((_lookup[i] >> 1) ^ Polynomial);
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
            Crc = _lookup[Crc ^ data];
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
            return Crc.ToString("x2");
        }
    }
}

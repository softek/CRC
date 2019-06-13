using System;
using System.Collections.Generic;
using System.Text;

namespace Softek.CRC
{
    public enum Endianness
    {
        Big,
        Little
    }

    public enum Crc8Type
    {
        AUTOSAR,
        Bluetooth,
        CCITT,
        Dallas,
        DARC,
        DVB,
        GSM_B,
        Maxim,
        SAE_J1850,
        WCDMA
    }

    public enum Crc16Type
    {
        ARINC,
        CCITT,
        CDMA2000,
        Chakravarty,
        DECT,
        DNP,
        IBM,
        OpenSafety_A,
        OpenSafety_B,
        Profibus,
        T10
    }

    public enum Crc32Type
    {
        Castagnoli,
        ISO_3309,
        Koopman,
        Koopman_2,
        Q
    }

}

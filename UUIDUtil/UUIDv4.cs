using System;

namespace TensionDev.UUID
{
    /// <summary>
    /// Class Library to generate Universally Unique Identifier (UUID) / Globally Unique Identifier (GUID) based on Version 4 (random).
    /// </summary>
    public class UUIDv4
    {
        /// <summary>
        /// Initialises a new GUID/UUID based on Version 4 (random)
        /// </summary>
        /// <returns>A new Uuid object</returns>
        public static Uuid NewUUIDv4()
        {
            Byte[] time = new Byte[8];
            Byte[] clockSequence = new Byte[2];
            Byte[] nodeID = new Byte[6];

            using (System.Security.Cryptography.RNGCryptoServiceProvider cryptoServiceProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetBytes(time);
                cryptoServiceProvider.GetBytes(clockSequence);
                cryptoServiceProvider.GetBytes(nodeID);
            }

            Byte[] hex = new Byte[16];

            hex[0] = time[4];
            hex[1] = time[5];
            hex[2] = time[6];
            hex[3] = time[7];

            hex[4] = time[2];
            hex[5] = time[3];

            hex[6] = (Byte)((time[0] & 0x0F) + 0x40);
            hex[7] = time[1];

            hex[8] = (Byte)((clockSequence[0] & 0x3F) + 0x80);
            hex[9] = clockSequence[1];

            hex[10] = nodeID[0];
            hex[11] = nodeID[1];
            hex[12] = nodeID[2];
            hex[13] = nodeID[3];
            hex[14] = nodeID[4];
            hex[15] = nodeID[5];

            Uuid Id = new Uuid(hex);

            return Id;
        }
    }
}

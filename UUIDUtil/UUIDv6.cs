// SPDX-License-Identifier: Apache-2.0
//
//   Copyright 2021 TensionDev <TensionDev@outlook.com>
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;

namespace TensionDev.UUID
{
    /// <summary>
    /// Class Library to generate Universally Unique Identifier (UUID) / Globally Unique Identifier (GUID) based on Version 6 (date-time).
    /// </summary>
    public static class UUIDv6
    {
        private static Int32 s_clock = Int32.MinValue;
        private static readonly DateTime s_epoch = new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Object s_initLock = new Object();
        private static readonly Object s_clockLock = new Object();

        /// <summary>
        /// Initialises a new GUID/UUID based on Version 6 (date-time)
        /// </summary>
        /// <returns>A new Uuid object</returns>
        public static Uuid NewUUIDv6()
        {
            return NewUUIDv6(DateTime.UtcNow);
        }

        /// <summary>
        /// Initialises a new GUID/UUID based on Version 6 (date-time), based on the given date and time.
        /// </summary>
        /// <param name="dateTime">Given Date and Time</param>
        /// <returns>A new Uuid object</returns>
        public static Uuid NewUUIDv6(DateTime dateTime)
        {
            return NewUUIDv6(dateTime, GetClockSequence(), GetNodeID());
        }

        /// <summary>
        /// Initialises a new GUID/UUID based on Version 6 (date-time), based on the given Node ID.
        /// </summary>
        /// <param name="nodeID">Given 48-bit Node ID</param>
        /// <returns>A new Uuid object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Uuid NewUUIDv6(Byte[] nodeID)
        {
            return NewUUIDv6(DateTime.UtcNow, GetClockSequence(), nodeID);
        }

        /// <summary>
        /// Initialises a new GUID/UUID based on Version 6 (date-time), based on the given date and time, Clock Sequence with Variant and Node ID.
        /// </summary>
        /// <param name="dateTime">Given Date and Time</param>
        /// <param name="clockSequence">Given 16-bit Clock Sequence with Variant</param>
        /// <param name="nodeID">Given 48-bit Node ID</param>
        /// <returns>A new Uuid object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Uuid NewUUIDv6(DateTime dateTime, Byte[] clockSequence, Byte[] nodeID)
        {
            if (clockSequence == null)
                throw new ArgumentNullException(nameof(clockSequence));

            if (clockSequence.Length < 2)
                throw new ArgumentException(String.Format("Clock Sequence contains less than 16-bit: {0} bytes", clockSequence.Length), nameof(clockSequence));

            if (nodeID == null)
                throw new ArgumentNullException(nameof(nodeID));

            if (nodeID.Length < 6)
                throw new ArgumentException(String.Format("Node ID contains less than 48-bit: {0} bytes", nodeID.Length), nameof(nodeID));

            TimeSpan timeSince = dateTime.ToUniversalTime() - s_epoch.ToUniversalTime();
            Int64 timeInterval = timeSince.Ticks << 4;

            Byte[] time = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(timeInterval));

            Byte[] hex = new Byte[16];

            hex[0] = time[0];
            hex[1] = time[1];
            hex[2] = time[2];
            hex[3] = time[3];

            hex[4] = time[4];
            hex[5] = time[5];

            hex[6] = (Byte)(((time[6] >> 4) & 0x0F) + 0x60);
            hex[7] = (Byte)((time[6] << 4) + (time[7] >> 4));

            hex[8] = clockSequence[0];
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

        /// <summary>
        /// Initialises the 48-bit Node ID and returns it.<br />
        /// Returns a randomly genrated 48-bit Node ID.
        /// </summary>
        /// <returns>A byte-array representing the 48-bit Node ID</returns>
        public static Byte[] GetNodeID()
        {
            using (System.Security.Cryptography.RNGCryptoServiceProvider cryptoServiceProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                Byte[] fakeNode = new Byte[6];
                cryptoServiceProvider.GetBytes(fakeNode);
                return fakeNode;
            }
        }

        /// <summary>
        /// Intialises the 14-bit Clock Sequence and returns the current value with the Variant.<br />
        /// Will return an incremented Clock Sequence on each call, modulo 14-bit.
        /// </summary>
        /// <returns>A byte-array representing the 14-bit Clock Sequence, together with the Variant</returns>
        public static Byte[] GetClockSequence()
        {
            lock (s_initLock)
            {
                if (s_clock < 0)
                {
                    using (System.Security.Cryptography.RNGCryptoServiceProvider cryptoServiceProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
                    {
                        Byte[] clockInit = new Byte[4];
                        cryptoServiceProvider.GetBytes(clockInit);
                        s_clock = BitConverter.ToInt32(clockInit, 0) & 0x3FFF;
                        s_clock |= 0x8000;
                    }
                }
            }

            Int32 result;
            lock (s_clockLock)
            {
                result = s_clock++;
                if (s_clock >= 0xC000)
                    s_clock = 0x8000;
            }

            return BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((Int16)result));
        }
    }
}

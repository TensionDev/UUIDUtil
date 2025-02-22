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
using System.Text;
using System.Text.RegularExpressions;

namespace TensionDev.UUID
{
    public sealed class Uuid : IComparable<Uuid>, IEquatable<Uuid>
    {
        private const string INVALID_FORMAT_STRING = "The format of s is invalid";
        private const string HEX_FORMAT_STRING = "{0:x2}";
        private readonly uint _time_low;
        private readonly ushort _time_mid;
        private readonly ushort _time_hi_and_version;
        private readonly byte _clock_seq_hi_and_reserved;
        private readonly byte _clock_seq_low;
        private readonly byte[] _node;

        /// <summary>
        /// A read-only instance of the Uuid object whose value is all zeros.
        /// </summary>
        public static readonly Uuid Empty = new Uuid();

        /// <summary>
        /// A read-only instance of the Uuid object whose value is all ones.
        /// </summary>
        public static readonly Uuid Max = new Uuid(uint.MaxValue, ushort.MaxValue, ushort.MaxValue, byte.MaxValue, byte.MaxValue, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });

        public Uuid()
        {
            _time_low = 0;
            _time_mid = 0;
            _time_hi_and_version = 0;
            _clock_seq_hi_and_reserved = 0;
            _clock_seq_low = 0;
            _node = new byte[6];
        }

        /// <summary>
        /// Initializes a new instance of the Uuid object by using the specified array of bytes.
        /// </summary>
        /// <param name="b">A 16-element byte array containing values with which to initialize the Uuid.</param>
        /// <exception cref="System.ArgumentNullException">b is null.</exception>
        /// <exception cref="System.ArgumentException">b is not 16 bytes long.</exception>
        public Uuid(byte[] b) : this()
        {
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (b.Length != 16)
                throw new ArgumentException("b is not 16 bytes long.", nameof(b));

            _time_low = BitConverter.ToUInt32(b, 0);
            _time_mid = BitConverter.ToUInt16(b, 4);
            _time_hi_and_version = BitConverter.ToUInt16(b, 6);
            _clock_seq_hi_and_reserved = b[8];
            _clock_seq_low = b[9];
            Array.Copy(b, 10, _node, 0, _node.Length);
        }

        /// <summary>
        /// Initializes a new instance of the Uuid object by using the value represented by the specified string.
        /// </summary>
        /// <param name="s">A string that contains a Uuid in one of the following formats ("d" represents a hexadecimal digit whose case is ignored)
        /// <para>32 contiguous hexadecimal digits:<br />dddddddddddddddddddddddddddddddd</para>
        /// <para>Groups of 8, 4, 4, 4, and 12 digits with hyphens between the groups.<br />The entire Uuid can optionally be enclosed in matching braces or parentheses:</para>
        /// <para>dddddddd-dddd-dddd-dddd-dddddddddddd<br />{dddddddd-dddd-dddd-dddd-dddddddddddd}<br />(dddddddd-dddd-dddd-dddd-dddddddddddd)</para>
        /// </param>
        /// <exception cref="System.ArgumentNullException">s is null</exception>
        /// <exception cref="System.FormatException">The format of s is invalid</exception>
        /// <exception cref="System.OverflowException">The format of s is invalid</exception>
        public Uuid(string s) : this()
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException(nameof(s), INVALID_FORMAT_STRING);

            if (s.Length != 32 && s.Length != 36 && s.Length != 38)
                throw new FormatException(INVALID_FORMAT_STRING);

            if (s.Length == 38 &&
                !(s.StartsWith("{") && s.EndsWith("}")) &&
                !(s.StartsWith("(") && s.EndsWith(")")))
            {
                throw new FormatException(INVALID_FORMAT_STRING);
            }

            string vs = s.Replace("{", "");
            vs = vs.Replace("}", "");
            vs = vs.Replace("(", "");
            vs = vs.Replace(")", "");

            if (vs.Length == 36)
            {
                Regex regex = new Regex(@"\b[0-9a-f]{8}\b-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-\b[0-9a-f]{12}\b", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
                MatchCollection matches = regex.Matches(vs);
                if (matches.Count == 0)
                    throw new FormatException(INVALID_FORMAT_STRING);
            }
            vs = vs.Replace("-", "");

            if (vs.Length != 32)
                throw new FormatException(INVALID_FORMAT_STRING);

            Byte[] b = new Byte[16];
            for (Int32 i = 0; i < vs.Length; i += 2)
            {
                try
                {
                    b[i / 2] = Byte.Parse(vs.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch (FormatException)
                {
                    throw new FormatException(INVALID_FORMAT_STRING);
                }
                catch (OverflowException)
                {
                    throw new OverflowException(INVALID_FORMAT_STRING);
                }
            }

            _time_low = BitConverter.ToUInt32(b, 0);
            _time_mid = BitConverter.ToUInt16(b, 4);
            _time_hi_and_version = BitConverter.ToUInt16(b, 6);
            _clock_seq_hi_and_reserved = b[8];
            _clock_seq_low = b[9];
            Array.Copy(b, 10, _node, 0, _node.Length);
        }

        /// <summary>
        /// Initializes a new instance of the Uuid object by using the specified integers and byte array.
        /// </summary>
        /// <param name="a">The first 4 bytes of the Uuid.</param>
        /// <param name="b">The next 2 bytes of the Uuid.</param>
        /// <param name="c">The next 2 bytes of the Uuid.</param>
        /// <param name="d">The next byte of the Uuid.</param>
        /// <param name="e">The next byte of the Uuid.</param>
        /// <param name="f">The next 6 bytes of the Uuid.</param>
        /// <exception cref="System.ArgumentNullException">f is null</exception>
        /// <exception cref="System.ArgumentException">f is not 6 bytes long</exception>
        public Uuid(uint a, ushort b, ushort c, byte d, byte e, byte[] f) : this()
        {
            if (f == null)
                throw new ArgumentNullException(nameof(f));

            if (f.Length != 6)
                throw new ArgumentException("f is not 6 bytes long");

            _time_low = (uint)System.Net.IPAddress.HostToNetworkOrder((int)a);
            _time_mid = (ushort)System.Net.IPAddress.HostToNetworkOrder((Int16)b);
            _time_hi_and_version = (ushort)System.Net.IPAddress.HostToNetworkOrder((Int16)c);
            _clock_seq_hi_and_reserved = d;
            _clock_seq_low = e;
            f.CopyTo(_node, 0);
        }

        /// <summary>
        /// Initializes a new instance of the Uuid object by using the specified integers and bytes.
        /// </summary>
        /// <param name="a">The first 4 bytes of the Uuid.</param>
        /// <param name="b">The next 2 bytes of the Uuid.</param>
        /// <param name="c">The next 2 bytes of the Uuid.</param>
        /// <param name="d">The next byte of the Uuid.</param>
        /// <param name="e">The next byte of the Uuid.</param>
        /// <param name="f">The next byte of the Uuid.</param>
        /// <param name="g">The next byte of the Uuid.</param>
        /// <param name="h">The next byte of the Uuid.</param>
        /// <param name="i">The next byte of the Uuid.</param>
        /// <param name="j">The next byte of the Uuid.</param>
        /// <param name="k">The next byte of the Uuid.</param>
        public Uuid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k) : this()
        {
            _time_low = (uint)System.Net.IPAddress.HostToNetworkOrder((int)a);
            _time_mid = (ushort)System.Net.IPAddress.HostToNetworkOrder((Int16)b);
            _time_hi_and_version = (ushort)System.Net.IPAddress.HostToNetworkOrder((Int16)c);
            _clock_seq_hi_and_reserved = d;
            _clock_seq_low = e;
            _node[0] = f;
            _node[1] = g;
            _node[2] = h;
            _node[3] = i;
            _node[4] = j;
            _node[5] = k;
        }

        //
        // Summary:
        //     Initializes a new instance of the Uuid object.
        //
        // Returns:
        //     A new Uuid object.
        /// <summary>
        /// Initializes a new instance of the Uuid object.
        /// </summary>
        /// <param name="input">A universally unique identifier in the proper format.</param>
        /// <returns>A new Uuid object.</returns>
        public static Uuid Parse(string input)
        {
            return new Uuid(input);
        }

        /// <summary>
        /// Converts the string representation of a Uuid to the equivalent Uuid object.
        /// </summary>
        /// <param name="input">The Uuid to convert.</param>
        /// <param name="result">The object that will contain the parsed value. If the method returns true, result contains a valid Uuid.
        /// If the method returns false, result equals Uuid.Empty.</param>
        /// <returns>true if the parse operation was successful; otherwise, false.</returns>
        public static bool TryParse(string input, out Uuid result)
        {
            bool vs = false;
            result = new Uuid();

            try
            {
                result = Parse(input);
                vs = true;
            }
            catch (Exception)
            {
                // Quietly suppress exception on Parse.
            }

            return vs;
        }

        public int CompareTo(object other)
        {
            if (other is Uuid u)
            {
                return CompareTo(u);
            }
            else
            {
                return 1;
            }
        }

        public int CompareTo(Uuid other)
        {
            if (other is null)
                return 1;

            if (_time_low.CompareTo(other._time_low) != 0)
                return _time_low.CompareTo(other._time_low);

            if (_time_mid.CompareTo(other._time_mid) != 0)
                return _time_mid.CompareTo(other._time_mid);

            if (_time_hi_and_version.CompareTo(other._time_hi_and_version) != 0)
                return _time_hi_and_version.CompareTo(other._time_hi_and_version);

            if (_clock_seq_hi_and_reserved.CompareTo(other._clock_seq_hi_and_reserved) != 0)
                return _clock_seq_hi_and_reserved.CompareTo(other._clock_seq_hi_and_reserved);

            if (_clock_seq_low.CompareTo(other._clock_seq_low) != 0)
                return _clock_seq_low.CompareTo(other._clock_seq_low);

            for (Int32 i = 0; i < _node.Length; i++)
            {
                if (_node[i].CompareTo(other._node[i]) != 0)
                    return _node[i].CompareTo(other._node[i]);
            }

            return 0;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified Uuid object represent the same value.
        /// </summary>
        /// <param name="other">An object to compare to this instance.</param>
        /// <returns>true if other is equal to this instance; otherwise, false.</returns>
        public bool Equals(Uuid other)
        {
            if (other is null)
                return false;

            int result = CompareTo(other);
            return result == 0;

        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>true if o is a Uuid that has the same value as this instance; otherwise,false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Uuid other)
            {
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return (_time_low, _time_mid, _time_hi_and_version, _clock_seq_hi_and_reserved, _clock_seq_low, _node).GetHashCode();
        }

        /// <summary>
        /// Returns a 16-element byte array that contains the value of this instance.
        /// </summary>
        /// <returns>A 16-element byte array.</returns>
        public byte[] ToByteArray()
        {
            Byte[] vs = new Byte[16];

            BitConverter.GetBytes(_time_low).CopyTo(vs, 0);
            BitConverter.GetBytes(_time_mid).CopyTo(vs, 4);
            BitConverter.GetBytes(_time_hi_and_version).CopyTo(vs, 6);
            BitConverter.GetBytes(_clock_seq_hi_and_reserved).CopyTo(vs, 8);
            BitConverter.GetBytes(_clock_seq_low).CopyTo(vs, 9);
            _node.CopyTo(vs, 10);

            return vs;
        }

        /// <summary>
        /// Returns the System.Guid equivalent of this instance.
        /// </summary>
        /// <returns>A System.Guid object.</returns>
        public Guid ToGuid()
        {
            return new Guid(ToString());
        }

        /// <summary>
        /// Return the Variant 2 version in System.Guid.
        /// </summary>
        /// <returns>A System.Guid object.</returns>
        public Guid ToVariant2()
        {
            byte newClockSeq = (byte)(_clock_seq_hi_and_reserved & 0x1F);
            newClockSeq = (byte)(newClockSeq | 0xC0);
            byte[] array = ToByteArray();
            array[8] = newClockSeq;
            Uuid variant2 = new Uuid(array);

            return variant2.ToGuid();
        }

        /// <summary>
        /// Returns the Variant 1 version in TensionDev.UUID.Uuid.
        /// </summary>
        /// <param name="guid">The System.Guid object to convert.</param>
        /// <returns>A TensionDev.UUID.Uuid object.</returns>
        public static Uuid ToVariant1(Guid guid)
        {
            Uuid variant2 = new Uuid(guid.ToString());
            byte newClockSeq = (byte)(variant2._clock_seq_hi_and_reserved & 0x3F);
            newClockSeq = (byte)(newClockSeq | 0x80);
            byte[] array = variant2.ToByteArray();
            array[8] = newClockSeq;
            Uuid variant1 = new Uuid(array);

            return variant1;
        }

        /// <summary>
        /// Returns a string representation of the value of this instance as per RFC 4122 Section 3.
        /// </summary>
        /// <returns>The value of this Uuid, formatted by using the "D" format specifier as follows:
        /// xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx where the value of the Uuid is represented as a
        /// series of lowercase hexadecimal digits in groups of 8, 4, 4, 4, and 12 digits and separated by hyphens.
        /// An example of a return value is "7d444840-9dc0-11d1-b245-5ffdce74fad2".</returns>
        public override string ToString()
        {
            return ToString("D");
        }

        /// <summary>
        /// Returns a string representation of the value of this Uuid instance, according to the provided format specifier.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of this Uuid. The format parameter can be "N", "D", "B" or "P". If format is null or an empty string (""), "D" is used.</param>
        /// <returns>The value of this Uuid, represented as a series of lowercase hexadecimal digits in the specified format.</returns>
        /// <exception cref="System.FormatException">The value of format is not null, an empty string (""), "N", "D", "B" or "P"</exception>
        public string ToString(string format)
        {
            if (String.IsNullOrEmpty(format))
            {
                return ToStringCannonical();
            }

            switch (format)
            {
                case "N":
                    return ToHexString();

                case "D":
                    return ToStringCannonical();

                case "B":
                    return ToStringBraces();

                case "P":
                    return ToStringParenthesis();

                default:
                    throw new FormatException("The value of format is not null, an empty string(\"\"), \"N\", \"D\", \"B\" or \"P\"");
            }
        }

        private string ToStringCannonical()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(BitConverter.ToString(BitConverter.GetBytes(_time_low)).Replace("-", ""));
            sb.Append("-");
            sb.Append(BitConverter.ToString(BitConverter.GetBytes(_time_mid)).Replace("-", ""));
            sb.Append("-");
            sb.Append(BitConverter.ToString(BitConverter.GetBytes(_time_hi_and_version)).Replace("-", ""));
            sb.Append("-");
            sb.AppendFormat(HEX_FORMAT_STRING, _clock_seq_hi_and_reserved);
            sb.AppendFormat(HEX_FORMAT_STRING, _clock_seq_low);
            sb.Append("-");
            sb.Append(BitConverter.ToString(_node).Replace("-", ""));

            return sb.ToString().ToLower();
        }

        private string ToHexString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(BitConverter.ToString(BitConverter.GetBytes(_time_low)).Replace("-", ""));
            sb.Append(BitConverter.ToString(BitConverter.GetBytes(_time_mid)).Replace("-", ""));
            sb.Append(BitConverter.ToString(BitConverter.GetBytes(_time_hi_and_version)).Replace("-", ""));
            sb.AppendFormat(HEX_FORMAT_STRING, _clock_seq_hi_and_reserved);
            sb.AppendFormat(HEX_FORMAT_STRING, _clock_seq_low);
            sb.Append(BitConverter.ToString(_node).Replace("-", ""));

            return sb.ToString().ToLower();
        }

        private string ToStringParenthesis()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");
            sb.Append(ToStringCannonical());
            sb.Append(")");

            return sb.ToString();
        }

        private string ToStringBraces()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            sb.Append(ToStringCannonical());
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Indicates whether the values of two specified Uuid objects are equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>true if a and b are equal; otherwise, false.</returns>
        public static bool operator ==(Uuid a, Uuid b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Indicates whether the values of two specified Uuid objects are not equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>true if a and b are not equal; otherwise, false.</returns>
        public static bool operator !=(Uuid a, Uuid b)
        {
            return !(a == b);
        }

        public static bool operator <(Uuid a, Uuid b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Uuid a, Uuid b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(Uuid a, Uuid b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Uuid a, Uuid b)
        {
            return a.CompareTo(b) >= 0;
        }
    }
}

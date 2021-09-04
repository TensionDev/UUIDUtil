using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProjectUUID
{
    public class UnitTestUUIDv1
    {
        [Fact]
        public void TestGetNodeID()
        {
            IList<byte[]> expectedNodeIDs = new List<byte[]>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                expectedNodeIDs.Add(nic.GetPhysicalAddress().GetAddressBytes());
            }

            byte[] nodeID = UUIDUtil.UUIDv1.GetNodeID();

            if (nics.Length > 0)
                Assert.Contains(nodeID, expectedNodeIDs);
            else
                Assert.True(nodeID.Length == 6);
        }

        [Fact]
        public void TestConsistentGetNodeID()
        {
            byte[] nodeID1 = UUIDUtil.UUIDv1.GetNodeID();
            byte[] nodeID2 = UUIDUtil.UUIDv1.GetNodeID();

            Assert.Equal(nodeID1, nodeID2);
        }

        [Fact]
        public void TestNewUUIDv1()
        {
            Guid expectedUUID = new Guid("164a714c-0c79-11ec-82a8-0242ac130003");

            byte[] nodeID = new byte[] { 0x02, 0x42, 0xac, 0x13, 0x00, 0x03 };
            byte[] clockSequence = new byte[] { 0x82, 0xa8 };
            DateTime dateTime = DateTime.Parse("2021-09-03T05:37:54.619630Z");
            Guid uuid = UUIDUtil.UUIDv1.NewUUIDv1(dateTime, clockSequence, nodeID);

            Assert.Equal(expectedUUID, uuid);
        }

        [Fact]
        public void TestGetClockSequence()
        {
            ConcurrentDictionary<UInt16, Boolean> concurrentDictionary = new ConcurrentDictionary<UInt16, Boolean>();
            Int32 expectedMaxSequence = 0x4000;

            Parallel.For(0, UInt16.MaxValue,
                clock =>
                {
                    Byte[] vs = UUIDUtil.UUIDv1.GetClockSequence();
                    UInt16 key = BitConverter.ToUInt16(vs);
                    concurrentDictionary.TryAdd(key, true);
                });

            Assert.Equal(expectedMaxSequence, concurrentDictionary.Values.Count);
            ICollection<UInt16> keys = concurrentDictionary.Keys;
            foreach (UInt16 key in keys)
            {
                Assert.InRange(key, 0x8000, 0xBFFF);
            }
        }

        [Fact]
        public void TestNewUUIDv1NullClockSequence()
        {
            byte[] nodeID = new byte[] { 0x02, 0x42, 0xac, 0x13, 0x00, 0x03 };
            byte[] clockSequence = null;
            Assert.Throws<ArgumentNullException>(() => UUIDUtil.UUIDv1.NewUUIDv1(DateTime.UtcNow, clockSequence, nodeID));
        }

        [Fact]
        public void TestNewUUIDv1ReducedClockSequence()
        {
            byte[] nodeID = new byte[] { 0x02, 0x42, 0xac, 0x13, 0x00, 0x03 };
            byte[] clockSequence = new byte[] { 0x82 };
            Assert.Throws<ArgumentException>(() => UUIDUtil.UUIDv1.NewUUIDv1(DateTime.UtcNow, clockSequence, nodeID));
        }

        [Fact]
        public void TestNewUUIDv1NullNodeID()
        {
            byte[] nodeID = null;
            byte[] clockSequence = new byte[] { 0x82, 0xa8 };
            Assert.Throws<ArgumentNullException>(() => UUIDUtil.UUIDv1.NewUUIDv1(DateTime.UtcNow, clockSequence, nodeID));
        }

        [Fact]
        public void TestNewUUIDv1ReducedNodeID()
        {
            byte[] nodeID = new byte[] { 0x02, 0x42, 0xac, 0x13 };
            byte[] clockSequence = new byte[] { 0x82, 0xa8 };
            Assert.Throws<ArgumentException>(() => UUIDUtil.UUIDv1.NewUUIDv1(DateTime.UtcNow, clockSequence, nodeID));
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProjectUUID
{
    public class UnitTestUUIDv4
    {
        [Fact]
        public void TestNewUUIDv4()
        {
            char expectedVersionField = '4';

            ConcurrentBag<String> concurrentBag = new ConcurrentBag<String>();

            Parallel.For(0, UInt16.MaxValue,
                body =>
                {
                    concurrentBag.Add(TensionDev.UUID.UUIDv4.NewUUIDv4().ToString());
                });

            foreach (String value in concurrentBag)
            {
                Assert.Equal(value[14], expectedVersionField);
            }
        }

        [Fact]
        public void TestUUIDVariantField()
        {
            IList<char> expectedVariantField = new List<char>() { '8', '9', 'a', 'b' };

            ConcurrentBag<String> concurrentBag = new ConcurrentBag<String>();

            Parallel.For(0, UInt16.MaxValue,
                body =>
                {
                    concurrentBag.Add(TensionDev.UUID.UUIDv4.NewUUIDv4().ToString());
                });

            foreach (String value in concurrentBag)
            {
                Assert.Contains<char>(value[19], expectedVariantField);
            }
        }

        [Fact]
        public void TestIsUUIDv4Withv1()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv1.NewUUIDv1();

            bool actual = TensionDev.UUID.UUIDv4.IsUUIDv4(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv4Withv3()
        {
            String name = "www.google.com";
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv3.NewUUIDv3(TensionDev.UUID.UUIDNamespace.DNS, name);

            bool actual = TensionDev.UUID.UUIDv4.IsUUIDv4(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv4Withv4()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv4.NewUUIDv4();

            bool actual = TensionDev.UUID.UUIDv4.IsUUIDv4(uuid);
            Assert.True(actual);
        }

        [Fact]
        public void TestIsUUIDv4Withv5()
        {
            String name = "www.contoso.com";
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv5.NewUUIDv5(TensionDev.UUID.UUIDNamespace.DNS, name);

            bool actual = TensionDev.UUID.UUIDv4.IsUUIDv4(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv4Withv6()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv6.NewUUIDv6();

            bool actual = TensionDev.UUID.UUIDv4.IsUUIDv4(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv4Withv7()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv7.NewUUIDv7();

            bool actual = TensionDev.UUID.UUIDv4.IsUUIDv4(uuid);
            Assert.False(actual);
        }
    }
}

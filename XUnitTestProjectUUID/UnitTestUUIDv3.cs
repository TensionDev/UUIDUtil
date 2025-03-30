using System;
using Xunit;

namespace XUnitTestProjectUUID
{
    public class UnitTestUUIDv3
    {
        [Fact]
        public void TestNewUUIDv3_DNS()
        {
            TensionDev.UUID.Uuid expectedGuid = new TensionDev.UUID.Uuid("de87628d-5377-3ba7-b31b-cde1cc8d423f");

            String name = "www.google.com";
            TensionDev.UUID.Uuid guid = TensionDev.UUID.UUIDv3.NewUUIDv3(TensionDev.UUID.UUIDNamespace.DNS, name);

            Assert.Equal(expectedGuid, guid);
        }

        [Fact]
        public void TestNewUUIDv3_URL()
        {
            TensionDev.UUID.Uuid expectedGuid = new TensionDev.UUID.Uuid("d39a36cc-b262-3c67-a6ca-0168e948bdd4");

            String name = "https://www.google.com";
            TensionDev.UUID.Uuid guid = TensionDev.UUID.UUIDv3.NewUUIDv3(TensionDev.UUID.UUIDNamespace.URL, name);

            Assert.Equal(expectedGuid, guid);
        }

        [Fact]
        public void TestNewUUIDv3_OID()
        {
            TensionDev.UUID.Uuid expectedGuid = new TensionDev.UUID.Uuid("ef4dc0a0-9fc8-368e-9413-0bbf811aca7b");

            String name = "1.0.3166.1";
            TensionDev.UUID.Uuid guid = TensionDev.UUID.UUIDv3.NewUUIDv3(TensionDev.UUID.UUIDNamespace.OID, name);

            Assert.Equal(expectedGuid, guid);
        }

        [Fact]
        public void TestNewUUIDv3_X500()
        {
            TensionDev.UUID.Uuid expectedGuid = new TensionDev.UUID.Uuid("87d4875f-af5a-3491-8c26-cef5a0d16aa0");

            String name = "/c=us/o=Sun/ou=People/cn=Rosanna Lee";
            TensionDev.UUID.Uuid guid = TensionDev.UUID.UUIDv3.NewUUIDv3(TensionDev.UUID.UUIDNamespace.X500, name);

            Assert.Equal(expectedGuid, guid);
        }

        [Fact]
        public void TestIsUUIDv3Withv1()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv1.NewUUIDv1();

            bool actual = TensionDev.UUID.UUIDv3.IsUUIDv3(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv3Withv3()
        {
            String name = "www.google.com";
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv3.NewUUIDv3(TensionDev.UUID.UUIDNamespace.DNS, name);

            bool actual = TensionDev.UUID.UUIDv3.IsUUIDv3(uuid);
            Assert.True(actual);
        }

        [Fact]
        public void TestIsUUIDv3Withv4()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv4.NewUUIDv4();

            bool actual = TensionDev.UUID.UUIDv3.IsUUIDv3(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv3Withv5()
        {
            String name = "www.contoso.com";
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv5.NewUUIDv5(TensionDev.UUID.UUIDNamespace.DNS, name);

            bool actual = TensionDev.UUID.UUIDv3.IsUUIDv3(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv3Withv6()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv6.NewUUIDv6();

            bool actual = TensionDev.UUID.UUIDv3.IsUUIDv3(uuid);
            Assert.False(actual);
        }

        [Fact]
        public void TestIsUUIDv3Withv7()
        {
            TensionDev.UUID.Uuid uuid = TensionDev.UUID.UUIDv7.NewUUIDv7();

            bool actual = TensionDev.UUID.UUIDv3.IsUUIDv3(uuid);
            Assert.False(actual);
        }
    }
}

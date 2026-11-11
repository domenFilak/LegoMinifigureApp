using LegoMinifigureApp.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LegoMinifigureApp.Tests.Security
{
    public class RateLimitKeysTests
    {
        [Fact]
        public void ForAddress_Ipv4_IsTheAddressItself()
        {
            var key = RateLimitKeys.ForAddress(IPAddress.Parse("203.0.113.7"));

            Assert.Equal("203.0.113.7", key);
        }

        [Fact]
        public void ForAddress_Ipv4MappedToIpv6_IsPlainIpv4()
        {
            var key = RateLimitKeys.ForAddress(IPAddress.Parse("::ffff:203.0.113.7"));

            Assert.Equal("203.0.113.7", key);
        }

        [Fact]
        public void ForAddress_Ipv6InSameBlock_GivesSameKey()
        {
            var first = RateLimitKeys.ForAddress(IPAddress.Parse("2001:db8:1234:5678:aaaa:bbbb:cccc:dddd"));
            var second = RateLimitKeys.ForAddress(IPAddress.Parse("2001:db8:1234:5678:1111:2222:3333:4444"));

            Assert.Equal(first, second);
            Assert.Equal("2001:db8:1234:5678::/64", first);
        }

        [Fact]
        public void ForAddress_Ipv6InDifferentBlocks_GivesDifferentKeys()
        {
            var first = RateLimitKeys.ForAddress(IPAddress.Parse("2001:db8:1234:5678::1"));
            var second = RateLimitKeys.ForAddress(IPAddress.Parse("2001:db8:1234:9999::1"));

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void ForAddress_NoAddress_IsUnknown()
        {
            var key = RateLimitKeys.ForAddress(null);

            Assert.Equal("unknown", key);
        }
    }
}

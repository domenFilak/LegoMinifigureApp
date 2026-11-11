using System.Net;
using System.Net.Sockets;

namespace LegoMinifigureApp.Security
{
    //which "client" request belongs to, for rate limiting
    public static class RateLimitKeys
    {
        public static string ForClient(HttpContext httpContext)
        {
            return ForAddress(httpContext.Connection.RemoteIpAddress);
        }

        public static string ForAddress(IPAddress? address)
        {
            if (address is null)
            {
                return "unknown";
            }

            //IPv4 address written in IPv6 form is treated as plain IPv4
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
            }

            //ipv6: one connection usually owns a whole /64 block of addresses, so count per block
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();

                for (var i = 8; i < 16; i++)
                {
                    bytes[i] = 0;
                }

                return new IPAddress(bytes) + "/64";
            }

            return address.ToString();
        }
    }
}

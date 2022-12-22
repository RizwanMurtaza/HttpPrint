using System.Diagnostics;

namespace HttpPrint.Client.Server
{
    public static class NetAclChecker
    {
        public static void AddAddress(string address)
        {
            RemoveAddress(address, Environment.UserDomainName, Environment.UserName);
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }
        public static void RemoveAddress(string address, string domain, string user)
        {
            var args = $@"http delete urlacl url={address} user={domain}\{user}";

            var psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi)?.WaitForExit();
        }

        public static void AddAddress(string address, string domain, string user)
        {
            var args = $@"http add urlacl url={address} user={domain}\{user}";

            var psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi)?.WaitForExit();
        }
    }
}

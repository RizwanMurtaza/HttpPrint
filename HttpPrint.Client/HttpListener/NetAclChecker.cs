using System.Diagnostics;

namespace HttpPrint.Client.HttpListener
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
            string args = string.Format(@"http delete urlacl url={0} user={1}\{2}", address, domain, user);

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi)?.WaitForExit();
        }

        public static void AddAddress(string address, string domain, string user)
        {
            string args = string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user);

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi)?.WaitForExit();
        }
    }
}

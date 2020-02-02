using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
namespace libc.hwid {
    public static class HwId {
        private enum Hardware {
            Motherboard,
            CPUID
        }
        public static string Generate() {
            var res = new[] {
                getInfo(Hardware.CPUID),
                getInfo(Hardware.Motherboard)
            };
            var input = string.Join("\n", res);
            var result = hash(input);
            return result;
        }
        private static string hash(string input) {
            using (var sha1 = new SHA1Managed()) {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }
        private static string wmi(string wmiClass, string wmiProperty) {
            var result = "";
            var mc = new ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (var o in moc) {
                var mo = (ManagementObject) o;
                //Only get the first one
                if (result == "")
                    try {
                        result = mo[wmiProperty].ToString();
                        break;
                    } catch {
                        // ignored
                    }
            }
            return result;
        }
        private static string dmidecode(string query, string find) {
            var cmd = new Cmd();
            var k = cmd.Run("/usr/bin/sudo", $" {query}", new CmdOptions {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStdOut = true,
                UseOSShell = false
            }, true);
            find = find.EndsWith(":") ? find : $"{find}:";
            var lines = k.Output.Split(new[] {
                    Environment.NewLine
                }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim(' ', '\t'));
            var line = lines.First(a => a.StartsWith(find));
            var res = line.Substring(line.IndexOf(find, StringComparison.Ordinal) + find.Length).Trim(' ', '\t');
            return res;
        }
        private static string getInfo(Hardware hw) {
            switch (hw) {
                case Hardware.Motherboard when AppInfo.IsLinux: {
                    var result = dmidecode("dmidecode -t 2", "Manufacturer");
                    return result;
                }
                case Hardware.Motherboard when AppInfo.IsWindows:
                    return wmi("Win32_BaseBoard", "Manufacturer");
                case Hardware.Motherboard when AppInfo.IsMacOS:
                    throw new NotImplementedException();
                case Hardware.CPUID when AppInfo.IsLinux: {
                    var res = dmidecode("dmidecode -t 4", "ID");
                    var parts = res.Split(' ').Reverse();
                    var result = string.Join("", parts);
                    return result;
                }
                case Hardware.CPUID when AppInfo.IsWindows:
                    return wmi("Win32_Processor", "ProcessorId");
                case Hardware.CPUID when AppInfo.IsMacOS:
                    throw new NotImplementedException();
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
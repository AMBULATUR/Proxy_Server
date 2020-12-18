using Knapcode.TorSharp;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyServer
{
    class Program
    {
        static void Main(string[] args)
        {

            ProxyServer server = new ProxyServer();
            server.Work();

            while (true)
            {

            }
        }
    }
    class ProxyServer
    {
        public async void Work()
        {
            await DoWork();
        }
        int counter = 0;
        public void NotAbs(int random)
        {
            var proxy = new Leaf.xNet.Socks5ProxyClient("localhost", 1338, random.ToString(), "passwrd");
            using (var request = new HttpRequest())
            {
                request.Proxy = proxy;
                try
                {
                    var response = request.Get("https://api.ipify.org").ToString();
                    Interlocked.Increment(ref counter);
                    Console.WriteLine($"{counter}_{response}");
                }
                catch (Exception ex)
                {
                    ex = null;
                }
            }
        }

        public async Task DoWork()
        {
            var settings = new TorSharpSettings
            {
                TorSettings = new TorSharpTorSettings
                {
                    SocksPort = 1338,
                    ControlPort = 1339,
                    ControlPassword = "foobar",
                    UseBridges = true,
                    Bridge = "obfs4 137.186.96.243:443 B98F4BBEF1530479B82AF4C8A50D5F41436F5C40 cert=SRXyYEoT7x3WkhiQLVeU5VMWCmZZgg14Udi+sdd3Hc/iwZ4DB07AtxoJr/Wo8L9YRmwVSw iat-mode=0",
                    ClientTransportPlugin = "meek_lite,obfs2,obfs3,obfs4,scramblesuit exec D:\\obfs4proxy.exe",
                },
                PrivoxySettings = new TorSharpPrivoxySettings { Port = 1337 },
                UseExistingTools = true,
                ZippedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorZipped"),
                ExtractedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorExtracted")
            };
            await new TorSharpToolFetcher(settings, new HttpClient()).FetchAsync();
            var proxy = new TorSharpProxy(settings);
            await proxy.ConfigureAndStartAsync();

            Random random = new Random();
            for (int i = 0; i < 500; i++)
            {

                var thread = new Thread(() =>
                {
                    NotAbs(random.Next());
                });
                thread.Start();
            }
        }
        а
    }
}

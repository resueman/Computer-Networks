using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Tracert
{
    class Program
    {
        static void Main()
        {
            var utility = new TracertUtility();
            while (true)
            {
                Console.Write("tracert ");
                var address = Console.ReadLine();

                try
                {
                    foreach (var tracertEntry in utility.Start(address, 30, 7000))
                    {
                        Console.WriteLine(tracertEntry);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine();
            }
        }
    }
}

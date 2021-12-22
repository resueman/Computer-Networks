using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;

namespace Tracert
{
    /// <summary>
    /// Реализация утилиты tracert
    /// </summary>
    class TracertUtility
    {
        /// <summary>
        /// Показывает все промежуточные узлы, через которые проходит пакет, 
        /// пока не доберётся до хоста с указанным IP-адресом
        /// </summary>
        /// <param name="ipAddress">IP-адрес хоста-получателя пакета</param>
        /// <param name="maxHops">Максимальное число хопов (узлов), через которые может пройти пакет</param>
        /// <param name="timeout">Время ожидания</param>
        /// <returns>Запись с информацией о прохождении промежуточного узла</returns>
        public IEnumerable<TracertEntry> Start(string ipAddress, int maxHops, int timeout)
        {
            if (!IPAddress.TryParse(ipAddress, out var address))
            {
                address = Dns.GetHostAddresses(ipAddress).FirstOrDefault();
                if (address == null)
                {
                    throw new ArgumentException("Не удается разрешить системное имя узла");
                }
            }

            if (maxHops < 1)
            {
                throw new ArgumentException("Максимальное число хопов должно быть болеше 0");
            }

            if (timeout < 1)
            {
                throw new ArgumentException("Время ожидания должно быть больше 0");
            }

            Console.WriteLine($"Трассировка маршрута {address} с максимальным числом прыжков 30:\n");

            var ping = new Ping();
            var pingOptions = new PingOptions(1, true);
            PingReply reply;
            var pingReplyTime = new Stopwatch();
            do
            {
                pingReplyTime.Start();
                reply = ping.Send(address, timeout, new byte[] { 0 }, pingOptions);
                pingReplyTime.Stop();

                var hopId = pingOptions.Ttl;
                var responseTime = pingReplyTime.ElapsedMilliseconds;
                var hostname = GetHostEntry(reply.Address);
                var adrs = reply.Address == null ? " -- " : reply.Address.ToString();

                yield return new TracertEntry(hopId, responseTime, hostname, adrs, reply.Status);
                
                ++pingOptions.Ttl;
                pingReplyTime.Reset();
            }
            while (reply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops);
        }

        private static string GetHostEntry(IPAddress address)
        {
            if (address == null)
            {
                return "";
            }

            try
            {
                return Dns.GetHostEntry(address).HostName;
            }
            catch (SocketException)
            {
                return "";
            }
        }
    }
}

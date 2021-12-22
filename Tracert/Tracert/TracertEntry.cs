using System.Net.NetworkInformation;

namespace Tracert
{
    /// <summary>
    /// Представляет из себя запись с информацией о прохождении промежуточного узла
    /// </summary>
    public class TracertEntry
    {
        public TracertEntry(int hopId, long responseTime, string hostname, string address, IPStatus replyStatus)
        {
            HopId = hopId;
            ResponseTime = responseTime;
            Hostname = hostname;
            Address = address;
            ReplyStatus = replyStatus;
        }

        /// <summary>
        /// Порядковый номер хопа (узла)
        /// </summary>
        public int HopId { get; private set; }

        /// <summary>
        /// Время ответа хопа (узла) в милисекундах
        /// </summary>
        public long ResponseTime { get; private set; }

        /// <summary>
        /// Имя хопа (узла)
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// IP-адрес хопа (узла)
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Статус ответа на запрос
        /// </summary>
        public IPStatus ReplyStatus { get; private set; }

        /// <summary>
        /// Возвращает строковое представление TracertEntry
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var hostname = string.IsNullOrEmpty(Hostname)
                ? Address
                : $"{Hostname} [{Address}]";

            var answer = ReplyStatus == IPStatus.TimedOut
                ? string.Format("{0,6} {1,9}  {2}", $"{HopId}  ", "*     ", "Превышен интервал ожидания для запроса")
                : string.Format("{0,6} {1,9}  {2}", $"{HopId}  ", $"{ResponseTime} ms  ", hostname);

            return answer;
        }
    }
}

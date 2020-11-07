using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HelpTools
{
  public class IpChecker
    {
      readonly System.Net.NetworkInformation.Ping _ping = new System.Net.NetworkInformation.Ping();
        System.Net.NetworkInformation.PingReply _pingReply = null;
        /// <summary>
        /// 检测IP地址是否可通
        /// </summary>
        /// <param name="ip">待检测IP</param>
        /// <returns></returns>
        private Boolean PingIp(string ip)
        {
            bool rlight;
            try
            {
                _pingReply = _ping.Send(ip);
                if (_pingReply == null || (_pingReply != null && _pingReply.Status != System.Net.NetworkInformation.IPStatus.Success))
                {
                    rlight = false;
                }
                else
                {
                    rlight = true;
                }
            }
            catch (Exception ex)
            {
                rlight = false;
            }
            return rlight;
        }

        /// <summary>
        /// 验证IP地址
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        public bool IsIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }
            var rx = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (!rx.IsMatch(ip))
            {
                return false;
            }
            return true;
        }
    }
}

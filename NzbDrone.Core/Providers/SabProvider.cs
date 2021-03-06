﻿using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using NLog;

namespace NzbDrone.Core.Providers
{
    public class SabProvider : IDownloadProvider
    {
        private readonly IConfigProvider _config;
        private readonly IHttpProvider _http;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SabProvider(IConfigProvider config, IHttpProvider http)
        {
            _config = config;
            _http = http;
        }

        #region IDownloadProvider Members

        public bool AddByUrl(string url, string title)
        {
            const string mode = "addurl";
            const string cat = "tv";
            string priority = _config.GetValue("Priority", String.Empty, false);
            string name = url.Replace("&", "%26");
            string nzbName = HttpUtility.UrlEncode(title);

            string action = string.Format("mode={0}&name={1}&priority={2}&cat={3}&nzbname={4}", mode, name, priority, cat, nzbName);
            string request = GetSabRequest(action);

            Logger.Debug("Adding report [{0}] to the queue.", nzbName);

            string response = _http.DownloadString(request).Replace("\n", String.Empty);
            Logger.Debug("Queue Repsonse: [{0}]", response);

            if (response == "ok")
                return true;

            return false;
        }

        public bool IsInQueue(string title)
        {
            const string action = "mode=queue&output=xml";
            string request = GetSabRequest(action);
            string response = _http.DownloadString(request);

            XDocument xDoc = XDocument.Parse(response);

            //If an Error Occurred, retuyrn)
            if (xDoc.Descendants("error").Count() != 0)
                return false;

            if (xDoc.Descendants("queue").Count() == 0)
                return false;

            //Get the Count of Items in Queue where 'filename' is Equal to goodName, if not zero, return true (isInQueue)))
            if ((xDoc.Descendants("slot").Where(s => s.Element("filename").Value.Equals(title, StringComparison.InvariantCultureIgnoreCase))).Count() != 0)
            {
                Logger.Debug("Episode in queue - '{0}'", title);

                return true;
            }

            return false; //Not in Queue
        }

        #endregion

        private string GetSabRequest(string action)
        {
            string sabnzbdInfo = _config.GetValue("SabnzbdInfo", String.Empty, false);
            string username = _config.GetValue("Username", String.Empty, false);
            string password = _config.GetValue("Password", String.Empty, false);
            string apiKey = _config.GetValue("ApiKey", String.Empty, false);

            return string.Format(@"http://{0}/sabnzbd/api?$Action&apikey={1}&ma_username={2}&ma_password={3}", sabnzbdInfo, apiKey, username, password).Replace("$Action", action);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RawlerLib
{
    /// <summary>
    /// 標準のWebClientではクッキーが使えないため拡張
    /// </summary>
    public class WebClientEx: System.Net.WebClient
    {

        private CookieContainer cookieContainer =new CookieContainer();

        public CookieContainer CookieContainer
        {
            get
            {
                return cookieContainer;
            }
            set
            {
                cookieContainer = value;
            }
        }

        public string UserAgent { get; set; }
        public string Referer { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);

            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                httpWebRequest.CookieContainer = this.cookieContainer;
                httpWebRequest.Referer = this.Referer;
                httpWebRequest.UserAgent = this.UserAgent;
            }

            return webRequest;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    [ContentProperty("Children")]
    [Serializable]
    public class Page : RawlerBase, IInputParameter
    {
        public Page()
            : base()
        {
        }

        public Page(string url)
            : base()
        {
            this.Url = url;
        }

        /// <summary>
        /// 初めに指定したURLを取得します。
        /// 現在読み込んでいるURLを取得するには GetCurrentUrl()を使ってください
        /// </summary>
        public virtual string Url { get; set; }

        protected string currentUrl = string.Empty;

        public RawlerCollection BeforeTrees { get; set; } = new RawlerCollection();


        //現在読み込んでいるURLを取得します。
        public string GetCurrentUrl()
        {
            return currentUrl;
        }
        //        public string CurrentUrl { get; set; }


        private string encoding = string.Empty;
        /// <summary>
        /// 文字コードの指定　
        /// </summary>
        public string Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                encoding = value;

            }
        }

        private string referer = string.Empty;

        public string Referer
        {
            get { return referer; }
            set { referer = value; }
        }

        /// <summary>
        /// HtmlとUrlをセットする。
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        public void SetHtmlUrl(string html,string url)
        {
            this.SetText(html);
            this.currentUrl=url;
            this.startUrl = url;
        }

        protected Encoding GetEncoding()
        {
            Encoding enc = null;
            if (encoding != null && encoding.Length > 0)
            {
                try
                {
                    enc = System.Text.Encoding.GetEncoding(encoding);
                }
                catch
                {
                    ReportManage.ErrReport(this, "文字コード:" + encoding + " は不正です。無視します。");
                    enc = null;
                }
            }
            return enc;
        }


        public string GetCurrentPage()
        {
            var client = GetWebClient();
            this.text = client.HttpGet(this.currentUrl);
            return this.text;
        }

        protected string startUrl = string.Empty;


        /// <summary>
        /// 開始したURLです。URLを取得するときはこちらを使ってください。
        /// </summary>
        /// <returns></returns>
        public string GetStartUrl()
        {
            return startUrl;
        }

       protected  Stack<string> urlStack = new Stack<string>();
        bool oncePageLoad = true;

        /// <summary>
        /// ページを取得した履歴を持ち、一度、読み込んだところは無視する。
        /// </summary>
        public bool OncePageLoad
        {
            get { return oncePageLoad; }
            set { oncePageLoad = value; }
        }

        /// <summary>
        /// オブジェクト開始時にURLの履歴を削除します。
        /// </summary>
        bool clearUrlHistory = true;

        public bool ClearUrlHistory
        {
            get { return clearUrlHistory; }
            set { clearUrlHistory = value; }
        }

        protected HashSet<string> urlHash = new HashSet<string>();

        public void PushUrl(string url)
        {
            var tmp = url.Replace("&#", "&&&&");
            var u = tmp.Split('#');
            if (u.Length > 0)
            {
                url = u[0].Replace("&&&&","&#");
            }
            if (oncePageLoad)
            {
                if (urlHash.Add(url))
                {
                    urlStack.Push(url);
                }
                else
                {
                    if (visbleErr)
                    {
                        ReportManage.ErrReport(this, "すでに読み込んだURLです。スルーします。　" + url);
                    }
                }
            }
            else
            {
                urlStack.Push(url);
            }
        }

        public void Reload()
        {
            urlStack.Push(this.GetCurrentUrl());
        }

        string pastUrl = string.Empty;

        public override void Run(bool runChildren)
        {
            if (clearUrlHistory)
            {
                urlHash.Clear();
            }
            pastUrl = string.Empty;
            string url = string.Empty;

            if (this.Url != null && this.Url.Length > 0)
            {
                url = Url.Convert(this);
                if (url.Contains("http") == false)
                {
                    var prePage = this.GetAncestorRawler().Skip(1).OfType<Page>();
                    if (prePage.Any() == true)
                    {
                        url = System.IO.Path.Combine(prePage.First().GetCurrentUrl(), this.Url);
                    }
                }
                this.startUrl = url;
                PushUrl(url);
            }
            else
            {
                if (this.Parent != null)
                {
                    this.startUrl = GetText();
                    PushUrl(GetText());
                }
            }

            while (urlStack.Count > 0)
            {
                if (ReadPage(urlStack.Pop()))
                {
                    RunChildren(true);
                }
            }
        }

        /// <summary>
        /// ページロードなしで、Runをします。
        /// </summary>
        public void RunWithoutPageLoad()
        {
            RunChildren(true);
        }

        

        protected bool ReadPage(string url)
        {
            var client = GetWebClient();

            parameterList.Clear();
            httpHeaderList.Clear();
            BeforeTrees.Run(this, GetText());

            if (InputParameterTree != null)
            {
                RawlerBase.GetText(GetText(), InputParameterTree, this);
            }

            if (MethodType == Tool.MethodType.GET)
            {
                this.text = client.HttpGet(url,parameterList,httpHeaderList);
            }
            else if (MethodType == Tool.MethodType.POST)
            {
                this.text = client.HttpPost(url, parameterList,httpHeaderList);
            }

            this.currentUrl = url;
            this.pastUrl = this.currentUrl;

            if (this.Text.Length > 0)
            {
                return true;
            }
            else
            {
                if (client.ErrMessage != null && ( client.ErrMessage.Contains("503") || client.ErrMessage.Contains("500")))
                {
                    ReportManage.ErrReport(this,$"{client.ErrMessage} {url}の読み込みに失敗しました。");
                }
                else
                {
                    if (visbleErr)
                    {
                        ReportManage.ErrReport(this, url + "の読み込みに失敗しました。");
                    }
                    if (ErrorEvent != null)
                    {
                        ErrorEvent(this, new EventArgs());
                    }
                    if (ErrEventTree != null)
                    {
                        ErrEventTree.SetParent();
                        Document d = new Document() { TextValue = client.ErrMessage };
                        d.SetParent(this);
                        d.AddChildren(ErrEventTree);
                        d.Run();
                    }

                }
                return false;
            }
        }

        protected bool visbleErr = true;

        public bool VisbleErr
        {
            get { return visbleErr; }
            set { visbleErr = value; }
        }
        public RawlerBase ErrEventTree { get; set; }

        public event EventHandler ErrorEvent;

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public RawlerBase InputParameterTree { get; set; }
        protected List<KeyValue> parameterList = new List<KeyValue>();

        /// <summary>
        /// formのinputの指定。加えるだけ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParameter(string key, string value)
        {
            parameterList.Add(new KeyValue() { Key = key, Value = value });
        }

        protected List<KeyValue> httpHeaderList = new List<KeyValue>();

        /// <summary>
        /// formのinputの指定。加えるだけ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHttpHeader(string key, string value)
        {
            httpHeaderList.Add(new KeyValue() { Key = key, Value = value });
        }


        /// <summary>
        /// formのinputの指定。加えるだけ置き換える。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ReplaceParameter(string key, string value)
        {
            var list = parameterList.Where(n => n.Key == key);
            if (list.Any())
            {
                list.First().SetValue(value);
            }
            else
            {
                parameterList.Add(new KeyValue(key, value));
            }
        }

        bool useReferer = true;

        public bool UseReferer
        {
            get { return useReferer; }
            set { useReferer = value; }
        }

        public HttpHeaders HttpHeaders { get; set; }

        public HttpClient GetWebClient()
        {
            HttpClient client = this.GetUpperRawler<HttpClient>();

            if(client == null)
            {
                ReportManage.ErrUpperNotFound<HttpClient>(this);
                ReportManage.ErrReport(this, "新しいHttpClientを作成します");
                client = new HttpClient();
            }

            if (useReferer)
            {
                if (pastUrl == string.Empty)
                {
                    var pages = this.GetUpperRawler<Page>();

                    client.Referer = pages?.GetCurrentUrl();
                }
                else
                {
                    client.Referer = pastUrl;
                }
                if (referer != null && referer.Length > 0)
                {
                    client.Referer = referer;
                }
            }
            else
            {
                client.Referer = string.Empty;
            }
            var enc = GetEncoding();
            if (enc != null)
            {
                client.Encoding = encoding;
            }

            client.HttpHeaders = HttpHeaders;

            return client;
        }
        private MethodType methodType = MethodType.GET;

        public MethodType MethodType
        {
            get { return methodType; }
            set { methodType = value; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new Page();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            clone.SetParent(parent);
            clone.children.Clear();
            this.CloneEvent(clone);
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }

        /// <summary>
        /// ページヘの読み込み済みのPageオブジェクトを返す
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Web.PageNoDownLoad CreatePage(string url,string html)
        {
            var page = new Web.PageNoDownLoad();
            page.SetPage(url, html);
            return page;
        }
    }

    public enum MethodType
    {
        GET, POST
    }
}

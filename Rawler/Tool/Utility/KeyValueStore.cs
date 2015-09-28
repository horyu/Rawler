﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class KeyValueStore : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<KeyValueStore>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            SetText(GetText());
            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }

        Dictionary<string, string> dic = new Dictionary<string, string>();

        public void SetKeyValue(string key, string value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public string GetKeyValue(string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                ReportManage.ErrReport(this, "Key（" + key + "）が見つかりません");
                return string.Empty;
            }
        }

        public void Clear()
        {
            dic.Clear();
        }

        /// <summary>
        /// 指定したキーが上流のKeyValueStoreにあるかを調べる
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool ContainsKey(RawlerBase rawler,params string[] keys)
        {
            var r = rawler.GetUpperRawler<KeyValueStore>();
            if(r !=null)
            {
                return  keys.All((n) => r.dic.ContainsKey(n));
            }
            else
            {
                ReportManage.ErrUpperNotFound<KeyValueStore>(rawler);
            }
            return false;
        }

        /// <summary>
        /// 指定したキーで上流のKeyValueStoreから値を取得する。
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueByKey(RawlerBase rawler,string key)
        {
            var r = rawler.GetAncestorRawler().OfType<KeyValueStore>();
            string val = null;
            foreach (var item in r)
            {
                if (item.dic.ContainsKey(key))
                {
                    val = item.dic[key];
                    break;
                }
            }
            if(val == null)
            {
                ReportManage.ErrReport(rawler, "key:" + key + "が見つかりません");
            }
            return val;
        }
    }


    public class GetKeyValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetKeyValue>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public string Key { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<KeyValueStore>();
            if (list.Any())
            {
                SetText(list.First().GetKeyValue(Key));
            }
            else
            {
                ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
            }
            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }

    public class SetKeyValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<SetKeyValue>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
        public string Key { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<KeyValueStore>();
            if (list.Any())
            {
                if (string.IsNullOrEmpty(Value))
                {
                    list.First().SetKeyValue(Key, GetText());
                }
                else
                {
                    list.First().SetKeyValue(Key, Value);
                }
            }
            else
            {
                ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
            }


            SetText(GetText());
            base.Run(runChildren);
        }

        public string Value { get; set; }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }

    public class CheckKeyValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CheckKeyValue>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
        public string Key { get; set; }
        public string Value { get; set; }
        bool result = true;

        public bool Result
        {
            get { return result; }
            set { result = value; }
        }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (string.IsNullOrEmpty(Key))
            {
                ReportManage.ErrReport(this, "CheckKeyValueのKeyが空です。");
            }
            else
            {
                var list = this.GetAncestorRawler().OfType<KeyValueStore>();
                if (list.Any())
                {
                    if ((list.First().GetKeyValue(Key) == Value) == result)
                    {
                        this.SetText(this.GetText());
                        base.Run(runChildren);
                    }
                }
                else
                {
                    ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
                }
            }

        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }
    }


    public class KeyValueStoreClear : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<KeyValueStoreClear>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<KeyValueStore>();
            if (list.Any())
            {
                list.First().Clear();
            }
            else
            {
                ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
            }
            SetText(GetText());
            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }


    public static class KeyValueLib
    {
        /// <summary>
        /// {key} という記法で上流のKeyValueの値を取得する。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rawler"></param>
        /// <returns></returns>
        public static string Convert(this string text, RawlerBase rawler)
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                var text2 = text.Replace("[[", "&(").Replace("]]", "&)");
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\[\w*\]");
                foreach (var item in r.Matches(text).OfType<System.Text.RegularExpressions.Match>())
                {
                    var key = item.Value.Replace("[", "").Replace("]", "");
                    var val = KeyValueStore.GetValueByKey(rawler, key);
                    if (val.Length > 0)
                    {
                        text2 = text2.Replace(item.Value, val);
                    }
                }
                return text2.Replace("&(", "[").Replace("&)", "]");
            }
        }



        //public static string Convert(this string text,RawlerBase rawler)
        //{
        //    var list = RawlerLib.Web.GetTagContentList(text, "{", "}", true);

        //}
    }
}

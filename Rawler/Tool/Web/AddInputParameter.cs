﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using System.Diagnostics;

namespace Rawler.Tool
{
    [DebuggerDisplay("{Key} | {Value}")]
    public class AddInputParameter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<AddInputParameter>(parent);
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
            var page = (IInputParameter)this.GetUpperInterface<IInputParameter>();
            if (page == null)
            {
                ReportManage.ErrUpperNotFound<IInputParameter>(this);
                return;
            }
            string key = this.Key;
            if (KeyTree != null)
            {
                key = RawlerBase.GetText(this.Parent.Text, KeyTree, this.Parent);
            }
            else
            {
                key = key.Convert(this);
            }

            if (Value == null)
            {
                if (AddType == AddInputParameterType.replece)
                {
                    page.ReplaceParameter(key, GetText());
                }
                else
                {
                    page.AddParameter(key, GetText());
                }
            }
            else
            {
                if (AddType == AddInputParameterType.replece)
                {
                    page.ReplaceParameter(key, Value.Convert(this));
                }
                else
                {
                    page.AddParameter(key, Value.Convert(this));
                }
            }

            base.Run(runChildren);
        }

        public RawlerBase KeyTree { get; set; }

        public string Key { get; set; }
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

        /// <summary>
        /// 追加時の挙動です。デフォルトでは、既にある値を置き換えます。
        /// </summary>
        public AddInputParameterType AddType { get; set; } = AddInputParameterType.replece;
    }
    public enum AddInputParameterType
    {
        add,replece
    }
}

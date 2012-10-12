﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTool
{
    /// <summary>
    /// RawlerTree2.xaml の相互作用ロジック
    /// </summary>
    public partial class RawlerTree2 : Rawler.Tool.Data
    {
        public RawlerTree2()
        {
            #region //書きかえ禁止
            InitializeComponent();
            this.SetParent();
            #endregion
        }

        /// <summary>
        /// 初めに読み込むページを設定する。
        /// </summary>
        /// <param name="url"></param>
        public void SetStartPage(string url)
        {
            startPage.Url = url;
        }
    }
}

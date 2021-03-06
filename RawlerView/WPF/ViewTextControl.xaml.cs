﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rawler.RawlerLib.WPF
{
    /// <summary>
    /// ViewTextControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ViewTextControl : UserControl
    {
        public ViewTextControl()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            textBox1.Text = text;
            try
            {
                webBrowser1.NavigateToString(text);
            }
            catch(Exception ex)
            {
                
            }
        }
        public string Text
        {
            set
            {
                SetText(value);
            }
        }
    }
}

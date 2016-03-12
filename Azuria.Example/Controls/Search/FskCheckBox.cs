﻿using System.Windows;
using System.Windows.Controls;
using Azuria.Main.Minor;

namespace Azuria.Example.Controls.Search
{
    public class FskCheckBox : CheckBox
    {
        public static readonly DependencyProperty FskProperty =
            DependencyProperty.Register(nameof(Fsk), typeof (Fsk)
                , typeof (FskCheckBox), new FrameworkPropertyMetadata(Fsk.Fsk0, FskPropertyChanged));

        public FskCheckBox()
        {
            this.Content = this.Fsk;
        }

        #region Properties

        public Fsk Fsk
        {
            get { return (Fsk) this.GetValue(FskProperty); }
            set { this.SetValue(FskProperty, value); }
        }

        #endregion

        #region

        private static void FskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FskCheckBox lCheckBox = d as FskCheckBox;
            if (lCheckBox != null && lCheckBox.Content is Fsk)
            {
                lCheckBox.Content = e.NewValue;
            }
        }

        #endregion
    }
}
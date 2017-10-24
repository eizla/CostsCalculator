using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace CostsCalculator
{
    class DecimalFilter : Java.Lang.Object, IInputFilter
    {
        //Pattern mPattern;
        String regex = "[0-9]+((\\.[0-9]{0," + (2 - 1) + "})?)||(\\.)?";
        public DecimalFilter(int digitsAfterZero)
        {
            //mPattern = Pattern.compile("[0-9]+((\\.[0-9]{0," + (digitsAfterZero - 1) + "})?)||(\\.)?");
            regex = "[0-9]+((\\.[0-9]{0," + (digitsAfterZero - 1) + "})?)||(\\.)?";
        }

        public Java.Lang.ICharSequence FilterFormatted(Java.Lang.ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(dest.ToString(), regex))
            {
                return new Java.Lang.String(string.Empty);
            }
            return null;
        }
    }
}
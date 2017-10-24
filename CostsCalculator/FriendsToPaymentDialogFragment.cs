using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Text.RegularExpressions;
using Android.Graphics;
using System.Globalization;
using Android.Text;

namespace CostsCalculator
{
    class FriendsToPaymentDialogFragment : DialogFragment
    {
        public event EventHandler<decimal> DialogClosed;
        public string return_am;
        public Boolean is_paying = false;
        private Regex reg = new Regex(@"^\d+(?:.\d{0,2})?$");
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.friendsToPaymentPicker, container, false);
            var editName = view.FindViewById<EditText>(Resource.Id.editTextFriendsAmount);
            var button = view.FindViewById<Button>(Resource.Id.button1);
            var radiobutton = view.FindViewById<RadioButton>(Resource.Id.radioButton1);
            radiobutton.Click += (object sender, EventArgs e) =>
            {
                is_paying = !is_paying;
            };
                 
            button.Click += delegate
            {
                ButtonClicked();
            };
            editName.SetFilters(new IInputFilter[] { /*new DecimalFilter(2),*/ new InputFilterLengthFilter(12) });
            editName.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {

                return_am = e.Text.ToString();
                if (!reg.IsMatch(return_am))
                {
                    
                    editName.SetTextColor(Color.Red);
                }
                else
                {
                    editName.SetTextColor(Color.Black);
                }
            };
                return view;
        }
        //public override void OnDismiss(IDialogInterface dialog)
        //{
        //    base.OnDismiss(dialog);
            

        //}
        public void ButtonClicked()
        {
            if (DialogClosed != null)
            {
                try
                {
                    if (reg.IsMatch(return_am))
                    {
                        var amoun = Convert.ToDecimal(return_am, CultureInfo.InvariantCulture);
                        if (!is_paying)
                        {
                            amoun = -amoun;
                        }
                        if (amoun == 0)
                        {
                            Toast.MakeText(this.Context, "Wrong number inserted.", ToastLength.Short).Show();
                        }
                        else
                        {
                            DialogClosed(this, amoun);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this.Context, "Wrong number inserted.", ToastLength.Short).Show();
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.StackTrace);
                    if (ex is FormatException || ex is OverflowException)
                    {
                        Toast.MakeText(this.Context, "Number to big or to small!", ToastLength.Short).Show();
                    }
                    if (ex is ArgumentNullException)
                    {
                        Toast.MakeText(this.Context, "Empty number!", ToastLength.Short).Show();
                    }
                }

            }
            this.Dismiss();
        }
    }
}
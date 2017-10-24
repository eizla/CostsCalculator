using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using CostsCalculator.Models;
using Android.Graphics;

namespace CostsCalculator.Resources.layout
{
    class TripPaymentAdapter : BaseAdapter
    {
        private Activity activity;

        //template
        private ObservableCollection<PaymentItem> friendsToPayment = new ObservableCollection<PaymentItem>();

        public TripPaymentAdapter(Activity activity, ObservableCollection<PaymentItem> friendsToPayment)
        {
            this.activity = activity;
            this.friendsToPayment = friendsToPayment;
        }

        public override int Count
        {
            get
            {
                return friendsToPayment.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.PaymentListViewDataTemplate, parent, false);        

            var txtName = view.FindViewById<TextView>(Resource.Id.textViewName);
            var txtAmount = view.FindViewById<TextView>(Resource.Id.textViewAmount);
            txtName.Text = friendsToPayment[position].Name;
            txtAmount.Text = "" + friendsToPayment[position].Amount;

            var textImage = view.FindViewById<TextView>(Resource.Id.circleImage);
            textImage.SetBackgroundColor(Color.ParseColor("#263238"));


            return view;
        }

        public static implicit operator TripPaymentAdapter(FriendsCustomAdapter v)
        {
            throw new NotImplementedException();
        }
    }
}
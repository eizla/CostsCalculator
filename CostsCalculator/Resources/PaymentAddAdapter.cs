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
using System.Collections.ObjectModel;
using CostsCalculator.Models;

namespace CostsCalculator.Resources
{
    class PaymentAddAdapter : BaseAdapter
    {

        private Activity activity;

        private List<Tuple<string, decimal>> list = new List<Tuple<string, decimal>>();

        public PaymentAddAdapter(Activity activity, List<Tuple<string, decimal>> list)
        {
            this.activity = activity;
            this.list = list;
        }

        public override int Count
        {
            get
            {
                return list.Count;
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
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.ListAddPayment, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textViewName);
            var txtAmount = view.FindViewById<TextView>(Resource.Id.textViewAmount);
            
            txtName.Text = list[position].Item1;
            var str = "";
            if (list[position].Item2 > 0)
            {
                str = "paid ";
            }
            else
            {
                str = "has to pay ";
            }
            txtAmount.Text = str + Math.Abs(list[position].Item2);

            return view;
        }
    }
}
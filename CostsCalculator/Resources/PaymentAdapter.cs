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
using Java.Lang;
using CostsCalculator.Models;
using System.Collections.ObjectModel;
using Android.Graphics;

namespace CostsCalculator.Resources
{
    class PaymentAdapter : BaseAdapter
    {

        private Activity activity;

        private ObservableCollection<UserPaymentItem> list = new ObservableCollection<UserPaymentItem>();
        List<UserItem> tripsFriends = new List<UserItem>();
        private List<UserItem> allList = new List<UserItem>();
        public PaymentAdapter(Activity activity, ObservableCollection<UserPaymentItem> list, ObservableCollection<UserItem> tripsFriends )
        {
            this.activity = activity;
            this.list = list;
            this.tripsFriends = new List<UserItem>(tripsFriends);
            allList = new List<UserItem>(MyFriendsActivity.friendsList);
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
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.PaymentAddListViewDataTemplate, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textViewName);
            var txtAmount = view.FindViewById<TextView>(Resource.Id.textViewAmount);

            var item = tripsFriends.Find(i => i.Id == list[position].UserId);
            if (item == null) item = allList.Find(i => i.Id == list[position].UserId);

            if (item != null) txtName.Text = item.Name;
            else txtName.Text = "User";
            txtAmount.Text = "" + list[position].Amount;

            return view;
        }
    }
}
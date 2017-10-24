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
using CostsCalculator.Resources.layout;
using CostsCalculator.Models;
using CostsCalculator.Resources;
using Android.Support.V7.App;
using System.Collections.ObjectModel;
using System.Globalization;
using Android.Support.V4.Widget;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace CostsCalculator
{
    [Activity(Label = "Payment", Theme = "@style/MyTheme")]
    class PaymentActivity : AppCompatActivity
    {
        PaymentItem paymentItem = new PaymentItem();
        private ListView listView;
        private TripItem tripItem;
        ObservableCollection<UserPaymentItem> paymentContributors = new ObservableCollection<UserPaymentItem>();
        private ObservableCollection<UserItem> tripFriends = new ObservableCollection<UserItem>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Payment);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var textViewName = FindViewById<TextView>(Resource.Id.textViewName);
            var textViewAmount = FindViewById<TextView>(Resource.Id.textViewAmount);

            listView = FindViewById<ListView>(Resource.Id.listView1);
            tripFriends = JsonConvert.DeserializeObject<ObservableCollection<UserItem>>(Intent.GetStringExtra("Friends"));
            paymentItem = JsonConvert.DeserializeObject<PaymentItem>(Intent.GetStringExtra("PaymentItem"));
            tripItem = JsonConvert.DeserializeObject<TripItem>(Intent.GetStringExtra("TripItem"));
            GetPaymentUser();
            textViewName.Text = paymentItem.Name;
            textViewAmount.Text = "" + paymentItem.Amount;

            var adapter = new PaymentAdapter(this, paymentContributors, tripFriends);
            listView.Adapter = adapter;

            var swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;

        }

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            GetPaymentUser();
            var adapter = new PaymentAdapter(this, paymentContributors, tripFriends);
            listView.Adapter = adapter;
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private async void GetPaymentUser()
        {
            paymentContributors = await DatabaseManager.DefaultManager.GetPaymentUsersForPaymentAsync(paymentItem);
            var adapterPayemnt = new PaymentAdapter(this, paymentContributors, tripFriends);
            listView.Adapter = adapterPayemnt;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.toolbarPayment, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.edit:
                    var activity = (Intent)null;
                    activity = new Intent(this, typeof(PaymentAddActivity));
                    activity.PutExtra("Payment", JsonConvert.SerializeObject(paymentItem));
                    activity.PutExtra("Friends", JsonConvert.SerializeObject(tripFriends));
                    activity.PutExtra("Trip", JsonConvert.SerializeObject(tripItem));
                    //   activity.PutExtra();
                    StartActivity(activity);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}
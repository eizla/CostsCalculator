using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using CostsCalculator.Models;
using CostsCalculator.Resources;
using Android.Support.V7.App;
using Newtonsoft.Json;

namespace CostsCalculator
{
    [Activity(Label = "History trips", Theme = "@style/MyTheme")]
    class HistoryTripsActivity : AppCompatActivity
    {
        private ObservableCollection<TripItem> tripsInPast = new ObservableCollection<TripItem>();
        public static SwipeRefreshLayout swipeContainer;
        private ListView listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            getTripsList();

            SetContentView(Resource.Layout.Trips);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            listView = FindViewById<ListView>(Resource.Id.listViewTrips);
            listView.ItemClick += listViewClick;
        }

        public void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            getTripsList();
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private async void getTripsList()
        {
            tripsInPast = await DatabaseManager.DefaultManager.GetTripItemsInPastAsync(HomeActivity1.userItem);
            var adapter = new TripsCustomAdapter(this, tripsInPast);
            listView = FindViewById<ListView>(Resource.Id.listViewTrips);
            listView.Adapter = adapter;
        }


        private void listViewClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            var activity = (Intent) null;
            activity = new Intent(this, typeof(TripActivity));
            TripItem trip = tripsInPast[e.Position];
            activity.PutExtra("Trip", JsonConvert.SerializeObject(trip));
            activity.PutExtra("Flag", JsonConvert.SerializeObject(true));
            StartActivity(activity);

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //ActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.toolbarHistory, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var activity = (Intent)null;

            switch (item.ItemId)
            {
                case Resource.Id.trips:
                    Finish();
                    break;
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}
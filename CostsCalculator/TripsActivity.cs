using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using CostsCalculator.Models;
using CostsCalculator.Resources;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Android.Support.V7.App;
using Android.Support.V4.View;

namespace CostsCalculator
{
    [Activity(Label = "Trips", Theme = "@style/MyTheme")]
    class TripsActivity : AppCompatActivity
    {
        public static ObservableCollection<TripItem> tripsList = new ObservableCollection<TripItem>();
        public static SwipeRefreshLayout swipeContainer;
        private ListView listView;
        private TripsCustomAdapter adapter;
        private SearchView _searchView1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            getTripsList();

            SetContentView(Resource.Layout.Trips);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            listView = FindViewById<ListView>(Resource.Id.listViewTrips);
            listView.ItemClick += listViewClick;
            listView.ItemLongClick += listViewLongClick;
        }

        public void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            getTripsList();
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private async void getTripsList()
        {
            tripsList = await DatabaseManager.DefaultManager.GetTripItemsAsync(HomeActivity1.userItem);
            adapter = new TripsCustomAdapter(this, tripsList);
            listView = FindViewById<ListView>(Resource.Id.listViewTrips);
            listView.Adapter = adapter;
        }

        private  void listViewLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);

            alert.SetTitle("Do you want close this trip?");

            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {
                TripItem trip = tripsList[e.Position];
                trip.IsCurrent = false;
                await DatabaseManager.DefaultManager.SaveTripItemAsync(trip, null);
            });

            alert.SetNegativeButton("No", (senderAlert, args) => {

            });
            RunOnUiThread(() => {
                alert.Show();
            });


        }

        private void listViewClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
            var activity = (Intent)null;
            activity = new Intent(this, typeof(TripActivity));
            TripItem trip = tripsList[e.Position];
            activity.PutExtra("Trip", JsonConvert.SerializeObject(trip));
            StartActivity(activity);
          
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //ActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.toolbarTrips, menu);

            var item = menu.FindItem(Resource.Id.searchTrips);
            var searchView = MenuItemCompat.GetActionView(item);

            _searchView1 = searchView.JavaCast<SearchView>();

            _searchView1.QueryTextChange += _searchView1_QueryTextChange;

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var activity = (Intent)null;

            switch (item.ItemId)
            {
                case Resource.Id.addNewTrip:
                    activity = new Intent(this, typeof(TripAddPageActivity));
                    activity.PutExtra("Trip", JsonConvert.SerializeObject(null));
                    StartActivity(activity);
                    break;
                case Resource.Id.history:
                    activity = new Intent(this, typeof(HistoryTripsActivity));
                    StartActivity(activity);
                    break;
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void _searchView1_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {

            if (e.NewText.Length > 0)
                adapter.filter(adapter, e.NewText);
            else
                getTripsList();
            adapter.NotifyDataSetChanged();
        }


    }
}
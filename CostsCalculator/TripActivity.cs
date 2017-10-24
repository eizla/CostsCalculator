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
using CostsCalculator.Resources.layout;
using Java.Util;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Android.Support.V7.App;

namespace CostsCalculator
{
    [Activity(Label ="Trip", Theme = "@style/MyTheme")]
    class TripActivity : AppCompatActivity
    {
        TextView textViewName, textViewStartDate, textViewEndDate, textViewDescription;
        Button buttonPayments, buttonFriends, buttonHistory, buttonSummary;
        ListView listView;
        ObservableCollection<PaymentItem> payments;
        ObservableCollection<UserItem> friends;
        ObservableCollection<HistoryItem> history;
        ObservableCollection<string> countedAlgo;
        TripPaymentAdapter adapter;
        private bool flag;
        String type = "payments";
        private TripItem tripItem = null;
        Algo2 algo;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            tripItem = JsonConvert.DeserializeObject<TripItem>(Intent.GetStringExtra("Trip"));
            try
            {
                flag = JsonConvert.DeserializeObject<bool>(Intent.GetStringExtra("Flag"));
            }
            catch
            {
                flag = false;
            }
            
            payments = new ObservableCollection<PaymentItem>();
            friends = new ObservableCollection<UserItem>();
            countedAlgo = new ObservableCollection<string>();

            history = new ObservableCollection<HistoryItem>();
            GetAll();

            SetContentView(Resource.Layout.TripLayout);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            textViewName = FindViewById<TextView>(Resource.Id.textViewName);
            textViewStartDate = FindViewById<TextView>(Resource.Id.textViewStartDate);
            textViewEndDate = FindViewById<TextView>(Resource.Id.textViewEndDate);
            textViewDescription = FindViewById<TextView>(Resource.Id.textViewDescription);
            buttonPayments = FindViewById<Button>(Resource.Id.buttonPayments);
            buttonFriends = FindViewById<Button>(Resource.Id.buttonFriends);
            buttonHistory = FindViewById<Button>(Resource.Id.buttonHistory);
            buttonSummary = FindViewById<Button>(Resource.Id.buttonSummary);
            listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.ItemClick += ListViewClick;

            textViewName.Text += tripItem.Name;
            textViewStartDate.Text += tripItem.StartDate.ToShortDateString();
            textViewEndDate.Text += tripItem.EndDate.ToShortDateString();
            if (tripItem.Description != string.Empty) textViewDescription.Text += tripItem.Description;
            else textViewDescription.Visibility = ViewStates.Invisible;

            ButtonClicked("payments");
           
            buttonPayments.Click += delegate
            {
                type = "payments";
                ButtonClicked("payments");
            };

            buttonFriends.Click += delegate
            {
                type = "friends";
                ButtonClicked("friends");
            };

            buttonHistory.Click += delegate
            {
                type = "history";
                ButtonClicked("history");
            };

            buttonSummary.Click += delegate
            {
                type = "summary";
                ButtonClicked("summary");
            };
            var swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;
        }

        private void ListViewClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (type)
            {
                case "payments":
                    var activity = new Intent(this, typeof(PaymentAddActivity));
                    PaymentItem item = payments[e.Position];
                    activity.PutExtra("Payment", JsonConvert.SerializeObject(item));
                    activity.PutExtra("Friends", JsonConvert.SerializeObject(friends));
                    activity.PutExtra("Trip", JsonConvert.SerializeObject(tripItem));
                    StartActivity(activity);
                    break;
                case "friends":
                    break;
                case "history":
                    break;
                default:
                    break;
            }
        }

        private async void GetAll()
        {
            payments = await DatabaseManager.DefaultManager.GetPaymentItemsAsync(tripItem);
            friends = await DatabaseManager.DefaultManager.GetUsersTripsItemsAsync(tripItem);
            history = await DatabaseManager.DefaultManager.GetHistoryItemsAsync(tripItem);
        }

        private async void PaymentsDisplay()
        {
            if(!payments.Any()) payments = await DatabaseManager.DefaultManager.GetPaymentItemsAsync(tripItem);
            adapter = new TripPaymentAdapter(this, payments);
            listView.Adapter = adapter;
        }

        private async void FriendsDisplay()
        {
            if (!friends.Any()) friends = await DatabaseManager.DefaultManager.GetUsersTripsItemsAsync(tripItem);
            friends = new ObservableCollection<UserItem>(friends.OrderBy(o => o.Name).ToList());
            var adapter = new FriendsCustomAdapter(this, friends);
            listView.Adapter = adapter;
        }

        private async void HistoryDisplay()
        {
            if (!history.Any()) history = await DatabaseManager.DefaultManager.GetHistoryItemsAsync(tripItem);
            if (!friends.Any()) friends = await DatabaseManager.DefaultManager.GetUsersTripsItemsAsync(tripItem);
            var adapter = new HistoryAdapter(this, history, friends);
            listView.Adapter = adapter;
        }

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            switch (type)
            {
                case "payments":
                    PaymentsRefresh();
                    break;
                case "friends":
                    FriendRefresh();
                    break;
                case "history":
                    HistoryRefresh();
                    break;
                case "summary":
                    SummaryRefresh();
                    break;
            }
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private async void FriendRefresh()
        {
            friends = await DatabaseManager.DefaultManager.GetUsersTripsItemsAsync(tripItem);
            friends = new ObservableCollection<UserItem>(friends.OrderBy(o => o.Name).ToList());
            var adapter = new FriendsCustomAdapter(this, friends);
            listView.Adapter = adapter;
        }

        private async void HistoryRefresh()
        {
            history = await DatabaseManager.DefaultManager.GetHistoryItemsAsync(tripItem);
            var adapter = new HistoryAdapter(this, history, friends);
            listView.Adapter = adapter;
        }

        private async void PaymentsRefresh()
        {
            payments = await DatabaseManager.DefaultManager.GetPaymentItemsAsync(tripItem);
            adapter = new TripPaymentAdapter(this, payments);
            listView.Adapter = adapter;
        }
        private async void SummaryRefresh()
        {
            algo = new Algo2(payments.ToList(), HomeActivity1.userItem, new HashSet<UserItem>(friends), tripItem);
            try
            {
                countedAlgo = await algo.Algorithms_start();
            }
            catch (Exception ex)
            {
                if(ex is ArgumentException)
                {
                    Toast.MakeText(this, "Upps data are wrong!", ToastLength.Short).Show();
                }
                if(ex is OverflowException)
                {
                    Toast.MakeText(this, "Numbers to big to sum!", ToastLength.Long).Show();
                }
                countedAlgo = new ObservableCollection<string>();
                countedAlgo.Add("Something went wrong!");
            }
            var adapter = new AlgoAdapter(this, countedAlgo);
            listView.Adapter = adapter;
        }
        private async void SummaryDisplay()
        {
            if (!countedAlgo.Any())
            {
                if (!friends.Any()) friends = await DatabaseManager.DefaultManager.GetUsersTripsItemsAsync(tripItem);
                if (!payments.Any()) payments = await DatabaseManager.DefaultManager.GetPaymentItemsAsync(tripItem);
                algo = new Algo2(payments.ToList(), HomeActivity1.userItem, new HashSet<UserItem>(friends), tripItem);
                try
                {
                    countedAlgo = await algo.Algorithms_start();
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                    {
                        Toast.MakeText(this, "Upps data are wrong!", ToastLength.Short).Show();
                    }
                    if (ex is OverflowException)
                    {
                        Toast.MakeText(this, "Numbers to big to sum!", ToastLength.Long).Show();
                    }
                    countedAlgo = new ObservableCollection<string>();
                    countedAlgo.Add("Something went wrong!");
                }
            }
             var adapter = new AlgoAdapter(this, countedAlgo);
            listView.Adapter = adapter;
            // listView.ItemClick -= listViewClick;
            //TODO catch summary exception, better algo start
        }

        private void ButtonClicked(String type)
        {
            switch (type)
            {
                case "payments":
                    PaymentsDisplay();
                    break;
                case "friends":
                    FriendsDisplay();
                    break;
                case "history":
                    HistoryDisplay();
                    break;
                case "summary":
                    SummaryDisplay();
                    break;
            }
        }

       public override bool OnCreateOptionsMenu(IMenu menu)
        {
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            var inflater = MenuInflater;
            if(!flag) inflater.Inflate(Resource.Menu.tripToolbar, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.add:
                    AddButtonClicked();
                    return true;
                case Resource.Id.edit:
                    var activity = new Intent(this, typeof(TripAddPageActivity));
                    activity.PutExtra("Trip", JsonConvert.SerializeObject(tripItem));
                    StartActivity(activity);
                    break;
                case Android.Resource.Id.Home:
                    Finish();
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void AddButtonClicked()
        {
            var activity = new Intent(this, typeof(PaymentAddActivity));
            activity.PutExtra("Friends", JsonConvert.SerializeObject(friends));
            activity.PutExtra("Trip", JsonConvert.SerializeObject(tripItem));
            StartActivity(activity);

        }
    }
}
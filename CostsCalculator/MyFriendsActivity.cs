using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using com.refractored.fab;
using CostsCalculator.Models;
using Android.Util;
using CostsCalculator.Resources;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Support.V4.View;
using Android.Runtime;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace CostsCalculator
{
    [Activity(Label = "My friends", Theme = "@style/MyTheme")]
    class MyFriendsActivity : AppCompatActivity, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        public static ObservableCollection<UserItem> friendsList = new ObservableCollection<UserItem>();
        private FriendsCustomAdapter adapter;
        private ListView lstData;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            GetFriendList();

            SetContentView(Resource.Layout.Friends);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            lstData = FindViewById<ListView>(Resource.Id.listViewFriends);
            lstData.ItemClick += ListViewClick;
            lstData.ItemLongClick += ListViewLongClick;
        }

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            GetFriendList();
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private async void GetFriendList()
        {
            friendsList = await DatabaseManager.DefaultManager.GetUsersItemsAsync(HomeActivity1.userItem);
            friendsList = new ObservableCollection<UserItem>(friendsList.OrderBy(o => o.Name).ToList());
            adapter = new FriendsCustomAdapter(this, friendsList);
            lstData = FindViewById<ListView>(Resource.Id.listViewFriends);
            lstData.Adapter = adapter;
        }


        private void ListViewClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle(friendsList[e.Position].Name);
            alert.SetMessage(friendsList[e.Position].Description);
            alert.SetPositiveButton("Ok", (senderAlert, args) => {
            });

            RunOnUiThread(() => {
                alert.Show();
            });
        }

        private void ListViewLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Do you want remove your friend?");

            alert.SetPositiveButton("Yes", (senderAlert, args) => {
                DatabaseManager.DefaultManager.DeleteFriend(friendsList[e.Position], HomeActivity1.userItem);
                friendsList.Remove(friendsList[e.Position]);
                adapter = new FriendsCustomAdapter(this, friendsList);
                lstData = FindViewById<ListView>(Resource.Id.listViewFriends);
                lstData.Adapter = adapter;
            });

            alert.SetNegativeButton("No", (senderAlert, args) => {
            });
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }
   
        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            Log.Info("EDMTDev", "OnScroll Implement");
        }

        public void OnScrollDown()
        {
            Log.Info("EDMTDev", "OnScrollDown Implement");
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {
            Log.Info("EDMTDev", "OnScrollStateChanged Implement");
        }

        public void OnScrollUp()
        {
            Log.Info("EDMTDev", "OnScrollUp Implement");
        }
    }
}
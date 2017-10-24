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
  
    [Activity(Label = "Find friend", Theme = "@style/MyTheme")]
    class FriendsActivity : AppCompatActivity, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        private ObservableCollection<UserItem> users = new ObservableCollection<UserItem>();
        private SearchView _searchView1;
        private FriendsCustomAdapter adapter;
        //private ArrayAdapter _adapter;
        public ListView lstData { get; set; } 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GetUsersList();
           
            SetContentView(Resource.Layout.Friends);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //GetUsersList();
            var swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;
            lstData = FindViewById<ListView>(Resource.Id.listViewFriends);
            lstData.ItemClick += ListViewClick;
        }

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            GetUsersList();
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private async void GetUsersList()
        {
            users = await DatabaseManager.DefaultManager.GetUsersAsync();
            if (users.Contains(HomeActivity1.userItem))
                users.Remove(HomeActivity1.userItem);
            users = new ObservableCollection<UserItem>((users.Except(MyFriendsActivity.friendsList)).OrderBy(o => o.Name).ToList());
            adapter = new FriendsCustomAdapter(this, users);
            var _users = users.ToList();
            
            lstData = FindViewById<ListView>(Resource.Id.listViewFriends);
            lstData.Adapter = adapter;
        }

        private void ListViewClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Do you want add to your friends?");

            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {
                FriendItem item = new FriendItem { FriendId = users[e.Position].Id, UserId = HomeActivity1.userItem.Id };
                await DatabaseManager.DefaultManager.SaveFriendItemAsync(item);
                users.Remove(users[e.Position]);
                adapter = new FriendsCustomAdapter(this, users);
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // ActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            MenuInflater.Inflate(Resource.Menu.toolbarFriends, menu);

            var item = menu.FindItem(Resource.Id.searchFriends);

            var searchView = MenuItemCompat.GetActionView(item);

            _searchView1 = searchView.JavaCast<SearchView>();

            _searchView1.QueryTextChange += _searchView1_QueryTextChange;

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }

        private void _searchView1_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
           
            if (e.NewText.Length > 0)
                adapter.filter(adapter, e.NewText);
            else
                GetUsersList();
           adapter.NotifyDataSetChanged();
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
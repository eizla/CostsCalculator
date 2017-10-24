using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CostsCalculator.Models;
using Android.Text;
using Android.Graphics;

namespace CostsCalculator
{
    [Activity(Label = "HOME", Theme = "@style/MyTheme")]
    public class HomeActivity1 : AppCompatActivity
    {
        private User user;
        public static UserItem userItem = new UserItem();
        private SupportToolbar mToolbar;
        private MyActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;

        private Button edit;
        public static string Mail = string.Empty;
        private EditText description;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Home1);
            user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
            
            userItem.Name = user.Name;

            CheckUser();
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

            SetSupportActionBar(mToolbar);

            
            TextView Name = FindViewById<TextView>(Resource.Id.textViewName);
            Name.Text = HomeActivity1.userItem.Name;

            var textImage = FindViewById<TextView>(Resource.Id.imageViewProfile);

            if (userItem.Color == null)
            {
                userItem.Color = GetRandomColor();
                CheckUser();
            }

            textImage.SetBackgroundColor(Color.ParseColor(userItem.Color));
            textImage.Text = Name.Text[0].ToString().ToUpper();

            LeftDrawerAdapter adapter = new LeftDrawerAdapter(this);
            mLeftDrawer.Adapter = adapter; 

            mLeftDrawer.ItemClick += (sender, e) =>
            {
                string str = mLeftDrawer.GetItemAtPosition(e.Position).ToString();
                mLeftDrawerItemClicked(str);
            };

            mDrawerToggle = new MyActionBarDrawerToggle(
                this,                           //Host Activity
                mDrawerLayout,                  //DrawerLayout
                Resource.String.openDrawer,     //Opened Message
                Resource.String.closeDrawer     //Closed Message
            );

            mDrawerLayout.SetDrawerListener(mDrawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            mDrawerToggle.SyncState();

            GetFriendList();
        }

       

        private async void GetFriendList()
        {
            MyFriendsActivity.friendsList = await DatabaseManager.DefaultManager.GetUsersItemsAsync(userItem);
        }

        private void mLeftDrawerItemClicked(string str)
        {
            switch (str)
            {  
                case "Profile":
                    var profileActivity = new Intent(this, typeof(ProfileActivity));
                    StartActivity(profileActivity);
                    break;
                case "Trips":
                    var tripsActivity = new Intent(this, typeof(TripsActivity));
                    StartActivity(tripsActivity);
                    break;
                case "My Friends":
                    var myFirendsActivity = new Intent(this, typeof(MyFriendsActivity));
                    StartActivity(myFirendsActivity);
                    break;
                case "Find Friends":
                    var findFriendsActivity = new Intent(this, typeof(FriendsActivity));
                    StartActivity(findFriendsActivity);
                    break;
                case "Settings":
                    var settingsActivity = new Intent(this, typeof(SettingsActivity));
                    StartActivity(settingsActivity);
                    break;
                case "Info":
                    var infoActivity = new Intent(this, typeof(InfoActivity));
                    StartActivity(infoActivity);
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerToggle.OnOptionsItemSelected(item);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }

            else
            {
                outState.PutString("DrawerState", "Closed");
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }

        private async void CheckUser()
        {
            ObservableCollection<UserItem> usersItems = await DatabaseManager.DefaultManager.GetUserItemsAsync(userItem.Name);
            if (usersItems.Count == 0)
            {
                userItem.Color = GetRandomColor();
                await DatabaseManager.DefaultManager.SaveUserItemAsync(userItem);
                Toast.MakeText(ApplicationContext, "Hello " + userItem.Name + ", thank you for register!", ToastLength.Long).Show();
            }
            else
            {
                userItem = usersItems[0];
                if (userItem.Color == null)
                {
                    userItem.Color = GetRandomColor();
                    await DatabaseManager.DefaultManager.SaveUserItemAsync(userItem);
                }
                Toast.MakeText(ApplicationContext, "Hello " + userItem.Name, ToastLength.Long).Show();
            }
        }

        private string GetRandomColor()
        {
            List<string> colors = new List<string>
            {
                "#CFD8DC",
                "#263238",
                "#26A69A",
                "#78909C",
                "#455A64",
                "#B0BEC5",
                "#80DEEA",
                "#37474F",
                "#546E7A",
                "#90A4AE",
                "#80D8FF",
                "#A7FFEB"
            };

            Random rnd = new Random();
            string color = colors[rnd.Next() % colors.Count];
            return color;
        }
    }
}
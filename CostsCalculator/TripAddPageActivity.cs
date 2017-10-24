using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using CostsCalculator.Models;
using CostsCalculator.Resources;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Android.Support.V7.App;
using Android.Text;

namespace CostsCalculator
{
    [Activity(Label = "Trip", Theme = "@style/MyTheme")]
    class TripAddPageActivity : AppCompatActivity
    {
        private Button buttonStartDate, buttonEndDate;
        private EditText titleField, descriptionField;
        private DateTime StartDate = DateTime.Now;
        private DateTime EndDate = DateTime.Now;
        private TripItem item;
        private Spinner spinner;
        private ListView listView;
        private ObservableCollection<UserItem> friendsTrip = new ObservableCollection<UserItem>();
        private ObservableCollection<UserItem> allFriends = new ObservableCollection<UserItem>();
        private List<UserItem> users;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.TripAddLayout);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            spinner = FindViewById<Spinner>(Resource.Id.spinnerFriends);
            buttonStartDate = FindViewById<Button>(Resource.Id.buttonStartDate);
            buttonEndDate = FindViewById<Button>(Resource.Id.buttonEndDate);
            titleField = FindViewById<EditText>(Resource.Id.editTextName);
            descriptionField = FindViewById<EditText>(Resource.Id.editTextDescription);
            listView = FindViewById<ListView>(Resource.Id.listViewFriends);
            listView.ItemClick += ListViewClick;

            titleField.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(25) });
            descriptionField.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(60) });

            var swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            buttonStartDate.Click += delegate
            {
                ButtonStartDate_OnClick();
            };

            buttonEndDate.Click += delegate
            {
                ButtonEndDate_OnClick();
            };

            users = new List<UserItem>();
            users.Add(new UserItem { Name = "Choose friend" });
            item = JsonConvert.DeserializeObject<TripItem>(Intent.GetStringExtra("Trip"));
            allFriends = MyFriendsActivity.friendsList;
            if (item != null)
            {
                GetFriends();
                titleField.Text = item.Name;
                descriptionField.Text = item.Description;
                StartDate = item.StartDate;
                EndDate = item.EndDate;
 
                if (allFriends.Contains(HomeActivity1.userItem))
                    allFriends.Remove(HomeActivity1.userItem);
                allFriends = new ObservableCollection<UserItem>(allFriends.Except(friendsTrip).ToList());
                allFriends = new ObservableCollection<UserItem>(allFriends.OrderBy(o => o.Name).ToList());
                users.AddRange(allFriends);
            }

            else
            {
                allFriends = new ObservableCollection<UserItem>(allFriends.OrderBy(o => o.Name).ToList());
                users.AddRange(allFriends);
            }

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
           
            var adapter = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, users);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            spinner.Adapter = adapter;
        }

        private void ListViewClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var friend = friendsTrip[e.Position];

            friendsTrip.Remove(friend);
            users.Add(friend);
            users.Remove(users.Find(x => x.Name == "Choose friend"));
            allFriends = new ObservableCollection<UserItem>(users.OrderBy(o => o.Name).ToList());
            users = new List<UserItem>();
            users.Add(new UserItem { Name = "Choose friend" });
            users.AddRange(allFriends);
            var adapter1 = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, friendsTrip);
            listView.Adapter = adapter1;
            var adapter2 = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, users);
            spinner.Adapter = adapter2;
            spinner.SetSelection(0);
        }

        private bool ShowAlert()
        {
            bool returnValue = false;
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);

            alert.SetTitle("Do you want add to your friends?");

            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                returnValue = true;
            });

            alert.SetNegativeButton("No", (senderAlert, args) => {
               
            });
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });

            return returnValue;
        }

        private async void GetFriends()
        {
            friendsTrip = await DatabaseManager.DefaultManager.GetUsersTripsItemsAsync(item);
            if (friendsTrip.Contains(HomeActivity1.userItem)) friendsTrip.Remove(HomeActivity1.userItem);
            friendsTrip = new ObservableCollection<UserItem>(friendsTrip.OrderBy(o => o.Name).ToList());
            var adapter = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, friendsTrip);
            listView.Adapter = adapter;
        }

        private void ButtonEndDate_OnClick()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                Toast.MakeText(this, time.ToLongDateString(), ToastLength.Short).Show();
                EndDate= time;
                buttonEndDate.Text = time.ToShortDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
           
        }

        private void ButtonStartDate_OnClick()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                Toast.MakeText(this, time.ToLongDateString(), ToastLength.Short).Show();
                StartDate = time;
                buttonStartDate.Text = time.ToShortDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
            
        }

        private void ButtonAddClicked()
        {
            SaveData();
            Notification not = new Notification();
            not.Notify("zielna16@gmail.com", "new trip", "blablabla");
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            {
                string friendString = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
                if (!friendString.Equals("Choose friend"))
                {
                    Toast.MakeText(this, friendString + " added", ToastLength.Long).Show();
                    var friend = users[e.Position];
                    users.Remove(friend);
                    friendsTrip.Add(friend);
                    friendsTrip = new ObservableCollection<UserItem>(friendsTrip.OrderBy(o => o.Name).ToList());
                    var adapter = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, users);
                    var adapter1 = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, friendsTrip);
                    listView.Adapter = adapter1;
                    spinner.Adapter = adapter;
                    spinner.SetSelection(0);
                }
            }
        }

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            var adapter1 = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, friendsTrip);
            listView.Adapter = adapter1;
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.tripAddToolbar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);

                alert.SetTitle("Do you want to leave?");

                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    
                    Finish();
                    
                });

                alert.SetNegativeButton("No", (senderAlert, args) => {
                    
                });

                RunOnUiThread(() => {
                    alert.Show();
                });

               
            }
            else if(item.ItemId == Resource.Id.done)
            {
                ButtonAddClicked();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SaveData()
        {
            string title = titleField.Text;
            string description = descriptionField.Text;
            bool flag = true;

             if (buttonStartDate.Text == "CHOOSE DATE" || buttonEndDate.Text == "CHOOSE DATE")
             {
                flag = false;
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);

                alert.SetTitle("Please set trip date. Do you want to set today's day?");

                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    if (buttonStartDate.Text == "CHOOSE DATE") buttonStartDate.Text = StartDate.ToShortDateString();
                    if (buttonEndDate.Text == "CHOOSE DATE") buttonEndDate.Text = EndDate.ToShortDateString();

                });

                alert.SetNegativeButton("No", (senderAlert, args) => {
                });
                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
             if (flag)
             {
                if (StartDate.Date > EndDate.Date)
                    Toast.MakeText(this, "Start day has to be previous than end day", ToastLength.Long).Show();
                else
                {

                    if (!title.Equals(String.Empty) && title[0] != ' ' && title[title.Length - 1] != ' ')
                    {
                        if (description.Length <= 60)
                        {
                            AddItem(title, description);
                            Finish();
                        }
                        else Toast.MakeText(this, "Description can have max 60 characters", ToastLength.Long).Show();
                    }
                    else
                    {

                        Toast.MakeText(this, "Enter valid title and description", ToastLength.Long).Show();
                    }
                }
            }

           
          }

        async void AddItem(string title, string description)
        {
            TripItem item;
            if (this.item == null)
            {
                friendsTrip.Add(HomeActivity1.userItem);
                item = new TripItem()
                {
                    Name = title,
                    Description = description,
                    StartDate = StartDate.Date,
                    EndDate = EndDate.Date,
                    OwnerId = HomeActivity1.userItem.Id,
                    IsCurrent = true
                };

                await DatabaseManager.DefaultManager.SaveTripItemAsync(item, new ObservableCollection<UserItem>(friendsTrip));
                HistoryItem historyItem = new HistoryItem(item.Id, HomeActivity1.userItem.Id, "created this trip");
                await DatabaseManager.DefaultManager.SaveHistoryItemAsync(historyItem);
            }
            else
            {
                HistoryItem historyItem;
                item = this.item;
                if (!friendsTrip.Contains(HomeActivity1.userItem)) friendsTrip.Add(HomeActivity1.userItem);
                if (item.Name != title)
                {
                    historyItem = new HistoryItem(item.Id,
                        HomeActivity1.userItem.Id, "changed trip name from " + item.Name + " to " + title);
                    await DatabaseManager.DefaultManager.SaveHistoryItemAsync(historyItem);
                    item.Name = title;
                }
                if (item.Description != description)
                {
                    Toast.MakeText(this, "Change description", ToastLength.Long).Show();
                    historyItem = new HistoryItem(item.Id,
                          HomeActivity1.userItem.Id, "changed description to " + description);
                      await DatabaseManager.DefaultManager.SaveHistoryItemAsync(historyItem);
                      item.Description = description;
                  }
                  if (item.StartDate != StartDate.Date)
                  {
                      historyItem = new HistoryItem(item.Id,
                          HomeActivity1.userItem.Id, "changed start date from " + item.StartDate.ToShortDateString() + " to " + StartDate.Date.ToShortDateString());
                      await DatabaseManager.DefaultManager.SaveHistoryItemAsync(historyItem);
                      item.StartDate = StartDate.Date;
                  }
                  if (item.EndDate != EndDate.Date)
                  {
                      historyItem = new HistoryItem(item.Id,
                          HomeActivity1.userItem.Id, "changed end date from " + item.EndDate.ToShortDateString()+ " to " + EndDate.Date.ToShortDateString());
                      await DatabaseManager.DefaultManager.SaveHistoryItemAsync(historyItem);
                      item.EndDate = EndDate.Date;
                  }
                Toast.MakeText(this, "Save", ToastLength.Long).Show();
                await DatabaseManager.DefaultManager.SaveTripItemAsync(item, new ObservableCollection<UserItem>(friendsTrip));

            }
        }
    }
}
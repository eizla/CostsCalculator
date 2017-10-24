using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;
using Android.Support.V7.App;
using System.Text.RegularExpressions;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Text;
using Microsoft.Identity.Client;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace CostsCalculator
{
    [Activity(Label = "Add Payment", Theme = "@style/MyTheme")]
    class PaymentAddActivity : AppCompatActivity
    {
        private PaymentItem paymentItem = null;
        List<Tuple<string, decimal>> friendsToPayment;
        private ObservableCollection<UserItem> friends;
        //private int friendsToPaymentCounter = 0;
        Spinner spinner;
        private List<UserItem> friendsList = new List<UserItem>();
        private ObservableCollection<UserPaymentItem> payments = new ObservableCollection<UserPaymentItem>();
        private string friendString;
        private TripItem tripItem;
        private ViewSwitcher viewSwitcher;
        ListView listView;
        TextView editText;
        EditText payName;
        TextView payNameStatic;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

          
                tripItem = JsonConvert.DeserializeObject<TripItem>(Intent.GetStringExtra("Trip"));
                friends = new ObservableCollection<UserItem>();
                friends = JsonConvert.DeserializeObject<ObservableCollection<UserItem>>(Intent.GetStringExtra("Friends"));
            try
            {
                paymentItem = JsonConvert.DeserializeObject<PaymentItem>(Intent.GetStringExtra("Payment"));
            }
            catch (Exception) {
                paymentItem = null;
            }
            
            friendsList.Add(new UserItem { Name = "Choose friend"});
            friendsList.AddRange(friends);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.PaymentAddLayout);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarAddPay);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);



            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinnerFriendsToPayment);
            listView = FindViewById<ListView>(Resource.Id.listViewFriendsToPayment);
            editText = FindViewById<TextView>(Resource.Id.textView6);
            viewSwitcher = FindViewById<ViewSwitcher>(Resource.Id.viewSwitcher1);
            //viewSwitcher.ShowNext();
            payName = viewSwitcher.FindViewById<EditText>(Resource.Id.editTextName);
            payNameStatic = viewSwitcher.FindViewById<TextView>(Resource.Id.editTextNameNot);
            payNameStatic.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(20) });
            payName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(20) });

            //editText.TextChanged += EditText_TextChanged;
            var swipeContainer = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += SwipeContainer_Refresh;
            var button = FindViewById<Button>(Resource.Id.button1);
            var button2 = FindViewById<Button>(Resource.Id.button2);
       
            button.Click += delegate {
                buttonClicked();
            };

            button2.Click += Button2_Click;
            spinner.ItemSelected += spinner_ItemSelected;

            var adapter = new ArrayAdapter<UserItem>(this, Android.Resource.Layout.SimpleSpinnerItem, friendsList);

            spinner.Adapter = adapter;
            friendsToPayment = new List<Tuple<string, decimal>>();
            if (paymentItem != null)
            {
                var friends_List = new List<UserItem>(friends);
                editText.Text = "" + paymentItem.Amount;
                payNameStatic.Text = paymentItem.Name;
                button.Text = "Edit";
                button2.Visibility = ViewStates.Visible;
                viewSwitcher.ShowNext();
                
               // foreach (var pay in payments)
               // {
                 //   var item = friends_List.Find(i => i.Id == pay.UserId);
                //    Tuple <string, decimal> tup = new Tuple<string, decimal>(item.Name, pay.Amount);
                //    friendsToPayment.Add(tup);
                //}


            }
            getPayments();
            //var adapterPayemnt = new PaymentAddAdapter(this, friendsToPayment);
            
            //listView.Adapter = adapterPayemnt;
            

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Do you want to delete whole payment?");

            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                DatabaseManager.DefaultManager.DeletePaymentItemAsync(this.paymentItem);
                this.OnBackPressed();


            });

            alert.SetNegativeButton("No", (senderAlert, args) => {
            });
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
            
        }

        private async void getPayments()
        {
            if (paymentItem != null)
            {
                payments = await DatabaseManager.DefaultManager.GetPaymentUsersForPaymentAsync(paymentItem);
                var friends_List = new List<UserItem>(friends);
                
                foreach (var pay in payments)
                {
                    var item = friends_List.Find(i => i.Id == pay.UserId);
                    
                    Tuple<string, decimal> tup;
                    if (item != null)
                    {
                        tup = new Tuple<string, decimal>(item.Name, pay.Amount);
                        if(!friendsToPayment.Exists(s => s.Item1 == tup.Item1))
                        {
                            friendsToPayment.Add(tup);
                        }
                    }
                }
                
                var adapterPayemnt = new PaymentAddAdapter(this, friendsToPayment);
                listView.Adapter = adapterPayemnt;
                listView.ItemClick += ListView_ItemClick;

            }
        }


        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            getPayments();
            (sender as SwipeRefreshLayout).Refreshing = false;

        }


        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Do you want to delete this payment?");

            alert.SetPositiveButton("Yes",  (senderAlert, args) =>
            {
                friendsToPayment.Remove(friendsToPayment[e.Position]);
                var adapterPayemnt = new PaymentAddAdapter(this, friendsToPayment);

                listView.Adapter = adapterPayemnt;
                try
                {
                    editText.Text = friendsToPayment.Where(s => s.Item2 > 0).Sum(s => s.Item2).ToString();
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "Sum to big!", ToastLength.Short).Show();
                }
                //apter = new FriendsCustomAdapter(this, users);
                //tData = FindViewById<ListView>(Resource.Id.listViewFriends);
                //tData.Adapter = adapter;
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
                return true;

            }
         /*   else if (item.ItemId == Resource.Id.done)
            {
                buttonClicked();
                return true;
            }*/

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.paymentAdd, menu);
            return true;
        }

        public  void buttonClicked()
        {
            if (!String.IsNullOrWhiteSpace(payName.Text.ToString()))
            {
                payNameStatic.Text = payName.Text;
            }
            var name = payNameStatic.Text.ToString();
            
            if (!String.IsNullOrWhiteSpace(name) && friendsToPayment.Count != 0)
            {               
                if (friendsToPayment.Sum(s => s.Item2) != 0)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle(String.Format("Payment missmatch, difference {0} will be added to your account!", -friendsToPayment.Sum(s => s.Item2)));
                    //Toast.MakeText(this, "Payment missmatch, difference will be added to your account!", ToastLength.Long).Show();
                    alert.SetPositiveButton("Accept", (senderAlert, args) =>
                    {
                        saveToDatabes();


                    });

                    alert.SetNegativeButton("Make correction", (senderAlert, args) =>
                    {

                    });

                    RunOnUiThread(() =>
                    {
                        alert.Show();

                    });


                }
                else
                {
                    saveToDatabes();
                }
            }
            else
            {
                Toast.MakeText(this, "Name and Payments can not be empty", ToastLength.Short).Show();
            }

        }

        private async void saveToDatabes()
        {
            try
            {
                var name = payNameStatic.Text.ToString().Trim();
                decimal cor = friendsToPayment.Sum(s => s.Item2);
                var my1 = friendsToPayment.Find(s => s.Item1 == HomeActivity1.userItem.Name);
                decimal my;
                if(my1 != null)
                {
                    my = my1.Item2;
                }
                else
                {
                    my = 0;
                }

                decimal sum;
                if (cor < 0)
                {
                    if (my >= 0)
                    {
                        sum = friendsToPayment.Where(s => s.Item2 < 0).Sum(s => Math.Abs(s.Item2));
                    }
                    else
                    {
                        sum = friendsToPayment.Where(s => s.Item2 > 0).Sum(s => s.Item2);
                    }
                }
                else
                {
                    if (my > 0)
                    {
                        sum = friendsToPayment.Where(s => s.Item2 < 0).Sum(s => Math.Abs(s.Item2));

                    }
                    else
                    {
                        sum = friendsToPayment.Where(s => s.Item2 > 0).Sum(s => s.Item2);
                    }
                    }
                PaymentItem payitem;
                if (paymentItem == null)
                {
                    payitem = new PaymentItem()
                    {
                        Amount = sum,
                        TripId = tripItem.Id,
                        Name = name,
                        UserCreatingPaymentId = HomeActivity1.userItem.Id

                    };
                    await DatabaseManager.DefaultManager.SavePaymentItemAsync(payitem);
                }
                else
                {
                    payitem = paymentItem;
                    payitem.Amount = sum;
                    await DatabaseManager.DefaultManager.SavePaymentItemAsync(payitem);
                }

                
                
                bool isHeHere = false;
                if (paymentItem == null)
                {
                    foreach (Tuple<string, decimal> tuple in friendsToPayment)
                    {
                        var user = friends.Where(s => s.Name == tuple.Item1).First().Id;
                        var amount = tuple.Item2;
                        if (user == HomeActivity1.userItem.Id)
                        {
                            isHeHere = true;
                            amount -= cor;
                        }
                        var payuseritem = new UserPaymentItem()
                        {
                            PayId = payitem.Id,
                            UserId = user,
                            Amount = amount

                        };
                        await DatabaseManager.DefaultManager.SaveUserPaymentItemAsync(payuseritem);

                    }
                    if (!isHeHere)
                    {
                        var payuseritem = new UserPaymentItem()
                        {
                            PayId = payitem.Id,
                            UserId = HomeActivity1.userItem.Id,
                            Amount = (-cor)

                        };
                        await DatabaseManager.DefaultManager.SaveUserPaymentItemAsync(payuseritem);
                    }
                }
                else
                {
                    foreach (Tuple<string, decimal> tuple in friendsToPayment)
                    {
                        var user = friends.Where(s => s.Name == tuple.Item1).First().Id;
                        var amount = tuple.Item2;
                        if (user == HomeActivity1.userItem.Id)
                        {
                            isHeHere = true;
                            amount -= cor;
                        }
                        var payuseritem = payments.Where(s => s.UserId == user).FirstOrDefault();
                        
                        if (payuseritem != null)
                        {
                            payuseritem.Amount = amount;
                            payments.Remove(payuseritem);
                        }
                        else
                        {
                            payuseritem = new UserPaymentItem()
                            {
                                PayId = payitem.Id,
                                UserId = user,
                                Amount = amount

                            };
                        }
                        await DatabaseManager.DefaultManager.SaveUserPaymentItemAsync(payuseritem);

                    }
                    if (!isHeHere)
                    {
                        var payuseritem = payments.Where(s => s.UserId == HomeActivity1.userItem.Id).FirstOrDefault();
                        if (payuseritem != null)
                        {
                            payuseritem.Amount = (-cor);
                            if (payuseritem.Amount != 0)
                            {
                                payments.Remove(payuseritem);
                            }
                        }
                        else
                        {
                            payuseritem = new UserPaymentItem()
                            {
                                PayId = payitem.Id,
                                UserId = HomeActivity1.userItem.Id,
                                Amount = (-cor)

                            };
                        }
                        if (payuseritem.Amount != 0)
                        {
                            await DatabaseManager.DefaultManager.SaveUserPaymentItemAsync(payuseritem);
                        }
                    }
                    foreach(UserPaymentItem paythis in payments)
                    {
                        DatabaseManager.DefaultManager.DeleteUserPaymentItemAsync(paythis);
                    }
                }
                HistoryItem historyItem;
                if(paymentItem == null) historyItem = new HistoryItem(tripItem.Id, HomeActivity1.userItem.Id, "added payment, amount: " + sum);
                else historyItem = new HistoryItem(tripItem.Id, HomeActivity1.userItem.Id, "edited payment, amount: " + sum);
                await DatabaseManager.DefaultManager.SaveHistoryItemAsync(historyItem);
                base.OnBackPressed();
            }
            catch
            {
                Toast.MakeText(this, "Upps", ToastLength.Short).Show();
            }
        }


        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            spinner = (Spinner)sender;
            {
                friendString = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
                if (!friendString.Equals("Choose friend"))
                {

                    FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    FriendsToPaymentDialogFragment t = new FriendsToPaymentDialogFragment();
                    t.DialogClosed += OnDialogClosed;
                    t.Show(transaction, "dialog fragment");
                    
                }
            }
            

        }
        void OnDialogClosed(object sender, decimal e)
        {
            var item = friendsToPayment.Where(s => s.Item1 == friendString).FirstOrDefault();
            if (null != item )
            {
                var t = Tuple.Create<string, decimal>(friendString, item.Item2 + e);
                friendsToPayment.Remove(item);
                friendsToPayment.Add(t);
            }
            else
            {
                friendsToPayment.Add(Tuple.Create<string, decimal>(friendString, e));
            }
            friendsToPayment.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            //friendsToPayment = friendsToPayment.OrderBy(s => s.Item2).ToList();
            var adapterPayemnt = new PaymentAddAdapter(this, friendsToPayment);

            listView.Adapter = adapterPayemnt;
            spinner.SetSelection(0);
            try
            {
                editText.Text = Math.Max( friendsToPayment.Where(s => s.Item2 > 0).Sum(s => s.Item2), friendsToPayment.Where(s => s.Item2 < 0).Sum(s => Math.Abs(s.Item2))).ToString();
            }
            catch(Exception)
            {
                Toast.MakeText(this, "Sum to big!", ToastLength.Short).Show();
            }
        }
    }
}
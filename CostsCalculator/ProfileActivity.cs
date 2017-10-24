using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CostsCalculator.Models;
using Newtonsoft.Json;
using Android.Support.V7.App;
using Android.Text;
using Microsoft.Identity.Client;
using Android.Graphics;

namespace CostsCalculator
{
    [Activity(Label = "Profile", Theme = "@style/MyTheme")]
    class ProfileActivity : AppCompatActivity
    {
        private Button edit;
        public static string Mail = string.Empty;
        private EditText description;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            CheckUser();
            
            SetContentView(Resource.Layout.Profile);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            description = FindViewById<EditText>(Resource.Id.editTextDescription);
            description.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(100) });
            if (HomeActivity1.userItem.Description == null)
                description.Text = "Type your description";
            else
                description.Text = HomeActivity1.userItem.Description;
            edit = FindViewById<Button>(Resource.Id.editButton);
            edit.Click += delegate
            {
                EditDescription_OnClicked();
            };
            TextView Name = FindViewById<TextView>(Resource.Id.textViewName);
            Name.Text = HomeActivity1.userItem.Name;

            var textImage = FindViewById<TextView>(Resource.Id.imageViewProfile);

            textImage.SetBackgroundColor(Color.ParseColor(HomeActivity1.userItem.Color));
            textImage.Text = Name.Text[0].ToString().ToUpper();
        }

        private async void EditDescription_OnClicked()
        {
            if (edit.Text == "Edit description")
            {
                edit.Text = "Save";
                description.Enabled = true;
            }
            else
            {
                edit.Text = "Edit description";
                description.Enabled = false;
                HomeActivity1.userItem.Description = description.Text;
                await DatabaseManager.DefaultManager.SaveUserItemAsync(HomeActivity1.userItem);
            }
        }


        private async void CheckUser()
        {
            ObservableCollection<UserItem> usersItems = await DatabaseManager.DefaultManager.GetUserItemsAsync(HomeActivity1.userItem.Name);
            if (usersItems.Count == 0)
            {
                await DatabaseManager.DefaultManager.SaveUserItemAsync(HomeActivity1.userItem);
            }
            else HomeActivity1.userItem = usersItems[0];
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // ActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}
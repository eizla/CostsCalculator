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
using Android.Support.V7.App;
using CostsCalculator.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;


namespace CostsCalculator
{
    [Activity(Label = "Settings", Theme = "@style/MyTheme")]
    class SettingsActivity : AppCompatActivity
    {
        RadioButton radioButton;
        TextView textView;
        private Button nameButton, passButton;
        bool isChecked;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Settings);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            textView = FindViewById<TextView>(Resource.Id.textView1);
            radioButton = FindViewById<RadioButton>(Resource.Id.radioButton1);

            AdjustViewsToDevice();

            nameButton = FindViewById<Button>(Resource.Id.nameButton);
            passButton = FindViewById<Button>(Resource.Id.passButton);

            nameButton.Click += delegate
            {
                EditProfile_OnClicked();
            };

            passButton.Click += delegate
            {
                ChangePassword_OnClicked();
            };

            isChecked = radioButton.Checked;
            radioButton.Click += delegate
            {
                radioButtonClicked();
            };
           
        }

        private async void ChangePassword_OnClicked()
        {
            try
            {
                await MainActivity.AuthenticationClient.AcquireTokenAsync(Constants.Scopes, MainActivity.ar.Scope[0], UiOptions.ActAsCurrentUser,
                   string.Empty, null, Constants.Authority, Constants.ResetPasswordPolicy);
            }
            catch (Exception) { }
        }

        private async void EditProfile_OnClicked()
        {
            try
            {
                AuthenticationResult result = await MainActivity.AuthenticationClient.AcquireTokenAsync(Constants.Scopes,
                MainActivity.ar.Scope[0], UiOptions.ActAsCurrentUser, string.Empty, null, Constants.Authority, Constants.EditingPolicy);
                MainActivity.ar = result;
                HomeActivity1.userItem.Name = result.User.Name;
                await DatabaseManager.DefaultManager.SaveUserItemAsync(HomeActivity1.userItem);
                TextView Name = FindViewById<TextView>(Resource.Id.textViewName);
                Name.Text = HomeActivity1.userItem.Name;
            }
            catch (Exception) { }
        }



        public void radioButtonClicked()
        {
            
            if (isChecked) {
                if (ProfileActivity.Mail.Equals(string.Empty))
                {
                    
                }

                radioButton.Checked = false;
                isChecked = false;
             }
            else
            {
                radioButton.Checked = true;
                isChecked = true;
                
            }
        }
        void OnDialogClosed(object sender, decimal e)
        {
           
        }

        private void AdjustViewsToDevice()
        {
            
            RelativeLayout.LayoutParams ll = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ll.SetMargins(10, 10, 10, 10); //left, top, right, bottom

            ll.Width = MainActivity.widthInDp - 100;
            textView.LayoutParameters = ll;
            textView.RequestLayout();



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
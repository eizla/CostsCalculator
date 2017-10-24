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

namespace CostsCalculator
{
    [Activity(Label = "Info", Theme = "@style/MyTheme")]
    class InfoActivity : AppCompatActivity
    {
        Button buttonJan, buttonJakub, buttonSebastian, buttonJoanna;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Info);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            buttonJan = FindViewById<Button>(Resource.Id.buttonJan);
            buttonJakub = FindViewById<Button>(Resource.Id.buttonJakub);
            buttonSebastian = FindViewById<Button>(Resource.Id.buttonSebastian);
            buttonJoanna = FindViewById<Button>(Resource.Id.buttonJoanna);

            buttonJan.Click += delegate
            {
                buttonClicked("Jan");

            };

            buttonJakub.Click += delegate
            {
                buttonClicked("Jakub");

            };

            buttonSebastian.Click += delegate
            {
                buttonClicked("Sebastian");

            };

            buttonJoanna.Click += delegate
            {
                buttonClicked("Joanna");

            };
        }

        private void buttonClicked(String name)
        {
            Intent intent = new Intent(Intent.ActionView);

            switch (name)
            {
                case "Jan":
                    // intent.SetData(Android.Net.Uri.Parse(""));
                    // StartActivity(intent);
                    Toast.MakeText(this, "Contact at mail address: jw.gor07@gmail.com", ToastLength.Long).Show();
                    break;
                case "Jakub":
                    intent.SetData(Android.Net.Uri.Parse("https://www.linkedin.com/in/jakub-piekarz-251bb0142/"));
                    StartActivity(intent);
                    break;
                case "Sebastian":
                    intent.SetData(Android.Net.Uri.Parse("https://www.linkedin.com/in/sebastian-pustelnik-56730311a/"));
                    StartActivity(intent);
                    break;
                case "Joanna":
                    intent.SetData(Android.Net.Uri.Parse("http://www.linkedin.com/in/joanna-zieli%C5%84ska-89698112a/"));
                    StartActivity(intent);
                    break;

            }
           
           
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnContextItemSelected(item);
        }
    }
}
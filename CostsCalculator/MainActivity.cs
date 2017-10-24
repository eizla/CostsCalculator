using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using UiOptions = Microsoft.Identity.Client.UiOptions;
using Android.Graphics;
using Android.Net;
using Android.Net.Wifi;

namespace CostsCalculator
{
    //, Theme = "@style/Theme.AppCompat.Light.NoActionBar"
    [Activity(Label = "CostsCalculator", MainLauncher = true, Icon = "@drawable/Icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        public static PublicClientApplication AuthenticationClient = new PublicClientApplication(Constants.ApplicationID);
        private Button button;
        private TextView textView;
        private ImageView imageView;
        public static int widthInDp { get; set; }
        public static int heightInDp { get; set; }
        private WifiManager wifiManager;
        public static AuthenticationResult ar;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            CurrentPlatform.Init();

            SetContentView(Resource.Layout.Main);
            button = FindViewById<Button>(Resource.Id.enterButton);
            textView = FindViewById<TextView>(Resource.Id.welcomeTextView);
            imageView = FindViewById<ImageView>(Resource.Id.imageView1);

            AdjustViewsToDevice();
            checkInternetConnection();

            button.Click += delegate
            {
                ButtonClicked();
            };
        }

        private void checkInternetConnection()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

            wifiManager = (WifiManager)this.GetSystemService(Context.WifiService);
            bool wifiEnabled = wifiManager.IsWifiEnabled;
            if (!wifiEnabled)
            {
                Toast.MakeText(this, "Turning on wifi", ToastLength.Short).Show();
                wifiManager.SetWifiEnabled(true);
            }
        }

        private void AdjustViewsToDevice()
        {
            var metrics = Resources.DisplayMetrics;
            widthInDp = (int)(metrics.WidthPixels);
            heightInDp = (int)(metrics.HeightPixels);

            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ll.SetMargins(10, 10, 10, 10); //left, top, right, bottom
            ll.Width = widthInDp - 200;
            ll.Gravity = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
            button.LayoutParameters = ll;
            button.RequestLayout();

            textView.LayoutParameters = ll;
            textView.RequestLayout();

            LinearLayout.LayoutParams ll_2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ll_2.SetMargins(10, 10, 10, 10);
            ll_2.Width = widthInDp - 200;
            ll_2.Height = heightInDp - 800;
            ll_2.Gravity = GravityFlags.CenterHorizontal;
            imageView.LayoutParameters = ll_2;
            imageView.RequestLayout();
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        private async void ButtonClicked()
        {
            AuthenticationClient.PlatformParameters = new PlatformParameters(this);
            try
            {
                 button.Visibility = ViewStates.Invisible;
               
                 var result = await AuthenticationClient.AcquireTokenAsync(
                  Constants.Scopes,
                  string.Empty,
                  UiOptions.SelectAccount,
                  string.Empty,
                  null,
                  Constants.Authority,
                  Constants.SignUpSignInPolicy);

                ar = result;
                
                var activity = new Intent(this, typeof(HomeActivity1));
                activity.PutExtra("User", JsonConvert.SerializeObject(result.User));
                StartActivity(activity);
            }
            catch (MsalException ex)
            {
                button.Visibility = ViewStates.Visible;
                if (ex.Message != null && ex.Message.Contains("AADB2C90118"))
                {
                    await OnForgotPassword();
                }
                if (ex.ErrorCode != "authentication_canceled")
                {
                    Toast.MakeText(ApplicationContext, "Some error during logging", ToastLength.Long).Show();
                }
            }
        }

        private async Task OnForgotPassword()
        {
            try
            {
                await AuthenticationClient.AcquireTokenAsync(Constants.Scopes, string.Empty, UiOptions.SelectAccount, string.Empty, null, Constants.Authority, Constants.ResetPasswordPolicy);
            }
            catch (Exception) { }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (wifiManager.IsWifiEnabled)
            {
                wifiManager.SetWifiEnabled(false);
            }
        }
    }
}


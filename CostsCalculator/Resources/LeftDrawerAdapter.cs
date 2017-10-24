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
using Java.Lang;

namespace CostsCalculator
{
    class LeftDrawerAdapter : BaseAdapter
    {
        private Activity activity;

        private List<string> dataSet = new List<string> { "Profile", "Trips", "My Friends", "Find Friends", "Settings", "Info" };

        public LeftDrawerAdapter(Activity activity)
        {
            this.activity = activity;

        }

        public override int Count
        {
            get
            {
                return dataSet.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return dataSet[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.LeftDrawerListItem, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textView);
            var imageView = view.FindViewById<ImageView>(Resource.Id.imageView);

            txtName.Text = dataSet[position];
            switch (dataSet[position])
            {
                case "Profile":
                    imageView.SetImageResource(Resource.Drawable.ic_person);
                    AdjustViewsToDevice(imageView);
                    break;
                case "Trips":
                    imageView.SetImageResource(Resource.Drawable.ic_travel);
                    AdjustViewsToDevice(imageView);
                    break;
                case "My Friends":
                    imageView.SetImageResource(Resource.Drawable.ic_group);
                    AdjustViewsToDevice(imageView);
                    break;
                case "Find Friends":
                    imageView.SetImageResource(Resource.Drawable.ic_group_add);
                    AdjustViewsToDevice(imageView);
                    break;
                case "Settings":
                    imageView.SetImageResource(Resource.Drawable.ic_settings);
                    AdjustViewsToDevice(imageView);
                    break;
                case "Info":
                    imageView.SetImageResource(Resource.Drawable.ic_info);
                    AdjustViewsToDevice(imageView);
                    break;

            }

            return view;
        }

        private void AdjustViewsToDevice(ImageView imageView)
        {


            RelativeLayout.LayoutParams ll = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ll.SetMargins(10, 10, 10, 10); //left, top, right, bottom
            ll.Width = 100;
            ll.Height = 100;

            imageView.LayoutParameters = ll;
            imageView.RequestLayout();

        }
    }
}
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
using System.Collections.ObjectModel;

namespace CostsCalculator.Resources
{
    class AlgoAdapter : BaseAdapter
    {

        private Activity activity;

        //template
        private ObservableCollection<string> summary = new ObservableCollection<string>();

        public AlgoAdapter(Activity activity, ObservableCollection<string> summary)
        {
            this.activity = activity;
            this.summary = summary;
        }
        public override int Count
        {
            get
            {
                return summary.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return summary[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.Algo, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textViewName);

            //todo: change to name
            txtName.Text = summary[position];



            return view;
        }


    }

}
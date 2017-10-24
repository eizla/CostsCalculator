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
using CostsCalculator.Models;
using Java.Lang;
using Android.Graphics;

namespace CostsCalculator.Resources
{
  
    class TripsCustomAdapter : BaseAdapter, IFilterable
    {
        private Activity activity;
        public Filter Filter { get; private set; }

        private ObservableCollection<TripItem> trips;

        public TripsCustomAdapter(Activity activity, ObservableCollection<TripItem> trips)
        {
            this.activity = activity;
            this.trips = trips;
        }
        public override int Count
        {
            get
            {
                return trips.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.TripListViewDataTemplate, parent, false);

            var textImage = view.FindViewById<TextView>(Resource.Id.circleImage);
            var txtName = view.FindViewById<TextView>(Resource.Id.textViewName);
            var txtStartDate = view.FindViewById<TextView>(Resource.Id.textViewStartDate);
            var txtEndDate = view.FindViewById<TextView>(Resource.Id.textViewEndDate);
            var txtDescription = view.FindViewById<TextView>(Resource.Id.textViewDescription);

            textImage.SetBackgroundColor(Color.ParseColor("#263238"));
            textImage.Text = trips[position].Name[0].ToString().ToUpper();

            txtName.Text = "Name: " + trips[position].Name;
            txtStartDate.Text = "Start Date:\n " +trips[position].StartDate.ToShortDateString();
            txtEndDate.Text = "End Date:\n " +trips[position].EndDate.ToShortDateString();
            if (trips[position].Description != string.Empty)
                txtDescription.Text = "Description: " + trips[position].Description;
            else txtDescription.Visibility = ViewStates.Invisible;

            return view;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }

        public void filter(TripsCustomAdapter adapter, string constraint)
        {
            if (constraint != null && constraint.Length > 0)
            {
                ObservableCollection<TripItem> matchList = new ObservableCollection<TripItem>();
                foreach (TripItem item in adapter.trips.ToList())

                {
                    
                    if (item.Name.StartsWith(constraint.ToString()))
                    {
                        matchList.Add(item);
                    }
                }

                adapter.trips = matchList;
                adapter.NotifyDataSetChanged();
            }

        }

    }
}
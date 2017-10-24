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
using CostsCalculator.Models;

namespace CostsCalculator
{
    class TripsActivityDialogFragement : DialogFragment
    {
        private TripItem tripItem;

        public TripsActivityDialogFragement(TripItem tripItem)
        {
            this.tripItem = tripItem;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.tripsActivityDialogFragment, container, false);

            return view;
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            
            switch (item.ItemId)
            {
                
                case Resource.Id.buttonYes:
                    DatabaseManager.DefaultManager.DeleteTripItem(tripItem);
                    return true;
                case Resource.Id.buttonNo:
                    return false;
            }

            return base.OnContextItemSelected(item);
        }
    }
}
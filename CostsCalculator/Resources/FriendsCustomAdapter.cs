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

    public class ViewHolder : Java.Lang.Object
    {
        public TextView txtName { get; set; }
        public TextView txtEmail { get; set; }
    }

    class FriendsCustomAdapter : BaseAdapter, IFilterable
    {
        private Activity activity;
        public Filter Filter { get; private set; }
        private ImageView imageView;
        private List<string> colors = new List<string>
        {
            "#CFD8DC",
            "#263238",
            "#26A69A",
            "#78909C",
            "#455A64",
            "#B0BEC5",
            "#80DEEA",
            "#37474F",
            "#546E7A",
            "#90A4AE",
            "#80D8FF",
            "#A7FFEB"
        };

        public ObservableCollection<UserItem> persons;

        public FriendsCustomAdapter(Activity activity, ObservableCollection<UserItem> persons)
        {
            this.activity = activity;
            this.persons = persons;
            Filter = new FriendsFilter(this);
        }

        public override int Count
        {
            get
            {
                return persons.Count;
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
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.FriendsListViewDataTemplate, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textViewName);
            txtName.Text = persons[position].Name;
            var textImage = view.FindViewById<TextView>(Resource.Id.imageView);
            Random rnd = new Random();
            string color = colors[rnd.Next() % colors.Count];
            textImage.SetBackgroundColor(Color.ParseColor(color));
            textImage.Text = persons[position].Name[0].ToString().ToUpper();
            return view;
        }

        private void AdjustViewsToDevice()
        {


            RelativeLayout.LayoutParams ll = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ll.SetMargins(10, 10, 10, 10); //left, top, right, bottom
            ll.Width = 100;
            ll.Height = 100;

          //  imageView.LayoutParameters = ll;
            //imageView.RequestLayout();



        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }

        public void filter(FriendsCustomAdapter adapter, string constraint)
        {
            if (constraint != null && constraint.Length > 0)
            {
                ObservableCollection<UserItem> matchList = new ObservableCollection<UserItem>();
                foreach (UserItem item in adapter.persons.ToList())

                {
                    if (item.Name.StartsWith(constraint.ToString()))
                    {
                        matchList.Add(item);
                    }
                }

                adapter.persons = matchList;
                adapter.NotifyDataSetChanged();
            }

            
        }

        private class FriendsFilter : Filter
        {
            private  readonly FriendsCustomAdapter friendsCustomAdapter;

            public FriendsFilter(FriendsCustomAdapter friendsCustomAdapter)
            {
                this.friendsCustomAdapter = friendsCustomAdapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {

                FilterResults results = new FilterResults();
                if (constraint != null && constraint.Length() > 0)
                {
                    List<UserItem> matchList = new List<UserItem>();
                    foreach (UserItem item in friendsCustomAdapter.persons.ToList())

                    {
                        if (item.Name.Contains(constraint.ToString().ToUpper()))
                        {
                            matchList.Add(item);
                        }
                    }

                    Java.Lang.Object[] resultsValues;
                    resultsValues = new Java.Lang.Object[matchList.Count];
                    for (int i = 0; i < matchList.Count; i++)
                    {
                        UserItem myObj = matchList[i];

                        resultsValues[i] = new JavaObjectWrapper<UserItem>() { Obj = myObj };
                    }

                    results.Count = matchList.Count;
                    results.Values = resultsValues;
                }
                return results;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                ObservableCollection<UserItem> items = new ObservableCollection<UserItem>();

                if (constraint != null && constraint.Length() > 0)
                {
                    Java.Lang.Object[] values = (Java.Lang.Object[])results.Values;
                    for (int i = 0; i < values.Length; i++)
                    {

                        items.Add(new UserItem
                        {
                            Name = values.GetValue(i).ToString()
                        }
                            );
                    }
                }
                
                friendsCustomAdapter.persons = items;
                friendsCustomAdapter.NotifyDataSetChanged();
            }
        }

        


        public class JavaObjectWrapper<T> : Java.Lang.Object { public T Obj { get; set; } }


    }
}
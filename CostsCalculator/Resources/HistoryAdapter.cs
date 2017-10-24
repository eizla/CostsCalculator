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
using Java.Lang;
using CostsCalculator.Models;
using Android.Graphics;

namespace CostsCalculator.Resources.layout
{
    class HistoryAdapter : BaseAdapter
    {
        private Activity activity;
        private ObservableCollection<HistoryItem> history = new ObservableCollection<HistoryItem>();
        private List<UserItem> users = new List<UserItem>();
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

        public HistoryAdapter(Activity activity, ObservableCollection<HistoryItem> history, ObservableCollection<UserItem> users)
        {
            this.activity = activity;
            this.history = history;
            this.users = new List<UserItem>(users);
        }

        public override int Count
        {
            get
            {
                return history.Count;
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
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.HistoryListViewDataTemplate, parent, false);

            var txtDesc = view.FindViewById<TextView>(Resource.Id.textViewDesc);
            var txtDate = view.FindViewById<TextView>(Resource.Id.textViewDate);
            var textImage = view.FindViewById<TextView>(Resource.Id.circleImage);

            Random rnd = new Random();
            string color = colors[rnd.Next() % colors.Count];
            textImage.SetBackgroundColor(Color.ParseColor(color));

            var item = users.Find(i => i.Id == history[position].UserId);

            if(item!=null)
                txtDesc.Text = item.Name + " " + history[position].Description;
            else
                txtDesc.Text = "User " + history[position].Description;

            txtDate.Text = "" + history[position].Date.Day +
                            "." + history[position].Date.Month +
                            "." + history[position].Date.Year +
                            "\n" + history[position].Date.Hour +
                            ":" + history[position].Date.Minute +
                            ":" + history[position].Date.Second;


            return view;
        }

        public static implicit operator HistoryAdapter(FriendsCustomAdapter v)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using CostsCalculator.Models;

namespace CostsCalculator.Resources
{

    public class Algo2
    {

        // TODO change algo after payment change
        private List<PaymentItem> payList;
        private UserItem thisUser;
        private HashSet<UserItem> trip_users;
        private static ObservableCollection<UserPaymentItem> user_payment;
        private TripItem tripitem;
        //private Tuple<ObservableCollection<string>, ObservableCollection<string>, ObservableCollection<string>> obserlist;
        //private decimal pay_sum;
        //private decimal return_sum;
        //private decimal your_sum;
        public Algo2(List<PaymentItem> paylist, UserItem thisuser, HashSet<UserItem> trip_Users, TripItem trip)
        {
            this.payList = paylist;
            this.thisUser = thisuser;
            this.trip_users = trip_Users;
            this.tripitem = trip;
            //this.user_payment = new ObservableCollection<UserPaymentItem>();
            trip_users.Add(thisUser);

        }
        //private Task<ObservableCollection<UserPaymentItem>> getpaymentsAsync()
        //{
        //    return  
        //}


        public async Task<ObservableCollection<string>> Algorithms_start()
        {
            user_payment = await DatabaseManager.DefaultManager.GetPaymentForTripAsync(tripitem); 
            if (payList == null || payList.Count == 0)
            {
                var err = new ObservableCollection<string>();
                err.Add("No payments detected.");
                return err;
            }

            //List<string> name_set = new List<string>();
            ////payList.Select(o => o.Name).Distinct(); TODO sprawdzić czy tak działa
            //foreach (PaymentItem item in payList)
            //{
            //    if (!name_set.Exists(s => s == item.Name))
            //        name_set.Add(item.Name);

            //}
            //var formated_pay_list = new List<Tuple<string, int>>();
            //var information_pay_list = new List<Tuple<string, int>>();
            //foreach (string name in name_set)
            //{
            //    var lists = payList.Where(o => o.Name == name);
            //    var len = lists.Count();
            //    var leng = ((int)(lists.First().Amount * 100)) % len;
            //    var amount = (((int)(lists.First().Amount * 100)) - leng) / len;
            //    Console.WriteLine(name + " " + len + " " + leng + " " + amount );
            //    information_pay_list.Add(Tuple.Create(lists.First().UserCreatingPaymentId, (int)(lists.First().Amount * 100)));

            //    if (lists.First().UserSharingPaymentId == null)
            //    {

            //        continue;
            //    }
            //    formated_pay_list.Add(Tuple.Create(lists.First().UserCreatingPaymentId, (int)(lists.First().Amount * 100)));
            //    foreach (PaymentItem item in lists)
            //    {
            //        if (leng == 0)
            //        {
            //            formated_pay_list.Add(Tuple.Create(item.UserSharingPaymentId, -(amount)));
            //        }
            //        else
            //        {
            //            formated_pay_list.Add(Tuple.Create(item.UserSharingPaymentId, -(amount + 1)));
            //            leng--;
            //        }
            //    }
            //}
            var dict = new Dictionary<string, long>();

            foreach (UserItem name in trip_users)
            {
                long sum = 0;
                var x = user_payment.Where(o => o.UserId == name.Id);
                foreach (UserPaymentItem item in x)
                {
                    sum += (long)(item.Amount*100); 
                }
                if (sum != 0)
                {
                    dict.Add(name.Name, sum);
                }
            }
            var sumlist = dict.ToList();
            sumlist.Sort((x, y) => x.Value.CompareTo(y.Value));
            var l = dict.Values;
            if (l.Sum() != 0)
            {
                throw new ArgumentException("Payment sum is not 0, it is: " + l.Sum());
            }
            var k = sumlist.Count - 1;
            var list = new List<Tuple<string, string, long>>();
            for (int i = 0; i <= k;)
            {
                if (sumlist[i].Value == 0)
                {
                    i++;
                    continue;
                }
                if (sumlist[k].Value == 0)
                {
                    k++;
                    continue;
                }
                if (Math.Abs(sumlist[i].Value) > sumlist[k].Value)
                {
                    list.Add(Tuple.Create(sumlist[i].Key, sumlist[k].Key, Math.Abs(sumlist[k].Value)));
                    var tmp = sumlist[i].Value + sumlist[k].Value;
                    sumlist[i] = new KeyValuePair<string, long>(sumlist[i].Key, tmp);
                    k--;
                }
                if (Math.Abs(sumlist[i].Value) < sumlist[k].Value)
                {
                    list.Add(Tuple.Create(sumlist[i].Key, sumlist[k].Key, Math.Abs(sumlist[i].Value)));
                    var tmp = sumlist[i].Value + sumlist[k].Value;
                    sumlist[k] = new KeyValuePair<string, long>(sumlist[k].Key, tmp);
                    i++;
                }
                if (Math.Abs(sumlist[i].Value) == sumlist[k].Value)
                {
                    list.Add(Tuple.Create(sumlist[i].Key, sumlist[k].Key, Math.Abs(sumlist[i].Value)));
                    sumlist[i] = new KeyValuePair<string, long>(sumlist[i].Key, 0);
                    sumlist[k] = new KeyValuePair<string, long>(sumlist[k].Key, 0);
                    i++;
                    k--;
                }
            }

            var res1 = new ObservableCollection<string>();
            var res2 = new ObservableCollection<string>();
            var res3 = new ObservableCollection<string>();
            if(list.Count == 0)
            {
                res1.Add("None has to pay.");
            }
            foreach (Tuple<string, string, long> item in list)
            {
                res1.Add(item.Item1 + " have to pay " + item.Item2 + " around: " + ((decimal)item.Item3) / 100);
            }
            /*
            var result = list.FindAll(s => s.Item1 == thisUser.Name);
            this.return_sum = ((decimal)result.Select(s => s.Item3).Sum())/100;
            foreach (Tuple<string, string, int> item in result)
            {
                res2.Add("You have to pay " + item.Item2 + " around: " + ((decimal)item.Item3) / 100);
            }
            result = list.FindAll(s => s.Item2 == thisUser.Name);
            this.pay_sum = ((decimal)result.Select(s => s.Item3).Sum())/100;
            foreach (Tuple<string, string, int> item in result)
            {
                res2.Add(item.Item1 + " have to pay you around: " + ((decimal)item.Item3) / 100);
            }
            */
            //var results = information_pay_list.FindAll(s => s.Item1 == thisUser.Id);
            //foreach (Tuple<string, int> item in results)
            //{
            //    res1.Add("You paid " + ((decimal)item.Item2) / 100);
            //}
            //this.your_sum = ((decimal)results.Select(s => s.Item2).Sum())/100;
            
            return res1;


        }

    }
}


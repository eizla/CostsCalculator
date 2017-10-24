using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

using CostsCalculator.Models;

namespace CostsCalculator
{
    class DatabaseManager
    {
        static DatabaseManager defaultInstance = new DatabaseManager();
        MobileServiceClient client;
        IMobileServiceTable<TripItem> tripTable;
        IMobileServiceTable<HistoryItem> historyTable;
        IMobileServiceTable<PaymentItem> paymentTable;
        IMobileServiceTable<UserItem> userTable;
        IMobileServiceTable<FriendItem> friendTable;
        IMobileServiceTable<TripUserItem> tripuserTable;
        IMobileServiceTable<UserPaymentItem> userpaymentTabel;
        private DatabaseManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            this.tripTable = client.GetTable<TripItem>();
            this.historyTable = client.GetTable<HistoryItem>();
            this.paymentTable = client.GetTable<PaymentItem>();
            this.userTable = client.GetTable<UserItem>();
            this.friendTable = client.GetTable<FriendItem>();
            this.tripuserTable = client.GetTable<TripUserItem>();
            this.userpaymentTabel = client.GetTable<UserPaymentItem>();
        }
        public static DatabaseManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient => client;

        public async Task SaveUserPaymentItemAsync(UserPaymentItem item)
        {
            if (item.Id == null)
            {
                await userpaymentTabel.InsertAsync(item);
            }
            else
            {
                await userpaymentTabel.UpdateAsync(item);
            }
        }
        public async void DeleteUserPaymentItemAsync(UserPaymentItem item)
        {
            await userpaymentTabel.DeleteAsync(item);
        }

        public async Task<ObservableCollection<UserPaymentItem>> GetPaymentUsersForPaymentAsync(PaymentItem paymentItem)
        {
            try
            { 
                IEnumerable<UserPaymentItem> users = await userpaymentTabel.Where(i => i.PayId == paymentItem.Id).ToEnumerableAsync();
                return new ObservableCollection<UserPaymentItem>(users);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }


        public async Task<ObservableCollection<UserPaymentItem>> GetPaymentForTripAsync(TripItem tripItem)
        {
            try
            {
                
                IEnumerable<PaymentItem> items = await paymentTable.Where(i => i.TripId == tripItem.Id).ToEnumerableAsync();
                List<UserPaymentItem> pays = new List<UserPaymentItem>();
                foreach (var id in items)
                {
                    IEnumerable<UserPaymentItem> user = await userpaymentTabel.Where(i => i.PayId == id.Id).ToEnumerableAsync();
                    pays.AddRange(user.ToList());
                }
                return new ObservableCollection<UserPaymentItem>(pays);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }
        public async Task<ObservableCollection<TripItem>> GetTripItemsAsync(UserItem item)
        {
            try
            {
                IEnumerable<TripUserItem> items = await tripuserTable.Where(tripUserItem => tripUserItem.UserId == item.Id).ToEnumerableAsync();
                List<TripItem> trips = new List<TripItem>();
                foreach (var id in items)
                {
                    IEnumerable<TripItem> user = await tripTable.Where(i => i.Id == id.TripId /*&& i.IsCurrent == true*/).ToEnumerableAsync();
                    trips.AddRange(user.ToList());
                }
                return new ObservableCollection<TripItem>(trips);

            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<TripItem>> GetTripItemsInPastAsync(UserItem item)
        {
            try
            {
                IEnumerable<TripUserItem> items = await tripuserTable.Where(tripUserItem => tripUserItem.UserId == item.Id).ToEnumerableAsync();
                List<TripItem> trips = new List<TripItem>();
                foreach (var id in items)
                {
                    IEnumerable<TripItem> user = await tripTable.Where(i => i.Id == id.TripId && i.IsCurrent == false).ToEnumerableAsync();
                    trips.AddRange(user.ToList());
                }
                return new ObservableCollection<TripItem>(trips);

            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<UserItem>> GetUsersAsync()
        {
            try
            {
                IEnumerable<UserItem> users = await userTable.ToEnumerableAsync();
                return new ObservableCollection<UserItem>(users);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SaveTripItemAsync(TripItem item, ObservableCollection<UserItem> users)
        {
            if (item.Id == null)
            {
                await tripTable.InsertAsync(item);
            }
            else if (users == null)
            {
                await tripTable.UpdateAsync(item);
                return;
            }
            else
            {
                await tripTable.UpdateAsync(item);
                DeleteUsersFromTripAsync(item);
            }
            foreach (var user in users)
            {
                await tripuserTable.InsertAsync(new TripUserItem() { TripId = item.Id, UserId = user.Id });
            }


        }

        public async void DeleteTripItem(TripItem item)
        {
            if (item != null)
            {
                //delete all history
                IEnumerable<HistoryItem> historyItems = await historyTable.Where(i => i.TripId == item.Id).ToEnumerableAsync();
                foreach(HistoryItem hitem in historyItems)
                {
                    await historyTable.DeleteAsync(hitem);
                }
                //delete all payments
                IEnumerable<PaymentItem> paymentItems = await paymentTable.Where(i => i.TripId == item.Id).ToEnumerableAsync();
                foreach (PaymentItem pitem in paymentItems)
                {
                    IEnumerable<UserPaymentItem> userPayitem = await userpaymentTabel.Where(i => i.PayId == pitem.Id).ToEnumerableAsync();
                    foreach(UserPaymentItem uitem in userPayitem)
                    {
                        await userpaymentTabel.DeleteAsync(uitem);
                    }
                    await paymentTable.DeleteAsync(pitem);
                }
                await tripTable.DeleteAsync(item);
            }
        }

        public async void DeleteUserFromTripAsync(TripItem tripItem, string userId)
        {
            IEnumerable<TripUserItem> tripUserItems = await tripuserTable.Where(i => i.TripId == tripItem.Id && i.UserId == userId).ToEnumerableAsync();
            foreach (TripUserItem item in tripUserItems)
            {
                await tripuserTable.DeleteAsync(item);
            }
        }

        public async void DeleteUsersFromTripAsync(TripItem tripItem)
        {
            IEnumerable<TripUserItem> tripUserItems = await tripuserTable.Where(i => i.TripId == tripItem.Id).ToEnumerableAsync();
            foreach (TripUserItem item in tripUserItems)
            {
                await tripuserTable.DeleteAsync(item);
            }
        }


        public async void DeleteFriend(UserItem friend, UserItem user)
        {
            IEnumerable<FriendItem> userItems = await friendTable.Where(i => i.FriendId == friend.Id && i.UserId==user.Id).ToEnumerableAsync();
            foreach (FriendItem item in userItems)
            {
                await friendTable.DeleteAsync(item);
            }
        }

        public async Task<ObservableCollection<HistoryItem>> GetHistoryItemsAsync(TripItem tripItem)
        {
            try
            {
                IEnumerable<HistoryItem> items = await historyTable.Where(i => i.TripId == tripItem.Id).ToEnumerableAsync();

                return new ObservableCollection<HistoryItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SaveHistoryItemAsync(HistoryItem item)
        {
            if (item.Id == null)
            {
                await historyTable.InsertAsync(item);
            }
            else
            {
                await historyTable.UpdateAsync(item);
            }
        }

        public async Task<bool> SaveFriendItemAsync(FriendItem item)
        {
            try
            {
                IEnumerable<FriendItem> items = await friendTable.Where(i => i.FriendId == item.FriendId && i.UserId == item.UserId).ToListAsync();
                if (!items.Any())
                {
                    await friendTable.InsertAsync(item);
                    return true;
                }
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return false;
        }

        public async Task<ObservableCollection<PaymentItem>> GetPaymentItemsAsync(TripItem item)
        {
            try
            {
                //IEnumerable<PaymentItem> items = await paymentTable.ToEnumerableAsync(); 
                IEnumerable<PaymentItem> items = await paymentTable.Where(PaymentItem => PaymentItem.TripId == item.Id).ToEnumerableAsync();        

                return new ObservableCollection<PaymentItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<UserItem>> GetUsersItemsAsync(UserItem userItem)
        {
            try
            {
                IEnumerable<FriendItem> items = await friendTable.Where(i => i.UserId == userItem.Id).ToEnumerableAsync();
                List<UserItem> users = new List<UserItem>();
                foreach (var id in items)
                {
                    IEnumerable<UserItem> user = await userTable.Where(i => i.Id == id.FriendId).ToEnumerableAsync();
                    users.AddRange(user.ToList());
                }
                return new ObservableCollection<UserItem>(users);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<UserItem>> GetUsersTripsItemsAsync(TripItem tripItem)
        {
            try
            {
                IEnumerable<TripUserItem> items = await tripuserTable.Where(i => i.TripId == tripItem.Id).ToEnumerableAsync();
                List<UserItem> users = new List<UserItem>();
                foreach (var id in items)
                {
                    IEnumerable<UserItem> user = await userTable.Where(i => i.Id == id.UserId).ToEnumerableAsync();
                    users.AddRange(user.ToList());
                }
                return new ObservableCollection<UserItem>(users);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }


        public async Task SavePaymentItemAsync(PaymentItem item)
        {
            if (item.Id == null)
            {
                await paymentTable.InsertAsync(item);
            }
            else
            {
                await paymentTable.UpdateAsync(item);
            }
        }

        public async void DeletePaymentItemAsync(PaymentItem item)
        {
            IEnumerable<UserPaymentItem> users = await userpaymentTabel.Where(s => s.Id == item.Id).ToEnumerableAsync();
            if (item != null)
                await paymentTable.DeleteAsync(item);
            foreach(UserPaymentItem user in users)
            {
                await userpaymentTabel.DeleteAsync(user);
            }

        }



        public async Task SaveUserItemAsync(UserItem item)
        {
            if (item.Id == null)
            {
                await userTable.InsertAsync(item);
            }
            else
            {
                await userTable.UpdateAsync(item);
            }
        }

        public async Task<ObservableCollection<UserItem>> GetUserItemsAsync(string name)
        {
            try
            {
                IEnumerable< UserItem > items = await userTable.Where(userItem => userItem.Name == name).ToListAsync();
                return new ObservableCollection<UserItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<string> GetUsernameAsync(string userId)
        {
            try
            {
                IEnumerable<UserItem> items = await userTable.Where(userItem => userItem.Id == userId).ToListAsync();
                return items.FirstOrDefault().Name;

            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<UserItem>> GetUserItemsSearchingAsync(string name)
        {
            try
            {
                IEnumerable<UserItem> items = await userTable.Where(userItem => userItem.Name == name || userItem.Name.StartsWith(name) || userItem.Name.Contains(name)).ToListAsync();
                return new ObservableCollection<UserItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }
    }
}

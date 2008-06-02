using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace LastFM
{
    class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        private Dispatcher UIThreadDispatcher = null;
        public ThreadSafeObservableCollection(Dispatcher uiThreadDispatcher)
        {
            UIThreadDispatcher = uiThreadDispatcher;
        }



        protected override void ClearItems()
        {
            if (UIThreadDispatcher.CheckAccess())
            {
                base.ClearItems();
            }
            else
            {
                UIThreadDispatcher.Invoke(DispatcherPriority.Send, new Action(ClearItems));
            }
        }
        protected override void InsertItem(int index, T item)
        {
            if (UIThreadDispatcher.CheckAccess())
            {
                base.InsertItem(index, item);

            }
            else
            {
                UIThreadDispatcher.Invoke(DispatcherPriority.Send, new Action<int, T>(InsertItem), index, item);
            }
        }
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (UIThreadDispatcher.CheckAccess())
            {
                base.MoveItem(oldIndex, newIndex);

            }
            else
            {
                UIThreadDispatcher.Invoke(DispatcherPriority.Send, new Action<int, int>(MoveItem), oldIndex, newIndex);
            }
        }
        protected override void RemoveItem(int index)
        {
            if (UIThreadDispatcher.CheckAccess())
            {
                base.RemoveItem(index);
            }
            else
            {
                UIThreadDispatcher.Invoke(DispatcherPriority.Send, new Action<int>(RemoveItem), index);
            }
        }
        protected override void SetItem(int index, T item)
        {
            if (UIThreadDispatcher.CheckAccess())
            {
                base.SetItem(index, item);
            }
            else
            {
                UIThreadDispatcher.Invoke(DispatcherPriority.Send, new Action<int, T>(SetItem), index, item);
            }
        }
    }
}

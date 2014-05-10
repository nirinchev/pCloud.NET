using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace pCloud.Data
{
    public class SortedObservableCollection<T> : ObservableCollection<T>
    {

        public ObservableCollection<SortDescriptorBase> SortDescriptors { get; private set; }

        public SortedObservableCollection()
        {
            this.InitializeSortDescriptionsCollection();
        }

        public SortedObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            this.InitializeSortDescriptionsCollection();
        }

        protected override void InsertItem(int index, T item)
        {
            var newIndex = this.GetIndex(item);
            if (newIndex == -1)
            {
                newIndex = 0;
            }
            base.InsertItem(newIndex, item);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();
        }

        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException();
        }

        private int GetIndex(T item)
        {
            return this.Concat(new[] { item }).AsQueryable().Sort(this.SortDescriptors).ToList().IndexOf(item);
        }

        private void InitializeSortDescriptionsCollection()
        {
            this.SortDescriptors = new ObservableCollection<SortDescriptorBase>();
            
        }
    }
}

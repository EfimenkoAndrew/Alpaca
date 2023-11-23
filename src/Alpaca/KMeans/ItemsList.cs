using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KMeans
{
    public class ItemsList : IItemsList
    {
        private readonly List<Item> _mItemsList;

        public ItemsList()
        {
            if (_mItemsList == null)
                _mItemsList = new List<Item>();
        }

        public void Add(Item item)
        {
            _mItemsList.Add(item);
        }

        public object Clone()
        {
            var targetItems = new ItemsList();
            foreach (var item in _mItemsList)
            {
                var targetAttribList = new AttribList();
                foreach (Attrib attrib in item.GetAttribList())
                    targetAttribList.Add(new Attrib(attrib.Name, attrib.Value));

                if (targetAttribList.Count() > 0)
                    targetItems.Add(new Item(item.ItemText, targetAttribList,
                        item.Distance, item.IsUser, item.Exists));
            }

            return targetItems;
        }

        public Item this[int iIndex]
        {
            get => _mItemsList[iIndex];
            set => _mItemsList[iIndex] = value;
        }

        public int Count()
        {
            return _mItemsList.Count();
        }

        public void RemoveAt(int iIndex)
        {
            _mItemsList.RemoveAt(iIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return _mItemsList.GetEnumerator();
        }

        IEnumerator<Item> IEnumerable<Item>.GetEnumerator()
        {
            return (IEnumerator<Item>)GetEnumerator();
        }
    }
}
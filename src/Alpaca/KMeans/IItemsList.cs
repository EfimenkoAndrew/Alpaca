using System;
using System.Collections.Generic;

namespace Alpaca.KMeans
{
    public interface IItemsList : ICloneable, IEnumerable<Item>
    {
        Item this[int iIndex] { get; set; }
        void Add(Item item);
        int Count();
        void RemoveAt(int iIndex);
    }
}
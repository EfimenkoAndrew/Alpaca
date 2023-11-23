using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KMeans
{
    public class AttribList : ICloneable, IEnumerable<Attrib>
    {
        private readonly List<Attrib> _mAttribList;

        public AttribList()
        {
            if (_mAttribList == null)
                _mAttribList = new List<Attrib>();
        }

        public Attrib this[int iIndex]
        {
            get => _mAttribList[iIndex];
            set => _mAttribList[iIndex] = value;
        }

        public object Clone()
        {
            var targetAttribList = new AttribList();
            foreach (var attrib in _mAttribList)
                targetAttribList.Add(attrib);

            return targetAttribList.Count() > 0 ? (AttribList)targetAttribList.Clone() : null;
        }

        public IEnumerator GetEnumerator()
        {
            return _mAttribList.GetEnumerator();
        }

        IEnumerator<Attrib> IEnumerable<Attrib>.GetEnumerator()
        {
            return (IEnumerator<Attrib>)GetEnumerator();
        }

        public void Add(Attrib attribItem)
        {
            _mAttribList.Add((Attrib)attribItem.Clone());
        }

        public int Count()
        {
            return _mAttribList.Count();
        }
    }
}
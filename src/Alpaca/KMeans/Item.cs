using System;

namespace Alpaca.KMeans
{
    public class Item : ICloneable
    {
        public Item(
            string itemText,
            AttribList attributes,
            double distance,
            bool isUser,
            bool exists)
        {
            if (AttribList == null)
                AttribList = attributes;

            IsUser = isUser;
            Exists = exists;
            ItemText = itemText;
            Distance = distance;
        }

        public string ItemText { get; set; }

        public double Distance { get; set; }

        public bool IsUser { get; set; }

        public bool Exists { get; set; }

        public AttribList AttribList { get; set; }

        public object Clone()
        {
            var targetItem = (Item)MemberwiseClone();
            var targetAttribList = new AttribList();
            foreach (Attrib attrib in AttribList)
                targetAttribList.Add(attrib);

            if (targetAttribList.Count() > 0)
                targetItem.AttribList = (AttribList)targetAttribList.Clone();

            targetItem.IsUser = IsUser;
            targetItem.Exists = Exists;
            targetItem.ItemText = ItemText;
            targetItem.Distance = Distance;

            return targetItem;
        }

        public AttribList GetAttribList()
        {
            return AttribList;
        }

        private double Relevance(string attrib1, string attrib2)
        {
            double nRelevance = 0;
            // Assigning the nLength variable the value of the smallest string length
            var nLength = attrib1.Length < attrib2.Length ? attrib1.Length : attrib2.Length;
            // Iterating through the two strings of character, comparing the pairs of items
            // from either attrib1 and attrib2. If the two characters are lexicographically equal
            // we're adding the value 1 / nLength to the nRelevance variable
            for (var iIndex = 0; iIndex < nLength; iIndex++)
                nRelevance += attrib1[iIndex] == attrib2[iIndex] ? (double)1 / nLength : 0;

            return nRelevance;
        }

        public double EuclDw(Item item)
        {
            var nCount = 0;
            var iIndex = 0;
            double nDistance = 0;
            // Iterating through the array of attributes and for each pair of either users or items
            // attributes computing the distance between those attributes. Then, each distance values
            // is added to the nDistance variable. During the computation we're also obtaining the
            // value of releavance between the lexicographical representations of those attributes
            while (iIndex < item.AttribList.Count() && iIndex < AttribList.Count())
            {
                // Compute the relevance between names of the pair of attributes
                var nRel = Relevance(item.AttribList[iIndex].Name, AttribList[iIndex].Name);
                if (nRel == 1) nCount++;

                // Computing the Eucledean distance between the pair of current attributes
                nDistance += Math.Pow(item.AttribList[iIndex].Value - AttribList[iIndex].Value, 2.0) *
                             (nRel > 0 ? nRel : 1);

                iIndex++;
            }

            // Returning the value of the distance between two vectors of attributes
            return Math.Sqrt(nDistance) * (1 / (nCount > 0 ? nCount : 0.01));
        }
    }
}
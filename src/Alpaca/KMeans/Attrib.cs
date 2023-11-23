using System;

namespace KMeans
{
    public class Attrib : ICloneable
    {
        public Attrib(string name, double value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public double Value { get; set; }

        public object Clone()
        {
            var targetAttrib = (Attrib)MemberwiseClone();
            targetAttrib.Name = Name;
            targetAttrib.Value = Value;
            return targetAttrib;
        }
    }
}
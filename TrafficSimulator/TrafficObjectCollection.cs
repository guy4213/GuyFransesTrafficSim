using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrafficSimulator
{
    public class TrafficObjectCollection
    {
        private List<TrafficObject> _items;

        public TrafficObject this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public void Add(TrafficObject obj)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Remove(TrafficObject obj)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void DrawAll(Graphics g, bool isNightMode)
        {
            throw new NotImplementedException();
        }

        public void UpdateAll()
        {
            throw new NotImplementedException();
        }

        public float GetCongestionRate()
        {
            throw new NotImplementedException();
        }

        public double GetTotalMileage()
        {
            throw new NotImplementedException();
        }
    }
}

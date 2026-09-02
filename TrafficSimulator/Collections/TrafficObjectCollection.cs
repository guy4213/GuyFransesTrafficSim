using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class TrafficObjectCollection
    {
        private List<TrafficObject> _items = new List<TrafficObject>();
        public TrafficObject this[int index]
        {
            get { return _items[index]; }
            set {   _items[index] = value; }
        }

        public List<TrafficObject> GetAllObjects()
        {
            return _items;
        }
        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(TrafficObject obj)
        {
           _items.Add(obj);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public void Remove(TrafficObject obj)
        {
            _items.Remove(obj);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void DrawAll(Graphics g, bool isNightMode)
        {

            _items.ForEach(item => item.Draw(g, isNightMode));

        }

        public void UpdateAll()
        {
           _items.ForEach(item => item.Move(this));
        }

        public float GetCongestionRate()
        {
            var roadUsers = _items.FindAll(item => item is RoadUser);
            if (roadUsers.Count == 0) return 0f;

            int slowedCount = roadUsers.FindAll(item => item.ActualSpeed < item.DesiredSpeed).Count;
            return (float)slowedCount / roadUsers.Count * 100f;
        }

        public double GetTotalMileage()
        {
            double total = 0;
            foreach (var item in _items)
            {
                if (item is RoadUser)
                {
                    total += item.X;
                }
            }
            return total;
        }
        public List<TrafficObject> GetObjectsInLane(int lane)
        {
            List<TrafficObject> laneItems = new List<TrafficObject>();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Lane == lane)
                {
                    laneItems.Add(_items[i]);
                }
            }
            return laneItems;
        }
    }
    
}

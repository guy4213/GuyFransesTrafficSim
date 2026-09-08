using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class TrafficObjectCollection
    {
        private List<TrafficObject> _items = new List<TrafficObject>();

        public Direction ActiveGreenDirection { get; set; } = Direction.Down;
        public bool IsNightMode { get; set; }
        [field: System.Runtime.Serialization.OptionalField]
        public bool IsAmber { get; set; }
        [field: System.Runtime.Serialization.OptionalField]
        public int PhaseTicks { get; set; }
        [System.Runtime.Serialization.OptionalField]
        private double _totalDistance;

        public bool HasActiveEmergency
        {
            get
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i] is EmergencyVehicle emergency && emergency.SirenOn)
                        return true;
                }
                return false;
            }
        }

        public bool HasMovingEmergencyVehicle
        {
            get
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i] is EmergencyVehicle emergency && emergency.ActualSpeed > 0)
                        return true;
                }
                return false;
            }
        }

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
            // Road-bound objects always use their lane's geometric centre.
            // Pedestrians and stations deliberately live on the pavement/curb.
            if (!(obj is Pedestrian) && !(obj is BusStation))
            {
                RoadLayout.CenterInLane(obj);
            }

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
            _totalDistance = 0;
        }

        public void DrawAll(Graphics g, bool isNightMode)
        {

            _items.ForEach(item => item.Draw(g, isNightMode));

        }

        public void UpdateAll(TrafficObject draggedObject = null)
        {
            foreach (var item in _items)
            {
                if (item == draggedObject) continue;
                int x = item.X, y = item.Y;
                item.Move(this);
                if (item is RoadUser && !(item is Pedestrian))
                {
                    // Lane changes are lateral; count forward travel only.
                    _totalDistance += RoadLayout.IsHorizontal(item.Direction) ? Math.Abs(item.X - x) : Math.Abs(item.Y - y);
                }
            }
        }

        public void RestoreFrom(TrafficObjectCollection saved)
        {
            // Preserve entity references (including a bus's last serviced station).
            _items = new List<TrafficObject>(saved._items);
            ActiveGreenDirection = saved.ActiveGreenDirection;
            IsNightMode = saved.IsNightMode;
            IsAmber = saved.IsAmber;
            PhaseTicks = saved.PhaseTicks;
            _totalDistance = saved._totalDistance;
        }

        public float GetCongestionRate()
        {
            var roadUsers = _items.FindAll(item => item is RoadUser && !(item is Pedestrian));
            if (roadUsers.Count == 0) return 0f;

            int slowedCount = roadUsers.FindAll(item => item.ActualSpeed < item.DesiredSpeed).Count;
            return (float)slowedCount / roadUsers.Count * 100f;
        }

        public double GetTotalMileage()
        {
            return _totalDistance;
        }
        public List<TrafficObject> GetObjectsInLane(Direction direction, int lane)
        {
            List<TrafficObject> laneItems = new List<TrafficObject>();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Direction == direction && _items[i].Lane == lane)
                {
                    laneItems.Add(_items[i]);
                }
            }
            return laneItems;
        }
    }
    
}

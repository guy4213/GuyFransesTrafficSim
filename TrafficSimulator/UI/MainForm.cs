using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrafficSimulator
{
    public partial class MainForm : Form
    {
        private readonly TrafficObjectCollection _trafficCollection = new TrafficObjectCollection();
        private readonly Timer _simTimer = new Timer();
        private bool _isNight = false;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _simTimer.Interval = 50;
            _simTimer.Tick += SimTimer_Tick;
            _simTimer.Start();
        }

        private void SimTimer_Tick(object sender, EventArgs e)
        {
            var objects = _trafficCollection.GetAllObjects();
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Move(_trafficCollection);
            }

            RemoveOutOfBoundsObjects();
            Invalidate();
        }

        private void RemoveOutOfBoundsObjects()
        {
            var objects = _trafficCollection.GetAllObjects();
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].X > ClientSize.Width + 100)
                {
                    _trafficCollection.Remove(objects[i]);
                }
            }
        }

        public void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(_isNight ? Color.FromArgb(30, 30, 40) : Color.LightGray);

            var objects = _trafficCollection.GetAllObjects();
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw(g, _isNight);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnPaint(this, e);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            OnKeyDown(this, e);
        }

        private void OnAddEntityClick(object sender, EventArgs e)
        {
            SpawnCar(0);
        }

        private void OnDeleteEntityClick(object sender, EventArgs e)
        {
            _trafficCollection.GetAllObjects().Clear();
            Invalidate();
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Simulation Files (*.bin)|*.bin|All Files (*.*)|*.*";
                sfd.Title = "שמירת מצב הסימולציה";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var fs = new System.IO.FileStream(sfd.FileName, System.IO.FileMode.Create))
                        {
#pragma warning disable SYSLIB0011
                            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            formatter.Serialize(fs, _trafficCollection);
#pragma warning restore SYSLIB0011
                        }
                        MessageBox.Show("הסימולציה נשמרה בהצלחה!", "שמירה", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("שגיאה בשמירה: " + ex.Message, "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnLoadClick(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Simulation Files (*.bin)|*.bin|All Files (*.*)|*.*";
                ofd.Title = "טעינת מצב סימולציה";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var fs = new System.IO.FileStream(ofd.FileName, System.IO.FileMode.Open))
                        {
#pragma warning disable SYSLIB0011
                            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            var loadedCollection = (TrafficObjectCollection)formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011

                            _trafficCollection.GetAllObjects().Clear();
                            foreach (var obj in loadedCollection.GetAllObjects())
                            {
                                _trafficCollection.Add(obj);
                            }
                        }
                        Invalidate();
                        MessageBox.Show("הסימולציה נטענה בהצלחה!", "טעינה", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("שגיאה בטעינה: " + ex.Message, "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void SpawnCar(int lane)
        {
            _trafficCollection.Add(new Car(0, GetLaneY(lane), lane, Direction.Right, CarModel.Sedan));
        }

        public void SpawnBus(int lane)
        {
            _trafficCollection.Add(new Bus(0, GetLaneY(lane), lane, Direction.Right));
        }

        public void SpawnEmergencyVehicle(int lane)
        {
            _trafficCollection.Add(new EmergencyVehicle(0, GetLaneY(lane), lane, Direction.Right));
        }

        public void SpawnPedestrian(int lane)
        {
            _trafficCollection.Add(new Pedestrian(0, GetLaneY(lane), lane, Direction.Right));
        }

        public void SpawnBicycle(int lane)
        {
            _trafficCollection.Add(new Bicycle(0, GetLaneY(lane), lane, Direction.Right));
        }

        public void SpawnBusStation(int x, int lane)
        {
            _trafficCollection.Add(new BusStation(x, GetLaneY(lane), lane));
        }

        public void SpawnHazard(int x, int lane)
        {
            _trafficCollection.Add(new RoadHazard(x, GetLaneY(lane), lane));
        }

        public void ToggleNightMode()
        {
            _isNight = !_isNight;
            Invalidate();
        }

        private int GetLaneY(int lane)
        {
            return 80 + (lane * 60);
        }
    }
}
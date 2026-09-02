using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TrafficSimulator
{
    public partial class MainForm : Form
    {
        private readonly TrafficObjectCollection _trafficCollection = new TrafficObjectCollection();
        private readonly Timer _simTimer = new Timer();
        private bool _isNight = false;
        private bool _isRunning = false;
        private int _lightPhase = 0;
        private int _lightTicks = 0;

        private static readonly Direction[] RoadDirections =
        {
            Direction.Right, // Road 1 - West
            Direction.Down,  // Road 2 - North
            Direction.Left,  // Road 3 - East
            Direction.Up     // Road 4 - South
        };

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _simTimer.Interval = 100;
            _simTimer.Tick += SimTimer_Tick;
            _simTimer.Start();
        }

        private void SimTimer_Tick(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                var objects = _trafficCollection.GetAllObjects();
                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i].Move(_trafficCollection);
                }

                RemoveOutOfBoundsObjects();
                AdvanceTrafficLight();
                UpdateAnalyticsLabel();
            }

            pictureBoxCanvas.Invalidate();
        }

        private void AdvanceTrafficLight()
        {
            _lightTicks++;
            if (_lightTicks > 44) // ~2.2s at 50ms/tick
            {
                _lightTicks = 0;
                _lightPhase = (_lightPhase + 1) % 3;
            }
        }

        private void UpdateAnalyticsLabel()
        {
            float congestion = _trafficCollection.GetCongestionRate();
            double mileage = _trafficCollection.GetTotalMileage();
            labelAnalytics.Text = $"Congestion: {congestion:F0}%   Total Mileage: {mileage:F0}";
        }

        private void RemoveOutOfBoundsObjects()
        {
            var objects = _trafficCollection.GetAllObjects();
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (RoadLayout.IsOutOfBounds(objects[i]))
                {
                    _trafficCollection.Remove(objects[i]);
                }
            }
        }

        public void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawJunction(g);

            var objects = _trafficCollection.GetAllObjects();
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw(g, _isNight);
            }
        }

        private void DrawJunction(Graphics g)
        {
            int w = RoadLayout.CanvasWidth;
            int h = RoadLayout.CanvasHeight;
            int cx = RoadLayout.CenterX;
            int cy = RoadLayout.CenterY;
            int rw = RoadLayout.RoadWidth;

            Color skyColor = _isNight ? Color.FromArgb(12, 13, 16) : Color.FromArgb(199, 204, 209);
            Color curbColor = _isNight ? Color.FromArgb(74, 77, 71) : Color.FromArgb(216, 211, 196);
            Color asphaltColor = _isNight ? Color.FromArgb(44, 47, 54) : Color.FromArgb(154, 160, 166);
            Color markingColor = _isNight ? Color.FromArgb(233, 228, 210) : Color.FromArgb(247, 244, 234);

            g.Clear(skyColor);

            using (Brush curbBrush = new SolidBrush(curbColor))
            {
                g.FillRectangle(curbBrush, 0, cy - rw / 2 - 10, w, rw + 20);
                g.FillRectangle(curbBrush, cx - rw / 2 - 10, 0, rw + 20, h);
            }

            using (Brush asphaltBrush = new SolidBrush(asphaltColor))
            {
                g.FillRectangle(asphaltBrush, 0, cy - rw / 2, w, rw);
                g.FillRectangle(asphaltBrush, cx - rw / 2, 0, rw, h);
            }

            using (Pen dash = new Pen(markingColor, 3) { DashPattern = new float[] { 6, 5 } })
            {
                // horizontal road: outer center line + lane split within each half
                g.DrawLine(dash, 0, cy, cx - rw / 2, cy);
                g.DrawLine(dash, cx + rw / 2, cy, w, cy);
                g.DrawLine(dash, 0, cy - rw / 4, cx - rw / 2, cy - rw / 4);
                g.DrawLine(dash, cx + rw / 2, cy - rw / 4, w, cy - rw / 4);
                g.DrawLine(dash, 0, cy + rw / 4, cx - rw / 2, cy + rw / 4);
                g.DrawLine(dash, cx + rw / 2, cy + rw / 4, w, cy + rw / 4);

                // vertical road
                g.DrawLine(dash, cx, 0, cx, cy - rw / 2);
                g.DrawLine(dash, cx, cy + rw / 2, cx, h);
                g.DrawLine(dash, cx - rw / 4, 0, cx - rw / 4, cy - rw / 2);
                g.DrawLine(dash, cx - rw / 4, cy + rw / 2, cx - rw / 4, h);
                g.DrawLine(dash, cx + rw / 4, 0, cx + rw / 4, cy - rw / 2);
                g.DrawLine(dash, cx + rw / 4, cy + rw / 2, cx + rw / 4, h);
            }

            using (Pen stop = new Pen(markingColor, 5))
            {
                g.DrawLine(stop, cx - rw / 2, cy + 6, cx - rw / 2, cy + rw / 2 - 4);
                g.DrawLine(stop, cx + rw / 2, cy - rw / 2 + 4, cx + rw / 2, cy - 6);
                g.DrawLine(stop, cx - 6, cy - rw / 2, cx - rw / 2 + 4, cy - rw / 2);
                g.DrawLine(stop, cx + rw / 2 - 4, cy + rw / 2, cx + 6, cy + rw / 2);
            }

            DrawRoadBadge(g, 1, 26, cy - rw / 2 - 16);
            DrawRoadBadge(g, 2, cx - rw / 2 - 16, 26);
            DrawRoadBadge(g, 3, w - 26, cy + rw / 2 + 16);
            DrawRoadBadge(g, 4, cx + rw / 2 + 16, h - 26);

            DrawTrafficLight(g, cx + rw / 2 + 8, cy - rw / 2 - 44);
        }

        private void DrawRoadBadge(Graphics g, int number, int x, int y)
        {
            using (Brush accent = new SolidBrush(Color.FromArgb(242, 183, 5)))
            using (Brush ink = new SolidBrush(Color.FromArgb(58, 43, 0)))
            using (Font font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (StringFormat fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.FillEllipse(accent, x - 12, y - 12, 24, 24);
                g.DrawString(number.ToString(), font, ink, x, y, fmt);
            }
        }

        private void DrawTrafficLight(Graphics g, int x, int y)
        {
            using (Brush housing = new SolidBrush(Color.FromArgb(17, 18, 20)))
            {
                g.FillRectangle(housing, x, y, 22, 58);
            }

            Color[] colors = { Color.FromArgb(255, 77, 77), Color.FromArgb(255, 210, 63), Color.FromArgb(61, 220, 115) };
            for (int i = 0; i < 3; i++)
            {
                Color c = (_isRunning && i == _lightPhase) ? colors[i] : Color.FromArgb(60, 255, 255, 255);
                using (Brush b = new SolidBrush(c))
                {
                    g.FillEllipse(b, x + 4, y + 4 + i * 18, 13, 13);
                }
            }
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
            Direction dir = RoadDirections[comboBoxRoad.SelectedIndex];
            int lane = (int)numericUpDownLane.Value;
            string type = comboBoxEntityType.SelectedItem as string;

            // static roadside objects are placed explicitly via Offset; vehicles
            // queue up bumper-to-bumper behind the stop line instead.
            if (type == "BusStation" || type == "RoadHazard")
            {
                int offset = (int)numericUpDownOffset.Value;
                Point staticPos = RoadLayout.GetSpawnPosition(dir, lane, offset);

                if (type == "BusStation")
                    _trafficCollection.Add(new BusStation(staticPos.X, staticPos.Y, lane, dir));
                else
                    _trafficCollection.Add(new RoadHazard(staticPos.X, staticPos.Y, lane, dir));

                pictureBoxCanvas.Invalidate();
                return;
            }

            int queueIndex = CountQueuedVehicles(dir, lane);
            Point pos = RoadLayout.GetQueuePosition(dir, lane, queueIndex);

            switch (type)
            {
                case "Bus":
                    _trafficCollection.Add(new Bus(pos.X, pos.Y, lane, dir));
                    break;
                case "EmergencyVehicle":
                    _trafficCollection.Add(new EmergencyVehicle(pos.X, pos.Y, lane, dir));
                    break;
                case "Pedestrian":
                    _trafficCollection.Add(new Pedestrian(pos.X, pos.Y, lane, dir));
                    break;
                case "Bicycle":
                    _trafficCollection.Add(new Bicycle(pos.X, pos.Y, lane, dir));
                    break;
                default:
                    _trafficCollection.Add(new Car(pos.X, pos.Y, lane, dir, CarModel.Sedan));
                    break;
            }

            pictureBoxCanvas.Invalidate();
        }

        private int CountQueuedVehicles(Direction dir, int lane)
        {
            int count = 0;
            var objects = _trafficCollection.GetObjectsInLane(dir, lane);
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is RoadUser) count++;
            }
            return count;
        }

        private void OnRunClick(object sender, EventArgs e)
        {
            _isRunning = !_isRunning;
            buttonRun.Text = _isRunning ? "⏸ Pause" : "▶ Run";
            buttonRun.BackColor = _isRunning ? Color.FromArgb(138, 31, 31) : Color.FromArgb(47, 125, 79);
        }

        private void OnToggleNightClick(object sender, EventArgs e)
        {
            ToggleNightMode();
        }

        private void OnDeleteEntityClick(object sender, EventArgs e)
        {
            _trafficCollection.GetAllObjects().Clear();
            pictureBoxCanvas.Invalidate();
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
                        SaveLoadManager.Save(_trafficCollection, sfd.FileName);
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
                        var loadedCollection = SaveLoadManager.Load(ofd.FileName);

                        _trafficCollection.GetAllObjects().Clear();
                        foreach (var obj in loadedCollection.GetAllObjects())
                        {
                            _trafficCollection.Add(obj);
                        }
                        pictureBoxCanvas.Invalidate();
                        MessageBox.Show("הסימולציה נטענה בהצלחה!", "טעינה", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("שגיאה בטעינה: " + ex.Message, "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void ToggleNightMode()
        {
            _isNight = !_isNight;
            buttonToggleNight.Text = _isNight ? "☀ Day" : "☾ Night";
            pictureBoxCanvas.Invalidate();
        }
    }
}

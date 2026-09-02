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

        private enum LightPhase { Green, Amber }
        private const int GreenDurationTicks = 60;
        private const int AmberDurationTicks = 15;
        private int _lightCycleIndex = 1;
        private LightPhase _currentPhase = LightPhase.Green;
        private int _phaseTicks = 0;

        private static readonly Direction[] RoadDirections =
        {
            Direction.Right, // Road 1 - West
            Direction.Down,  // Road 2 - North
            Direction.Left,  // Road 3 - East
            Direction.Up     // Road 4 - South
        };

        private TrafficObject _draggedObject;
        private Point _dragGrabOffset;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            ApplyStartLightSelection();
            SeedInitialTraffic();

            _simTimer.Interval = 100;
            _simTimer.Tick += SimTimer_Tick;
            _simTimer.Start();
        }

        private void SeedInitialTraffic()
        {
            foreach (Direction dir in RoadDirections)
            {
                Point p0 = RoadLayout.GetQueuePosition(dir, 0, 0);
                _trafficCollection.Add(new Car(p0.X, p0.Y, 0, dir, CarModel.Sedan));
            }

            Point busStop1 = RoadLayout.GetRoadsideStaticPosition(Direction.Right, 1, 90);
            _trafficCollection.Add(new BusStation(busStop1.X, busStop1.Y, 1, Direction.Right));

            Point busStop2 = RoadLayout.GetRoadsideStaticPosition(Direction.Down, 1, 90);
            _trafficCollection.Add(new BusStation(busStop2.X, busStop2.Y, 1, Direction.Down));

            Point bus1 = RoadLayout.GetQueuePosition(Direction.Right, 1, 1);
            _trafficCollection.Add(new Bus(bus1.X, bus1.Y, 1, Direction.Right));
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
            _phaseTicks++;

            if (_currentPhase == LightPhase.Green && _phaseTicks > GreenDurationTicks)
            {
                _currentPhase = LightPhase.Amber;
                _phaseTicks = 0;
            }
            else if (_currentPhase == LightPhase.Amber && _phaseTicks > AmberDurationTicks)
            {
                _lightCycleIndex = (_lightCycleIndex + 1) % RoadDirections.Length;
                _currentPhase = LightPhase.Green;
                _phaseTicks = 0;
                _trafficCollection.ActiveGreenDirection = RoadDirections[_lightCycleIndex];
            }
        }

        private void ApplyStartLightSelection()
        {
            _lightCycleIndex = comboBoxStartLight.SelectedIndex;
            _currentPhase = LightPhase.Green;
            _phaseTicks = 0;
            _trafficCollection.ActiveGreenDirection = RoadDirections[_lightCycleIndex];
            pictureBoxCanvas.Invalidate();
        }

        private void OnStartLightChanged(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                ApplyStartLightSelection();
            }
        }

        private void UpdateAnalyticsLabel()
        {
            float congestion = _trafficCollection.GetCongestionRate();
            double mileage = _trafficCollection.GetTotalMileage();
            string emergencyNote = _trafficCollection.HasActiveEmergency ? "   🚨 EMERGENCY OVERRIDE" : "";
            labelAnalytics.Text = $"Congestion: {congestion:F0}%   Total Mileage: {mileage:F0}{emergencyNote}";
        }

        private void RemoveOutOfBoundsObjects()
        {
            var objects = _trafficCollection.GetAllObjects();
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                bool gone = objects[i] is Pedestrian
                    ? RoadLayout.IsPedestrianDoneCrossing(objects[i])
                    : RoadLayout.IsOutOfBounds(objects[i]);

                if (gone)
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

            using (Brush crosswalkBrush = new SolidBrush(markingColor))
            {
                foreach (Direction d in RoadDirections)
                {
                    DrawCrosswalk(g, d, crosswalkBrush);
                }
            }

            DrawRoadBadge(g, 1, 26, cy - rw / 2 - 16);
            DrawRoadBadge(g, 2, cx - rw / 2 - 16, 26);
            DrawRoadBadge(g, 3, w - 26, cy + rw / 2 + 16);
            DrawRoadBadge(g, 4, cx + rw / 2 + 16, h - 26);

            // each light sits ahead of its road's stop line, on the right-hand side of travel
            DrawTrafficLightFor(g, Direction.Right, cx - rw / 2 - 14, cy + rw / 2 + 14); // Road 1 - West -> SW
            DrawTrafficLightFor(g, Direction.Down, cx - rw / 2 - 14, cy - rw / 2 - 14);  // Road 2 - North -> NW
            DrawTrafficLightFor(g, Direction.Left, cx + rw / 2 + 14, cy - rw / 2 - 14);  // Road 3 - East -> NE
            DrawTrafficLightFor(g, Direction.Up, cx + rw / 2 + 14, cy + rw / 2 + 14);    // Road 4 - South -> SE
        }

        private void DrawCrosswalk(Graphics g, Direction road, Brush stripeBrush)
        {
            int cx = RoadLayout.CenterX;
            int cy = RoadLayout.CenterY;
            int rw = RoadLayout.RoadWidth;

            if (RoadLayout.IsHorizontal(road))
            {
                int x = road == Direction.Right ? cx - rw / 2 - 26 : cx + rw / 2 + 26;
                int top = cy - rw / 2, bottom = cy + rw / 2;
                for (int y = top + 6; y < bottom; y += 18)
                {
                    g.FillRectangle(stripeBrush, x - 9, y, 18, 8);
                }
            }
            else
            {
                int y = road == Direction.Down ? cy - rw / 2 - 26 : cy + rw / 2 + 26;
                int left = cx - rw / 2, right = cx + rw / 2;
                for (int x = left + 6; x < right; x += 18)
                {
                    g.FillRectangle(stripeBrush, x, y - 9, 8, 18);
                }
            }
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

        private void DrawTrafficLightFor(Graphics g, Direction dir, int anchorX, int anchorY)
        {
            int x = anchorX - 11;
            int y = anchorY - 29;

            using (Brush housing = new SolidBrush(Color.FromArgb(17, 18, 20)))
            {
                g.FillRectangle(housing, x, y, 22, 58);
            }

            bool isActiveRoad = dir == RoadDirections[_lightCycleIndex];
            int activeIdx;
            if (!isActiveRoad)
                activeIdx = 0; // red
            else
                activeIdx = _currentPhase == LightPhase.Green ? 2 : 1; // green / amber

            Color[] colors = { Color.FromArgb(255, 77, 77), Color.FromArgb(255, 210, 63), Color.FromArgb(61, 220, 115) };
            for (int i = 0; i < 3; i++)
            {
                Color c = (i == activeIdx) ? colors[i] : Color.FromArgb(60, 255, 255, 255);
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

            // static objects are placed explicitly via Offset (measured back from the stop
            // line); bus stations are pushed onto the curb so they don't block the lane.
            if (type == "BusStation" || type == "RoadHazard")
            {
                int offset = (int)numericUpDownOffset.Value;

                if (type == "BusStation")
                {
                    Point stationPos = RoadLayout.GetRoadsideStaticPosition(dir, lane, offset);
                    _trafficCollection.Add(new BusStation(stationPos.X, stationPos.Y, lane, dir));
                }
                else
                {
                    Point hazardPos = RoadLayout.GetLaneStaticPosition(dir, lane, offset);
                    _trafficCollection.Add(new RoadHazard(hazardPos.X, hazardPos.Y, lane, dir));
                }

                pictureBoxCanvas.Invalidate();
                return;
            }

            if (type == "Pedestrian")
            {
                Point crossPos = RoadLayout.GetCrosswalkSpawn(dir);
                _trafficCollection.Add(new Pedestrian(crossPos.X, crossPos.Y, lane, dir));
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

        private TrafficObject FindObjectAt(Point p)
        {
            var objects = _trafficCollection.GetAllObjects();
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].GetBounds().Contains(p)) return objects[i];
            }
            return null;
        }

        private void PictureBoxCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            TrafficObject hit = FindObjectAt(e.Location);

            if (e.Button == MouseButtons.Right)
            {
                if (hit != null)
                {
                    _trafficCollection.Remove(hit);
                    pictureBoxCanvas.Invalidate();
                }
                return;
            }

            if (e.Button == MouseButtons.Left && hit != null)
            {
                _draggedObject = hit;
                _dragGrabOffset = new Point(e.X - hit.X, e.Y - hit.Y);
            }
        }

        private void PictureBoxCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedObject == null) return;

            _draggedObject.X = e.X - _dragGrabOffset.X;
            _draggedObject.Y = e.Y - _dragGrabOffset.Y;
            pictureBoxCanvas.Invalidate();
        }

        private void PictureBoxCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            _draggedObject = null;
        }

        private void PictureBoxCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            TrafficObject hit = FindObjectAt(e.Location);
            if (hit == null) return;

            float factor = e.Delta > 0 ? 1.1f : 0.9f;
            hit.Width = Math.Max(8, Math.Min(160, (int)(hit.Width * factor)));
            hit.Height = Math.Max(6, Math.Min(120, (int)(hit.Height * factor)));
            pictureBoxCanvas.Invalidate();
        }

        public void ToggleNightMode()
        {
            _isNight = !_isNight;
            buttonToggleNight.Text = _isNight ? "☀ Day" : "☾ Night";
            pictureBoxCanvas.Invalidate();
        }
    }
}

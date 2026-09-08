using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TrafficSimulator;

internal static class Program
{
    private static int checks;
    private static readonly Direction[] Directions = { Direction.Right, Direction.Down, Direction.Left, Direction.Up };
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        PedestrianRoutes();
        Obstacles();
        PersistenceAndBus();
        InterfaceAndSeed();
        Console.WriteLine($"PASS: {checks} assertions; pedestrians, drag, seed, obstacles, lights, bus, serialization, drawing.");
    }

    private static void Check(bool condition, string description)
    {
        checks++;
        if (!condition) throw new Exception(description);
    }

    private static void PedestrianRoutes()
    {
        for (int x = -20; x <= 780; x += 40)
        for (int y = -20; y <= 520; y += 30)
        {
            var p = new Pedestrian(x, y, 0, Direction.Right);
            Check(!RoadLayout.IsOnRoad(p.GetBounds()), $"Spawn on road {x},{y}");
            Check(p.X >= 0 && p.Y >= 0 && p.X + p.Width <= 760 && p.Y + p.Height <= 500, "Projection outside canvas");
            p.X = 380; p.Y = 250;
            p.PlaceOnSidewalk();
            Check(!RoadLayout.IsOnRoad(p.GetBounds()), "Drag projection on road");
        }
        foreach (Point spawn in new[] { new Point(100, 100), new Point(520, 100), new Point(100, 370), new Point(520, 370) })
        {
            var p = new Pedestrian(spawn.X, spawn.Y, 0, Direction.Right);
            var all = new TrafficObjectCollection();
            bool crossed = false, finishedCrossing = false;
            for (int tick = 0; tick < 1200 && !RoadLayout.IsOutOfBounds(p); tick++)
            {
                all.ActiveGreenDirection = Directions[(tick / 30) % 4];
                p.Move(all);
                if (RoadLayout.IsOnRoad(p.GetBounds()))
                {
                    Check(p.IsCrossing, "Walking on asphalt outside crossing state");
                    int cross = RoadLayout.IsHorizontal(p.Direction) ? p.Y + p.Height / 2 : p.X + p.Width / 2;
                    Check(Directions.Any(d => RoadLayout.GetCrosswalkCenterCoordinate(d) == cross), "Not centred on a crosswalk");
                    crossed = true;
                }
                if (crossed && !p.IsCrossing)
                {
                    finishedCrossing = true;
                    Check(!RoadLayout.IsOnRoad(p.GetBounds()), "Exited crossing onto asphalt");
                }
            }
            Check(crossed && finishedCrossing && RoadLayout.IsOutOfBounds(p), "Route must cross then leave the viewport");
        }
        var waiting = new Pedestrian(250, 154, 0, Direction.Right);
        var traffic = new TrafficObjectCollection { ActiveGreenDirection = Direction.Down };
        var ambulance = new EmergencyVehicle(600, 200, 0, Direction.Left);
        traffic.Add(ambulance);
        for (int tick = 0; tick < 150; tick++) waiting.Move(traffic);
        Check(!waiting.IsCrossing && !RoadLayout.IsOnRoad(waiting.GetBounds()), "Moving ambulance must block entry even without siren");
        traffic.Remove(ambulance);
        for (int tick = 0; tick < 20; tick++) waiting.Move(traffic);
        Check(waiting.IsCrossing, "Crossing did not resume when ambulance left");
    }

    private static void Obstacles()
    {
        foreach (Direction direction in Directions)
        {
            var all = new TrafficObjectCollection { ActiveGreenDirection = direction };
            var car = new Car(0, 0, 0, direction, CarModel.Sedan);
            RoadLayout.PlaceInQueue(car, 0);
            var hazard = new RoadHazard(car.X, car.Y, 0, direction);
            RoadLayout.CenterInLane(hazard);
            RoadLayout.Advance(hazard, 90);
            all.Add(car); all.Add(hazard);
            for (int tick = 0; tick < 6; tick++)
            {
                car.Move(all);
                Check(!car.GetBounds().IntersectsWith(hazard.GetBounds()), "Car drove through hazard " + direction);
            }
            Check(car.Lane == 1, "Car should overtake a hazard when lane clear");

            all.Clear();
            car = new Car(0, 0, 0, direction, CarModel.Sedan);
            RoadLayout.PlaceInQueue(car, 0);
            hazard = new RoadHazard(car.X, car.Y, 0, direction);
            RoadLayout.Advance(hazard, 85);
            all.Add(car); all.Add(hazard);
            var blocker = new Bus(car.X, car.Y, 1, direction);
            all.Add(blocker);
            for (int tick = 0; tick < 6; tick++) car.Move(all);
            Check(car.Lane == 0 && car.ActualSpeed == 0, "Blocked lane should force waiting " + direction);
            Check(!car.GetBounds().IntersectsWith(hazard.GetBounds()) && !car.GetBounds().IntersectsWith(blocker.GetBounds()), "Blocked overtake overlap");

            all.Clear();
            RoadLayout.PlaceInQueue(car, 0);
            all.Add(car);
            all.ActiveGreenDirection = Directions.First(d => d != direction);
            Point before = new Point(car.X, car.Y);
            car.Move(all);
            Check(before == new Point(car.X, car.Y) && !RoadLayout.HasCrossedStopLine(car), "Red stop " + direction);
            all.ActiveGreenDirection = direction;
            all.IsAmber = true;
            car.Move(all);
            Check(before == new Point(car.X, car.Y), "Amber allowed new entry");
        }
    }

    private static void PersistenceAndBus()
    {
        var all = new TrafficObjectCollection { IsNightMode = true, IsAmber = true, PhaseTicks = 7, ActiveGreenDirection = Direction.Left };
        all.Add(new Car(0, 0, 0, Direction.Right, CarModel.Sedan));
        var bus = new Bus(500, 290, 0, Direction.Right);
        var station = new BusStation(550, 356, 0, Direction.Right);
        all.Add(bus); all.Add(station);
        all.Add(new Bicycle(0, 0, 1, Direction.Down));
        all.Add(new RoadHazard(0, 0, 1, Direction.Up));
        all.Add(new EmergencyVehicle(100, 0, 1, Direction.Left));
        all.Add(new Pedestrian(100, 100, 0, Direction.Right));
        bus.Move(all);
        Check(bus.IsStoppedAtStation && bus.PassengerCount == 1 && station.WaitingPassengers == 4, "Bus failed station at exactly 50 pixels");
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bin");
        try
        {
            SaveLoadManager.Save(all, path);
            var loaded = SaveLoadManager.Load(path);
            Check(loaded.Count == 7 && loaded.GetAllObjects().Select(o => o.GetType()).Distinct().Count() == 7, "Entity serialization");
            Check(loaded.IsNightMode && loaded.IsAmber && loaded.PhaseTicks == 7 && loaded.ActiveGreenDirection == Direction.Left, "Scene serialization");
            foreach (bool night in new[] { false, true })
            {
                all.IsNightMode = night;
                SaveLoadManager.Save(all, path);
                Check(SaveLoadManager.Load(path).IsNightMode == night, "Day/night roundtrip");
            }
            var cloneBus = loaded.GetAllObjects().OfType<Bus>().Single();
            var cloneStation = loaded.GetAllObjects().OfType<BusStation>().Single();
            for (int i = 0; i < 18; i++) cloneBus.Move(loaded);
            Check(cloneBus.PassengerCount == 1 && cloneStation.WaitingPassengers == 4, "Bus boarded twice after load");
            all.RestoreFrom(loaded);
            Check(all.IsNightMode && all.IsAmber, "Restore lost settings");

            var route = new TrafficObjectCollection { ActiveGreenDirection = Direction.Down };
            var walker = new Pedestrian(253, 154, 0, Direction.Right);
            route.Add(walker);
            for (int i = 0; i < 12; i++) walker.Move(route);
            Check(walker.IsCrossing, "Mid-crossing fixture");
            SaveLoadManager.Save(route, path);
            var restoredRoute = SaveLoadManager.Load(path);
            var restoredWalker = (Pedestrian)restoredRoute[0];
            for (int i = 0; i < 120; i++)
            {
                walker.Move(route);
                restoredWalker.Move(restoredRoute);
                Check(walker.X == restoredWalker.X && walker.Y == restoredWalker.Y && walker.IsCrossing == restoredWalker.IsCrossing,
                    "Load changed an in-progress pedestrian route");
            }
        }
        finally { File.Delete(path); }
        foreach (Direction direction in Directions)
        {
            all = new TrafficObjectCollection { ActiveGreenDirection = direction };
            var car = new Car(0, 0, 0, direction, CarModel.Sedan);
            RoadLayout.PlaceInQueue(car, 0); all.Add(car); all.UpdateAll();
            Check(all.GetTotalMileage() == 90, "Distance not measured in every direction");
        }
    }

    private static T Field<T>(MainForm form, string name) => (T)typeof(MainForm).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(form);
    private static void Invoke(MainForm form, string name, params object[] args) => typeof(MainForm).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(form, args);

    private static void InterfaceAndSeed()
    {
        using var form = new MainForm();
        var all = Field<TrafficObjectCollection>(form, "_trafficCollection");
        Check(all.GetAllObjects().Select(o => o.GetType()).Distinct().Count() == 7, "Seed missing types");
        foreach (Direction direction in Directions)
        {
            var point = RoadLayout.GetRoadsideStaticPosition(direction, 0, 0);
            Check(!RoadLayout.IsOnRoad(new BusStation(point.X, point.Y, 0, direction).GetBounds()), "Station on asphalt");
        }
        foreach (var p in all.GetAllObjects().OfType<Pedestrian>()) Check(!RoadLayout.IsOnRoad(p.GetBounds()), "Seed pedestrian on road");
        var pedestrian = all.GetAllObjects().OfType<Pedestrian>().First();
        Invoke(form, "PictureBoxCanvas_MouseDown", null, new MouseEventArgs(MouseButtons.Left, 1, pedestrian.X + 2, pedestrian.Y + 2, 0));
        Invoke(form, "PictureBoxCanvas_MouseMove", null, new MouseEventArgs(MouseButtons.Left, 0, 380, 250, 0));
        Check(!RoadLayout.IsOnRoad(pedestrian.GetBounds()), "Drag preview on road");
        Invoke(form, "PictureBoxCanvas_MouseUp", null, new MouseEventArgs(MouseButtons.Left, 0, 380, 250, 0));
        Check(!RoadLayout.IsOnRoad(pedestrian.GetBounds()), "Drop on road");
        Field<ComboBox>(form, "comboBoxEntityType").SelectedItem = "Bus";
        Field<NumericUpDown>(form, "numericUpDownLane").Value = 1;
        Invoke(form, "OnAddEntityClick", null, EventArgs.Empty);
        Check(all.GetAllObjects().Last().Lane == 0, "Manual bus not in right lane");
        foreach (bool night in new[] { false, true })
        {
            if (night) form.ToggleNightMode();
            using var bitmap = new Bitmap(760, 500);
            using var graphics = Graphics.FromImage(bitmap);
            form.OnPaint(null, new PaintEventArgs(graphics, new Rectangle(0, 0, 760, 500)));
            bitmap.Save(Path.Combine(AppContext.BaseDirectory, night ? "qa-night.png" : "qa-day.png"));
        }
        Check(all.IsNightMode, "Night state not synchronized");
        Invoke(form, "OnRunClick", null, EventArgs.Empty);
        for (int tick = 0; tick < 800; tick++)
        {
            Invoke(form, "SimTimer_Tick", null, EventArgs.Empty);
            foreach (var vehicle in all.GetAllObjects().OfType<RoadUser>().Where(v => !(v is Pedestrian)))
            foreach (var other in all.GetAllObjects().Where(o => o != vehicle))
                Check(!vehicle.GetBounds().IntersectsWith(other.GetBounds()), $"Seed overlap at tick {tick}: {vehicle.GetType().Name}/{other.GetType().Name}");
        }
        Check(all.GetTotalMileage() > 0, "Seed simulation did not move");
        Invoke(form, "OnDeleteEntityClick", null, EventArgs.Empty);
        Check(all.Count == 0 && all.GetTotalMileage() == 0, "Delete all did not reset scene");
    }
}

using GameEngine;
using GameEngine.Inputs;
using GameEngine.Maths;
using GameEngine.Maths.Vectors;
using GameEngine.UI;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Font = GameEngine.Font.Font;

namespace RopePhysics
{
    public class Game : Engine
    {
        const float Gravity = 9.8f;
        const int numbIterations = 50;
        const int pointRadius = 20;
        const int minPointRadius = 2;
        const float MovementSpeed = 250f;

        public static float WindowWidth { get; private set; }
        public static float WindowHeight { get; private set; }

        private List<Point> points;
        private List<Stick> sticks;

        private Point selectedPoint;

        private UIManager ui;
        private Font font;

        private bool simulating = false;

        public static float Zoom { get; private set; } = 1f;
        public static Vector2F Pos { get; private set; } = Vector2F.Zero;

        protected override void Initialize()
        {
            GameWindow.WindowStyle = WindowStyle.None;
            GameWindow.ResizeMode = ResizeMode.NoResize;
            GameWindow.ReInit();

            WindowWidth = GameWindow.Buffer.Width;
            WindowHeight = GameWindow.Buffer.Height;

            points = new List<Point>();
            sticks = new List<Stick>();

            GameWindow.Input.UnHook();
            GameWindow.Input.MouseDown += OnMouseDown;
            GameWindow.Input.KeyDown += OnKeyDown;
            GameWindow.Input.MouseMove += OnMouseMove;
            GameWindow.Input.MouseWheel += OnMouseWheel;
            GameWindow.Input.MouseUp += OnMouseUp;
            GameWindow.Input.Hook();

            font = new Font(Util.ReadResource("Roboto-Thin.ttf"), 1);
            ui = new UIManager(this, font);
        }

        private void OnMouseUp(object sender, IMouseEventArgs args)
        {
            if (args.Buttons == MouseButtons.Right && selectedPoint != null)
                selectedPoint = null;
        }

        private void OnMouseWheel(object sender, IMouseEventArgs args)
        {
            if (args.WheelDelta > 0)
                Zoom += 0.05f * Zoom;
            else if (args.WheelDelta < 0)
                Zoom -= 0.05f * Zoom;

            if (Zoom < 0.075f)
                Zoom = 0.075f;
            else if (Zoom > 2.5f)
                Zoom = 2.5f;
        }

        private void OnMouseMove(object sender, IMouseEventArgs args)
        {
            if ((simulating || GameWindow.Input.GetKey(Key.Delete).pressed) && GameWindow.Input.mouseButtons == MouseButtons.Right) {
                List<Point> pointToRemove = new List<Point>();
                Vector2F pos = (Vector2F)GameWindow.Input.mousePosScreen;
                for (int i = 0; i < points.Count; i++)
                    if (Util.Project(points[i].Pos).Distance(pos) < pointRadius / 2f) {
                        pointToRemove.Add(points[i]);
                        points.RemoveAt(i);
                        i--;
                    }

                for (int i = 0; i < sticks.Count; i++) {
                    if (Util.LineToPointDistance(Util.Project(sticks[i].PointA.Pos), Util.Project(sticks[i].PointB.Pos), (Vector2F)args.Position) < 4f
                        || IsIntersecting(Util.Project(sticks[i].PointA.Pos), Util.Project(sticks[i].PointB.Pos),
                        GameWindow.Input.mousePosScreen, (Vector2F)args.Position)) {
                        sticks.RemoveAt(i);
                        i--;
                    }
                    else
                        for (int j = 0; j < pointToRemove.Count; j++)
                            if (sticks[i].PointA == pointToRemove[j] || sticks[i].PointB == pointToRemove[j]) {
                                sticks.RemoveAt(i);
                                i--;
                                break;
                            }
                }
            }
        }

        private void OnKeyDown(object sender, IKeyEventArgs args)
        {
            if (args.Key == Key.Space)
                simulating = !simulating;
            else if (args.Key == Key.C) {
                points.Clear();
                sticks.Clear();
                Pos = Vector2F.Zero;
                Zoom = 1f;
            }
        }

        private void OnMouseDown(object sender, IMouseEventArgs args)
        {
            if (!simulating){
                if (args.Buttons == MouseButtons.Left) {
                    Vector2F pos = Util.UnProject((Vector2F)args.Position);
                    points.Add(new Point() { Locked = false, Pos = pos, PrevPos = pos });
                }
                else if (args.Buttons == MouseButtons.Middle) {
                    Point p = GetClicked(args.Position, pointRadius);
                    if (p != null)
                        p.Locked = !p.Locked;
                } else if (args.Buttons == MouseButtons.Right) {
                    Vector2F pos = Util.UnProject((Vector2F)args.Position);

                    if (selectedPoint == null) {
                        Point p = GetClicked(args.Position, pointRadius);
                        if (p != null)
                            selectedPoint = p;
                    } else {
                        Point p = GetClicked(args.Position, pointRadius);
                        if (p != null) {
                            float lenght = new Vector2F(Math.Abs(selectedPoint.Pos.X - p.Pos.X),
                                Math.Abs(selectedPoint.Pos.Y - p.Pos.Y)).Lenght();

                            Stick s = new Stick() { PointA = selectedPoint, PointB = p, Length = lenght };
                            bool found = false;
                            for (int i = 0; i < sticks.Count; i++)
                                if (s == sticks[i]) {
                                    found = true;
                                    break;
                                }

                            if (!found)
                                sticks.Add(s);
                            selectedPoint = p;
                        }
                    }
                }
            }
        }

        protected override void drawInternal()
        {
            GameWindow.Input.Update();
            float delta = FpsCounter.DeltaTimeF;

            Console.Title = FpsCounter.FpsString + $" Z: {Zoom} P: {Pos}";

            if (GameWindow.Input.GetKey(Key.W).pressed)
                Pos += Vector2F.UnitY * delta * MovementSpeed;
            else if (GameWindow.Input.GetKey(Key.S).pressed)
                Pos += -Vector2F.UnitY * delta * MovementSpeed;
            if (GameWindow.Input.GetKey(Key.A).pressed)
                Pos += Vector2F.UnitX * delta * MovementSpeed;
            else if (GameWindow.Input.GetKey(Key.D).pressed)
                Pos += -Vector2F.UnitX * delta * MovementSpeed;

            if (GameWindow.Input.mouseButtons == MouseButtons.Right && selectedPoint != null && !GameWindow.Input.GetKey(Key.Delete).pressed) {
                Point p = GetClicked(GameWindow.Input.mousePosScreen, pointRadius);
                if (p != null) {
                    float lenght = new Vector2F(Math.Abs(selectedPoint.Pos.X - p.Pos.X),
                        Math.Abs(selectedPoint.Pos.Y - p.Pos.Y)).Lenght();

                    Stick s = new Stick() { PointA = selectedPoint, PointB = p, Length = lenght };
                    bool found = false;
                    for (int i = 0; i < sticks.Count; i++)
                        if (s == sticks[i]) {
                            found = true;
                            break;
                        }

                    if (!found)
                        sticks.Add(s);
                    selectedPoint = p;
                }
            }
            else if ((simulating || GameWindow.Input.GetKey(Key.Delete).pressed) && GameWindow.Input.mouseButtons == MouseButtons.Right) {
                List<Point> pointToRemove = new List<Point>();
                Vector2F pos = (Vector2F)GameWindow.Input.mousePosScreen;
                for (int i = 0; i < points.Count; i++)
                    if (Util.Project(points[i].Pos).Distance(pos) < pointRadius / 2f) {
                        pointToRemove.Add(points[i]);
                        points.RemoveAt(i);
                        i--;
                    }

                for (int i = 0; i < sticks.Count; i++)
                    if (Util.LineToPointDistance(Util.Project(sticks[i].PointA.Pos), Util.Project(sticks[i].PointB.Pos), pos) < 4f) {
                        sticks.RemoveAt(i);
                        i--;
                    } else
                        for (int j = 0; j < pointToRemove.Count; j++)
                            if (sticks[i].PointA == pointToRemove[j] || sticks[i].PointB == pointToRemove[j]) {
                                sticks.RemoveAt(i);
                                i--;
                                break;
                            }
            }

            if (simulating)
                Simulate(delta);

            Clear(Color.Black);

            for (int i = 0; i < sticks.Count; i++)
                DrawLine(Util.Project((Vector2I)sticks[i].PointA.Pos), Util.Project((Vector2I)sticks[i].PointB.Pos), Color.White);
            for (int i = 0; i < points.Count; i++)
                FillCircle(Util.Project((Vector2I)points[i].Pos), Math.Max((int)((pointRadius / 4) * Zoom), minPointRadius), points[i].Locked ? Color.Red : Color.White);

            if (!simulating) {
                if (selectedPoint != null) {
                    Vector2I pos = GameWindow.Input.mousePosScreen;
                    DrawLine(Util.Project((Vector2I)selectedPoint.Pos), pos, Color.Cyan);
                }
            }

            ui.RenderAndCaschText($"Controlls\n" +
                $"  Move->WSAD\n" +
                $"  Zoom->Mouse wheel\n" +
                $"  Play/Pause->Space\n" +
                $">\n" +
                $"  Create point->LeftMB\n" +
                $"  Create line->RightMB\n" +
                $"  Toggle point movable->MiddleMB\n" +
                $"  Clear map->C\n" +
                $"||\n" +
                $"  RML->Delete stick",
                14, Color.Wheat.ToArgb(), 20, 20);
            string text = simulating ? "||" : ">";
            int size = 50;
            ui.RenderAndCaschText(text, size, Color.White.ToArgb(), 
                GameWindow.Width / 2 - GameEngine.Font.FontRender.GetTextSize(font, text, size).Width / 2, 50);
        }

        private bool IsIntersecting(Vector2F a, Vector2F b, Vector2F c, Vector2F d)
        {
            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            // Detect coincident lines (has a problem, read below)
            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        private void Simulate(float delta)
        {
            for (int i = 0; i < points.Count; i++) {
                Point p = points[i];
                if (p.Locked)
                    continue;

                Vector2F posBeforeUpdate = p.Pos;
                p.Pos += p.Pos - p.PrevPos;
                p.Pos += new Vector2F(0f, 1f) * Gravity * delta;
                p.PrevPos = posBeforeUpdate;
            }

            for (int j = 0; j < numbIterations; j++)
                for (int i = 0; i < sticks.Count; i++) {
                    Stick s = sticks[i];
                    Vector2F center = (s.PointA.Pos + s.PointB.Pos) / 2f;
                    Vector2F dir = (s.PointA.Pos - s.PointB.Pos).Normalized();
                    if (!s.PointA.Locked)
                        s.PointA.Pos = center + dir * s.Length / 2f;
                    if (!s.PointB.Locked)
                        s.PointB.Pos = center - dir * s.Length / 2f;
                }
        }

        private Point GetClicked(Vector2D mousePos, float maxDistance)
        {
            Vector2F pos = Util.UnProject((Vector2F)mousePos);

            float closestDistance = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < points.Count; i++) {
                float distance = points[i].Pos.Distance(pos);
                if (distance < closestDistance) {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }

            float _max = Zoom <= 1f ? maxDistance / Zoom : maxDistance * Zoom;
            if (closestIndex != -1 && closestDistance < _max)
                return points[closestIndex];
            else
                return null;
        }
    }
}

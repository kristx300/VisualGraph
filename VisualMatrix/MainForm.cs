using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisualGraph
{
    public partial class MainForm : Form
    {
        private FormStatusBar status;

        private FormStatusBar Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                var sb = new StringBuilder();
                sb.Append("Status: ");
                switch (value)
                {
                    case FormStatusBar.Nothing:
                        sb.Append("Пустой");
                        break;

                    case FormStatusBar.AddItem:
                        sb.Append("Добавление элементов");
                        break;

                    case FormStatusBar.AddLink:
                        sb.Append("Добавление связей");
                        break;

                    default:
                        sb.Append("Пустой");
                        break;
                }
                FormStatusStrip.Items[0].Text = sb.ToString();
            }
        }

        private const int R = 30;
        private Random random = new Random();
        private Point buffer;
        private bool[] used;
        private List<List<int>> names;
        private bool[,] Links { get; set; }
        private List<Ellipse> Ellipses { get; set; }
        private List<Color> Colors { get; set; }
        private List<LinkColor> LColors { get; set; }
        private Queue<int> QueueToDraw { get; set; }

        public MainForm()
        {
            InitializeComponent();
            Status = FormStatusBar.Nothing;
            Ellipses = new List<Ellipse>();
            QueueToDraw = new Queue<int>();
            Colors = new List<Color>();
            LColors = new List<LinkColor>();
            for (int i = 0; i < 100; i++)
            {
                Colors.Add(GetRandomColor());
            }
            Links = new bool[0, 0];
            used = new bool[0];
        }

        #region Visual

        private void ButtonAddItem_Click(object sender, EventArgs e)
        {
            Status = FormStatusBar.AddItem;
        }

        private void ButtonAddLink_Click(object sender, EventArgs e)
        {
            Status = FormStatusBar.AddLink;
        }

        private static Color GetBlackColor()
        {
            return Color.FromArgb(0, 0, 0);
        }

        private Color GetRandomColor()
        {
            return Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
        }

        private static Color GetVisitColor()
        {
            return Color.FromArgb(61, 153, 112);
        }

        private static SolidBrush GetBlackColorSolidBrush()
        {
            return new SolidBrush(GetBlackColor());
        }

        private static Pen GetBlackColorPen()
        {
            return new Pen(GetBlackColor(), 2);
        }

        private static Pen GetVisitColorPen()
        {
            return new Pen(GetVisitColor(), 2);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (Status)
            {
                case FormStatusBar.Nothing:
                    break;

                case FormStatusBar.AddItem:
                    DrawEllipse(e.X, e.Y);
                    break;

                case FormStatusBar.AddLink:
                    MouseDownLine(e.X, e.Y);
                    break;

                default:
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (Status)
            {
                case FormStatusBar.Nothing:
                    break;

                case FormStatusBar.AddItem:
                    DrawEllipse(e.X, e.Y);
                    break;

                case FormStatusBar.AddLink:
                    MouseUpLine(e.X, e.Y);
                    break;

                default:
                    break;
            }
        }

        private void DrawEllipse(int x, int y)
        {
            using (var p = GetBlackColorPen())
            {
                using (var g = CreateGraphics())
                {
                    if (Ellipses.FirstOrDefault(e => e.IsIn(x, y)) == null)
                    {
                        g.DrawEllipse(p, x, y, R, R);
                        g.DrawString(Ellipses.Count.ToString(), new Font("Arial", 10), GetBlackColorSolidBrush(), x + 10, y + 10);

                        Ellipses.Add(new Ellipse { X = x, Y = y });
                        Links = new bool[Ellipses.Count, Ellipses.Count];
                        LColors = new List<LinkColor>();
                        ReDrawGraph();
                    }
                }
            }
        }

        private void MouseDownLine(int x, int y)
        {
            buffer = new Point(x, y);
        }

        private void MouseUpLine(int x, int y)
        {
            if (Status == FormStatusBar.AddLink && buffer != null)
            {
                using (var p = GetBlackColorPen())
                {
                    using (var g = CreateGraphics())
                    {
                        var e1 = Ellipses.IndexOf(Ellipses.FirstOrDefault(f => f.IsIn(buffer.X, buffer.Y)));
                        var e2 = Ellipses.IndexOf(Ellipses.FirstOrDefault(f => f.IsIn(x, y)));

                        if (e1 < 0 || e2 < 0)
                        {
                            return;
                        }

                        if (Links[e1, e2] == true)
                        {
                            return;
                        }
                        else
                        {
                            Links[e1, e2] = true;
                        }

                        if (Links[e2, e1] == true)
                        {
                            return;
                        }
                        else
                        {
                            Links[e2, e1] = true;
                        }

                        var point = new Point { X = ((Ellipses[e1].X + 15 + Ellipses[e2].X + 15) / 2), Y = ((Ellipses[e1].Y + 15 + Ellipses[e2].Y + 15) / 2) };
                        var w = random.Next(0, 150);
                        g.DrawString(string.Format("{0}({1})", LColors.Count, w), new Font("Arial", 10), GetBlackColorSolidBrush(), point);
                        g.DrawLine(p, Ellipses[e1].X + 15, Ellipses[e1].Y, Ellipses[e2].X + 15, Ellipses[e2].Y);
                        LColors.Add(new LinkColor { F = Ellipses[e1], S = Ellipses[e2], FIndex = e1, SIndex = e2, Weight = w });
                    }
                }
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            Status = FormStatusBar.Nothing;
            using (var g = CreateGraphics())
            {
                g.Clear(BackColor);
            }
            Ellipses = new List<Ellipse>();
            Links = new bool[Ellipses.Count, Ellipses.Count];
            LColors = new List<LinkColor>();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            ReDrawGraph();
        }

        private void ReDrawV()
        {
            using (var g = CreateGraphics())
            {
                using (var p = GetBlackColorPen())
                {
                    int counter = 0;
                    foreach (var item in Ellipses)
                    {
                        g.FillEllipse(item.Brush, item.X, item.Y, R, R);
                        g.DrawString(counter.ToString(), new Font("Arial", 10), GetBlackColorSolidBrush(), item.X + 10, item.Y + 10);
                        counter++;
                    }
                }
            }
        }

        private void ReDrawE()
        {
            using (var g = CreateGraphics())
            {
                foreach (var item in LColors)
                {
                    using (var p = new Pen(item.Brush, 2))
                    {
                        g.DrawLine(p, item.F.X + 15, item.F.Y, item.S.X + 15, item.S.Y);
                    }
                }
            }
        }

        private void ReDrawGraph()
        {
            using (var g = CreateGraphics())
            {
                using (var p = GetBlackColorPen())
                {
                    g.Clear(BackColor);
                    int counter = 0;
                    foreach (var item in Ellipses)
                    {
                        g.DrawEllipse(p, item.X, item.Y, R, R);
                        g.DrawString(counter.ToString(), new Font("Arial", 10), GetBlackColorSolidBrush(), item.X + 10, item.Y + 10);
                        counter++;
                    }
                    counter = 0;
                    foreach (var item in LColors)
                    {
                        g.DrawLine(p, item.F.X + 15, item.F.Y, item.S.X + 15, item.S.Y);

                        var point = new Point { X = ((item.F.X + 15 + item.S.X + 15) / 2), Y = ((item.F.Y + 15 + item.S.Y + 15) / 2) };
                        g.DrawString(string.Format("{0}({1})", counter, item.Weight), new Font("Arial", 10), GetBlackColorSolidBrush(), point);
                        counter++;
                    }
                }
            }
        }

        #endregion Visual

        #region Helpers

        private bool DFSForHamiltonianPath(int v, bool[,] links, int[] label, int instack_count)
        {
            if (instack_count == links.GetLength(0))
            {
                return true;
            }
            for (int i = 0; i < links.GetLength(0); i++)
            {
                if (links[v, i] && label[i] == 0)
                {
                    label[i] = 1;
                    if (DFSForHamiltonianPath(i, links, label, instack_count + 1))
                    {
                        return true;
                    }
                    label[i] = 0;
                }
            }
            return false;
        }

        private bool CheckHamiltonianPath(bool[,] links)
        {
            int[] label = Enumerable.Range(0, Ellipses.Count).Select(n => 0).ToArray();

            for (int i = 0; i < links.GetLength(0); i++)
            {
                label[i] = 1;
                if (DFSForHamiltonianPath(i, links, label, 1))
                    return true;
                label[i] = 0;
            }
            return false;
        }

        private void DFS(int v)
        {
            used[v] = true;
            for (int i = 0; i < Links.GetLength(1); i++)
            {
                if (!used[i] && Links[v, i])
                {
                    DFS(i);
                }
            }
        }

        private void DFSDraw(int v)
        {
            used[v] = true;
            DrawVisit(v);
            for (int i = 0; i < Links.GetLength(1); i++)
            {
                if (!used[i] && Links[v, i])
                {
                    DFSDraw(i);
                }
            }
        }

        private void BFSDraw(int s)
        {
            Queue<int> queue = new Queue<int>();
            used = new bool[Ellipses.Count];
            used[s] = true;
            DrawVisit(s);
            queue.Enqueue(s);
            while (queue.Count != 0)
            {
                int v = queue.Dequeue();
                for (int i = 0; i < Links.GetLength(0); i++)
                {
                    if (!used[i] && Links[v, i])
                    {
                        used[i] = true;
                        queue.Enqueue(i);
                        DrawVisit(i);
                    }
                }
            }
        }

        private void DrawVisit(int i)
        {
            QueueToDraw.Enqueue(i);
        }

        private void TimerToDraw_Tick(object sender, EventArgs e)
        {
            if (QueueToDraw.Count == 0)
            {
                return;
            }
            var i = QueueToDraw.Dequeue();
            if (Ellipses.Count < i + 1)
            {
                return;
            }
            using (var p = GetVisitColorPen())
            {
                using (var g = CreateGraphics())
                {
                    g.DrawEllipse(p, Ellipses[i].X, Ellipses[i].Y, R, R);
                }
            }
        }

        private void CalculateChains()
        {
            used = new bool[Ellipses.Count];
            names = new List<List<int>>();
            for (int i = 0; i < Ellipses.Count; i++)
            {
                DFS(i);
                var lst = new List<int>();
                for (int j = 0; j < Ellipses.Count; j++)
                {
                    if (used[j] == true && !names.Any(x => x.Any(y => y == j)))
                    {
                        lst.Add(j);
                    }
                }
                if (lst.Count != 0)
                {
                    names.Add(lst);
                }
            }
        }

        private string ColorE()
        {
            for (int i = 0; i < LColors.Count; i++)
            {
                int col = 1;
                flag:
                LColors[i].Color = Colors[col];
                for (int j = 0; j < LColors.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (LColors[j].FIndex == LColors[i].FIndex ||
                        LColors[j].FIndex == LColors[i].SIndex ||
                        LColors[j].SIndex == LColors[i].FIndex ||
                        LColors[j].SIndex == LColors[i].SIndex)
                    {
                        if (LColors[j].Color == LColors[i].Color)
                        {
                            col++;
                            goto flag;
                        }
                    }
                }
            }
            var sb = new StringBuilder();
            for (int u = 0; u < LColors.Count; u++)
            {
                sb.AppendLine(string.Format("Ребро между [{0}] [{1}] -> имеет цвет [{2}]", LColors[u].FIndex, LColors[u].SIndex, LColors[u].Color.ToString()));
            }
            return sb.ToString();
        }

        private string ColorV()
        {
            int[] result = Enumerable.Range(0, Ellipses.Count).Select(n => -1).ToArray();
            result[0] = 0;

            bool[] available = Enumerable.Range(0, Ellipses.Count).Select(n => true).ToArray();

            for (int u = 1; u < Ellipses.Count; u++)
            {
                for (int i = 0; i < Ellipses.Count; i++)
                {
                    if (result[i] != -1 && Links[u, i])
                    {
                        available[result[i]] = false;
                    }
                }
                int cr;
                for (cr = 0; cr < Ellipses.Count; cr++)
                {
                    if (available[cr])
                        break;
                }
                result[u] = cr;
                available = Enumerable.Range(0, Ellipses.Count).Select(n => true).ToArray();
            }

            var sb = new StringBuilder();
            for (int u = 0; u < Ellipses.Count; u++)
            {
                Ellipses[u].Color = Colors[result[u]];
                sb.AppendLine(string.Format("Вершина [{0}] -> Цвет [{1}]", u, Colors[result[u]].ToString()));
            }
            return sb.ToString();
        }

        private string ShortestPath(int start, int finish)
        {
            List<int> d = Enumerable.Range(0, Ellipses.Count).Select(n => int.MaxValue).ToList();
            d[start] = 0;
            List<int> p = Enumerable.Range(0, Ellipses.Count).Select(n => -1).ToList();
            for (; ; )
            {
                bool any = false;
                for (int j = 0; j < LColors.Count; j++)
                {
                    if (d[LColors[j].FIndex] <= int.MaxValue)
                    {
                        if (d[LColors[j].SIndex] > d[LColors[j].FIndex] + LColors[j].Weight)
                        {
                            d[LColors[j].SIndex] = d[LColors[j].FIndex] + LColors[j].Weight;
                            p[LColors[j].SIndex] = LColors[j].FIndex;
                            any = true;
                        }
                    }
                }
                if (!any)
                {
                    break;
                }
            }
            if (d[finish] == int.MaxValue)
            {
                return "Нету пути";
            }
            else
            {
                string path = string.Empty;

                for (int cr = finish; cr != -1; cr = p[cr])
                {
                    path += cr;
                }
                path = new string(path.Reverse().ToArray());

                return path;
            }
        }

        #endregion Helpers

        #region Actions

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Ellipses.Count == 0)
            {
                MessageBox.Show("Нету вершин графа");
                return;
            }
            if (Links.All(x => x == false))
            {
                MessageBox.Show("Нету ребер графа, граф не связный");
                return;
            }
            CalculateChains();
            string msg = used.All(x => x == true) && names.Count == 1 ? "Связный граф" : "Не связный граф";
            msg += "\n";
            int counter = 1;
            foreach (var item in names)
            {
                msg += counter + " компонент {";
                msg += string.Join(",", item.Select(x => x.ToString()).ToArray());
                msg += "}\n";
                counter++;
            }
            MessageBox.Show(msg, "Связность графа и его компоненты");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            used = new bool[Ellipses.Count];
            TimerToDraw.Start();
            DFSDraw(0);
        }

#pragma warning disable IDE0018

        private void Button6_Click(object sender, EventArgs e)
        {
            used = new bool[Ellipses.Count];
            int s1;
            int s2;
            if (!int.TryParse(Fields.Text.Substring(0, Fields.Text.IndexOf(" ") + 1), out s1))
            {
                s1 = 0;
            }
            if (!int.TryParse(Fields.Text.Substring(Fields.Text.IndexOf(" ") + 1), out s2))
            {
                s2 = 0;
            }
            if (s1 < 0 || s1 > Ellipses.Count)
            {
                s1 = 0;
            }
            if (s2 < 0 || s2 > Ellipses.Count)
            {
                s2 = Ellipses.Count - 1;
            }
            MessageBox.Show(ShortestPath(s1, s2), "Кратчайший путь");
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            used = new bool[Ellipses.Count];
            TimerToDraw.Start();
            BFSDraw(0);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            CalculateChains();
            bool connectedGraph = used.All(x => x == true) && names.Count == 1 ? true : false;
            int[] degrees = new int[Ellipses.Count];
            if (connectedGraph)
            {
                msg = "Граф связный ->";
                for (int i = 0; i < degrees.Length; i++)
                {
                    for (int j = 0; j < degrees.Length; j++)
                    {
                        if (Links[i, j])
                        {
                            degrees[i]++;
                        }
                    }
                }
                if (degrees.All(x => x % 2 == 0))
                {
                    msg += " граф является эйлеровым, так как все вершины четные";
                }
                else if (degrees.Count(x => x % 2 != 0) == 0 || degrees.Count(x => x % 2 != 0) == 2)
                {
                    msg += " граф является эйлеровым, так как число вершин с нечётной степенью равно нулю или двум";
                }
                else
                {
                    msg += " граф не является эйлеровым, так как не все вершины четные";
                }
                for (int i = 0; i < degrees.Length; i++)
                {
                    msg += Environment.NewLine + string.Format("[{0}] = {1}", i, degrees[i]);
                }
            }
            else
            {
                msg = "Граф не связный -> граф не является эйлеровым";
            }
            MessageBox.Show(msg, "Эйлеров граф");
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            var res = CheckHamiltonianPath(Links);
            string msg = res ? "Граф является Гамильтоновым" : "Граф не является Гамильтоновым";
            MessageBox.Show(msg, "Гамильтонов граф");
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            string msg = ColorV();
            ReDrawV();
            MessageBox.Show(msg, "Раскраска вершин");
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            var msg = ColorE();
            ReDrawE();
            MessageBox.Show(msg, "Реберная раскраска");
        }

        #endregion Actions
    }
}
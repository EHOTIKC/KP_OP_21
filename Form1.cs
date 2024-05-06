using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace KP_OP_21
{
    public partial class Form1 : Form
    {

        private void button1_Click(object sender, EventArgs e)
        {
            //KruskalAlgorithm();
            //PrimAlgorithm();
            BoruvkaAlgorithm();
        }


        private void PrimAlgorithm()
        {
            // Ініціалізуємо матрицю суміжності
            InitializeAdjacencyMatrix();

            // Індекси вибраних вершин
            HashSet<int> selectedVertices = new HashSet<int>();
            selectedVertices.Add(0); // Починаємо з першої вершини

            // Кількість вершин
            int vertexCount = vertices.Count;

            // Масив, що містить індекси ребер, які утворюють мінімальне остовне дерево
            minimumSpanningTreeEdgeIDs.Clear();

            while (selectedVertices.Count < vertexCount)
            {
                double minWeight = double.MaxValue;
                Edge minEdge = null;
                int minVertexIndex = -1;

                // Проходимось по кожній вершині, що вже включена у мінімальне остовне дерево
                foreach (var vertexIndex in selectedVertices)
                {
                    // Проходимось по всіх ребрах вершини
                    foreach (var edge in vertices[vertexIndex].Edges)
                    {
                        // Отримуємо індекс іншої вершини, яка не є частиною мінімального остовного дерева
                        int otherVertexIndex = edge.GetOtherVertexIndex(vertexIndex);

                        // Якщо інша вершина ребра ще не вибрана та вага менша за мінімальну
                        if (!selectedVertices.Contains(otherVertexIndex) && edge.Weight < minWeight)
                        {
                            minWeight = edge.Weight;
                            minEdge = edge;
                            minVertexIndex = otherVertexIndex;
                        }
                    }
                }

                // Якщо знайдено ребро
                if (minEdge != null && minVertexIndex != -1)
                {
                    // Позначаємо ребро як вибране
                    minEdge.IsInMinimumSpanningTree = true;
                    minimumSpanningTreeEdgeIDs.Add(minEdge.Id);

                    // Додаємо нову вершину до множини вибраних вершин
                    selectedVertices.Add(minVertexIndex);
                }
                else
                {
                    // Якщо немає доступних ребер, виходимо з циклу
                    break;
                }
            }

            // Позначаємо всі вершини, які з'єднані з мінімальним остовним деревом, як вибрані
            foreach (var vertex in vertices)
            {
                if (selectedVertices.Contains(vertex.Id))
                {
                    vertex.vertexIsInMinimumSpanningTree = true;
                }
            }

            // Оновлюємо відображення форми
            Refresh();
        }


        private void BoruvkaAlgorithm()
        {
            // Ініціалізуємо матрицю суміжності
            InitializeAdjacencyMatrix();

            // Кількість вершин
            int vertexCount = vertices.Count;

            // Створюємо список, що містить індекси ребер, які утворюють мінімальне остовне дерево
            minimumSpanningTreeEdgeIDs.Clear();

            // Поки не утвориться мінімальне остовне дерево
            while (minimumSpanningTreeEdgeIDs.Count < vertexCount - 1)
            {
                // Створюємо масив, що містить індекси найдешевших ребер для кожного компоненту
                int[] cheapestEdges = new int[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    cheapestEdges[i] = -1; // Ініціалізуємо значенням -1, яке вказуватиме, що ребро ще не знайдено
                }

                // Перебираємо всі ребра графу
                foreach (var edge in edges)
                {
                    int rootStart = FindRoot(edge.StartVertex);
                    int rootEnd = FindRoot(edge.EndVertex);

                    // Якщо ребро сполучає два різні компоненти
                    if (rootStart != rootEnd)
                    {
                        // Якщо це найдешевше ребро для одного з компонентів або жодний з компонентів не має ще ребра
                        if (cheapestEdges[rootStart] == -1 || edge.Weight < edges[cheapestEdges[rootStart]].Weight)
                        {
                            cheapestEdges[rootStart] = edges.IndexOf(edge);
                        }
                        if (cheapestEdges[rootEnd] == -1 || edge.Weight < edges[cheapestEdges[rootEnd]].Weight)
                        {
                            cheapestEdges[rootEnd] = edges.IndexOf(edge);
                        }
                    }
                }

                // Додаємо найдешевші ребра до мінімального остовного дерева
                foreach (int edgeIndex in cheapestEdges)
                {
                    if (edgeIndex != -1 && !minimumSpanningTreeEdgeIDs.Contains(edgeIndex))
                    {
                        minimumSpanningTreeEdgeIDs.Add(edgeIndex);
                        edges[edgeIndex].IsInMinimumSpanningTree = true;
                    }
                }
            }

            // Оновлюємо відображення форми
            Refresh();
        }

        // Метод для знаходження кореня компоненту, до якого входить дана вершина
        private int FindRoot(Vertex vertex)
        {
            int index = vertices.IndexOf(vertex);
            while (vertex != vertices[index])
            {
                vertex = vertices[index];
                index = vertices.IndexOf(vertex);
            }
            return index;
        }




        // Функція для знаходження кількості різних компонент зв'язності
        private int GetDistinctComponentsCount(int[] component)
        {
            HashSet<int> distinctComponents = new HashSet<int>(component);
            return distinctComponents.Count;
        }



        private void KruskalAlgorithm()
        {
            // Ініціалізуємо матрицю суміжності
            InitializeAdjacencyMatrix();

            // Кількість вершин
            int vertexCount = vertices.Count;

            DisjointSet disjointSet = new DisjointSet(vertexCount);

            // Обираємо ребра для мінімального остовного дерева
            foreach (var edge in edges.OrderBy(e => e.Weight))
            {
                int rootStart = disjointSet.Find(vertices.IndexOf(edge.StartVertex));
                int rootEnd = disjointSet.Find(vertices.IndexOf(edge.EndVertex));
                if (rootStart != rootEnd)
                {
                    // Якщо вершини не знаходяться в одній компоненті, додаємо ребро
                    edge.IsInMinimumSpanningTree = true;
                    disjointSet.Union(vertices.IndexOf(edge.StartVertex), vertices.IndexOf(edge.EndVertex));
                }
            }

            // Оновлюємо відображення форми
            Refresh();
        }




        private Edge CalculateEdgeFromIndex(int edgeIndex)
        {
            // Розраховуємо координати вершин, які утворюють ребро за їхніми індексами
            Button startButton = buttons[edgeIndex];
            int startIndex = buttons.IndexOf(startButton);
            //Vertex startPoint = new Vertex(startButton.Location.X + startButton.Width / 2, startButton.Location.Y + startButton.Height / 2);
            Vertex startPoint = new Vertex(startButton.Location.X + startButton.Width / 2, startButton.Location.Y + startButton.Height / 2);


            // Знаходимо вершину, з якою з'єднано ребро
            int connectedIndex = -1;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (adjacencyMatrix[startIndex, i] != 0)
                {
                    connectedIndex = i;
                    break;
                }
            }

            Button endButton = buttons[connectedIndex];
            Vertex endPoint = new Vertex(endButton.Location.X + endButton.Width / 2, endButton.Location.Y + endButton.Height / 2);

            return new Edge(startPoint, endPoint);
        }

        private void InitializeAdjacencyMatrix()
        {
            int size = vertices.Count;
            adjacencyMatrix = new double[size, size];

            // Заповнюємо матрицю суміжності вагами ребер
            foreach (var edge in edges)
            {
                int startIndex = vertices.IndexOf(edge.StartVertex);
                int endIndex = vertices.IndexOf(edge.EndVertex);
                adjacencyMatrix[startIndex, endIndex] = edge.Weight;
                adjacencyMatrix[endIndex, startIndex] = edge.Weight; // Симетричність матриці
            }
        }

        private Edge CalculateEdgeFromIndex(int vertexIndex, int connectedIndex)
        {
            // Розраховуємо координати вершин, які утворюють ребро за їхніми індексами
            Button startButton = buttons[vertexIndex];
            Vertex startVertex = new Vertex(startButton.Location.X + startButton.Width / 2, startButton.Location.Y + startButton.Height / 2);

            Button endButton = buttons[connectedIndex];
            Vertex endVertex = new Vertex(endButton.Location.X + endButton.Width / 2, endButton.Location.Y + endButton.Height / 2);

            return new Edge(startVertex, endVertex);
        }





        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Малюємо ребра, які утворюють мінімальне остовне дерево
            foreach (int edgeIndex in minimumSpanningTreeEdgeIDs)
            {
                Edge edge = CalculateEdgeFromIndex(edgeIndex);
                e.Graphics.DrawLine(Pens.Blue, edge.StartVertex.point, edge.EndVertex.point);
            }

            // Малюємо інші ребра
            foreach (var edge in edges)
            {
                if (edge.IsInMinimumSpanningTree)
                {
                    edge.Color = Color.Transparent;
                }

                e.Graphics.DrawLine(new Pen(edge.Color), edge.StartVertex.point, edge.EndVertex.point);
            }
        }

        enum Mode
        {
            addVerticle = 0,
            removePoint = 1,
        }
        Mode currentMode = 0;
        List<Button> buttons = new List<Button>(); // Список кнопок на формі
        List<Edge> edges = new List<Edge>(); // Список ребер
        List<Vertex> vertices = new List<Vertex>(); // Список вершин
        double[,] adjacencyMatrix; // Матриця суміжності
        List<int> minimumSpanningTreeEdgeIDs = new List<int>(); // Індекси ребер утвореного остовного дерева

        public Form1()
        {
         //   minimumSpanningTreeEdgeIDs[1] = vertices.Count();
            InitializeComponent();
        }


        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentMode == Mode.addVerticle)
            {
                vertices.Add(new Vertex(e.Location.X, e.Location.Y)); // Додаємо новий об'єкт вершини з координатами

                // Розширюємо матрицю суміжності при додаванні вершини
                ResizeAdjacencyMatrix();

                // Додавання кнопки для вершини
                Button newButton = new Button();
                newButton.Location = new Point(e.Location.X - 20, e.Location.Y - 20);
                newButton.Size = new Size(40, 40);
                newButton.BackColor = Color.Black;
                newButton.ForeColor = Color.White;
                newButton.Text = (vertices.Count).ToString();
                newButton.Click += new EventHandler(VerticleButtons);
                buttons.Add(newButton);
                this.Controls.Add(newButton);
                Refresh();
            }
        }



        bool isFirst = true;
        Button firstBut;
        Button secondBut;


        private void VerticleButtons(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                    firstBut = clickedButton;
                }
                else
                {
                    isFirst = true;
                    secondBut = clickedButton;
                }

                if (firstBut != null && secondBut != null)
                {
                    int firstIndex = buttons.IndexOf(firstBut);
                    int secondIndex = buttons.IndexOf(secondBut);

                    if (firstIndex != -1 && secondIndex != -1)
                    {
                        Vertex start = vertices[firstIndex];
                        Vertex end = vertices[secondIndex];

                        // Вага ребра
                        int weight = (int)adjacencyMatrix[firstIndex, secondIndex];

                        // Додаємо з'єднання між вершинами у матрицю суміжності
                        adjacencyMatrix[firstIndex, secondIndex] = weight;
                        adjacencyMatrix[secondIndex, firstIndex] = weight; // Зберігаємо симетричність

                        Edge newEdge = new Edge(start, end);
                        newEdge.Weight = weight; // Встановлюємо вагу ребра
                        edges.Add(newEdge);

                        // Додаємо ребро до вершин
                        start.AddEdge(newEdge);
                        end.AddEdge(newEdge);

                        this.Invalidate();
                        // Показуємо діалог для встановлення ваги ребра (якщо потрібно)
                        ShowEdgeWeightDialog(newEdge);

                        firstBut = null;
                        secondBut = null;
                    }
                }
            }
        }


        private void ResizeAdjacencyMatrix()
        {
            int size = vertices.Count;
            double[,] newMatrix = new double[size, size];

            if (adjacencyMatrix != null)
            {
                // Копіюємо існуючі дані у нову матрицю
                for (int i = 0; i < Math.Min(size - 1, adjacencyMatrix.GetLength(0)); i++)
                {
                    for (int j = 0; j < Math.Min(size - 1, adjacencyMatrix.GetLength(1)); j++)
                    {
                        newMatrix[i, j] = adjacencyMatrix[i, j];
                    }
                }
            }

            adjacencyMatrix = newMatrix;
        }

        private void PointRemove(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Control parentControl = clickedButton.Parent;

            parentControl.Controls.Remove(clickedButton);
            vertices.RemoveAt(vertices.Count - 1);
            ResizeAdjacencyMatrix();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var edge in edges)
            {
                PointF startPoint = new PointF((float)edge.StartVertex.X, (float)edge.StartVertex.Y);
                PointF endPoint = new PointF((float)edge.EndVertex.X, (float)edge.EndVertex.Y);

                Pen edgePen;
                if (edge.IsInMinimumSpanningTree)
                {
                    edgePen = new Pen(Color.Blue);
                }
                else
                {
                    edgePen = new Pen(Color.Black);
                }

                e.Graphics.DrawLine(edgePen, startPoint, endPoint);
            }
        }


        private void ShowEdgeWeightDialog(Edge edge)
        {
            while (true)
            {
                using (var dialog = new Form())
                {
                    dialog.Text = "Enter Edge Weight";
                    dialog.StartPosition = FormStartPosition.CenterParent;

                    TextBox weightTextBox = new TextBox();
                    weightTextBox.Location = new Point(50, 50);
                    weightTextBox.Width = 100;
                    dialog.Controls.Add(weightTextBox);

                    Button okButton = new Button();
                    okButton.Text = "OK";
                    okButton.DialogResult = DialogResult.OK;
                    okButton.Location = new Point(50, 100);
                    dialog.Controls.Add(okButton);

                    Button cancelButton = new Button();
                    cancelButton.Text = "Cancel";
                    cancelButton.DialogResult = DialogResult.Cancel;
                    cancelButton.Location = new Point(150, 100);
                    dialog.Controls.Add(cancelButton);

                    dialog.FormClosing += (sender, e) =>
                    {
                        if (dialog.DialogResult == DialogResult.OK)
                        {
                            float weight;
                            if (float.TryParse(weightTextBox.Text, out weight))
                            {
                                edge.Weight = weight;
                                this.Invalidate();
                            }
                            else
                            {
                                MessageBox.Show("Invalid weight value! Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                e.Cancel = true;
                            }
                        }
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            vertices.Clear();
            edges.Clear();
            minimumSpanningTreeEdgeIDs.Clear();

            foreach (Button button in buttons)
            {
                this.Controls.Remove(button);
                button.Dispose();
            }

            buttons.Clear();
            adjacencyMatrix = null;
            Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (currentMode == Mode.addVerticle)
            {
                currentMode = Mode.removePoint;
                Button clickedButton = (Button)sender;
                clickedButton.BackColor = Color.Gray;
            }
            else
            {
                currentMode = Mode.addVerticle;
                Button clickedButton = (Button)sender;
                clickedButton.BackColor = Color.White;
            }
        }
    }


    public class Edge
    {
        private static int edgeCount = 0; // Лічильник ребер

        public Edge(Vertex start, Vertex end)
        {
            StartVertex = start;
            EndVertex = end;
            IsInMinimumSpanningTree = false; // Початкове значення прапорця
            Color = Color.Black; // Початковий колір ребра
            Id = edgeCount++; // Присвоюємо ребру унікальний ідентифікатор
        }

        public int GetOtherVertexIndex(int startVertexIndex)
        {
            return StartVertex.Id == startVertexIndex ? EndVertex.Id : StartVertex.Id;
        }

        public int Id { get; private set; } // Ідентифікатор ребра
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        public double Weight { get; set; } // Вага ребра
        public bool IsInMinimumSpanningTree { get; set; } // Прапорець, що вказує, чи належить ребро до мінімального остовного дерева
        public Color Color { get; set; } // Колір ребра
    }



    public class Vertex
    {
        private static int vertexCount = -1; // Кількість створених вершин
        public int Id { get; private set; } // Ідентифікатор вершини
        public double X { get; set; } // Координата X
        public double Y { get; set; } // Координата Y
        public int Degree { get; set; } // Ступінь вершини
        public List<Edge> Edges { get; set; } // Список ребер, що з'єднують цю вершину
        public Point point { get; set; }
        public bool vertexIsInMinimumSpanningTree { get; set; } // Прапорець, що вказує, чи належить вершина до мінімального остовного дерева


        public Vertex(double x, double y)
        {
            Id = ++vertexCount;
            X = x;
            Y = y;
            Degree = 0;
            Edges = new List<Edge>();
            point = new Point((int)x, (int)y);
            vertexIsInMinimumSpanningTree = false; // Початкове значення прапорця

        }

        // Додати ребро до вершини
        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
            Degree++;
        }

        // Перевірити, чи вершина має з'єднання з іншою вершиною
        public bool IsConnectedTo(Vertex otherVertex)
        {
            foreach (var edge in Edges)
            {
                if (edge.StartVertex == this && edge.EndVertex == otherVertex ||
                    edge.StartVertex == otherVertex && edge.EndVertex == this)
                {
                    return true;
                }
            }
            return false;
        }

    }


    public class DisjointSet
    {
        private int[] parent;

        public DisjointSet(int size)
        {
            parent = new int[size];
            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
            }
        }

        public int Find(int x)
        {
            if (parent[x] != x)
            {
                parent[x] = Find(parent[x]);
            }
            return parent[x];
        }

        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);
            if (rootX != rootY)
            {
                parent[rootX] = rootY;
            }
        }
    }


}

using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace KP_OP_21
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            interfaceButtons.Add(button1);
            interfaceButtons.Add(button2);
            interfaceButtons.Add(button5);
            interfaceButtons.Add(button6);
            interfaceButtons.Add(button7);

            button1.Click += new EventHandler(this.StartSearchClick);
            button2.Click += new EventHandler(this.ClearGraph);
            button5.Click += new EventHandler(this.AddVetexMode);
            button6.Click += new EventHandler(this.AddEdgeMode);
            button7.Click += new EventHandler(this.SaveGraphButton_Click);
        }

        enum SearchMethod
        {
            Method1,
            Method2,
            Method3
        }
        enum Mode
        {
            defaultMode = -1,
            AddVertex = 0,
            AddEdges = 1,
        }
        Mode currentMode = Mode.defaultMode;
        List<Button> buttons = new List<Button>(); // Список кнопок на формі
        List<Edge> edges = new List<Edge>(); // Список ребер
        List<Vertex> vertices = new List<Vertex>(); // Список вершин
        double[,] adjacencyMatrix; // Матриця суміжності
        List<int> minimumSpanningTreeEdgeIDs = new List<int>(); // Індекси ребер утвореного остовного дерева
        List<Button> interfaceButtons = new List<Button>(); // Список кнопок інтерфейсу



        bool isFirst = true;
        Button firstBut;
        Button secondBut;


        private void SaveGraphButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveGraphToFile(saveFileDialog.FileName);
            }
        }

        private void SaveGraphToFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Збереження вершин
                    writer.WriteLine(vertices.Count);
                    foreach (var vertex in vertices)
                    {
                        writer.WriteLine($"{vertex.Id} {vertex.X} {vertex.Y} {vertex.Degree}");
                    }

                    // Збереження ребер
                    writer.WriteLine(edges.Count);
                    foreach (var edge in edges)
                    {
                        writer.WriteLine($"{edge.StartVertex.Id} {edge.EndVertex.Id} {edge.Weight}");
                    }

                    // Збереження матриці суміжності
                    for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                        {
                            writer.Write($"{adjacencyMatrix[i, j]} ");
                        }
                        writer.WriteLine();
                    }
                }

                MessageBox.Show("Граф успішно збережено у файл!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка збереження у файл: " + ex.Message);
            }
        }






        private void StartSearchClick(object sender, EventArgs e)
        {
            if (vertices.Count < 2)
            {
                MessageBox.Show("Додайте ще вершини для запуску алгоритму пошуку", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (edges.Count < 1)
            {
                MessageBox.Show("Додайте ще ребра для запуску алгоритму пошуку", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool allConnected = true;

            foreach (var vertex in vertices)
            {
                if (!vertex.IsConnectedToGraph)
                {
                    allConnected = false;
                    break;
                }
            }

            if (allConnected)
            {
                if (comboBox1.SelectedIndex != -1)
                {
                    string selectedMethod = comboBox1.SelectedItem.ToString();
                    switch (selectedMethod)
                    {
                        case "Prim":
                            PrimAlgorithm();
                            break;
                        case "Boruvka":
                            BoruvkaAlgorithm();
                            break;
                        case "Kruskal":
                            KruskalAlgorithm();
                            break;
                        default:
                            MessageBox.Show("Будь ласка, оберіть метод пошуку!");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, оберіть метод пошуку!");
                }
            }
            else
            {
                MessageBox.Show("Не всі вершини приєднані до графа! Перевірте з'єднання між вершинами.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private bool IsGraphConnected()
        {
            HashSet<int> visitedVertices = new HashSet<int>();
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(0);
            visitedVertices.Add(0);

            while (queue.Count > 0)
            {
                int currentVertex = queue.Dequeue();

                foreach (var edge in vertices[currentVertex].Edges)
                {
                    int neighborVertex = edge.GetOtherVertexIndex(currentVertex);

                    if (!visitedVertices.Contains(neighborVertex))
                    {
                        visitedVertices.Add(neighborVertex);
                        queue.Enqueue(neighborVertex);
                    }
                }
            }

            return visitedVertices.Count == vertices.Count;
        }


        private void PrimAlgorithm()
        {
            InitializeAdjacencyMatrix();

            if (!IsGraphConnected())
            {
                MessageBox.Show("Граф не є зв'язним, неможливо обрахувати мінімальне остовне дерево", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Індекси вибраних вершин
            HashSet<int> selectedVertices = new HashSet<int>();
            selectedVertices.Add(0);

            // Кількість вершин
            int vertexCount = vertices.Count;

            minimumSpanningTreeEdgeIDs.Clear();
            int iterations = 0;
            int edgeCount = 0; // Лічильник для перевірки на нескінченний цикл

            while (selectedVertices.Count < vertexCount)
            {
                iterations++;
                double minWeight = double.MaxValue;
                Edge minEdge = null;
                int minVertexIndex = -1;

                // Проходимо по кожній вершині, що вже включена у мінімальне остовне дерево
                foreach (var vertexIndex in selectedVertices)
                {
                    // Проходимо по всіх ребрах вершини
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
                    edgeCount++;
                }
                else
                {
                    break;
                }
            }

            if (edgeCount != vertexCount - 1)
            {
                MessageBox.Show("Не можливо обрахувати даним методом", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (var vertex in vertices)
            {
                if (selectedVertices.Contains(vertex.Id))
                {
                    vertex.vertexIsInMinimumSpanningTree = true;
                }
            }
            ResetEdgeStatus();
            Refresh();
            MessageBox.Show($"Алгоритм Прима завершився за {iterations} ітерацій.");
        }


        private void BoruvkaAlgorithm()
        {
            InitializeAdjacencyMatrix();


            if (!IsGraphConnected())
            {
                MessageBox.Show("Граф не є зв'язним, неможливо обрахувати мінімальне остовне дерево", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int vertexCount = vertices.Count;

            minimumSpanningTreeEdgeIDs.Clear();

            int iterations = 0;
            while (minimumSpanningTreeEdgeIDs.Count < vertexCount - 1)
            {
                iterations++;
                int[] cheapestEdges = new int[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    cheapestEdges[i] = -1;
                }

                bool addedEdge = false;

                foreach (var edge in edges)
                {
                    int rootStart = FindRoot(edge.StartVertex);
                    int rootEnd = FindRoot(edge.EndVertex);

                    if (rootStart != rootEnd)
                    {
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

                foreach (int edgeIndex in cheapestEdges)
                {
                    if (edgeIndex != -1 && !minimumSpanningTreeEdgeIDs.Contains(edgeIndex))
                    {
                        minimumSpanningTreeEdgeIDs.Add(edgeIndex);
                        edges[edgeIndex].IsInMinimumSpanningTree = true;
                        addedEdge = true;
                    }
                }

                if (!addedEdge)
                {
                    MessageBox.Show("Неможливо обрахувати мінімальне остовне дерево даним методом", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetEdgeStatus();
                    return;
                }
            }
            ResetEdgeStatus();

            Refresh();
            MessageBox.Show($"Алгоритм Бору́вка завершився за {iterations} ітерацій.");
        }


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


        private void KruskalAlgorithm()
        {
            InitializeAdjacencyMatrix();
            ResetEdgeStatus();


            if (!IsGraphConnected())
            {
                MessageBox.Show("Граф не є зв'язним, неможливо обрахувати мінімальне остовне дерево", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int vertexCount = vertices.Count;
            DisjointSet disjointSet = new DisjointSet(vertexCount);

            int iterations = 0;
            int edgeCount = 0;

            foreach (var edge in edges.OrderBy(e => e.Weight))
            {
                iterations++;
                int rootStart = disjointSet.Find(vertices.IndexOf(edge.StartVertex));
                int rootEnd = disjointSet.Find(vertices.IndexOf(edge.EndVertex));
                if (rootStart != rootEnd)
                {
                    edge.IsInMinimumSpanningTree = true;
                    disjointSet.Union(vertices.IndexOf(edge.StartVertex), vertices.IndexOf(edge.EndVertex));
                    edgeCount++;
                }

                if (edgeCount == vertexCount - 1)
                {
                    break;
                }
            }

            if (edgeCount != vertexCount - 1)
            {
                MessageBox.Show("Не можливо обрахувати даним методом", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Refresh();
            MessageBox.Show($"Алгоритм Крускала завершився за {iterations} ітерацій.");
        }

        private void ResetEdgeStatus()
        {
            foreach (var edge in edges)
            {
                if (!minimumSpanningTreeEdgeIDs.Contains(edge.Id))
                {
                    edge.IsInMinimumSpanningTree = false;
                }
            }
        }

        private Edge CalculateEdgeFromIndex(int edgeIndex)
        {
            Edge edge = edges[edgeIndex];

            Vertex startVertex = edge.StartVertex;

            Vertex endVertex = edge.EndVertex;

            return edge;
        }



        private void InitializeAdjacencyMatrix()
        {
            int size = vertices.Count;
            adjacencyMatrix = new double[size, size];

            foreach (var edge in edges)
            {
                int startIndex = vertices.IndexOf(edge.StartVertex);
                int endIndex = vertices.IndexOf(edge.EndVertex);
                adjacencyMatrix[startIndex, endIndex] = edge.Weight;
                adjacencyMatrix[endIndex, startIndex] = edge.Weight;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (int edgeIndex in minimumSpanningTreeEdgeIDs)
            {
                Edge edge = CalculateEdgeFromIndex(edgeIndex);
                e.Graphics.DrawLine(Pens.Blue, edge.StartVertex.point, edge.EndVertex.point);
            }

            foreach (var edge in edges)
            {
                if (!edge.IsInMinimumSpanningTree)
                {
                    e.Graphics.DrawLine(Pens.Black, edge.StartVertex.point, edge.EndVertex.point);
                }
            }
        }






        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentMode == Mode.AddVertex)
            {
                vertices.Add(new Vertex(e.Location.X, e.Location.Y)); // Додаємо новий об'єкт вершини з координатами

                ResizeAdjacencyMatrix();

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




        private void VerticleButtons(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (currentMode == Mode.AddEdges)
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

                        if (firstIndex != -1 && secondIndex != -1 && firstIndex != secondIndex)
                        {
                            bool edgeExists = false;
                            Edge existingEdge = null;
                            foreach (var edge in edges)
                            {
                                if ((edge.StartVertex.Id == firstIndex && edge.EndVertex.Id == secondIndex) || (edge.StartVertex.Id == secondIndex && edge.EndVertex.Id == firstIndex))
                                {
                                    edgeExists = true;
                                    existingEdge = edge;
                                    break;
                                }
                            }

                            if (edgeExists)
                            {
                                int weight = (int)adjacencyMatrix[firstIndex, secondIndex];
                                existingEdge.Weight = weight;
                                this.Invalidate();
                                ShowEdgeWeightDialog(existingEdge);
                            }
                            else
                            {
                                Vertex start = vertices[firstIndex];
                                Vertex end = vertices[secondIndex];
                                vertices[firstIndex].IsConnectedToGraph = true;
                                vertices[secondIndex].IsConnectedToGraph = true;

                                int weight = (int)adjacencyMatrix[firstIndex, secondIndex];

                                adjacencyMatrix[firstIndex, secondIndex] = weight;
                                adjacencyMatrix[secondIndex, firstIndex] = weight;

                                Edge newEdge = new Edge(start, end);
                                newEdge.Weight = weight;
                                edges.Add(newEdge);

                                start.AddEdge(newEdge);
                                end.AddEdge(newEdge);

                                this.Invalidate();
                                ShowEdgeWeightDialog(newEdge);
                            }
                            InitializeAdjacencyMatrix();
                            firstBut = null;
                            secondBut = null;
                        }
                        else
                        {
                            isFirst = !isFirst;
                            MessageBox.Show("Оберіть дві різні вершини", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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

                PointF weightPosition = new PointF((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);

                Font font = new Font(this.Font.FontFamily, 12, FontStyle.Bold);

                e.Graphics.DrawString(edge.Weight.ToString(), font, Brushes.Black, weightPosition);
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
                            if (int.TryParse(weightTextBox.Text, out int weight) && weight > 0 && weight <= 10000000)
                            {
                                edge.Weight = weight;
                                this.Invalidate();
                            }
                            else
                            {
                                MessageBox.Show("Недійсне значення ваги! Будь ласка, введіть додатне ціле число не більше 10000000.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void ClearGraph(object sender, EventArgs e)
        {
            isFirst = true;
            firstBut = null;
            secondBut = null;
            ResetEdgeStatus();
            SetButtonsColor();
            currentMode = Mode.defaultMode;
            vertices.Clear();
            edges.Clear();
            minimumSpanningTreeEdgeIDs.Clear();

            Edge.edgeCount = 0;
            Vertex.ResetVertexCount();

            foreach (Button button in buttons)
            {
                this.Controls.Remove(button);
                button.Dispose();
            }

            buttons.Clear();
            adjacencyMatrix = null;
            Refresh();
        }



        private void SetButtonsColor()
        {
            foreach (Button v in interfaceButtons)
            {
                v.BackColor = Color.White;
            }
        }

        private void AddVetexMode(object sender, EventArgs e)
        {
            if (!isFirst)
            {
                MessageBox.Show("Спершу оберіть другу вершину", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetButtonsColor();

            if (currentMode == Mode.AddVertex)
            {
                currentMode = Mode.defaultMode;
            }
            else
            {
                currentMode = Mode.AddVertex;
                Button clickedButton = (Button)sender;
                clickedButton.BackColor = Color.Gray;
            }

        }


        private void AddEdgeMode(object sender, EventArgs e)
        {

            if (vertices.Count < 2)
            {
                MessageBox.Show("Додайте ще вершини для створення ребер", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (currentMode == Mode.AddEdges)
            {
                if (!isFirst)
                {
                    MessageBox.Show("Спершу оберіть другу вершину", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                SetButtonsColor();

                currentMode = Mode.defaultMode;
            }
            else
            {
                SetButtonsColor();

                currentMode = Mode.AddEdges;
                Button clickedButton = (Button)sender;
                clickedButton.BackColor = Color.Gray;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }



    public class GraphData
    {
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
        public double[,] AdjacencyMatrix { get; set; }
    }

    public class Edge
    {
        public static int edgeCount = 0; // Лічильник ребер

        public Edge(Vertex start, Vertex end)
        {
            StartVertex = start;
            EndVertex = end;
            IsInMinimumSpanningTree = false;
            Id = edgeCount++;
        }

        public int GetOtherVertexIndex(int startVertexIndex)
        {
            return StartVertex.Id == startVertexIndex ? EndVertex.Id : StartVertex.Id;
        }

        public readonly int Id;
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        public double Weight { get; set; }
        public bool IsInMinimumSpanningTree { get; set; }
    }






    public class Vertex
    {
        private static int vertexCount = -1; // Кількість створених вершин
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Degree { get; set; }
        [JsonIgnore]
        public List<Edge> Edges { get; set; } // Список ребер, що з'єднують цю вершину
        public Point point { get; set; }
        public bool vertexIsInMinimumSpanningTree { get; set; }
        public bool IsConnectedToGraph { get; set; }


        public Vertex(double x, double y)
        {
            Id = ++vertexCount;
            X = x;
            Y = y;
            Degree = 0;
            Edges = new List<Edge>();
            point = new Point((int)x, (int)y);
            vertexIsInMinimumSpanningTree = false;
            IsConnectedToGraph = false;

        }
        public void UpdateIndices(int removedIndex)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                Edge edge = Edges[i];
                if (edge.StartVertex.Id == removedIndex || edge.EndVertex.Id == removedIndex)
                {
                    Edges.RemoveAt(i);
                    Degree--;
                    i--;
                }
                else
                {
                    edge.StartVertex.Id = edge.StartVertex.Id > removedIndex ? edge.StartVertex.Id - 1 : edge.StartVertex.Id;
                    edge.EndVertex.Id = edge.EndVertex.Id > removedIndex ? edge.EndVertex.Id - 1 : edge.EndVertex.Id;
                   }
            }
        }

        public static void ResetVertexCount()
        {
            vertexCount = -1;
        }


        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
            Degree++;
        }


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

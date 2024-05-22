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
            //RemoveVertex = 2,
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
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveGraphToFile(saveFileDialog.FileName);
            }
        }
        private void SaveGraphToFile(string filePath)
        {
            try
            {
                GraphData graphData = new GraphData
                {
                    Vertices = vertices,
                    Edges = edges,
                    AdjacencyMatrix = adjacencyMatrix // Додайте матрицю суміжності до об'єкта GraphData
                };

                string json = JsonConvert.SerializeObject(graphData);
                File.WriteAllText(filePath, json);

                MessageBox.Show("Граф успішно збережено у файл!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка збереження у файл: " + ex.Message);
            }
        }


        private void StartSearchClick(object sender, EventArgs e)
        {


            // Перевірка, чи є хочаб дві вершини
            if (vertices.Count < 2)
            {
                MessageBox.Show("Додайте ще вершини для запуску алгоритму пошуку", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Перевірте, чи є хочаб одне ребро
            if (edges.Count < 1)
            {
                MessageBox.Show("Додайте ще ребра для запуску алгоритму пошуку", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //GenerateRandomGraph(3);
            bool allConnected = true; // Прапорець, що вказує, чи всі вершини приєднані до графа

            // Перевірка, чи всі вершини приєднані до графа
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
                    // Отримайте обраний метод пошуку з ComboBox
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
                // Виведення повідомлення про помилку, якщо не всі вершини приєднані до графа
                MessageBox.Show("Не всі вершини приєднані до графа! Перевірте з'єднання між вершинами.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
                    // Якщо немає доступних ребер, виходимо з циклу
                    break;
                }
            }

            // Перевіряємо, чи знайдено достатньо ребер для мінімального остовного дерева
            if (edgeCount != vertexCount - 1)
            {
                MessageBox.Show("Не можливо обрахувати даним методом", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Позначаємо всі вершини, які з'єднані з мінімальним остовним деревом, як вибрані
            foreach (var vertex in vertices)
            {
                if (selectedVertices.Contains(vertex.Id))
                {
                    vertex.vertexIsInMinimumSpanningTree = true;
                }
            }
            ResetEdgeStatus();
            // Оновлюємо відображення форми
            Refresh();
            MessageBox.Show($"Алгоритм Прима завершився за {iterations} ітерацій.");
        }


        private void BoruvkaAlgorithm()
        {
            // Ініціалізуємо матрицю суміжності
            InitializeAdjacencyMatrix();

            // Кількість вершин
            int vertexCount = vertices.Count;

            // Створюємо список, що містить індекси ребер, які утворюють мінімальне остовне дерево
            minimumSpanningTreeEdgeIDs.Clear();

            int iterations = 0;
            // Поки не утвориться мінімальне остовне дерево
            while (minimumSpanningTreeEdgeIDs.Count < vertexCount - 1)
            {
                iterations++;
                // Створюємо масив, що містить індекси найдешевших ребер для кожного компоненту
                int[] cheapestEdges = new int[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    cheapestEdges[i] = -1; // Ініціалізуємо значенням -1, яке вказуватиме, що ребро ще не знайдено
                }

                bool addedEdge = false; // Перевірка чи було додане хоч одне ребро

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
                        addedEdge = true; // Вказуємо, що було додане ребро
                    }
                }

                // Якщо жодне ребро не було додане, виводимо помилку
                if (!addedEdge)
                {
                    MessageBox.Show("Неможливо обрахувати мінімальне остовне дерево даним методом", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetEdgeStatus();
                    return;
                }
            }
            ResetEdgeStatus();

            // Оновлюємо відображення форми
            Refresh();
            MessageBox.Show($"Алгоритм Бору́вка завершився за {iterations} ітерацій.");
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


        private void KruskalAlgorithm()
        {
            // Ініціалізуємо матрицю суміжності
            InitializeAdjacencyMatrix();
            ResetEdgeStatus();

            // Кількість вершин
            int vertexCount = vertices.Count;
            DisjointSet disjointSet = new DisjointSet(vertexCount);

            int iterations = 0;
            int edgeCount = 0;// Лічильник доданих ребер

            // Обираємо ребра для мінімального остовного дерева
            foreach (var edge in edges.OrderBy(e => e.Weight))
            {
                iterations++;
                int rootStart = disjointSet.Find(vertices.IndexOf(edge.StartVertex));
                int rootEnd = disjointSet.Find(vertices.IndexOf(edge.EndVertex));
                if (rootStart != rootEnd)
                {
                    // Якщо вершини не знаходяться в одній компоненті, додаємо ребро
                    edge.IsInMinimumSpanningTree = true;
                    disjointSet.Union(vertices.IndexOf(edge.StartVertex), vertices.IndexOf(edge.EndVertex));
                    edgeCount++;
                }

                // Якщо ми знайшли достатньо ребер для MST, виходимо з циклу
                if (edgeCount == vertexCount - 1)
                {
                    break;
                }
            }

            // Перевіряємо, чи знайдено достатньо ребер для мінімального остовного дерева
            if (edgeCount != vertexCount - 1)
            {
                MessageBox.Show("Не можливо обрахувати даним методом", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Оновлюємо відображення форми
            Refresh();
            MessageBox.Show($"Алгоритм Крускала завершився за {iterations} ітерацій.");
        }

        private void ResetEdgeStatus()
        {
            // Позначаємо всі ребра, які не входять до мінімального остовного дерева, як false
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
            // Отримуємо ребро, пов'язане з кнопкою
            Edge edge = edges[edgeIndex];

            // Отримуємо вершину, яка починає ребро
            Vertex startVertex = edge.StartVertex;

            // Отримуємо вершину, яка кінчає ребро
            Vertex endVertex = edge.EndVertex;

            // Повертаємо ребро, пов'язане з вказаним індексом
            return edge;
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
                if (!edge.IsInMinimumSpanningTree)
                {
                    e.Graphics.DrawLine(Pens.Black, edge.StartVertex.point, edge.EndVertex.point);
                }

                //e.Graphics.DrawLine(new Pen(edge.Color), edge.StartVertex.point, edge.EndVertex.point);
            }
        }






        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentMode == Mode.AddVertex)
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




        private void VerticleButtons(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (currentMode == Mode.AddEdges)
                {
                    // Логіка для додавання вершини
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

                        // Перевірка на рівність ідентифікаторів вершин
                        if (firstIndex != -1 && secondIndex != -1 && firstIndex != secondIndex)
                        {
                            // Перевірка на наявність ребра між вершинами
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
                                // Заміна ваги існуючого ребра
                                int weight = (int)adjacencyMatrix[firstIndex, secondIndex];
                                existingEdge.Weight = weight;
                                this.Invalidate();
                                // Запит нової ваги ребра
                                ShowEdgeWeightDialog(existingEdge);
                            }
                            else
                            {
                                Vertex start = vertices[firstIndex];
                                Vertex end = vertices[secondIndex];
                                vertices[firstIndex].IsConnectedToGraph = true;
                                vertices[secondIndex].IsConnectedToGraph = true;

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
                                // Показуємо діалог для встановлення ваги ребра
                                ShowEdgeWeightDialog(newEdge);
                            }
                            InitializeAdjacencyMatrix();
                            firstBut = null;
                            secondBut = null;
                        }
                        else
                        {
                            isFirst = !isFirst; // Повертаємо isFirst до попереднього стану
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

                // Відображення ваги ребра
                PointF weightPosition = new PointF((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);

                // Встановлення більшого розміру шрифту
                Font font = new Font(this.Font.FontFamily, 12, FontStyle.Bold); // Змініть розмір шрифту тут

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
                                MessageBox.Show("Invalid weight value! Please enter a positive integer not greater than 10000000.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Встановлення лічильника vertexCount на значення -1
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
            //currentMode = Mode.defaultMode;
            foreach (Button v in interfaceButtons)
            {
                v.BackColor = Color.White;
            }
        }

        private void AddVetexMode(object sender, EventArgs e)
        {
            if (!isFirst)
            {
                // Перевірка, чи перша вершина вже обрана
                MessageBox.Show("Спершу оберіть другу вершину", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Повертаємося, не робимо нічого більше
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
                // Перевірка, чи існує достатня кількість вершин для додавання ребер
                MessageBox.Show("Додайте ще вершини для створення ребер", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Повертаємося, не робимо нічого більше
            }

            if (currentMode == Mode.AddEdges)
            {
                if (!isFirst)
                {
                    // Перевірка, чи перша вершина вже обрана
                    MessageBox.Show("Спершу оберіть другу вершину", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Повертаємося, не робимо нічого більше
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


        private void TempGenerateRandomGraph(int size)
        {
            ClearGraph(null, null);
            // Очищення списків вершин, ребер та кнопок вершин
            vertices.Clear();
            edges.Clear();
            minimumSpanningTreeEdgeIDs.Clear();

            // Видалення кнопок вершин з форми і зі списку контролів
            foreach (Button btn in buttons)
            {
                Controls.Remove(btn);
            }
            buttons.Clear(); // Очищення списку кнопок вершин

            // Створення випадкових координат для вершин та додавання їх до списку вершин
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                double x = random.Next(20, this.ClientSize.Width - 20);
                double y = random.Next(20, this.ClientSize.Height - 20);
                Vertex newVertex = new Vertex(x, y);
                vertices.Add(newVertex);
            }


            for (int i = 0; i < size; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    //double weight = random.NextDouble() * 10; // Випадкова вага від 0 до 10
                    int weight = random.Next(1, 11); // Випадкова вага від 0 до 10
                    Vertex start = vertices[i];
                    Vertex end = vertices[j];

                    // Створення ребра та додавання його до списку ребер та вершин
                    Edge newEdge = new Edge(start, end);
                    newEdge.Weight = weight; // Встановлюємо вагу ребра
                    edges.Add(newEdge);
                    start.AddEdge(newEdge);
                    end.AddEdge(newEdge);

                    // Збільшення ступеня кожної вершини
                    start.Degree++;
                    end.Degree++;

                    // Додавання вершин до списку з'єднаних вершин
                    if (!start.IsConnectedToGraph)
                    {
                        start.IsConnectedToGraph = true;
                    }
                    if (!end.IsConnectedToGraph)
                    {
                        end.IsConnectedToGraph = true;
                    }
                }
            }
            // Створення кнопок для вершин та їх додавання на форму
            foreach (Vertex vertex in vertices)
            {
                Button newButton = new Button();
                newButton.Location = new Point((int)vertex.X - 20, (int)vertex.Y - 20);
                newButton.Size = new Size(40, 40);
                newButton.BackColor = Color.Black;
                newButton.ForeColor = Color.White;
                newButton.Text = vertices.IndexOf(vertex).ToString();
                newButton.Click += new EventHandler(VerticleButtons);
                buttons.Add(newButton);
                Controls.Add(newButton);
            }

            // Відображення на формі
            Refresh();
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
            IsInMinimumSpanningTree = false; // Початкове значення прапорця
            Id = edgeCount++; // Присвоюємо ребру унікальний ідентифікатор
        }

        public int GetOtherVertexIndex(int startVertexIndex)
        {
            return StartVertex.Id == startVertexIndex ? EndVertex.Id : StartVertex.Id;
        }

        public readonly int Id; // Ідентифікатор ребра
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        public double Weight { get; set; } // Вага ребра
        public bool IsInMinimumSpanningTree { get; set; } // Прапорець, що вказує, чи належить ребро до мінімального остовного дерева
    }






    public class Vertex
    {
        private static int vertexCount = -1; // Кількість створених вершин
        public int Id { get; set; } // Ідентифікатор вершини
        public double X { get; set; } // Координата X
        public double Y { get; set; } // Координата Y
        public int Degree { get; set; } // Ступінь вершини
        [JsonIgnore]
        public List<Edge> Edges { get; set; } // Список ребер, що з'єднують цю вершину
        public Point point { get; set; }
        public bool vertexIsInMinimumSpanningTree { get; set; } // Прапорець, що вказує, чи належить вершина до мінімального остовного дерева
        public bool IsConnectedToGraph { get; set; } // Прапорець, що вказує, чи приєднана вершина до графа


        public Vertex(double x, double y)
        {
            Id = ++vertexCount;
            X = x;
            Y = y;
            Degree = 0;
            Edges = new List<Edge>();
            point = new Point((int)x, (int)y);
            vertexIsInMinimumSpanningTree = false; // Початкове значення прапорця
            IsConnectedToGraph = false; // Початкове значення прапорця

        }
        public void UpdateIndices(int removedIndex)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                Edge edge = Edges[i];
                if (edge.StartVertex.Id == removedIndex || edge.EndVertex.Id == removedIndex)
                {
                    // Видалення з'єднання з цією вершиною
                    Edges.RemoveAt(i);
                    Degree--;
                    i--; // Зменшуємо лічильник, оскільки ми видалили одне з'єднання
                }
                else
                {
                    // Оновлюємо індекси ребра
                    edge.StartVertex.Id = edge.StartVertex.Id > removedIndex ? edge.StartVertex.Id - 1 : edge.StartVertex.Id;
                    edge.EndVertex.Id = edge.EndVertex.Id > removedIndex ? edge.EndVertex.Id - 1 : edge.EndVertex.Id;
                   }
            }
        }

        public static void ResetVertexCount()
        {
            vertexCount = -1;
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

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
        public Form1()
        {
            InitializeComponent();

            interfaceButtons.Add(button1);
            interfaceButtons.Add(button2);
            interfaceButtons.Add(button3);
            interfaceButtons.Add(button4);
            interfaceButtons.Add(button5);
            interfaceButtons.Add(button6);

            // Додайте обробники подій для кнопок інтерфейсу
            button1.Click += new System.EventHandler(this.button1_Click);
            button2.Click += new System.EventHandler(this.ClearGraph);
            button3.Click += new System.EventHandler(this.button3_Click);
            button4.Click += new System.EventHandler(this.EnterSizeOfGeneratedGraph);
            button5.Click += new System.EventHandler(this.AddVetixMode);
            button6.Click += new System.EventHandler(this.AddEdgeMode);
        }

        // Перерахування для представлення методів пошуку
        enum SearchMethod
        {
            Method1,
            Method2,
            Method3
        }
        enum Mode
        {
            defaultMode= -1,
            AddVertex = 0,
            AddEdges = 1,
            RemoveVertex = 2,
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Перевірте, чи обраний метод пошуку
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


        private void button1_Click(object sender, EventArgs e)
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
            ResetEdgeStatus();

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
            ResetEdgeStatus();

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

        private void RemoveVertex(Vertex vertex)
        {
            // Видалення ребер, які приєднані до вершини
            foreach (Edge edge in vertex.Edges.ToList())
            {
                RemoveEdge(edge);
            }

            // Видалення кнопки вершини з форми та зі списку контролів
            Button buttonToRemove = buttons.FirstOrDefault(btn => btn.Text == vertex.Id.ToString());
            if (buttonToRemove != null)
            {
                Controls.Remove(buttonToRemove);
                buttons.Remove(buttonToRemove);
            }

            // Видалення вершини зі списку вершин
            vertices.Remove(vertex);
        }

        private void RemoveEdge(Edge edge)
        {
            // Видалення ребра зі списку ребер
            edges.Remove(edge);

            // Видалення ребра зі списку ребер, які приєднані до кожної вершини
            edge.StartVertex.Edges.Remove(edge);
            edge.EndVertex.Edges.Remove(edge);
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
                                // Показуємо діалог для встановлення ваги ребра (якщо потрібно)
                                ShowEdgeWeightDialog(newEdge);
                            }

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
                else if (currentMode == Mode.RemoveVertex)
                {
                    PointRemove(sender, e);
                }
            }
        }




        private void PointRemove(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int index = buttons.IndexOf(clickedButton);

            // Отримання вершини, яку потрібно видалити
            Vertex removedVertex = vertices[index];

            // Видалення всіх ребер, що з'єднуються з видаленою вершиною
            foreach (Edge edge in removedVertex.Edges)
            {
                edges.Remove(edge);
            }

            // Оновлення ребер, що з'єднують інші вершини
            foreach (Vertex vertex in vertices)
            {
                vertex.UpdateIndices(index);
            }

            // Видалення вершини зі списку вершин та видалення кнопки зі списку кнопок
            vertices.RemoveAt(index);
            buttons.RemoveAt(index);

            // Видалення кнопки з форми
            Controls.Remove(clickedButton);

            // Оновлення матриці суміжності
            ResizeAdjacencyMatrix();
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

        private void ClearGraph(object sender, EventArgs e)
        {
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




        private void button3_Click(object sender, EventArgs e)
        {
            if (currentMode == Mode.AddVertex)
            {
                currentMode = Mode.RemoveVertex;
                Button clickedButton = (Button)sender;
                clickedButton.BackColor = Color.Gray;
            }
            else
            {
                currentMode = Mode.AddVertex;
                Button clickedButton = (Button)sender;
                clickedButton.BackColor = Color.White;
            }
        }




        private void EnterSizeOfGeneratedGraph(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Введіть розмірність графу";
                dialog.StartPosition = FormStartPosition.CenterParent;

                Label label = new Label();
                label.Text = "Розмірність:";
                label.Location = new Point(20, 20);
                dialog.Controls.Add(label);

                TextBox textBox = new TextBox();
                textBox.Location = new Point(160, 20);
                dialog.Controls.Add(textBox);

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(20, 60);
                dialog.Controls.Add(okButton);

                Button cancelButton = new Button();
                cancelButton.Text = "Скасувати";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(100, 60);
                dialog.Controls.Add(cancelButton);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    int size;
                    if (int.TryParse(textBox.Text, out size) && size > 0)
                    {
                        GenerateRandomGraph(size);
                    }
                    else
                    {
                        MessageBox.Show("Некоректне значення розмірності графу. Будь ласка, введіть додатне ціле число.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SetButtonsColor()
        {
            //currentMode = Mode.defaultMode;
            foreach (Button v in interfaceButtons)
            {
                v.BackColor = Color.White;
            }
        }

        private void AddVetixMode(object sender, EventArgs e)
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


        private void GenerateRandomGraph(int size)
        {
            ClearGraph(null,null);
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
                    int weight = random.Next(1,11); // Випадкова вага від 0 до 10
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

        private void EdgeButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int edgeIndex = Convert.ToInt32(clickedButton.Tag); // Отримуємо індекс ребра, яке було натиснуто

                // Отримуємо відповідне ребро за індексом
                Edge edge = edges[edgeIndex];

                // Показуємо діалог для встановлення нової ваги ребра
                ShowEdgeWeightDialog(edge);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }



    public class EdgeButton : Button
    {
        public Edge Edge { get; set; }

        public EdgeButton(Edge edge)
        {
            Edge = edge;
            Text = $"{edge.Id}";
            Location = new System.Drawing.Point(0, 0); // Задайте розміщення
            Size = new System.Drawing.Size(100, 30); // Задайте розмір
            Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold); // Задайте шрифт
            ForeColor = System.Drawing.Color.White; // Задайте колір тексту
            BackColor = System.Drawing.Color.Blue; // Задайте колір фону
        }
    }

    public class Edge
    {
        public static int edgeCount = 0; // Лічильник ребер

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

        // Параметр Id більше не властивість, а вже змінна, щоб його можна було присвоїти тільки при створенні об'єкта
        public readonly int Id; // Ідентифікатор ребра
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        public double Weight { get; set; } // Вага ребра
        public bool IsInMinimumSpanningTree { get; set; } // Прапорець, що вказує, чи належить ребро до мінімального остовного дерева
        public Color Color { get; set; } // Колір ребра
    }






    public class Vertex
        {
            private static int vertexCount = -1; // Кількість створених вершин
            public int Id { get; set; } // Ідентифікатор вершини
            public double X { get; set; } // Координата X
            public double Y { get; set; } // Координата Y
            public int Degree { get; set; } // Ступінь вершини
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

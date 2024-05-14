using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KP_OP_21
{
    partial class Form1
    {
        //private List<Point> vertices = new List<Point>(); // Вершини графа
        //private List<Button> buttons = new List<Button>(); // Кнопки вершин
        //private List<Edge> edges = new List<Edge>(); // Ребра графа (вага, вершина 1, вершина 2)




        ///////
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Font = new System.Drawing.Font("Arial", 12F);
            this.button1.Location = new System.Drawing.Point(2, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(256, 86);
            this.button1.TabIndex = 0;
            this.button1.Text = "пошук мінімального дерева";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.Font = new System.Drawing.Font("Arial", 12F);
            this.button2.Location = new System.Drawing.Point(782, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(231, 86);
            this.button2.TabIndex = 1;
            this.button2.Text = "очистити все";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.Font = new System.Drawing.Font("Arial", 12F);
            this.button3.Location = new System.Drawing.Point(1019, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(231, 86);
            this.button3.TabIndex = 2;
            this.button3.Text = "очистити точку (не працює)";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Arial", 10F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Prim",
            "Boruvka",
            "Kruskal"});
            this.comboBox1.Location = new System.Drawing.Point(2, 92);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(256, 27);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.Text = "оберіть метод пошуку";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.Window;
            this.button4.Font = new System.Drawing.Font("Arial", 12F);
            this.button4.Location = new System.Drawing.Point(1256, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(231, 86);
            this.button4.TabIndex = 4;
            this.button4.Text = "згенерувати граф";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.Window;
            this.button5.Font = new System.Drawing.Font("Arial", 12F);
            this.button5.Location = new System.Drawing.Point(258, 0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(256, 86);
            this.button5.TabIndex = 5;
            this.button5.Text = "додати вершину";
            this.button5.UseVisualStyleBackColor = false;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.SystemColors.Window;
            this.button6.Font = new System.Drawing.Font("Arial", 12F);
            this.button6.Location = new System.Drawing.Point(520, 0);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(256, 86);
            this.button6.TabIndex = 6;
            this.button6.Text = "додати ребро";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.White;
            this.button7.Font = new System.Drawing.Font("Arial", 12F);
            this.button7.Location = new System.Drawing.Point(1019, 92);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(231, 86);
            this.button7.TabIndex = 7;
            this.button7.Text = "зберегти в файл";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.White;
            this.button8.Font = new System.Drawing.Font("Arial", 12F);
            this.button8.Location = new System.Drawing.Point(1019, 184);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(231, 86);
            this.button8.TabIndex = 8;
            this.button8.Text = "зчитати файл";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1924, 1055);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private Button button2;
        private Button button3;
        private ComboBox comboBox1;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
    }
}


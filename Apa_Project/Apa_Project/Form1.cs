using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Apa_Project.AStar;
using static Apa_Project.BFS;
using static Apa_Project.Dijkstra;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Apa_Project
{

    public partial class Form1 : Form
    {
        Button[,] buttons;
        int rows = 0;
        int columns = 0;
        int buttonSize = 15;
        Pair Start;
        Pair Finish;
        List <Pair> checkpoints = new List <Pair> ();
        List<BFSPoint> points;
        List<double> stopwatches = new List <double> ();
        List<int> distances = new List <int> ();
        PrimsGenerator primsGenerator;
        int setcp = 0;
        int needtoreset = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void InitializeDynamicButtons()
        {
            primsGenerator = new PrimsGenerator(rows, columns);
            primsGenerator.GenerateMaze();
        }

        private async Task DrawMazeFromButtonsOnUI()
        {
            int count = 0;

            await Task.Run(() =>
            {
                buttons = new Button[rows, columns];

                this.tableLayoutPanel1.Invoke(new Action(() =>
                {
                    ResizeGrid();
                    for (int row = 0; row < rows; row++)
                    {
                        this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSize));
                    }

                    for (int col = 0; col < columns; col++)
                    {
                        this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, buttonSize));
                    }

                }));
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        buttons[row, col] = new Button
                        {
                            BackColor = primsGenerator.maze[row, col] != 0 ? Color.White : Color.Black,
                            Dock = DockStyle.Fill,
                            FlatStyle = FlatStyle.Flat,
                            Margin = new Padding(0),
                            //Name = $"button_{row}_{col}",
                            //FlatAppearance = { BorderSize = 0 }
                        };
                        buttons[row, col].FlatAppearance.BorderSize = 0;

                        this.tableLayoutPanel1.Invoke(new Action(() => this.tableLayoutPanel1.Controls.Add(buttons[row, col], col, row)));
                    }
                }
            });
        }

        private void ResizeGrid()
        {
            this.tableLayoutPanel1.Controls.Clear();
            this.tableLayoutPanel1.RowStyles.Clear();
            this.tableLayoutPanel1.ColumnStyles.Clear();

            this.tableLayoutPanel1.RowCount = rows;
            this.tableLayoutPanel1.ColumnCount = columns;

            this.tableLayoutPanel1.Size = new Size(columns * buttonSize, rows * buttonSize);

            this.tableLayoutPanel1.AutoSize = false;
            this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.Left = (this.panel2.Width - this.tableLayoutPanel1.Width) / 2;
            this.tableLayoutPanel1.Top = (this.ClientSize.Height - this.tableLayoutPanel1.Height) / 2;
        }

        private async Task ShowPathAst()
        {
            Pair source = new Pair(Start.first, Start.second);
            Pair destination = new Pair(Finish.first, Finish.second);
            A_Star(primsGenerator.maze, source, destination);
            foreach (var pair in AStVisited)
            {
                buttons[pair.first, pair.second].BackColor = Color.OrangeRed;
                await Task.Delay(1);
            }
            foreach (var pair in Final)
            {
                buttons[pair.first, pair.second].BackColor = Color.Blue;
                await Task.Delay(20); 
            }
            buttons[Start.first, Start.second].BackColor = Color.Green;
            buttons[Finish.first, Finish.second].BackColor = Color.Red;
            MessageBox.Show($"Visited cells: {AStVisited.Count}, Time: {Aststopwatch.Elapsed.TotalMilliseconds} ms, Distance: {AStdistance}");
            needtoreset = 1;
        }

        private async Task ShowPathBFS()
        {
            BFSPoint source = new BFSPoint(Start.first, Start.second);
            BFSPoint destination = new BFSPoint(Finish.first, Finish.second);
            int l = BFSearch(primsGenerator.maze, source, destination);

            foreach (var pair in BFSVisited)
            {
                buttons[pair.x, pair.y].BackColor = Color.DarkOrange;
                await Task.Delay(1);
            }
            foreach (var pair in BFSPath)
            {
                buttons[pair.x, pair.y].BackColor = Color.LightGreen;
                await Task.Delay(20);
            }
            buttons[Start.first, Start.second].BackColor = Color.Green;
            buttons[Finish.first, Finish.second].BackColor = Color.Red;
            MessageBox.Show($"Visited cells: {BFSVisited.Count}, Time: {BFSstopwatch.Elapsed.TotalMilliseconds} ms, Distance: {l}");
            needtoreset = 1;
        }

        private async Task ShowPathDij()
        {
            DijPair source = new DijPair(Start.first, Start.second);
            DijPair destination = new DijPair(Finish.first, Finish.second);
            int l = DijPath(primsGenerator.maze, source, destination);
            foreach (var pair in DVisited)
            {
                buttons[pair.Row, pair.Col].BackColor = Color.YellowGreen;
                await Task.Delay(1);
            }
            foreach (var pair in DPath)
            {
                buttons[pair.Row, pair.Col].BackColor = Color.Yellow;
                await Task.Delay(20);
            }
            buttons[Start.first, Start.second].BackColor = Color.Green;
            buttons[Finish.first, Finish.second].BackColor = Color.Red;
            MessageBox.Show($"Visited cells: {DVisited.Count}, Length: {l}, Time: {Dstopwatch.Elapsed.TotalMilliseconds} ms");
            needtoreset = 1;
        }

        private async Task RunApp()
        {
            setcp = 1;
            stopwatches.Clear();
            distances.Clear();
            BFSPoint source = new BFSPoint(1, 0);
            BFSPoint destination = new BFSPoint(rows - 2, columns - 1);
            Pair dest = new Pair(rows - 2, columns - 1);
            checkpoints.Add(dest);
            foreach (var pair in checkpoints)
            {
                if (checkpoints.Count != 0)
                {
                    destination.x = pair.first;
                    destination.y = pair.second;
                }
                int l = BFSearch(primsGenerator.maze, source, destination);
                double time = BFSstopwatch.Elapsed.TotalMilliseconds;
                stopwatches.Add(time);
                distances.Add(l);
                source.x = destination.x;
                source.y = destination.y;
                int prev_row = -1;
                int prev_col = -1;
                if (destination.x != rows - 2 || destination.y != columns - 1)
                {
                    buttons[destination.x, destination.y].BackColor = Color.Yellow;
                }
                for (int i = 1; i < BFSPath.Count(); i++)
                {
                    var pair2 = BFSPath[i];
                    if (prev_col != -1 && prev_row != -1)
                    {
                        buttons[prev_row, prev_col].BackColor = Color.White;

                    }
                    buttons[pair2.x, pair2.y].BackColor = Color.Blue;
                    await Task.Delay(50);
                    prev_col = pair2.y;
                    prev_row = pair2.x;
                }
                if (source.x != rows - 2 || source.y != columns - 1)
                {
                    buttons[source.x, source.y].BackColor = Color.LightGreen;
                }
            }
            string message = "Final result: \n";
            double totaltime = 0;
            int totaldistance = 0;
            for (int i = 0; i < distances.Count; i++)
            {
                totaltime += stopwatches[i];
                totaldistance += distances[i];
                if (i == 0)
                {
                    message += $"S -> 1: Time: {stopwatches[i]} ms, Distance: {distances[i]}{Environment.NewLine}";
                }
                else if (i > 0 && i < distances.Count - 1)
                {
                    message += $"{i} -> {i + 1}: Time: {stopwatches[i]} ms, Distance: {distances[i]}{Environment.NewLine}";
                }
                else message += $"{i} -> F: Time: {stopwatches[i]} ms, Distance: {distances[i]}{Environment.NewLine}";
            }
            message += $"Total time: {totaltime} ms{Environment.NewLine}";
            message += $"Total distance: {totaldistance}{Environment.NewLine}";
            MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            setcp = 1;
            int possibletogenerate = 0;
            if (int.TryParse(textBox1.Text, out int height) && int.TryParse(textBox2.Text, out int width))
            {
                if (height < 10 || height > 75 || width < 10 || width > 100)
                {
                    MessageBox.Show("The dimensions do not correspond to the limits!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    rows = height;
                    columns = width;
                    if (rows % 2 == 0)
                    {
                        rows++;
                    }
                    if (columns % 2 == 0)
                    {
                        columns++;
                    }
                    if (rows <= 35 && columns <= 51)
                    {
                        buttonSize = 20;
                    }
                    else if (rows <= 51 && columns <= 75)
                    {
                        buttonSize = 15;
                    }
                    else buttonSize = 10;
                    Start = new Pair(1, 0);
                    Finish = new Pair(rows - 2, columns - 1);
                    possibletogenerate = 1;
                }
            }
            else
            {
                MessageBox.Show("Enter a valid number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (possibletogenerate == 1)
            {
                await Task.Run(() => InitializeDynamicButtons());
                await DrawMazeFromButtonsOnUI();
                button1.Text = "Generate a new maze";
                button3.Enabled = true;
                button9.Enabled = true;
                button10.Enabled = true;
                buttons[1, 0].BackColor = Color.Green;
                buttons[1, 0].Text = "S";
                buttons[rows - 2, columns - 1].BackColor = Color.Green;
                buttons[rows - 2, columns - 1].Text = "F";
            }
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearMaze();
            checkpoints.Clear();
            setcp = 0;
            int cpcount = 0;
            Pair cp;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    int row = r;
                    int col = c;
                    buttons[row, col].Click += (send, eargs) =>
                    {
                        if (primsGenerator.maze[row, col] == 1  && setcp == 0)
                        {
                            if (cpcount < 10)
                            {
                                Button clickedButton = (Button)send;
                                clickedButton.BackColor = Color.DarkOrange;
                                clickedButton.Text = (++cpcount).ToString();
                                cp.first = row;
                                cp.second = col;
                                checkpoints.Add(cp);
                            }
                            else
                            {
                                return;
                            }
                        }
                    };
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            setcp = 1;
            await ShowPathAst();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            setcp = 1;
            await ShowPathBFS();
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            setcp = 1;
            await ShowPathDij();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            setcp = 1;
            int pressed = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    int row = r;
                    int col = c;
                    buttons[row, col].Click += (send, eargs) =>
                    {
                        if (pressed == 0)
                        {
                            Button clickedButton = (Button)send;
                            clickedButton.BackColor = Color.Green;
                            Start.first = row;
                            Start.second = col;
                            pressed = 1;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    };
                }
            }
            if (primsGenerator.maze[Start.first, Start.second] == 1)
            {
                buttons[Start.first, Start.second].BackColor = Color.White;
            }
            else buttons[Start.first, Start.second].BackColor = Color.Black;
            buttons[Start.first, Start.second].Text = " ";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button3.Visible = true;
            setcp = 1;
            int pressed = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    int row = r;
                    int col = c;
                    buttons[row, col].Click += (send, eargs) =>
                    {
                        if (pressed == 0)
                        {
                            Button clickedButton = (Button)send;
                            clickedButton.BackColor = Color.Red;
                            Finish.first = row;
                            Finish.second = col;
                            pressed = 1;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    };
                }
            }
            if (primsGenerator.maze[Finish.first, Finish.second] == 1)
            {
                buttons[Finish.first, Finish.second].BackColor = Color.White;
            }
            else buttons[Finish.first, Finish.second].BackColor = Color.Black;
            buttons[Finish.first, Finish.second].Text = " ";
        }

        private void clearMaze()
        {
            needtoreset = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (primsGenerator.maze[i,j] == 1 && buttons[i, j].BackColor != Color.White)
                    {
                        buttons[i,j].BackColor = Color.White;
                    }
                    if (buttons[i,j].Text != " ")
                    {
                        buttons[i,j].Text = " ";
                    }
                }
            }
            buttons[Start.first, Start.second].BackColor = Color.Green;
            buttons[Start.first, Start.second].Text = "S";
            buttons[Finish.first, Finish.second].BackColor = Color.Red;
            buttons[Finish.first, Finish.second].Text = "F";
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            setcp = 1;
            await RunApp();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button9.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            label3.Text = "       Select an Algorithm";
            label4.Visible = false;
            label5.Visible = false;
            button10.Visible = false;

            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            button11.Visible = true;
            button12.Visible = true;

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (needtoreset == 0)
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button9.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                label3.Text = "Enter the maze dimensions";
                label4.Visible = true;
                label5.Visible = true;
                button10.Visible = true;

                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button8.Visible = false;
                button11.Visible = false;
                button12.Visible = false;
                buttons[Start.first, Start.second].BackColor = Color.White;
                buttons[Start.first, Start.second].Text = "";
                buttons[1, 0].BackColor = Color.Green;
                buttons[1, 0].Text = "S";
                Start.first = 1;
                Start.second = 0;
                buttons[Finish.first, Finish.second].BackColor = Color.White;
                buttons[Finish.first, Finish.second].Text = "";
                Finish.first = rows - 2;
                Finish.second = columns - 1;
                buttons[rows - 2, columns - 1].BackColor = Color.Green;
                buttons[rows - 2, columns - 1].Text = "F";
            }
            else MessageBox.Show("Reset the maze before going to the main menu.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void button12_Click(object sender, EventArgs e)
        {
            clearMaze();
        }
    }
}


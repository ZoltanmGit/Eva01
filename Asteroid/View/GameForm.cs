using System;
using Asteroid.Model;
using Asteroid.Persistence;
using System.Drawing;
using System.Windows.Forms;

namespace Asteroid
{
    public partial class GameForm : Form
    {
        private IAsteroidDataAccess _dataAccess;
        private Panel[,] _panelGrid;
        private GameModel _model;
        private Timer _timer;
        
        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            _dataAccess = new AsteroidFileDataAccess();
            _model = new GameModel(_dataAccess);
            GenerateTable();
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(Timer_Tick);
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyEvent);

            button1.BackColor = Color.ForestGreen;
        }

        private void Timer_Tick(Object sender, EventArgs e)
        {
            _model.ModelTick();
            CheckGameOver();
            UpdateView();
        }
        private void KeyEvent(object sender, KeyEventArgs e) //Keyup Event 
        {
            if (e.KeyCode == Keys.A)
            {

                _model.MoveLeft();

            }
            else if (e.KeyCode == Keys.D)
            {
                _model.MoveRight();
            }
            CheckGameOver();
            UpdateView();
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void GenerateTable()
        {
            this.tableLayoutPanel1.ColumnCount = _model.ColumnCount;
            this.tableLayoutPanel1.RowCount = _model.RowCount;

            this.tableLayoutPanel1.ColumnStyles.Clear();
            this.tableLayoutPanel1.RowStyles.Clear();

            this.tableLayoutPanel1.Dock = DockStyle.Fill;

            for (int i = 0; i < _model.ColumnCount; i++)
            {
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100 / _model.ColumnCount));
            }
            for (int i = 0; i < _model.RowCount; i++)
            {
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100 / _model.RowCount));
            }

            _panelGrid = new Panel[_model.RowCount, _model.ColumnCount];
            for (int i = 0; i < _model.RowCount; i++)
            {
                for (int j = 0; j < _model.ColumnCount; j++)
                {

                    _panelGrid[i, j] = new Panel();
                    _panelGrid[i, j].Dock = DockStyle.Fill;
                    _panelGrid[i, j].Margin = new Padding(1);
                    this.tableLayoutPanel1.Controls.Add(_panelGrid[i, j], j, i);
                }
            }
            UpdateView();
        }
        private void UpdateView()
        {
            for (int i = 0; i< _model.RowCount; i++)
            {
                for(int j = 0; j< _model.ColumnCount; j++)
                {
                    if(_model.Table[i,j] == 1)
                    {
                        _panelGrid[i, j].BackColor = Color.OrangeRed;
                    }
                    else if(_model.Table[i, j] == 0)
                    {
                        _panelGrid[i, j].BackColor = Color.DimGray;
                    }
                    else if (_model.Table[i, j] == 2)
                    {
                        _panelGrid[i, j].BackColor = Color.BlueViolet;
                    }
                }
            }
            if (_model.bGameStopped)
            {
                button1.BackColor = Color.ForestGreen;
            }
            else
            {
                button1.BackColor = Color.IndianRed;
            }
            if(_timer != null)
            {
                toolStripStatusLabel2.Text = TimeSpan.FromMilliseconds(_model.GameTime).ToString();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_model.bGameStopped) { _model.bGameStopped = false; button1.BackColor = Color.IndianRed; _timer.Start(); saveGameToolStripMenuItem.Enabled = false;loadGameToolStripMenuItem.Enabled = false; }
            else if(!_model.bGameStopped) { _model.bGameStopped = true; button1.BackColor = Color.ForestGreen;_timer.Stop(); saveGameToolStripMenuItem.Enabled = true; loadGameToolStripMenuItem.Enabled = true; }
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            NewGame();
        }
        private void NewGame()
        {
            saveGameToolStripMenuItem.Enabled = true;
            loadGameToolStripMenuItem.Enabled = true;
            //_model = new GameModel();
            _model.NewGame();
            UpdateView();
        }
        private void CheckGameOver()
        {
            UpdateView(); //To Get accurate time at the bottom
            if (_model.bIsGameOver)
            {
                _timer.Stop();
                MessageBox.Show("Game Over"+Environment.NewLine + 
                                "Your time was: "+ TimeSpan.FromMilliseconds(_model.GameTime).ToString());
                NewGame();
            }
        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private async void loadGameToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.Enabled;
            _timer.Stop();

            if (openFileDialog.ShowDialog() == DialogResult.OK) // ha kiválasztottunk egy fájlt
            {
                try
                {
                    // játék betöltése
                    await _model.LoadGameAsync(openFileDialog.FileName);
                    UpdateView();
                }
                catch (AsteroidDataException)
                {
                    MessageBox.Show("Játék betöltése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a fájlformátum.", "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    _model.NewGame();
                }
            }

            if (restartTimer)
                _timer.Start();
        }
        private async void saveGameToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.Enabled;
            _timer.Stop();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // játé mentése
                    await _model.SaveGameAsync(saveFileDialog.FileName);
                }
                catch (AsteroidDataException)
                {
                    MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (restartTimer)
                _timer.Start();
        }
    }
}

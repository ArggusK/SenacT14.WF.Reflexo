using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF.Reflexo
{
    public partial class Form1 : Form
    {
        private Button btnStart;
        private Button btnTarget;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer colorTimer;
        private Random random;
        private Stopwatch stopwatch;
        private int clickCount;
        private Label lblPlacar;
        List<Color> cores = new List<Color>() { Color.Blue, Color.Red, Color.Brown };
        private List<double> ultimosTempos = new List<double>();
        private double melhorTempo = double.MaxValue;

        public Form1()
        {
            InitializeComponent();
            this.Text = "reflex";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            btnStart = new Button()
            {
                Text = "Start",
                Size = new Size(100, 50)
            };

            btnStart.Click += StartGame;
            this.Controls.Add(btnStart);

            this.btnTarget = new Button()
            {
                Size = new Size(100, 100),
                BackColor = Color.Red,
                Visible = false,
            };
            btnTarget.Click += btnTargetClick;
            this.Controls.Add(btnTarget);

            timer = new System.Windows.Forms.Timer();
            timer.Tick += ShowTargetButton;

            colorTimer = new System.Windows.Forms.Timer();
            colorTimer.Interval = 3000;
            colorTimer.Tick += ColorTimer_Tick;

            random = new Random();
            stopwatch = new Stopwatch();

            lblPlacar = new Label();
            lblPlacar.Text = "...";
            lblPlacar.Location = new Point(50, 50);
            lblPlacar.Size = new Size(60, 140);
            lblPlacar.Visible = true;
            this.Controls.Add(lblPlacar);
        }

        private void StartGame(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            StartNewRound();
        }

        private void StartNewRound()
        {
            timer.Interval = random.Next(1000, 3000);
            timer.Start();
        }

        private void ShowTargetButton(object sender, EventArgs e)
        {
            timer.Stop();
            int x = random.Next(50, this.ClientSize.Width - 70);
            int y = random.Next(50, this.ClientSize.Height - 70);
            btnTarget.Location = new Point(x, y);

            btnTarget.Visible = true;
            stopwatch.Restart();
            clickCount = 0;

            colorTimer.Start();
            ChangeButtonColor();
        }

        private void btnTargetClick(object sender, EventArgs e)
        {
            if (btnTarget.BackColor == Color.Yellow)
            {
                clickCount++;

                if (clickCount == 2)
                {
                    int novaLarguraBotao = btnTarget.Size.Width - 10;
                    int novaAlturaBotao = btnTarget.Size.Height - 10;
                    btnTarget.Size = new Size(novaLarguraBotao, novaAlturaBotao);

                    if (novaAlturaBotao <= 20 && novaLarguraBotao <= 20)
                    {
                        MessageBox.Show("Fim de Jogo");
                        btnStart.Enabled = true;
                        btnStart.Visible = true;
                        lblPlacar.Text = "...";
                        stopwatch.Stop();
                        timer.Stop();
                        btnTarget.Size = new Size(100, 100);
                    }
                    else
                    {
                        stopwatch.Stop();
                        btnTarget.Visible = false;
                        long tempoAtual = stopwatch.ElapsedMilliseconds;

                        if (tempoAtual < melhorTempo)
                        {
                            melhorTempo = tempoAtual;
                        }

                        ultimosTempos.Add(tempoAtual);
                        if (ultimosTempos.Count > 5)
                        {
                            ultimosTempos.RemoveAt(0);
                        }

                        string pontuacaoTexto = "";
                        foreach (double ultimoTempo in ultimosTempos)
                        {
                            pontuacaoTexto += $"{ultimoTempo} ms\n";
                        }
                        lblPlacar.Text = pontuacaoTexto;

                        double media = ultimosTempos.Any() ? ultimosTempos.Average() : 0;

                        string mensagem = $"Tempo atual: {tempoAtual}ms\n\n" +
                                          $"Melhor tempo: {melhorTempo}ms\n\n" +
                                          $"Média dos últimos 5: {media:F1}ms";

                        MessageBox.Show(mensagem, "Resultado");
                        Task.Delay(500).ContinueWith(_ => StartNewRound(), TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    clickCount = 0;
                }
            }
            else
            {
                MessageBox.Show("Cor errada!", "Erro");
                btnTarget.Visible = false;
                colorTimer.Stop();
                StartNewRound();
            }
        }

        private void ChangeButtonColor()
        {
            if (random.Next(2) == 0)
            {
                btnTarget.BackColor = Color.Yellow;
                colorTimer.Stop();
            }
            else
            {
                btnTarget.BackColor = cores[random.Next(cores.Count)];
                colorTimer.Start();
            }
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            btnTarget.Visible = false;
            colorTimer.Stop();
            StartNewRound();
        }
    }
}
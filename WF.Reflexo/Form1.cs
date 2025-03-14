using System.Diagnostics;
using System.Linq;

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
        private Label label;
        List<Color> cores = new List<Color>() { Color.Blue, Color.Red, Color.Brown };
        private List<double> ultimosTempos = new List<double>();
        private double melhorTempo = double.MaxValue;

        public Form1()
        {
            InitializeComponent();
            // determina o titulo da tela
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
                Size = new Size(60, 60),
                BackColor = Color.Red,
                Visible = false,
            };
            btnTarget.Click += btnTargetClick;

            // adciona o botao na tela, mas oculto
            this.Controls.Add(btnTarget);

            timer = new System.Windows.Forms.Timer();
            timer.Tick += ShowTargetButton;

            colorTimer = new System.Windows.Forms.Timer();
            colorTimer.Interval = 3000;
            colorTimer.Tick += ColorTimer_Tick;

            random = new Random();
            stopwatch = new Stopwatch();

            label = new Label();
            label.Text = "...";
            label.Location = new Point(50, 50);
            label.Size = new Size(60, 140);
            label.Visible = true;
            this.Controls.Add(label);
        }

        private void StartGame(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            StartNewRound();
        }

        private void StartNewRound()
        {
            // determina um timer aleatorio entre 1 e 3 segundos
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
            clickCount++;

            if (btnTarget.BackColor == Color.Yellow)
            {
                clickCount++;

                if (clickCount == 2)
                {
                    stopwatch.Stop();
                    btnTarget.Visible = false;
                    long tempoAtual = stopwatch.ElapsedMilliseconds;


                    // atualiza o melhor tempo se necess�rio
                    if (tempoAtual < melhorTempo)
                    {
                        melhorTempo = tempoAtual;
                    }

                    // mant�m apenas os �ltimos 5 tempos na lista
                    ultimosTempos.Add(tempoAtual);
                    if (ultimosTempos.Count > 5)
                    {
                        ultimosTempos.RemoveAt(0);
                    }

                    string pontuacaoTexto = "";
                    foreach(double ultimoTempo in ultimosTempos)
                    {
                        pontuacaoTexto += $" {ultimoTempo} ms \n";
                    }
                    label.Text = pontuacaoTexto;

                    // calcula a m�dia dos tempos
                    double media = ultimosTempos.Any() ? ultimosTempos.Average() : 0;

                    // exibe o resultado
                    string mensagem = $"Tempo atual: {tempoAtual}ms\n\n" +
                                       $"Melhor tempo: {melhorTempo}ms\n\n" +
                                       $"M�dia dos �ltimos 5: {media:F1}ms";

                    MessageBox.Show(mensagem, "Resultado");
                    Task.Delay(500).ContinueWith(_ => StartNewRound(), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void ChangeButtonColor()
        {
            // 50% de chance por algum motivo
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
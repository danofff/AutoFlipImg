using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AutoFlipImg
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cancelToken = new CancellationTokenSource();
        public Form1()
        {
            InitializeComponent();
            this.Text = "Flip img";
            Button b1 = new Button
            {
                Location = new Point
                {
                    X = 10,
                    Y = 40
                }
            };
            b1.Text = "Flip";
            b1.Click += button1_Click;
            this.Controls.Add(b1);
            TextBox t1 = new TextBox
            {
                Location = new Point
                {
                    X = 10,
                    Y = 10
                }
            };
            this.Controls.Add(t1);

            Button b2 = new Button
            {
                Location = new Point
                {
                    X = 100,
                    Y = 40
                }
            };
            b2.Text = "Cancel";
            b2.Click += button2_Click;
            this.Controls.Add(b2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Text = "Flip img";
            Task.Factory.StartNew(()=> 
            {
                ProcessFiles();
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cancelToken.Cancel();
        }

        private void ProcessFiles()
        {
            ParallelOptions parOpts = new ParallelOptions();
            parOpts.CancellationToken = cancelToken.Token;
            parOpts.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

            string[] files = Directory.GetFiles(@"C:\Users\vershkov_da\Desktop\img", "*.jpg", SearchOption.AllDirectories);
            string newDir = @"C:\Users\vershkov_da\Desktop\flip_img";
            Directory.CreateDirectory(newDir);

            try
            {
                Parallel.ForEach(files,parOpts, currentfile =>
                {
                    string fileName = Path.GetFileName(currentfile);
                    using (Bitmap bitmap = new Bitmap(currentfile))
                    {
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        bitmap.Save(Path.Combine(newDir, fileName));
                        this.Invoke((Action)delegate
                        {
                            this.Text = fileName;
                        });
                    }
                    Thread.Sleep(2000);
                });
            }
            catch(OperationCanceledException ex)
            {
                this.Invoke((Action)delegate
                {
                    this.Text = ex.Message;

                });
            }
        }
    }
}

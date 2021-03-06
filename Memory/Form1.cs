﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory
{
    public partial class Form1 : Form
    {

        FormStartGame startSpielForm;
        MemoryListenKontroller MemorieKontroller;
        GameKontroller spielKontroller;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startSpielForm = new FormStartGame();
            startSpielForm.StartPosition = FormStartPosition.CenterParent;
            startSpielForm.ShowDialog();
            if(!startSpielForm.Startgame)
            {
                Application.Exit();
                return;
            }

            List<Player> player = new List<Player>();

            startSpielForm.PlayerNames().ForEach(k => player.Add(new Player(k)));
            spielKontroller = new GameKontroller(player);
            this.comboBox1.Items.AddRange(spielKontroller.Player.ToArray());
            this.comboBox1.SelectedItem = spielKontroller.ActivePlayer;
            DirectoryInfo d = new DirectoryInfo("./Bilder/");

            List<string> urls = new List<string>();

            foreach (var file in d.GetFiles("*.jpg"))
            {
                urls.Add(file.FullName);
            }

            MemorieKontroller = new MemoryListenKontroller(memoriePanel, urls.Skip(1).ToList(), urls[0], 100, 8);
            MemorieKontroller.SPIELER_FERTIG_EVENT += (o, ee) =>
            {
                if(ee.Punkt)
                {
                    spielKontroller.ActivePlayer.Kacheln.Add(ee.Kachel);

                }
                else
                {
                    spielKontroller.NaechsterSpieler();
                    this.comboBox1.SelectedItem = spielKontroller.ActivePlayer;
                }
                this.lblPunkteStandanzeigen.Text = $"Spieler {spielKontroller.ActivePlayer.name} hat {spielKontroller.ActivePlayer.Kacheln.Count} Punkte";
                panelPunkte.Controls.Clear();
                var list = new List<PictureBox>();
                spielKontroller.ActivePlayer.Kacheln.ForEach(k => list.Add(new PictureBox() 
                {
                    ImageLocation = k.ImageLocation,
                    Width = 100, Height = 100 ,
                    Margin= new Padding(5,5,5,5),
                    SizeMode=PictureBoxSizeMode.StretchImage
                }));
                panelPunkte.Controls.AddRange(list.ToArray());

                if(MemorieKontroller.UebrigeKacheln.Count == 0)
                {
                    if(MessageBox.Show($"{spielKontroller.Player.OrderBy(k => k.Kacheln.Count).First().name} hat gewonnen. Wollen Sie nochmal spielen?" ,
                        "Spielende",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        spielKontroller.Player.ForEach(k => k.Kacheln.Clear());
                        MemorieKontroller.Startgame();
                        panelPunkte.Controls.Clear();
                        this.lblPunkteStandanzeigen.Text = $"Spieler {spielKontroller.ActivePlayer.name} hat {spielKontroller.ActivePlayer.Kacheln.Count} Punkte";

                    }
                }

            };
        }
    }
}

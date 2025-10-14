//-------------------Libraries importeren----------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;

class Program
{
    static void Main()
    {
        bord spel = new bord();
        spel.nieuwSpel();
    }
}

//-----------------SPELBORD AANMAKEN---------------------------
class bord // Klasse met alle methoden en variabelen voor het bord
{
    //-----------------Variabelen---------------------------
    private int beurt = 1;
    private int afmeting = 1;
    private int[,] tabel = new int[6, 6];
    private Bitmap plaatje;
    private Label afbeelding;
    private Form scherm;
    private int breedte = 600;
    private int hoogte = 600;
    private int vakBreedte = 100;
    private int vakHoogte = 100;

    

    //-----------------Methoden-----------------------------
    //-------- Stenen tekenen --------------------
    void tekenStenen(int x, int y, int kleur)
    {
        // Berekeningen voor het tekenen van de stenen
        int steenBreedte = (vakBreedte * 3 / 4);
        int steenHoogte = (vakHoogte * 3 / 4);
        int steenX = x / vakBreedte;
        int steenY = y / vakHoogte;

        // Controleer of de klik binnen het bord valt
        if (x<0 || x>=breedte || y<0 || y>=hoogte)
        {
            return;
        }

        // Zorg dat er niet dubbel op een vak wordt geklikt
        if (tabel[steenX, steenY] != 0)
        {
            return;
        }

        // Bijhouden waar de stenen liggen in de tabel
        tabel[steenX, steenY] = kleur;

        // Kleur van de steen bepalen en tekenen
        Graphics g = Graphics.FromImage(afbeelding.Image);

        if (kleur == 1) // Witte steen, speler 1
        {
            g.FillEllipse(Brushes.White, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
            g.DrawEllipse(Pens.Black, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
            

        }
        else if (kleur == 2) //Zwarte steen, speler 2
        {
            g.FillEllipse(Brushes.Black, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
            g.DrawEllipse(Pens.White, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
        }

        if (beurt == 1)
            beurt = 2;
        else
            beurt = 1;


        afbeelding.Refresh();
    }
    
    public void tekenBeginStenen()
    {
        tekenStenen (300, 300, 1); //Wit
        tekenStenen (300, 299, 2); //Zwart
        tekenStenen (299, 300, 2); //Zwart
        tekenStenen (299, 299, 1); //Wit
        beurt = 1; //Zodat speler 1 (wit) begint
    }

    // Methode om mogelijke zetten te tonen
    void tekenMogelijkeStenen()
    {
        for (int x = 0; x<tabel.GetLength(0); x++)
        {
            for (int y = 0; y<tabel.GetLength(1); y++)
            {
                if (IsEenMogelijkeZet(x, y))
                {
                    int steenX = x;
                    int steenY = y;
                    int steenBreedte = (vakBreedte * 3 / 4);
                    int steenHoogte = (vakHoogte * 3 / 4);
                    Graphics g = Graphics.FromImage(afbeelding.Image);
                    g.DrawEllipse(Pens.Red, steenX * vakBreedte + vakBreedte / 4, steenY * vakHoogte + vakHoogte / 4, steenBreedte / 2, steenHoogte / 2);
                }


            }

        }
        afbeelding.Refresh();
    }

    Boolean IsEenMogelijkeZet(int x, int y)
    {
        //for loop voor alle richtingen van maken
       
        int dx = -1;
        int dy = 0;

        int checkX = x + dx;
        int checkY = y + dy;

        // Controleer of de zet binnen de grenzen van het bord valt
        if (checkX < 0 || checkX >= tabel.GetLength(0) || checkY < 0 || checkY >= tabel.GetLength(1))
        {
            return false;
        }
        int steenX = x;
        int steenY = y;
        if (tabel[steenX, steenY] != 0)
        {
            return false;
        }

        int andere = 1;
        if (beurt == 1)
        {
            andere = 2;
        }
            // Controleer of het vakje al bezet is
            if (tabel[checkX, checkY] != andere)
            {
                return false;
            }
            while (tabel[checkX, checkY] == andere)
            {
                checkX += dx;
                checkY += dy;
                if (checkX < 0 || checkX >= tabel.GetLength(0) || checkY < 0 || checkY >= tabel.GetLength(1))
                {
                    return false;
                }
            }
            if (tabel [checkX, checkY] == beurt)
            {
                return true;
            }
          
        return false;
    }


    public void nieuwSpel()
    {
        // Window aanmaken
        scherm = new Form();
        scherm.Text = "A.2 - Reversi";
        scherm.BackColor = Color.LightYellow;
        scherm.ClientSize = new Size(1000, 1000);

        afbeelding = new Label();
        afbeelding.Location = new Point(100, 100);
        afbeelding.Size = new Size(600, 600);
        afbeelding.BackColor = Color.LightGreen;
        scherm.Controls.Add(afbeelding);

        plaatje = new Bitmap(600, 600);
        afbeelding.Image = plaatje;

        //-----------------Gegevens van de speler------------------------
        TextBox speler1 = new TextBox();
        speler1.Location = new Point(750, 100);
        speler1.Size = new Size(200, 30);
        speler1.Text = "Speler 1";
        speler1.BackColor = Color.LightGray;
        scherm.Controls.Add(speler1);

        TextBox speler2 = new TextBox();
        speler2.Location = new Point(750, 150);
        speler2.Size = new Size(200, 30);
        speler2.Text = "Speler 2";
        speler2.BackColor = Color.LightGray;
        scherm.Controls.Add(speler2);


        //----------------Buttons aanmaken----------------------
        //4x4 bord
        Button afmeting4 = new Button();
        afmeting4.Location = new Point(20, 50);
        afmeting4.Size = new Size(50, 30);
        afmeting4.Text = "4x4";
        afmeting4.BackColor = Color.LightGray;
        scherm.Controls.Add(afmeting4);

        //6x6 bord
        Button afmeting6 = new Button();
        afmeting6.Text = "6x6";
        afmeting6.Location = new Point(80, 50);
        afmeting6.Size = new Size(50, 30);
        afmeting6.BackColor = Color.LightGray;
        scherm.Controls.Add(afmeting6);

        //8x8 bord
        Button afmeting8 = new Button();
        afmeting8.Text = "8x8";
        afmeting8.Location = new Point(140, 50);
        afmeting8.Size = new Size(50, 30);
        afmeting8.BackColor = Color.LightGray;
        scherm.Controls.Add(afmeting8);

        //10x10 bord
        Button afmeting10 = new Button();
        afmeting10.Text = "10x10";
        afmeting10.Location = new Point(200, 50);
        afmeting10.Size = new Size(50, 30);
        afmeting10.BackColor = Color.LightGray;
        scherm.Controls.Add(afmeting10);

        // Eventhandlers voor de buttons
        //Standaardbord

        plaatje = IntArrayToBitmap();
        afbeelding.Image = plaatje;

        //4x4 bord
        void kiesAfmeting4(object o, EventArgs ea)
        {
            afmeting = 0;
            tabel = new int[4, 4];
            plaatje = IntArrayToBitmap();
            afbeelding.Image = plaatje;
            tekenBeginStenen();
        }
        afmeting4.Click += kiesAfmeting4;

        //6x6 bord
        void kiesAfmeting6(object o, EventArgs ea)
        {
            afmeting = 1;
            tabel = new int[6, 6];
            plaatje = IntArrayToBitmap();
            afbeelding.Image = plaatje;
            tekenBeginStenen();
        }
        afmeting6.Click += kiesAfmeting6;

        //8x8 bord
        void kiesAfmeting8(object o, EventArgs ea)
        {
            afmeting = 2;
            tabel = new int[8, 8];
            plaatje = IntArrayToBitmap();
            afbeelding.Image = plaatje;
            tekenBeginStenen();
        }
        afmeting8.Click += kiesAfmeting8;

        //10x10 bord
        void kiesAfmeting10(object o, EventArgs ea)
        {
            afmeting = 3;
            tabel = new int[10, 10];
            plaatje = IntArrayToBitmap();
            afbeelding.Image = plaatje;
            tekenBeginStenen();
        }
        afmeting10.Click += kiesAfmeting10;

        // Muis eventhandler
        void muisKlik(object o, MouseEventArgs mea)
        {
            int x = mea.X;
            int y = mea.Y;

            tekenStenen(x, y, beurt);
            tekenMogelijkeStenen();
        }
        scherm.Refresh();
        scherm.MouseClick += muisKlik;
        afbeelding.MouseClick += muisKlik;

        tekenBeginStenen();

        Application.Run(scherm);
    }
    // Methode om het bord te tekenen
    public Bitmap IntArrayToBitmap()
    {
        int grootte = 6; // Standaardgrootte

        if (afmeting == 0) // 4x4 bord
        {
            grootte = 4;

            if (tabel.GetLength(0) != grootte)  // Controleer of de de tabel al het juiste aantal rijen heeft
                tabel = new int[4, 4];          // Zo niet, maak een nieuwe tabel aan
        }
        else if (afmeting == 1) // 6x6 bord
        {
            grootte = 6;
            if (tabel.GetLength(0) != grootte)
                tabel = new int[6, 6];
        }
        else if (afmeting == 2) // 8x8 bord
        {
            grootte = 8;
            if (tabel.GetLength(0) != grootte)
                tabel = new int[8, 8];
        }
        else if (afmeting == 3) // 10x10 bord
        {
            grootte = 10;
            if (tabel.GetLength(0) != grootte)
                tabel = new int[10, 10];
        }
        else // Foutafhandeling
        {
            grootte = 6;
        }

        // Berekeningen voor het tekenen van het bord
        vakBreedte = 600 / grootte;
        vakHoogte = 600 / grootte;

        // Bord tekenen
        Bitmap bitmap = new Bitmap(600, 600);
        Graphics g = Graphics.FromImage(bitmap);
        g.DrawRectangle(Pens.Black, 0, 0, breedte - 1, hoogte - 1);

        for (int vakX = 0; vakX < grootte; vakX++)
        {
            for (int vakY = 0; vakY < grootte; vakY++)
            {
                g.DrawRectangle(Pens.Black, vakX * vakBreedte, vakY * vakHoogte, vakBreedte - 1, vakHoogte - 1);
            }
        }
        return bitmap;
    }
}

class spelverloop
{

}
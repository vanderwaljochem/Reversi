//-------------------Libraries importeren----------------------
using System; // Voor basis functionaliteiten
using System.Drawing; // Voor het tekenen van de stenen en het bord
using System.Windows.Forms; // Voor het maken van het GUI
using System.Collections.Generic; // Voor de lijst van te wisselen stenen

class Program // Hoofdprogramma om het spel te starten
{
    static void Main() // Main methode om het spel te starten
    {
        bord spel = new bord(); // Maak een nieuw bord aan
        spel.nieuwSpel(); // Start een nieuw spel
    }
}

//-----------------SPELBORD AANMAKEN---------------------------
class bord // Klasse met alle methoden en variabelen voor het bord
{
    //-----------------Variabelen---------------------------
    private int beurt = 1; // 1 is speler 1 (wit), 2 is speler 2 (zwart)
    private int afmeting = 1; // 0 is 4x4, 1 is 6x6, 2 is 8x8, 3 is 10x10
    private int[,] tabel = new int[6, 6];
    private Bitmap plaatje;
    private Label afbeelding;
    private Form scherm;
    private int breedte = 600;
    private int hoogte = 600;
    private int vakBreedte = 100;
    private int vakHoogte = 100;
    private bool ondersteuning = true;
    private TextBox speler1;
    private TextBox speler2;
    private Label aandebeurt;


    //-----------------Methoden-----------------------------

    //-------- Stenen tekenen --------------------
    public void tekenStenen(int x, int y, int kleur)
    {
        // Berekeningen voor het tekenen van de stenen
        int steenBreedte = (vakBreedte * 3 / 4);
        int steenHoogte = (vakHoogte * 3 / 4);
        int steenX = x / vakBreedte;
        int steenY = y / vakHoogte;

        // Controleer of de klik binnen het bord valt
        if (steenX < 0 || steenX >= tabel.GetLength(0) || steenY < 0 || steenY >= tabel.GetLength(1))
        {
            return;
        }

        // Zorg dat er niet dubbel op een vak wordt geklikt
        if (tabel[steenX, steenY] != 0)
        {
            return;
        }

        // Controleren of de zet mogelijk is
        if (!IsEenMogelijkeZet(steenX, steenY))
        {
            return;
        }

        // Bijhouden waar de stenen liggen in de tabel
        tabel[steenX, steenY] = kleur;

        // Wissel de kleuren van de stenen indien nodig
        WisselKleurVanStenen(steenX, steenY); 

        // Kleur van de steen bepalen en tekenen
        Graphics g = Graphics.FromImage(afbeelding.Image);

        if (kleur == 1) // Witte steen, speler 1
        {
            g.FillEllipse(Brushes.White, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
            g.DrawEllipse(Pens.Black, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
        }
        else if (kleur == 2) // Zwarte steen, speler 2
        {
            g.FillEllipse(Brushes.Black, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
            g.DrawEllipse(Pens.White, steenX * vakBreedte + vakBreedte / 8, steenY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
        }

        // Flip de beurt

        if (beurt == 1)
            beurt = 2;
        else
            beurt = 1;

        plaatje = IntArrayToBitmap();
        afbeelding.Image = plaatje;

        afbeelding.Refresh();
        if (ondersteuning)
        {
            tekenMogelijkeStenen();  // Roep deze aan om de nieuwe mogelijke zetten te tonen voor de volgende beurt
            afbeelding.Refresh();
        }
        UpdateBeurt();
    }

    public void tekenBeginStenen()
    {
        int middenX = tabel.GetLength(0) / 2;
        int middenY = tabel.GetLength(1) / 2;

        // Voor een 6x6 bord (pas aan als nodig, maar dit is standaard)
        tabel[middenX, middenY] = 1;  // Wit
        tabel[middenX, middenY - 1] = 2;  // Zwart
        tabel[middenX - 1, middenY] = 2;  // Zwart
        tabel[middenX - 1, middenY -1] = 1;  // Wit
        beurt = 1;  // Speler 1 begint

        plaatje = IntArrayToBitmap();  // Herteken het bord met beginstenen
        afbeelding.Image = plaatje;
    }

    // Methode om mogelijke zetten te tonen
    void tekenMogelijkeStenen()
    {
        plaatje = IntArrayToBitmap();  // Herteken het bord met stenen, wat oude markeringen verwijdert
        afbeelding.Image = plaatje;    // Update de afbeelding

        Graphics g = Graphics.FromImage(afbeelding.Image);  // Nu op de nieuwe bitmap

        for (int x = 0; x < tabel.GetLength(0); x++)
        {
            for (int y = 0; y < tabel.GetLength(1); y++)
            {
                if (IsEenMogelijkeZet(x, y))  // Controleer voor de huidige beurt
                {
                    Pen penKleur;
                    if (beurt == 1)
                        penKleur = Pens.Red;
                    else if (beurt == 2)
                        penKleur = Pens.Green;
                    else
                        penKleur = Pens.LightYellow; //Onzichtbaar voor fouten (zelfde kleur als achtergrond)

                    int steenBreedte = (vakBreedte * 3 / 4);
                    int steenHoogte = (vakHoogte * 3 / 4);
                    g.DrawEllipse(penKleur, x * vakBreedte + vakBreedte / 4, y * vakHoogte + vakHoogte / 4, steenBreedte / 2, steenHoogte / 2);
                }
            }
        }

        afbeelding.Refresh();  // Vernieuw de afbeelding om de markeringen te tonen
    }

    bool IsEenMogelijkeZet(int x, int y)
    {
        if (tabel[x, y] != 0) return false;  // Vakje moet leeg zijn

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (CheckRichting(x, y, dx, dy)) return true;
            }
        }
        return false;
    }

    bool CheckRichting(int x, int y, int dx, int dy)
    {
        int checkX = x + dx;
        int checkY = y + dy;
        int andere;
        
        if (beurt == 1)
            andere = 2;
        else
            andere = 1;

        if (checkX < 0 || checkX >= tabel.GetLength(0) || checkY < 0 || checkY >= tabel.GetLength(1)) return false;
        if (tabel[checkX, checkY] != andere) return false;

        while (true)
        {
            checkX += dx;
            checkY += dy;
            if (checkX < 0 || checkX >= tabel.GetLength(0) || checkY < 0 || checkY >= tabel.GetLength(1)) return false;
            if (tabel[checkX, checkY] == 0) return false;
            if (tabel[checkX, checkY] == beurt) return true;
        }
    }

    bool VakkenGevuld()
    {
        if IsEenMogelijkeZet() == true{
            continue
        }
        else 
        {
            return 
        }

    }
    void WisselKleurVanStenen(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (CheckRichting(x, y, dx, dy))
                {
                    WisselInRichting(x, y, dx, dy);
                }
            }
        }
    }
    void WisselInRichting(int x, int y, int dx, int dy)
    {
        int andere;
        if (beurt == 1)
            andere = 2;
        else
            andere = 1;

        int checkX = x + dx;
        int checkY = y + dy;

        // Lijst om de stenen op te slaan
        List<(int, int)> teWisselen = new List<(int, int)>();

        while (checkX >= 0 && checkX < tabel.GetLength(0) &&
               checkY >= 0 && checkY < tabel.GetLength(1))
        {
            if (tabel[checkX, checkY] == andere)
            {
                // Tegenstandersteen gevonden, opslaan
                teWisselen.Add((checkX, checkY));
            }
            else if (tabel[checkX, checkY] == beurt)
            {
                // Wissel alle stenen ertussen
                foreach ((int wisselX, int wisselY) in teWisselen)
                {
                    tabel[wisselX, wisselY] = beurt;
                }
                return;
            }
            else
            {
                return;
            }

            // Ga verder in dezelfde richting
            checkX += dx;
            checkY += dy;
        }
    }

    void UpdateBeurt()
    {
        if (beurt == 1)
            aandebeurt.Text = $"Aan de beurt: {speler1.Text}, (wit)";
        else
            aandebeurt.Text = $"Aan de beurt: {speler2.Text}, (zwart)";
    }

    public void nieuwSpel()
    {
        // Window aanmaken
        scherm = new Form();
        scherm.Text = "Switch 'n Score!";
        scherm.BackColor = Color.LightPink;
        scherm.ClientSize = new Size(1000, 1000);

        afbeelding = new Label();
        afbeelding.Location = new Point(100, 100);
        afbeelding.Size = new Size(600, 600);
        afbeelding.BackColor = Color.LightYellow;
        scherm.Controls.Add(afbeelding);

        plaatje = new Bitmap(600, 600);
        afbeelding.Image = plaatje;

        // Uitleg over het spel, klik op OK om verder te gaan
        MessageBox.Show("Speler 1 is wit en begint het spel, speler 2 is zwart. Druk op OK om de reversi te starten");

        //-----------------Labels aanmaken----------------------
        Font fontGroot = new Font("Times New Roman", 20);
        Font fontKlein = new Font("Times New Roman", 12);

        Label titel = new Label();
        titel.Location = new Point(20, 10);
        titel.Size = new Size(200, 30);
        titel.Text = "Switch 'n Score!";
        titel.Width = 250;
        titel.Font = fontGroot;

        Label uitleg = new Label();
        uitleg.Location = new Point(300, 10);
        uitleg.Size = new Size(500, 30);
        uitleg.Text = "Kies een afmeting en voer de namen van de spelers in.";
        uitleg.Width = 700;
        uitleg.Font = fontGroot;

        Label spelers = new Label();
        spelers.Location = new Point(750, 75);
        spelers.Size = new Size(200, 30);
        spelers.Text = "Spelers:";
        spelers.Width = 200;
        spelers.Font = fontKlein;

        aandebeurt = new Label();
        aandebeurt.Location = new Point(750, 200);
        aandebeurt.Size = new Size(200, 100);
        aandebeurt.Width = 200;
        aandebeurt.Font = fontKlein;

        scherm.Controls.Add(titel);
        scherm.Controls.Add(uitleg);
        scherm.Controls.Add(spelers);
        scherm.Controls.Add(aandebeurt);

        //-----------------Gegevens van de speler------------------------
        speler1 = new TextBox();
        speler1.Location = new Point(750, 100);
        speler1.Size = new Size(200, 30);
        speler1.Text = "Speler 1";
        speler1.BackColor = Color.LightGreen;
        scherm.Controls.Add(speler1);

        speler2 = new TextBox();
        speler2.Location = new Point(750, 150);
        speler2.Size = new Size(200, 30);
        speler2.Text = "Speler 2";
        speler2.BackColor = Color.LightGreen;
        scherm.Controls.Add(speler2);

    //----------------Buttons aanmaken----------------------
    Button afmeting4 = new Button();
        afmeting4.Location = new Point(20, 50);
        afmeting4.Size = new Size(50, 30);
        afmeting4.Text = "4x4";
        afmeting4.BackColor = Color.LightGreen;
        scherm.Controls.Add(afmeting4);

        Button afmeting6 = new Button();
        afmeting6.Text = "6x6";
        afmeting6.Location = new Point(80, 50);
        afmeting6.Size = new Size(50, 30);
        afmeting6.BackColor = Color.LightGreen;
        scherm.Controls.Add(afmeting6);

        Button afmeting8 = new Button();
        afmeting8.Text = "8x8";
        afmeting8.Location = new Point(140, 50);
        afmeting8.Size = new Size(50, 30);
        afmeting8.BackColor = Color.LightGreen;
        scherm.Controls.Add(afmeting8);

        Button afmeting10 = new Button();
        afmeting10.Text = "10x10";
        afmeting10.Location = new Point(200, 50);
        afmeting10.Size = new Size(50, 30);
        afmeting10.BackColor = Color.LightGreen;
        scherm.Controls.Add(afmeting10);

        Button help = new Button();
        help.Text = "HELP!";
        help.Location = new Point(900, 600);
        help.Size = new Size(75, 75);
        help.BackColor = Color.LightGreen;
        scherm.Controls.Add(help);
        //Eventhandlers voor de buttons

        //4x4 bord
        void kiesAfmeting4(object o, EventArgs ea)
        {
            afmeting = 0;
            tabel = new int[4, 4];
            plaatje = IntArrayToBitmap();
            afbeelding.Image = plaatje;
            tekenBeginStenen();
            tekenMogelijkeStenen();
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
            tekenMogelijkeStenen();
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
            tekenMogelijkeStenen();
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
            tekenMogelijkeStenen();
        }
        afmeting10.Click += kiesAfmeting10;

        // Muis eventhandler
        void muisKlik(object o, MouseEventArgs mea)
        {
            int x = mea.X;
            int y = mea.Y;
            tekenStenen(x, y, beurt);
            WisselKleurVanStenen(x, y);
        }
        scherm.MouseClick += muisKlik;
        afbeelding.MouseClick += muisKlik;

        // Help eventhandler
        void helpKnop(object o, EventArgs ea)
        {
            ondersteuning = !ondersteuning; // draai de waarde om
        }
        help.Click += helpKnop;

        tekenBeginStenen();
        if (ondersteuning)
        {    
            tekenMogelijkeStenen();
            afbeelding.Refresh();
        }

        UpdateBeurt();
        Application.Run(scherm);
    }

    public Bitmap IntArrayToBitmap()
    {
        int grootte = tabel.GetLength(0);
        vakBreedte = 600 / grootte;
        vakHoogte = 600 / grootte;

        Bitmap bitmap = new Bitmap(600, 600);
        Graphics g = Graphics.FromImage(bitmap);

        for (int vakX = 0; vakX < grootte; vakX++)
        {
            for (int vakY = 0; vakY < grootte; vakY++)
            {
                g.DrawRectangle(Pens.Black, vakX * vakBreedte, vakY * vakHoogte, vakBreedte, vakHoogte);

                if (tabel[vakX, vakY] == 1)
                {
                    int steenBreedte = (vakBreedte * 3 / 4);
                    int steenHoogte = (vakHoogte * 3 / 4);
                    g.FillEllipse(Brushes.White, vakX * vakBreedte + vakBreedte / 8, vakY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
                    g.DrawEllipse(Pens.Black, vakX * vakBreedte + vakBreedte / 8, vakY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
                }
                else if (tabel[vakX, vakY] == 2)
                {
                    int steenBreedte = (vakBreedte * 3 / 4);
                    int steenHoogte = (vakHoogte * 3 / 4);
                    g.FillEllipse(Brushes.Black, vakX * vakBreedte + vakBreedte / 8, vakY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
                    g.DrawEllipse(Pens.White, vakX * vakBreedte + vakBreedte / 8, vakY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
                }
            }
        }
        return bitmap;
    }
}
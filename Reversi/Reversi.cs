//-------------------Libraries importeren----------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


class Programma // Hoofdprogramma om het spel te starten
{
    static void Main() // Main methode om het spel te starten
    {
        Gebruiker scherm = new Gebruiker();
        scherm.nieuwSpel();
    }
}

//-----------------KLASSE VOOR HET TEKENEN---------------------------
class Tekenen
{
    private int vakBreedte;
    private int vakHoogte;

    // Zet de int array om naar een bitmap voor weergave
    public Bitmap IntArrayToBitmap(int[,] tabel)
    {
        int grootte = tabel.GetLength(0);
        vakBreedte = 600 / grootte;
        vakHoogte = 600 / grootte;

        Bitmap bitmap = new Bitmap(600, 600);
        Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.LightYellow); // Maak de achtergrond van het bord vak geel

        for (int vakX = 0; vakX < grootte; vakX++)
        {
            for (int vakY = 0; vakY < grootte; vakY++)
            {
                g.DrawRectangle(Pens.Black, vakX * vakBreedte, vakY * vakHoogte, vakBreedte, vakHoogte);

                if (tabel[vakX, vakY] == 1) // Wit
                {
                    int steenBreedte = (vakBreedte * 3 / 4);
                    int steenHoogte = (vakHoogte * 3 / 4);
                    g.FillEllipse(Brushes.White, vakX * vakBreedte + vakBreedte / 8, vakY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
                    g.DrawEllipse(Pens.Black, vakX * vakBreedte + vakBreedte / 8, vakY * vakHoogte + vakHoogte / 8, steenBreedte, steenHoogte);
                }
                else if (tabel[vakX, vakY] == 2) // Zwart
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

    public int GetVakBreedte(int grootte)
    {
        return 600 / grootte;
    }

    public int GetVakHoogte(int grootte)
    {
        return 600 / grootte;
    }
}

//-----------------SPELBORD -------------------------------
class Bord
{
    //-----------------Variabelen---------------------------
    private int beurt = 1; // 1 is speler 1 (wit), 2 is speler 2 (zwart)
    private int afmeting = 1; // 0 is 4x4, 1 is 6x6, 2 is 8x8, 3 is 10x10
    private int[,] tabel = new int[6, 6];

    // Eigenschappen om toegang te geven tot de private variabelen
    public int Beurt { get { return beurt; } set { beurt = value; } }
    public int[,] Tabel { get { return tabel; } set { tabel = value; } }
    public int Afmeting { get { return afmeting; } set { afmeting = value; } }

    // Beginstenen tekenen
    public Bord()
    {
        tekenBeginStenen();
    }

    //-----------------Methoden-----------------------------

    public void tekenBeginStenen()
    {
        // Reset de tabel voor een nieuw spel
        tabel = new int[tabel.GetLength(0), tabel.GetLength(1)];

        int middenX = tabel.GetLength(0) / 2;
        int middenY = tabel.GetLength(1) / 2;

        // Beginpositie van de stenen
        tabel[middenX, middenY] = 1;      // Wit
        tabel[middenX, middenY - 1] = 2;  // Zwart
        tabel[middenX - 1, middenY] = 2;  // Zwart
        tabel[middenX - 1, middenY - 1] = 1; // Wit
        beurt = 1; // Speler 1 begint
    }

    // Geeft terug of een zet geldig is
    public bool IsEenMogelijkeZet(int x, int y)
    {
        if (tabel[x, y] != 0) 
            return false; // Vakje moet leeg zijn

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) 
                    continue;

                if (CheckRichting(x, y, dx, dy)) 
                    return true;
            }
        }
        return false;
    }

    // Controleert in een bepaalde richting of er stenen omgedraaid kunnen worden
    private bool CheckRichting(int x, int y, int dx, int dy)
    {
        int checkX = x + dx;
        int checkY = y + dy;
        int andere;

        if (beurt == 1)
            andere = 2;
        else
            andere = 1;

        // Richting niet mogelijk
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

    // Voert de zet uit, plaatst de steen en wisselt kleuren
    public void WisselStenen(int x, int y)
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
        // Plaats de nieuwe steen NA het bepalen welke stenen gewisseld moeten worden
        tabel[x, y] = beurt;
    }

    // Keert de stenen in een specifieke richting om
    private void WisselInRichting(int x, int y, int dx, int dy)
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
                // Steen van tegenstander gevonden en opslaan
                teWisselen.Add((checkX, checkY));
            }
            else if (tabel[checkX, checkY] == beurt)
            {
                // Wissel stenen 
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

    // Controleert of het spel afgelopen is (dus geen mogelijke zetten meer voor beide spelers)
    public bool Winnen()
    {
        int huidigeBeurt = beurt;

        // Controle uitvoeren voor speler 1 
        beurt = 1;
        if (HeeftMogelijkeZet())
        {
            beurt = huidigeBeurt; 
            return false;
        }

        // Controle uitvoeren voor speler 2
        beurt = 2;
        if (HeeftMogelijkeZet())
        {
            beurt = huidigeBeurt;
            return false;
        }

        // Beide spelers hebben geen zetten meer
        beurt = huidigeBeurt; 
        return true;
    }

    // Controle of er een zet mogelijk is voor de huidige speler
    private bool HeeftMogelijkeZet()
    {
        for (int x = 0; x < tabel.GetLength(0); x++)
        {
            for (int y = 0; y < tabel.GetLength(1); y++)
            {
                if (IsEenMogelijkeZetVoorSpeler(x, y, beurt))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Wordt gebruikt voor de Winnen() methode om te controleren of een speler een zet kan doen
    private bool IsEenMogelijkeZetVoorSpeler(int x, int y, int speler)
    {
        if (tabel[x, y] != 0) return false;

        int oorspronkelijkeBeurt = beurt;
        beurt = speler; // Beurt even tijdelijk aanpassen
        bool resultaat = IsEenMogelijkeZet(x, y);
        beurt = oorspronkelijkeBeurt; // Herstel beurt
        return resultaat;
    }

    // Wisselt de beurt (1 -> 2 of 2 -> 1)
    public void FlipBeurt()
    {
        if (beurt == 1)
            beurt = 2;
        else
            beurt = 1;
    }

    // Telt de score voor de winnaarsboodschap
    public (int wit, int zwart) TelScore()
    {
        int wit = 0;
        int zwart = 0;
        for (int aantalX = 0; aantalX < tabel.GetLength(0); aantalX++)
        {
            for (int aantalY = 0; aantalY < tabel.GetLength(1); aantalY++)
            {
                if (tabel[aantalX, aantalY] == 1) wit++;
                else if (tabel[aantalX, aantalY] == 2) zwart++;
            }
        }
        return (wit, zwart);
    }

    // Past de tabelgrootte aan
    public void PasAfmetingAan(int nieuweAfmeting)
    {
        afmeting = nieuweAfmeting;
        int grootte;
        if (afmeting == 0) grootte = 4;
        else if (afmeting == 1) grootte = 6;
        else if (afmeting == 2) grootte = 8;
        else grootte = 10;
        tabel = new int[grootte, grootte];
        tekenBeginStenen(); // Start met de nieuwe afmeting
    }
}

//-----------------KLASSE VOOR DE WINDOWS FORMS---------------------------
class Gebruiker
{
    // Variabelen
    private Bord spelBord = new Bord(); 
    private Tekenen tekenHulp = new Tekenen(); 
    private Bitmap plaatje;
    private Label afbeelding;
    private Form scherm;
    private int breedte = 600;
    private int hoogte = 600;
    private int vakBreedte = 100; // Wordt later overschreven door TekenHulp
    private int vakHoogte = 100;  // Wordt later overschreven door TekenHulp
    private bool ondersteuning = true;
    private TextBox speler1;
    private TextBox speler2;
    private Label aandebeurt;
    private Label score = new Label();

    // Methode om de spelersnamen en beurtstatus te updaten
    private void UpdateBeurt()
    {
        if (spelBord.Beurt == 1)
            aandebeurt.Text = $"Aan de beurt: {speler1.Text}, (wit)";
        else
            aandebeurt.Text = $"Aan de beurt: {speler2.Text}, (zwart)";
    }

    // Tekent de mogelijke zetten als rode of groene cirkels
    void tekenMogelijkeStenen()
    {
        // Haal het basisplaatje met stenen op
        plaatje = tekenHulp.IntArrayToBitmap(spelBord.Tabel);
        afbeelding.Image = plaatje;

        Graphics g = Graphics.FromImage(afbeelding.Image);
        int grootte = spelBord.Tabel.GetLength(0);
        vakBreedte = tekenHulp.GetVakBreedte(grootte);
        vakHoogte = tekenHulp.GetVakHoogte(grootte);

        for (int x = 0; x < grootte; x++)
        {
            for (int y = 0; y < grootte; y++)
            {
                if (spelBord.IsEenMogelijkeZet(x, y))
                {
                    Pen penKleur;
                    if (spelBord.Beurt == 1)
                        penKleur = Pens.Red;
                    else if (spelBord.Beurt == 2)
                        penKleur = Pens.Green;
                    else
                        penKleur = Pens.LightYellow;

                    int steenBreedte = (vakBreedte * 3 / 4);
                    int steenHoogte = (vakHoogte * 3 / 4);
                    // Teken een kleinere cirkel in het midden van het vakje
                    g.DrawEllipse(penKleur, x * vakBreedte + vakBreedte / 4, y * vakHoogte + vakHoogte / 4, steenBreedte / 2, steenHoogte / 2);
                }
            }
        }
        afbeelding.Refresh();
    }


    // Tekent de stenen, kijkt of de zet mogelijk is en wisselt de stenen
    public void tekenStenen(int x, int y, int kleur)
    {
        // Al berekende vakafmetingen
        int grootte = spelBord.Tabel.GetLength(0);
        vakBreedte = tekenHulp.GetVakBreedte(grootte);
        vakHoogte = tekenHulp.GetVakHoogte(grootte);

        int steenX = x / vakBreedte;
        int steenY = y / vakHoogte;

        // Controleer of de klik binnen het bord valt
        if (steenX < 0 || steenX >= grootte || steenY < 0 || steenY >= grootte)
        {
            return;
        }

        // Zorg dat er niet dubbel op een vak wordt geklikt
        if (spelBord.Tabel[steenX, steenY] != 0)
        {
            return;
        }

        // Controleren of de zet mogelijk is
        if (!spelBord.IsEenMogelijkeZet(steenX, steenY))
        {
            return;
        }

        // Wissel de kleuren van de stenen en plaats de nieuwe steen
        spelBord.WisselStenen(steenX, steenY);

        // Flip de beurt
        spelBord.FlipBeurt();

        // Update de score
        (int wit, int zwart) = spelBord.TelScore();
        score.Text = $"Score:\n {speler1.Text} (wit): {wit}\n {speler2.Text} (zwart): {zwart}";

        // Herteken het bord op basis van de nieuwe tabel
        plaatje = tekenHulp.IntArrayToBitmap(spelBord.Tabel);
        afbeelding.Image = plaatje;
        afbeelding.Refresh();

        // Teken de mogelijke zetten voor de nieuwe beurt
        if (ondersteuning)
        {
            tekenMogelijkeStenen();
            afbeelding.Refresh();
        }

        UpdateBeurt();

        // Controleer op winst
        ControleerWinst();

    }

    // Vergelijk het aantal stenen en laat de winnaar zien
    private void ControleerWinst()
    {
        if (spelBord.Winnen())
        {
            (int wit, int zwart) = spelBord.TelScore();

            string winnaar;
            if (wit > zwart)
                winnaar = speler1.Text + " (wit)";
            else if (zwart > wit)
                winnaar = speler2.Text + " (zwart)";
            else
                winnaar = "Remise!";

            MessageBox.Show($"Spel afgelopen!\n Wit: {wit}\n Zwart: {zwart}\n Winnaar: {winnaar}");
        }
    }

    // Hoofdmethode om het spel te starten
    public void nieuwSpel()
    {
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

        // De tabel omzetten naar een bitmap
        plaatje = tekenHulp.IntArrayToBitmap(spelBord.Tabel);
        afbeelding.Image = plaatje;

        // Uitleg over het spel, klik op OK om verder te gaan
        MessageBox.Show("Speler 1 is wit en begint het spel, speler 2 is zwart. Druk op OK om de reversi te starten");

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

        score.Location = new Point(750, 300);
        score.Size = new Size(200, 100);
        score.Width = 200;
        score.Font = fontKlein;

        (int wit, int zwart) = spelBord.TelScore();
        score.Text = $"Score:\n {speler1.Text} (wit): {wit}\n {speler2.Text} (zwart): {zwart}";

        aandebeurt = new Label();
        aandebeurt.Location = new Point(750, 200);
        aandebeurt.Size = new Size(200, 100);
        aandebeurt.Width = 250;
        aandebeurt.Font = fontKlein;

        scherm.Controls.Add(titel);
        scherm.Controls.Add(uitleg);
        scherm.Controls.Add(spelers);
        scherm.Controls.Add(aandebeurt);
        scherm.Controls.Add(score);

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
        help.Text = "HELP! (Mogelijke Zetten)";
        help.Location = new Point(750, 550);
        help.Size = new Size(225, 40);
        help.BackColor = Color.LightGreen;
        scherm.Controls.Add(help);

        Button nieuwSpelKnop = new Button();
        nieuwSpelKnop.Text = "NIEUW SPEL";
        nieuwSpelKnop.Location = new Point(750, 600);
        nieuwSpelKnop.Size = new Size(100, 75);
        nieuwSpelKnop.BackColor = Color.LightGreen;
        scherm.Controls.Add(nieuwSpelKnop);

        Button beurtOverslaan = new Button();
        beurtOverslaan.Text = "PASS";
        beurtOverslaan.Location = new Point(850, 600);
        beurtOverslaan.Size = new Size(75, 75);
        beurtOverslaan.BackColor = Color.LightGreen;
        scherm.Controls.Add(beurtOverslaan);

        //-----------------Eventhandlers----------------------

        // 4x4 bord
        void kiesAfmeting4(object o, EventArgs ea)
        {
            spelBord.PasAfmetingAan(0);
            beurtResetEnTeken();
        }
        afmeting4.Click += kiesAfmeting4;

        // 6x6 bord
        void kiesAfmeting6(object o, EventArgs ea)
        {
            spelBord.PasAfmetingAan(1);
            beurtResetEnTeken();
        }
        afmeting6.Click += kiesAfmeting6;

        // 8x8 bord
        void kiesAfmeting8(object o, EventArgs ea)
        {
            spelBord.PasAfmetingAan(2);
            beurtResetEnTeken();
        }
        afmeting8.Click += kiesAfmeting8;

        // 10x10 bord
        void kiesAfmeting10(object o, EventArgs ea)
        {
            spelBord.PasAfmetingAan(3);
            beurtResetEnTeken();
        }
        afmeting10.Click += kiesAfmeting10;

        // Reset methode voor afmeting knoppen en nieuw spel
        void beurtResetEnTeken()
        {
            spelBord.Beurt = 1;
            plaatje = tekenHulp.IntArrayToBitmap(spelBord.Tabel);
            afbeelding.Image = plaatje;

            if (ondersteuning)
            {
                tekenMogelijkeStenen();
                afbeelding.Refresh();
            }
            UpdateBeurt();
        }

        // Muis eventhandler
        void muisKlik(object o, MouseEventArgs mea)
        {
            int x = mea.X;
            int y = mea.Y;
            tekenStenen(x, y, spelBord.Beurt);
        }
        afbeelding.MouseClick += muisKlik;

        // Help eventhandler
        void helpKnop(object o, EventArgs ea)
        {
            ondersteuning = !ondersteuning; // draai dus de waarde om
            if (ondersteuning)
            {
                tekenMogelijkeStenen();
                afbeelding.Refresh();
            }
            else
            {
                // Herteken bord zonder mogelijke zetten
                plaatje = tekenHulp.IntArrayToBitmap(spelBord.Tabel);
                afbeelding.Image = plaatje;
                afbeelding.Refresh();
            }
        }
        help.Click += helpKnop;

        // Nieuw spel eventhandler
        void nieuwSpelKnop_Click(object o, EventArgs ea)
        {
            // Reset het spelbord naar de standaard 6x6 en herteken
            spelBord.PasAfmetingAan(1);
            beurtResetEnTeken();
        }
        nieuwSpelKnop.Click += nieuwSpelKnop_Click;

        // Beurt overslaan eventhandler
        void beurtOverslaanKnop(object o, EventArgs ea)
        {
            spelBord.FlipBeurt();

            if (spelBord.Winnen())
            {
                ControleerWinst();
            }
            else
            {
                if (ondersteuning)
                {
                    tekenMogelijkeStenen();
                    afbeelding.Refresh();
                }
                UpdateBeurt();
            }
        }
        beurtOverslaan.Click += beurtOverslaanKnop;

        tekenMogelijkeStenen(); // Teken de startstenen en mogelijke zetten
        UpdateBeurt();

        if (spelBord.Winnen())
        {
            UpdateBeurt();
            ControleerWinst();
        }
        else
        {
            UpdateBeurt();
        }


        Application.Run(scherm);
    }
}
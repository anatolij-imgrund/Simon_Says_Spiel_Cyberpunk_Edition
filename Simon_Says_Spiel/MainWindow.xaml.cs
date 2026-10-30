using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Media;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Simon_Says_Spiel
{
    public partial class MainWindow : Window
    {
        static int n = 1; // die Länge von der Reihenfolge
        int player_1_click; // wie viel Mal hat Spieler 1 Taste gedrückt...
        int player_2_click; // ... und Spieler 2
        string[] colors = { "Rot", "Grün", "Blau", "Gelb" }; // Array mit Farben für randome Auswahl
        // List ist wie ein Array nur bissl praktische)
        List<string> reihenfolge = new(); // Liste wo die ausgewählte Farben gespeichert werden
        bool player_1_won = true; // ob Spieler 1 im Spiel noch ("false" wird beim Fehler gestellt un damit Spiel für ihn zur ende...) ...
        bool player_2_won = true; // ... und Spieler 2
        bool is_input_1_free; // ob Spieler noch Tasten drücken darf...
        bool is_input_2_free; // ... und Spieler 2
        int player_1_points; // Punkte von Spieler 1 werden beim Gewinn erhöht (noch nicht programmiert, werden nicht angezeigt)...
        int player_2_points; // ... und Spieler 2
        bool is_erste = true;

        SoundPlayer soundPlayer = new SoundPlayer();      
        
        bool mute = false; // beim Start wird Stummmodus ausgeschaltet... also man muss Background Music hören

        public MainWindow() // Hauptfenster von Programm. Wird autimatisch von WPF erstellt. Wenn ich richtig verstanden habe alle Befehle, die drinn stehen werden beim Start ausgeführt
        {
            InitializeComponent(); // wie Name sagt. Wird Kunstwerk aus MainWindow.xaml auf dem Bildschirm "gezeichnet"

            Background_Music(); // Spielt Music

            this.KeyDown += PlayButton; // denn beim Programstart "Start Button" ist aktiv, wird mit dem Befehl eine Leertastebetätigung erwartet (nicht notwendig, kann man mit dem Maus drauf clicken)
        }

        void PlayButton(object sender, KeyEventArgs e) // "Start Button" aktivierung mit Leertaste
        {
            if(start_btn.IsEnabled == true) // wenn Button aktiv ist...
            {
                if (e.Key == Key.Space) // wenn Leertaste gedrückt...
                { 
                    Button_Click(null, null); // aktivieren Funktion von dem "Start Button"
                    this.KeyDown -= PlayButton;
                }
            } // sonst wird auf Leertaste nicht reagiert
        }

        void Background_Music() // es ist klar :)
        {
            soundPlayer.SoundLocation = "cyberpunk_neon.wav";
            if (!mute) // wenn Stummmodus aus...
            {
                soundPlayer.PlayLooping(); // wird Music ohne ende (looping) gespielt
            }
            else // sonst...
            {
                soundPlayer.Stop(); // Music bleibt stehen)
            }
        }

        private void sound_btn_Click(object sender, RoutedEventArgs e) // betätigung des Lautsprecherbuttons im Eckchen des Programmfensters 
        {
            if (mute) // wenn Stummmodus ist aus... 
            {
                mute = false; // wird "Stumm" ein...
                Background_Music(); // play/stop ? stop
            }
            else // sonst...
            {
                mute = true; // "Stumm" aus...
                Background_Music(); // play/stop ? play
            }
        }

        private void close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        async private void Button_Click(object sender, RoutedEventArgs e) // betätigung von "Start Button"
        {
            start_btn.IsEnabled = false; // "Start Button" deaktivieren (wird nicht auf clicks und Leertaste reagieren)
            text_block_player_1.Text = "";
            text_block_player_2.Text = "";
            level_block.Text = $"Level: {n}";            
            await Countdown();
            player_1_click = 0; // clicks von Spieler 1 auf 0
            player_2_click = 0; // und Spieler 2 auch
            if(n == 1)
            {
                player_1_points = 0;
                player_2_points = 0;
            }

            // Neue Reihenfolge generieren und anzeigen //
            Reihenfolge_generieren();
            await Show_Reihenfolge();
            //  //  //  //  //  //  //  //  //  //  //  //

            text_block.Text = "Ihr seid dran!"; // ändert Text damit Spieler wissen, wann sie spelen dürfen :)
            is_input_1_free = true; // Eingabe mit Tasten für Spieler 1 freigeben...
            is_input_2_free = true; // und für Spieler 2 auch

            // komische Moment, selbst nicht ganz verstehe wie functioniert, aber functioniert :)
            // this. sagt, dass wir mit diesem Program (Fenster) arbeiten
            // KeyDown ist eine... hmmm...Methode?? die für Tasteneingabe verantwortlich
            // mit += wird zur KeyDown gesagt, dass wir eingabe für Function Player_(1/2)_KeyDown brauchen
            // also Programm wird eine Tasteneingabe erwarten.....
            this.KeyDown += Player_1_KeyDown; // Spieler 1 darf Tasten nutzen...
            this.KeyDown += Player_2_KeyDown; // Spieler 2 auch :)
        }

        async private Task Countdown()
        {
            text_block.Visibility = Visibility.Hidden;
            text_block_player_1.Visibility = Visibility.Hidden;
            text_block_player_2.Visibility = Visibility.Hidden;
            countdown_block.Visibility = Visibility.Visible;
            await Task.Delay(700);
            countdown_block.Text = "2";
            await Task.Delay(700);
            countdown_block.Text = "1";
            await Task.Delay(700);
            countdown_block.Text = "";
            countdown_block.Visibility = Visibility.Hidden;
            countdown_block.Text = "3";
            text_block.Visibility = Visibility.Visible;
            text_block_player_1.Visibility = Visibility.Visible;
            text_block_player_2.Visibility = Visibility.Visible;
            text_block.Text = "AI ist dran!";
            await Task.Delay(700);
        }

        private void Reihenfolge_generieren() // Was könnte das sein? :D
        {
            // Die Liste wird gelöscht und mit neuen Elementen gefüllt //
            reihenfolge.Clear();
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                reihenfolge.Add(colors[random.Next(colors.Length)]); // zur Liste wird ein randome Element aus Array mit Farben hinzugefügt... n Mal
            }
            //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //
        }

        async private Task Show_Reihenfolge() // ok...
        {
            // Die gespeicherte Liste wird angezeigt //
            foreach (var color in reihenfolge) // für jedes Element in der frischgeborene Liste...
            {
                switch (color) // vergleichen dieses Element mit...
                {
                    case "Rot":
                        r_h.Visibility = Visibility.Visible; // r_h (für red hell) Bild wird auf dem Fenster des Programms gezeigt... als ob es leuchtet...
                        await Task.Delay(500); // ... aber nur eine Halbe Secunde (kann man mit dem auch beim programmieren spielen)))...
                        r_h.Visibility = Visibility.Hidden; // ... wieder versteckt ...
                        await Task.Delay(500); // ... und hier brauchen die Augen auch Pause... sonst sieht es unangenehm :)
                        break;

                    case "Grün": // ---//---
                        g_h.Visibility = Visibility.Visible;
                        await Task.Delay(500);
                        g_h.Visibility = Visibility.Hidden;
                        await Task.Delay(500);
                        break;

                    case "Blau": // ---//---
                        b_h.Visibility = Visibility.Visible;
                        await Task.Delay(500);
                        b_h.Visibility = Visibility.Hidden;
                        await Task.Delay(500);
                        break;

                    case "Gelb": // ---//---
                        y_h.Visibility = Visibility.Visible;
                        await Task.Delay(500);
                        y_h.Visibility = Visibility.Hidden;
                        await Task.Delay(500);
                        break;
                }
            }
            //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //
        }

        async void Player_1_KeyDown(object sender, KeyEventArgs e) // Befehle beim Tastenbetätigung von Spieler 1
        {
            // player hat so viel Versuchen die Tasten zu tippen wie die Länge der Liste und wenn Eingabe freigeschaltet//
            if (player_1_click < reihenfolge.Count && is_input_1_free)
            {              
                string player_1_choice; // variable Platzhalter für eingabe
                switch (e.Key) // vergleichen gedrückte Taste mit...
                {
                    case Key.W: // W Taste...
                        player_1_choice = "Grün";
                        g_h_player_1.Visibility = Visibility.Visible; // b_h_player_1 (für blue hell) Bild wird auf dem Fenster des Programms gezeigt... als ob es leuchtet...
                        this.KeyDown -= Player_1_KeyDown; // Tasteneingabe aus, um mit kurze blink die gewählte Farbe zeigen und gleichzeitig Range-fehler zu vermeiden... 
                        await Task.Delay(200); // 0,2 Secunde reicht's...
                        g_h_player_1.Visibility = Visibility.Hidden; // ... wieder ausblenden...
                        await Task.Delay(50); // ... und eine sehr wichtige Pause, um Fehler zu vermeiden bei langgedrückte Taste
                        this.KeyDown += Player_1_KeyDown; // wieder eingabe aktivieren
                        break;
                    case Key.A: // ---//---
                        player_1_choice = "Blau";
                        b_h_player_1.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_1_KeyDown;
                        await Task.Delay(200);
                        b_h_player_1.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_1_KeyDown;
                        break;
                    case Key.S: // ---//---
                        player_1_choice = "Gelb";
                        y_h_player_1.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_1_KeyDown;
                        await Task.Delay(200);
                        y_h_player_1.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_1_KeyDown;
                        break;
                    case Key.D: // ---//---
                        player_1_choice = "Rot";
                        r_h_player_1.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_1_KeyDown;
                        await Task.Delay(200);
                        r_h_player_1.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_1_KeyDown;
                        break;
                    default: // eigentlich sind nicht nötig aber stört nicht... falls wurde eine nicht erwartete Taste gedrückt, setzt variable auf "null" und die weitere berbeitung wird nicht möglich... wird auf nächste Taste gewartet...
                        player_1_choice = null;
                        break;
                }

                // wenn Wert gespeichert wurde, prüft ob Player richtige Farbe getippt
                if (player_1_choice != null)
                {
                    Win_Lose_1(player_1_choice, player_1_click);
                    player_1_click++; // und click-zähler wir um 1 erhöht (braucht man um mit List-länge (n) zu vergleichen)
                }
            }

            // Wenn es keine Versuche mehr gibt 
            if (player_1_click == reihenfolge.Count && is_input_1_free)
            {
                if (is_erste)
                {
                    player_1_points += 100;
                    is_erste = false;
                }
                else
                {
                    is_erste = true;
                }
                // ..... mit -= wird keine Tasteneingabe mehr erwartet :)
                this.KeyDown -= Player_1_KeyDown;
                Reset_1(); // Ergebnis der Runde zeigen und Spiel entweder mit erhöhte List-länge (beide Spieler sind richtig) oder von Anfang an (ein oder beide Speler ist/sind falsch) neu starten
            }
        }

        async void Player_2_KeyDown(object sender, KeyEventArgs e) // sehe Player_1_KeyDown oben...
        {
            if (player_2_click < reihenfolge.Count && is_input_2_free)
            {               
                string player_2_choice;
                switch (e.Key)
                {
                    case Key.Up:
                        player_2_choice = "Grün";
                        g_h_player_2.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_2_KeyDown;
                        await Task.Delay(200);
                        g_h_player_2.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_2_KeyDown;
                        break;
                    case Key.Left:
                        player_2_choice = "Blau";
                        b_h_player_2.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_2_KeyDown;
                        await Task.Delay(200);
                        b_h_player_2.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_2_KeyDown;
                        break;
                    case Key.Down:
                        player_2_choice = "Gelb";
                        y_h_player_2.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_2_KeyDown;
                        await Task.Delay(200);
                        y_h_player_2.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_2_KeyDown;
                        break;
                    case Key.Right:
                        player_2_choice = "Rot";
                        r_h_player_2.Visibility = Visibility.Visible;
                        this.KeyDown -= Player_2_KeyDown;
                        await Task.Delay(200);
                        r_h_player_2.Visibility = Visibility.Hidden;
                        await Task.Delay(50);
                        this.KeyDown += Player_2_KeyDown;
                        break;
                    default:
                        player_2_choice = null;
                        break;
                }

                if (player_2_choice != null)
                {
                    Win_Lose_2(player_2_choice, player_2_click);
                    player_2_click++;
                }
            }

            if (player_2_click == reihenfolge.Count && is_input_2_free)
            {
                if (is_erste)
                {
                    player_2_points += 100;
                    is_erste = false;
                }
                else
                {
                    is_erste = true;
                }
                this.KeyDown -= Player_2_KeyDown;
                Reset_2();
            }
        }

        public void Reset_1() // Reset für Spieler 1
        {
            if (player_1_won) // wenn Spieler alle Farben richtig gedrückt...
            {
                is_input_1_free = false; // Tasteneingabe aus (braucht man für GameRestart())
            }
            else // sonst...
            {
                this.KeyDown -= Player_1_KeyDown; // hmmm... ob man das hier noch braucht? =) 
                is_input_1_free = false; // Tasteneingabe aus
            }
            GameRestart(); // wenn Runde ist zu ende wird nächste oder neue gestartet
        }

        public void Reset_2() // sehe oben Reset_1
        {
            if (player_2_won)
            {                
                is_input_2_free = false;
            }
            else
            {
                this.KeyDown -= Player_2_KeyDown;
                is_input_2_free = false;
            }
            GameRestart();
        }

        public void GameRestart() 
        {            
            if (!is_input_1_free && !is_input_2_free) // wenn eingabe von beide Spieler ist aus (damit berechnung nicht dann beginnt wenn nur ein Spieler fertig ist )))...
            {
                if (player_1_won && player_2_won) // ... wenn beide Spieler richtig geantwortet...
                { 
                    text_block.Text = "Es geht weiter!"; // wird es im Haupt-Textblock gezeigt...
                    n++; // ... n um 1 erhöht...
                }
                else if ((player_1_won && !player_2_won) || (!player_1_won && player_2_won) || (!player_1_won && !player_2_won)) // ... oder ein oder beide falsch...
                {
                    if (player_1_points > player_2_points)
                    {
                        text_block.Text = $"Player 1 gewinnt!";
                        text_block_player_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF03"));
                        text_block_player_1.Text = $"{player_1_points} Punkte"; // wird es im Haupt-Textblock gezeigt...
                        text_block_player_2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF0909")); // wird es im Textblock von Spieler gezeigt...
                        text_block_player_2.Text = $"{player_2_points} Punkte";
                    }
                    else if (player_1_points < player_2_points)
                    {
                        text_block.Text = $"Player 2 gewinnt!";
                        text_block_player_2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF03"));
                        text_block_player_2.Text = $"{player_2_points} Punkte"; // wird es im Haupt-Textblock gezeigt...
                        text_block_player_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF0909")); // wird es im Textblock von Spieler gezeigt...
                        text_block_player_1.Text = $"{player_1_points} Punkte";
                    }
                    else
                    {
                        text_block.Text = $"Unentschieden!";
                        text_block_player_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF03"));
                        text_block_player_1.Text = $"{player_1_points} Punkte";
                        text_block_player_2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF03"));
                        text_block_player_2.Text = $"{player_2_points} Punkte";
                    }                     
                    n = 1; // ... n auf 1 gesetzt damit Spiel von vorn mit List-länge 1 beginnt...
                    reihenfolge = new(); // ... und alte Liste gelöscht und neu erstellt
                }

                start_btn.IsEnabled = true; // "Start Buttom" wieder aktiviert
                player_1_click = 0; // Spiler 1 clicks auf 0
                player_2_click = 0; // Spieler 2 auch
                is_input_1_free = false; // Tastatureingabe für Spieler 1 deaktiviert
                is_input_2_free = false; // Spieler 2 auch
                player_1_won = true; // Gewinn-schalter für Spieler 1 aktiviert
                player_2_won = true; // Spieler 2 auch
                this.KeyDown += PlayButton;
            }
        }

        private void Win_Lose_1(string wahl, int index) 
        {
            // Anzeige ob Player richtige oder falsche Element geraten //
            if (player_1_won) // wenn "Gewinn-schalter" ist "ein"...
            {
                if (wahl == reihenfolge[index]) // ... wenn ausgewählte Farbe ist wie von PC ausgewählte...
                {
                    
                    text_block_player_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF03")); // wird es im Textblock von Spieler gezeigt...
                    text_block_player_1.Text = "Richtig";
                    player_1_won = true; // Gewinn-schalter bleibt "ein" (evtl. braucht man das hier nicht... kann man löschen)))
                    player_1_points += 100; // + 1 Punkt
                }
                else // sonst...
                {
                    text_block_player_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF0909")); // wird es im Textblock von Spieler gezeigt...
                    text_block_player_1.Text = "Falsch";
                    player_1_won = false; // Gewinn-schalter wird "aus"
                    Reset_1(); // wird Reset mit alle Folgen gerufen...
                }
            }
            //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //
        }

        private void Win_Lose_2(string wahl, int index) // sehe oben Win_Lose_1
        {
            if (player_2_won)
            {
                if (wahl == reihenfolge[index])
                {
                    text_block_player_2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF03"));
                    text_block_player_2.Text = "Richtig";
                    player_2_won = true;
                    player_2_points += 100;
                }
                else
                {
                    text_block_player_2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF0909"));
                    text_block_player_2.Text = "Falsch";
                    player_2_won = false;
                    Reset_2();
                }
            }
        }
    }
}
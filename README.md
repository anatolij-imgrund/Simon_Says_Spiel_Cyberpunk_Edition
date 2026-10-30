# Simon Says – Cyberpunk Edition

Ein Merkspiel nach dem Vorbild des Klassikers *Simon Says* – im Neon-Look und für **zwei Spieler gleichzeitig an einer Tastatur**.

Eine KI zeigt eine Reihenfolge aufleuchtender Farben. Beide Spieler müssen sie so schnell und genau wie möglich nachtippen. Schaffen es beide, wird die Reihenfolge um eine Farbe länger. Sobald sich jemand vertippt, ist die Runde vorbei und die Punkte entscheiden, wer gewinnt.

Entstanden als Teamprojekt im 2. Semester der Ausbildung zum Fachinformatiker für Anwendungsentwicklung.

## Spielablauf

1. Mit **Leertaste** (oder Klick auf den Play-Button) starten.
2. Countdown 3 – 2 – 1, dann ist die KI dran und lässt die Farben nacheinander aufleuchten.
3. „Ihr seid dran!“ – beide Spieler tippen die Reihenfolge gleichzeitig nach.
4. Haben beide alles richtig, geht es mit einem Level mehr weiter (die Reihenfolge wird um eine Farbe länger).
5. Vertippt sich ein Spieler, endet das Spiel und der Gewinner wird angezeigt.

### Steuerung

| Farbe | Spieler 1 | Spieler 2 |
|---|---|---|
| 🟢 Grün | `W` | `↑` |
| 🔵 Blau | `A` | `←` |
| 🟡 Gelb | `S` | `↓` |
| 🔴 Rot | `D` | `→` |

### Punkte

- Jede richtig getippte Farbe: **100 Punkte**
- Wer eine Runde als Erster fehlerfrei abschließt, bekommt **100 Bonuspunkte** – Tempo lohnt sich also
- Am Ende gewinnt, wer mehr Punkte hat (oder es steht unentschieden)

## Weitere Details

- Neon-Oberfläche im Cyberpunk-Stil mit eigenem, randlosem Fenster (zum Verschieben ins Fenster klicken und ziehen)
- Hintergrundmusik in Dauerschleife, über den Lautsprecher-Button stumm schaltbar
- Levelanzeige und Rückmeldung „Richtig“ / „Falsch“ für jeden Spieler

## Technik

| Bereich | Umsetzung |
|---|---|
| Sprache / Plattform | C#, .NET 8 (nur Windows) |
| Oberfläche | WPF (XAML) |
| Ablauf | `async`/`await` mit `Task.Delay` für das Aufleuchten der Farben und den Countdown |
| Ton | `System.Media.SoundPlayer` |

## Starten

Voraussetzungen: Windows und das [.NET 8 SDK](https://dotnet.microsoft.com/download)

Am einfachsten: `Simon_Says_Spiel.sln` in Visual Studio öffnen und mit **F5** starten.

Oder über die Kommandozeile bauen und das Spiel aus dem Ausgabeordner starten:

```powershell
git clone https://github.com/anatolij-imgrund/Simon_Says_Spiel_Cyberpunk_Edition.git
cd Simon_Says_Spiel_Cyberpunk_Edition
dotnet build
cd Simon_Says_Spiel\bin\Debug\net8.0-windows
.\Simon_Says_Spiel.exe
```

> Die Musikdatei `cyberpunk_neon.wav` wird im aktuellen Arbeitsordner gesucht – deshalb das Spiel aus dem Ausgabeordner `bin\Debug\net8.0-windows\` starten, wo die Datei liegt.

## Quellen für Grafiken und Ton

Die Grafiken und die Hintergrundmusik stammen aus frei verfügbaren Sammlungen im Internet. Die genauen Quellen werden hier ergänzt.

| Datei(en) | Quelle | Lizenz |
|---|---|---|
| Hintergrundbild (`assets/background.jpg`) | *wird ergänzt* | *wird ergänzt* |
| Rahmen, Tasten und Symbole (`assets/*.png`) | *wird ergänzt* | *wird ergänzt* |
| KI-Figur (`assets/robo_ai.png`) | *wird ergänzt* | *wird ergänzt* |
| Hintergrundmusik (`cyberpunk_neon.wav`) | *wird ergänzt* | *wird ergänzt* |

Falls du Urheber eines dieser Werke bist: Bitte melde dich über ein [Issue](https://github.com/anatolij-imgrund/Simon_Says_Spiel_Cyberpunk_Edition/issues). Ich ergänze die Quellenangabe dann sofort oder entferne die Datei auf Wunsch.

## Hinweis

Dies ist ein nicht-kommerzielles Lernprojekt aus der Ausbildung.

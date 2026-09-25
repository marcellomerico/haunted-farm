# 📓 Lernnotizen – Haunted Farm

Meine Notizen beim Lernen von **C#** und **Godot 4** mit diesem Projekt.
Pro Meilenstein ein Abschnitt: was ich gelernt habe, welche Fehler ich gemacht habe und was ich daraus mitnehme.

💼 = Wo mir das im Berufsalltag als C#-Entwickler begegnet

## Inhalt

- [Meilenstein 1: Spielfigur & Bewegung](#meilenstein-1-spielfigur--bewegung)
- [Meilenstein 2: Tilemap & Farm-Szene](#meilenstein-2-tilemap--farm-szene)

---

## Meilenstein 1: Spielfigur & Bewegung

**Ergebnis:** Spielfigur läuft mit WASD/Pfeiltasten in vier Richtungen, spielt die passende Laufanimation ab und bleibt beim Loslassen stehen.

### Godot-Konzepte

| Konzept | Was es ist / wofür |
|---|---|
| `CharacterBody2D` | Node für selbst gesteuerte Figuren. Bringt `Velocity` (Pixel/Sekunde als `Vector2`) und `MoveAndSlide()` mit, das bewegt und Kollisionen behandelt. Alternativen: `Node2D`/`Sprite2D` (keine Kollision), `RigidBody2D` (komplett physikgesteuert). |
| `_Ready()` | Läuft einmal, wenn der Node in der Szene ist. Setup, z. B. Referenzen holen. |
| `_Process(double delta)` | Läuft pro gezeichnetem Frame (schwankt). Für Visuelles. |
| `_PhysicsProcess(double delta)` | Läuft fest 60×/Sekunde. Für Bewegung und Kollision. |
| `delta` | Zeit seit dem letzten Aufruf in Sekunden. |
| `MoveAndSlide()` | Berücksichtigt `delta` intern → **nicht** selbst mit `delta` multiplizieren. |
| Input Map | Projekteinstellungen → Eingabe-Zuordnung. Aktionen (z. B. `move_left`) mit mehreren Tasten. Code fragt die Aktion ab, nicht die Taste. |
| `Input.GetVector(l, r, o, u)` | Gibt die Richtung als `Vector2` zurück, bereits normalisiert (diagonal ≈ 0.71) → diagonal nicht schneller. |
| Koordinaten | Y zeigt **nach unten**. Hoch = negatives Y. |
| `AnimatedSprite2D.Play("name")` | Gleiche Animation nochmal aufrufen → läuft weiter. Wechsel auf eine andere → startet bei Frame 0. |
| `AnimatedSprite2D.Stop()` | Hält an und springt auf Frame 0. |
| `GetNode<T>("Pfad")` | Referenz auf einen Node holen. Einmal in `_Ready()` holen, in einem Feld speichern, nicht jeden Frame suchen. |

### C#-Konzepte

| Konzept | Beispiel | Erklärung |
|---|---|---|
| Namespace | `using Godot;` | Bindet einen „Ordner“ von Klassen ein. 💼 In jeder C#-Datei. |
| `partial class` | `public partial class Player` | Klasse über mehrere Dateien verteilt. Bei Godot Pflicht (Source Generator erzeugt den zweiten Teil). Klassenname = Dateiname! |
| Vererbung | `Player : CharacterBody2D` | Player *ist ein* CharacterBody2D und erbt alles. 💼 ASP.NET-Controller erben von `ControllerBase`. |
| `override` | `public override void _Ready()` | Methode der Elternklasse ersetzen. Tipp: `override` + Leertaste in VS Code zeigt alle überschreibbaren Methoden. |
| Property | `public float Speed { get; set; } = 80f;` | Sieht aus wie eine Variable, intern Getter/Setter. 💼 Standard in jeder Datenklasse. |
| Attribut | `[Export]` | Etikett mit Zusatzinfos. `[Export]` zeigt den Wert im Inspector. 💼 `[HttpGet]`, `[Required]` in ASP.NET. |
| Generics | `GetNode<AnimatedSprite2D>(...)` | Typparameter in spitzen Klammern. 💼 `List<string>`, `Dictionary<int, User>`. |
| Struct vs. Class | `Vector2`, `int`, `DateTime` | Struct wird beim Weitergeben **kopiert**, Class als **Referenz** übergeben. 💼 Beliebte Interviewfrage! |
| Operator Overloading | `direction * Speed` | `*` ist für `Vector2` definiert und multipliziert beide Komponenten. |
| `float` vs. `double` | `80f` | `float` ist ungenauer, reicht für Spiele. Ohne `f` ist eine Kommazahl ein `double`. |
| `if` / `else if` / `else` | | In einer Kette gewinnt **nur der erste Treffer**. Ein neues `if` startet eine **unabhängige** Abfrage. |

**Namenskonventionen** (Microsoft-Standard):

- `PascalCase` → Klassen, Methoden, Properties (`Speed`, `MoveAndSlide`)
- `camelCase` → lokale Variablen (`direction`)
- `_camelCase` → private Felder (`_sprite`)
- Modifier-Reihenfolge: `public override void`

### Fehler, die ich gemacht habe

1. **Falscher Aktionsname:** `"ui_left"` statt `"move_left"` → WASD ging nicht, nur Pfeiltasten.
   *Lektion:* Strings als Bezeichner werden vom Compiler nicht geprüft. Später Konstanten nutzen.
2. **Vergessenes `else`:** Zwei getrennte `if`-Ketten für X und Y. Bei Diagonalen trafen beide zu, zwei verschiedene `Play()`-Aufrufe pro Frame haben sich gegenseitig auf Frame 0 zurückgesetzt, und die Figur ist eingefroren.
   *Lektion:* Randfälle testen. Bei reinen Richtungen fiel der Fehler nicht auf.

### Debugging-Werkzeuge

- **Print-Debugging:** `GD.Print("zweig xy")` in jeden Zweig und schauen, welcher wirklich läuft.
- **Erst Ausgabe/Logs lesen, dann raten.**
- **Formatieren** (Shift+Option+F in VS Code): Saubere Einrückung zeigt, welches `else` zu welchem `if` gehört.

### Prinzipien

- **Magic Numbers vermeiden:** Werte benennen (`Speed`) statt nackter Zahlen.
- **YAGNI** (*You Aren't Gonna Need It*): Keine Zweige/Features für Fälle bauen, die schon abgedeckt sind.
- **Einfach schlägt clever**, solange es funktioniert.
- **Kleine Commits** nach jedem funktionierenden Schritt.

---

## Meilenstein 2: Tilemap & Farm-Szene

**Ergebnis:** Eigene Farm-Szene im Herbst-Look (Gras, Teich, Zäune, Deko), Spielfigur läuft darüber, Kamera folgt. `Farm.tscn` ist die Hauptszene.

### Godot-Konzepte

| Konzept | Was es ist / wofür |
|---|---|
| Tile | Kleines quadratisches Grafikstück (bei uns 16×16 px). Welten werden daraus zusammengesetzt wie Lego. |
| TileSet | Die „Palette“: Tileset-Bild + Tile-Größe. Godot zerschneidet das Bild in Tiles. Hier werden später auch Kollisionen definiert. |
| `TileMapLayer` | Der Node, auf den man mit dem TileSet malt. Der alte `TileMap`-Node ist seit Godot 4.3 **veraltet** → Achtung bei älteren Tutorials. |
| Mehrere Layer | `Ground` (Boden, Wasser), `Soil` (umgegrabene Erde, wird später per Code verändert), `Decoration` (Blumen, Steine). |
| Zeichenreihenfolge | Im Szenenbaum wird von oben nach unten gezeichnet → was **weiter unten** steht, liegt optisch **drüber**. |
| Resource (`.tres`) | Datenobjekt als Datei, das mehrere Nodes **gemeinsam** nutzen. Alle drei Layer nutzen dieselbe `farm_tileset.tres` → Änderungen gelten überall. |
| Instanz | Eine fertige Szene (z. B. `Player.tscn`) in eine andere ziehen. Bauplan bleibt die Original-Szene, wie Objekt und Klasse. |
| Kind-Nodes | Position ist **relativ** zum Eltern-Node. `Camera2D` als Kind vom `Player` folgt automatisch, ohne Code. |
| Hauptszene | Projekteinstellungen → Anwendung → Ausführen. **F5** startet die Hauptszene, **F6** die gerade offene. |
| Export-Werte bei Instanzen | Eine Instanz kann `[Export]`-Werte überschreiben. Erkennbar am kleinen Rückgängig-Pfeil im Inspector. |

**Mal-Werkzeuge** (Reiter TileMap unten):

| Taste | Werkzeug | Wofür |
|---|---|---|
| D | Stift | Einzelne Tiles |
| R | Rechteck | Flächen füllen |
| L | Linie | Zäune, Wege |
| B | Eimer | Zusammenhängende Fläche füllen |
| P | Pipette | Tile aus der Karte aufnehmen |
| E | Radierer | Löschen (oder Rechtsklick beim Malen) |

💡 Mehrere Tiles mit **Shift** auswählen + **Würfel-Symbol** → zufällige Verteilung, wirkt natürlicher.

### Tilemap oder eigene Szene?

| Tilemap | Eigene Szene |
|---|---|
| Flach, Spieler läuft drüber oder daneben | Hat Höhe, Spieler kann **dahinter** stehen (→ Y-Sorting) |
| Kein Verhalten | Tut etwas (wackeln, wachsen, geöffnet werden) |
| Gras, Wege, Wasser, Blumen, kleine Steine, Zäune | Bäume, Häuser, Truhen, Pflanzen (Bank vorerst als Platzhalter-Tile) |

**Y-Sorting:** Was weiter unten auf dem Bildschirm steht, wird drübergezeichnet. So verdeckt eine Baumkrone die Figur, wenn sie dahinter steht.

### Assets

- `tiles.png`: 864×800 px, 16×16-Tiles, vier Jahreszeiten nebeneinander (Sommer, Frühling, **Herbst** ← nutze ich, Winter). Innerhalb einer Jahreszeit bleiben, sonst passen die Farben nicht.
- `tree_shake.png`: 4 Spalten × 8 Zeilen à 32×32 → 8 Baumtypen mit je 4 Frames Wackel-Animation. Wird später eine eigene Baum-Szene.

### Szenen-Struktur

```
Farm (Node2D)              ← Farm.tscn = Hauptszene
├── Ground (TileMapLayer)
├── Soil (TileMapLayer)
├── Decoration (TileMapLayer)
└── Player (Instanz von Player.tscn)
    ├── AnimatedSprite2D
    └── Camera2D
```

Dorf, Dungeon und später das Startmenü werden **eigene Szenen** nach demselben Muster.

### Fehler, die ich gemacht habe

1. **Farm mit „Editierbaren Kindern“ in `Main` bearbeitet.** Änderungen landen dann womöglich in `main.tscn` statt in `Farm.tscn`. Löschen der Instanz hätte Arbeit vernichten können.
   *Rettung:* Rechtsklick → **Lokal machen** → **Zweig als Szene speichern**.
2. **`Camera2D` unter `AnimatedSprite2D` statt unter `Player`.** Funktioniert, ist aber unsauber: Die Kamera gehört zum Spieler, nicht zur Grafik.
3. **Input-Aktionen plötzlich weg.** Die Input Map steckt in `project.godot`, die Datei war auf einem alten Stand.
   *Lektion:* Mit `git diff project.godot` nachsehen, was sich geändert hat.

### Git

| Befehl / Praxis | Wofür |
|---|---|
| `git status` | Vor jedem Commit: Was wird eigentlich committet? |
| `git diff <datei>` | Was hat sich seit dem letzten Commit geändert? |
| `wip:`-Commit | *Work in progress*, Sicherung vor riskanten Umbauten |
| Atomare Commits | Ein Commit = eine logische Änderung (`feat`, `refactor`, `fix` getrennt). Gezielt mit `git add <datei>`. 💼 Leichtere Reviews, gezieltes `git revert`. |
| Dateien im **Godot**-Dateisystem löschen | Nicht im Finder → Godot prüft Verweise. |
| Alten Code löschen statt auskommentieren | Er liegt ja in der Git-History. 💼 Auskommentierter Code wird in Reviews angemerkt. |

### Prinzipien

- **Separation of Concerns:** Jede Ebene hat genau eine Aufgabe (z. B. `Soil` nur für umgegrabene Erde). 💼 Schichtenarchitektur (UI / Logik / Datenbank).
- **Single Source of Truth:** Ein gemeinsames TileSet statt Kopien. 💼 Zentrale Konfiguration statt verstreuter Werte.
- **Erst planen, dann bauen:** Eine grobe Skizze der Karte hilft beim Malen und bei der Struktur.

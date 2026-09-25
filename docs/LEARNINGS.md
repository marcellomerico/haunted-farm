# 📓 Lernnotizen – Haunted Farm

Meine Notizen beim Lernen von **C#** und **Godot 4** mit diesem Projekt.
Pro Meilenstein ein Abschnitt: was ich gelernt habe, welche Fehler ich gemacht habe und was ich daraus mitnehme.

💼 = Wo mir das im Berufsalltag als C#-Entwickler begegnet

## Inhalt

- [Meilenstein 1: Spielfigur & Bewegung](#meilenstein-1-spielfigur--bewegung)

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

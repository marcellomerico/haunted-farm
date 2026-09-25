# 👻 Haunted Farm

Ein kleines 2D-Farming-Spiel im Pixel-Art-Stil, gebaut mit **Godot 4** und **C#**.

Das Projekt ist in erster Linie ein **Lernprojekt**: Ich lerne damit C# und Godot Schritt für Schritt, Meilenstein für Meilenstein. Was ich dabei lerne (inklusive meiner Fehler), halte ich in [docs/LEARNINGS.md](docs/LEARNINGS.md) fest.

## Stand

| Meilenstein | Inhalt | Status |
|---|---|---|
| 1 | Spielfigur mit Bewegung und Laufanimationen in vier Richtungen | ✅ |
| 2 | Farm-Szene im Herbst-Look aus Tilemaps, Kamera folgt der Figur | ✅ |
| … | Kollisionen, Y-Sorting, Bäume, Felder bestellen, Dorf, Dungeon, Startmenü | 🔜 |

## Steuerung

| Taste | Aktion |
|---|---|
| `W` `A` `S` `D` / Pfeiltasten | Laufen |

## Technik

- **Engine:** Godot 4.7 (.NET-Version)
- **Sprache:** C#
- **Renderer:** GL Compatibility
- **Auflösung:** 480 × 270 intern, pixelgenau hochskaliert (Integer-Scaling)
- **Tiles:** 16 × 16 px

## Projektstruktur

```
haunted-farm/
├── scenes/
│   ├── farm/        Farm.tscn (Hauptszene), farm_tileset.tres
│   └── player/      Player.tscn, Player.cs
├── assets/          Grafiken (Drittanbieter-Assets nicht im Repo, siehe unten)
├── docs/
│   └── LEARNINGS.md Lernnotizen pro Meilenstein
└── project.godot
```

## Projekt starten

1. [Godot 4.7 .NET](https://godotengine.org/download) und das [.NET SDK](https://dotnet.microsoft.com/download) installieren.
2. Repository klonen:
   ```bash
   git clone https://github.com/marcellomerico/haunted-farm.git
   ```
3. Die Assets besorgen und nach `assets/third_party/` legen (siehe unten).
4. Projekt in Godot öffnen und mit **F5** starten.

## Assets

Die Grafiken stammen aus den **Cozy**-Asset-Packs von [shubibubi](https://shubibubi.itch.io/) (u. a. *Cozy Farm*, *Cozy People*, *Cozy Town*). Deren Lizenz erlaubt die Nutzung in eigenen Spielen, aber **keine Weitergabe** der Dateien. Deshalb ist `assets/third_party/` per `.gitignore` ausgeschlossen.

Wer das Projekt selbst starten will, muss die Packs selbst herunterladen bzw. kaufen und dort ablegen.

## Lernnotizen

In [docs/LEARNINGS.md](docs/LEARNINGS.md) steht pro Meilenstein:

- welche Godot- und C#-Konzepte neu waren,
- welche Fehler ich gemacht habe und was ich daraus gelernt habe,
- wo mir das später im Berufsalltag als C#-Entwickler begegnet.

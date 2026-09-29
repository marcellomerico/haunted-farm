# Haunted Farm – Bauplan Oberwelt (A bis Z)

Stand: 29.09.2026 · Grundlage: Weltplan v3 (`map_v3_szenen.png`, `map_v3_kollision.png`, `bauplan_<szene>_v3.png`) und `sprite_referenz.png`

**Reihenfolge:** Farm → Nebelsee → Spukwald → Dorf → Strand → Szenen verbinden.
Das Dorf kommt spät, weil es die meisten Gebäude hat. Bis dahin sitzt das Rezept „Gebäude-Szene erben und anpassen“.

**Regel für jeden Schritt:** Erst bauen, dann testen, dann committen. Code gibt's nur, wo er wirklich nötig ist (Spielfigur, Szenenwechsel), und dann mit Erklärung.

---

## Spickzettel

### Sheets (Dateien erkennst du an der Pixelgröße)

| Kürzel | Größe | Inhalt | Dein Dateiname |
|---|---|---|---|
| S1 | 1503×1072 | Häuser & Farmgebäude | ____________ |
| S2 | 128×256 | kleine Bäume 32×32 (Grenzwald) | ____________ |
| S3 | 864×800 | Haupt-Tileset: Boden, Zäune, Steine, Büsche, Bäume | ____________ |
| S4 | 160×208 | Natur: Bäume, Pilze, Steine, Blumen | ____________ |
| S5 | 1152×1216 | Dorf-Spezialgebäude (Pub, Rathaus, Bibliothek …) | ____________ |
| S6 | 1344×640 | Züge (brauchen wir nicht) | – |
| S7 | 720×1072 | Dorf-Deko: Brunnen, Brücke, Gräber, Steg | ____________ |

Sprite-Angaben stehen in diesem Plan immer so: **`S3 · 144, 448 · 16×16`**. Das heißt Sheet 3, x = 144, y = 448, Breite 16, Höhe 16 in Pixeln.
In Godot gibst du genau diese Werte ein: bei `Sprite2D` unter **Region → Rect**, im TileSet über die Atlas-Kacheln.
Alle Sprites mit Bild findest du in `sprite_referenz.png`.

### Szenen, Kamera-Limits und Übergänge

Die Koordinaten sind **szenen-lokal** (0,0 = oben links in der Szene) und in Tiles angegeben. Für Pixel rechnest du ×16.

| Szene | Größe (Tiles) | Kamera-Limit (px): links, oben, rechts, unten | Übergänge (Tile x, y → Ziel) |
|---|---|---|---|
| `Farm.tscn` | 96×108 | 0, 0, 1536, 1728 | (88, 74) → Dorf · (88, 21) → See · (52, 103) → Strand |
| `See.tscn` | 88×58 | 0, 0, 1408, 928 | (8, 33) → Farm · (40, 53) → Dorf · (80, 27) → Wald |
| `Wald.tscn` | 82×120 | 0, 0, 1312, 1920 | (8, 27) → See · (8, 74) → Dorf · (35, 115) → Strand |
| `Dorf.tscn` | 88×72 | 0, 0, 1408, 1152 | (8, 38) → Farm · (40, 5) → See · (80, 26) → Wald · (50, 67) → Strand |
| `Strand.tscn` | 188×46 | 0, 0, 3008, 736 | (24, 9) → Farm · (102, 9) → Dorf · (159, 9) → Wald |

Waldrand: seitlich mindestens **15 Tiles**, oben/unten mindestens **9 Tiles**. Die Kamera zeigt 30×17 Tiles.

### Was ist abbaubar, was nicht?

| Ding | Wie gebaut | Abbaubar? |
|---|---|---|
| Grenzwald-Bäume | **Tiles** auf eigener TileMapLayer | **nie** |
| Zaun an der Farmgrenze | Tiles mit Kollision | nie |
| Gebäude, Landmarken (alter Baum, Grabsteine) | eigene Szenen / Tiles mit Kollision | nie |
| Steine, Büsche, Unkraut, junge Bäume auf der Farm | eigene Szenen, Gruppe **`abbaubar`** | **ja** (Logik kommt später) |
| Alles im Dorf | Tiles / Szenen ohne Gruppe `abbaubar` | nie |

Die Regel dahinter: Nur was in der Gruppe `abbaubar` ist, kann später abgebaut werden. Grenzbäume sind Tiles, also niemals abbaubar.

---

## Phase 0 – Vorbereitung (kein Code)

**0.1 Ordnerstruktur anlegen**

```
res://
├── assets/thirdparty/...        (hast du schon)
├── tilesets/                    oberwelt.tres
├── scenes/
│   ├── world/                   Farm.tscn, See.tscn, Wald.tscn, Dorf.tscn, Strand.tscn
│   ├── player/                  Player.tscn
│   └── objects/
│       ├── gebaeude/            Gebaeude.tscn (Basis) + geerbte Gebäude
│       ├── abbaubar/            Abbaubar.tscn (Basis) + Stein/Busch/Baum-Varianten
│       └── deko/                feste Deko-Szenen
├── scripts/
└── docs/                        PLAN.md, Weltplan-PNGs, sprite_referenz.png
```

**0.2 Sheets zuordnen.** Trag oben in die Tabelle deine Dateinamen ein.

**0.3 Projekteinstellungen prüfen**
- Texture Filter: Nearest
- Viewport 480×270, Stretch Mode `viewport`
- Unter *Projekteinstellungen → Layer-Namen → 2D-Physik* die Ebenen benennen:
  1. `Welt` (Wände, Wasser, Gebäude)
  2. `Spieler`
  3. `Abbaubar`
  4. `Interaktion`

  *Warum:* Kollisionsebenen legen fest, wer mit wem zusammenstößt. Mit Namen verlierst du später nicht den Überblick.

**0.4 Git:** eigener Branch `map-farm`. Die Pläne kommen nach `docs/`.

✅ Fertig, wenn die Ordner stehen, die Pläne im Repo liegen und die Layer benannt sind.

---

## Phase 1 – TileSet & Boden der Farm

**1.1 TileSet-Ressource** `tilesets/oberwelt.tres` anlegen: S3 als Atlas, Kachelgröße 16×16.

**1.2 Terrain-Set „Boden“** anlegen (Modus *Match Corners and Sides*) mit den Terrains Gras, Sand (= Weg), Erde, Wasser.
Die Übergänge sind im Sheet immer gleich aufgebaut: 3×3-Insel plus 4 Innenecken, also 13 Kacheln pro Übergang. Die Peering-Bits machen wir zusammen, das ist beim ersten Mal fummelig.

| Was | Region |
|---|---|
| Gras (Füllung) | `S3 · 16, 16 · 16×16` |
| Gras↔Sand | `S3 · 0, 96 · 96×48` |
| Gras↔Erde | `S3 · 0, 144 · 96×48` |
| Gras↔Wasser | `S3 · 0, 192 · 96×48` |
| Wasser (Füllung) | `S3 · 192, 208 · 16×16` |

**1.3 Physik-Layer im TileSet** anlegen (Ebene `Welt`) und allen Wasser-Kacheln ein volles Kollisions-Rechteck geben.

**1.4 `Farm.tscn` anlegen**

```
Farm (Node2D)
├── Boden        (TileMapLayer)   Gras, Wege, Teich
├── DekoFlach    (TileMapLayer)   Grasbüschel, Blumen – keine Kollision
├── Grenzwald    (TileMapLayer, Y-Sort an)
├── Grenze       (TileMapLayer)   unsichtbare Kollision + Zaun
├── Objekte      (Node2D, Y-Sort an)  Spieler, Gebäude, Abbaubares, Deko
└── Uebergaenge  (Node2D)         Ausgänge + Spawnpunkte
```

**1.5 Boden malen** nach `bauplan_farm_v3.png`:
- die ganze Fläche 96×108 mit Gras (auch unter dem Grenzwald)
- dann den Weg Farmhaus → Ausgänge mit dem Terrain-Pinsel „Sand“
- dann den Teich mit „Wasser“

Das Raster im Bauplan zeigt alle 8 Tiles eine Koordinate.

**1.6 Grasbüschel:** `S3 · 80, 0 · 112×16` (7 Stück), locker auf `DekoFlach` verteilen.

🧠 **Neu:** TileSet, Atlas, Terrain/Autotiling, TileMapLayer, Physik-Layer.
✅ Fertig, wenn der Boden aussieht wie im Bauplan, nur ohne Objekte.

---

## Phase 2 – Spielfigur & Kamera (erster kleiner C#-Code)

**2.1 `Player.tscn`** anlegen:
- `CharacterBody2D` als Wurzel
- darunter ein `Sprite2D` (Platzhalter reicht)
- darunter eine `CollisionShape2D`: kleines Rechteck, **nur an den Füßen**, etwa 10×6 px, damit der Spieler hinter Dächer und Baumkronen laufen kann
- Kollisionsebene `Spieler`, Maske `Welt`

**2.2 `Player.cs`** schreiben: Bewegung in 8 Richtungen. Den Code erklären wir Block für Block, wenn du so weit bist.

**2.3 `Camera2D`** als Kind vom Player: Limits laut Tabelle (Farm: 0, 0, 1536, 1728), Position Smoothing optional.

**2.4 Player** in `Objekte` vor dem Farmhaus platzieren.

🧠 **Neu:** CharacterBody2D, `_PhysicsProcess`, `Input.GetVector`, `MoveAndSlide`, Kamera-Limits.
💼 **Im Job:** Methoden überschreiben (`override`), Vektoren, Frame-basierte Updates.
✅ Fertig, wenn du laufen kannst und die Kamera am Szenenrand stehen bleibt.

---

## Phase 3 – Grenze: Grenzwald + unsichtbare Wand

**3.1** S2 als zweiten Atlas ins TileSet holen. Kachelgröße 32×32 (die Bäume sind so groß).
- Bei jedem Baum *Texture Origin* bzw. *Y-Sort Origin* auf den Stamm setzen, damit die Tiefensortierung stimmt.
- Grenzbäume Farm: `S2 · 0, 0` · `S2 · 0, 32` (Laub) und `S2 · 0, 96` · `S2 · 0, 128` · `S2 · 0, 192` (Nadel), je 32×32.

**3.2 Grenzwald malen** auf `Grenzwald`:
- dicht und versetzt: jede Reihe um 1 Tile verschoben, sodass sich die Kronen überlappen
- Laub und Nadel gemischt
- Vorlage ist der Bauplan: überall, wo im Bauplan Wald ist

**3.3 Unsichtbare Wand** auf `Grenze`:
- Leg im TileSet eine eigene Kachel „Kollision“ an (volles Rechteck, Ebene `Welt`).
- Mal sie entlang der **roten Linie** aus dem Bauplan, eine Tile-Reihe dick.
- Die Kachel im Spiel unsichtbar machen: `Grenze` → Modulate Alpha 0, oder eine leere Kachel mit Kollision nehmen.

  *Warum nicht jeder Baum mit eigener Kollision?* Weil eine durchgehende Linie keine Lücken hat und sich schneller malen lässt.

**3.4 Zaun als klares Signal „hier ist Schluss“** an der Farmgrenze, genau auf der roten Linie:
- `S3 · 64, 464` Zaun links · `S3 · 80, 464` Mitte · `S3 · 96, 464` rechts · `S3 · 48, 464` Pfosten (senkrecht), je 16×16
- An den 3 Ausgängen bleibt der Zaun offen.

**3.5 Test:** Lauf die komplette Farm am Rand ab. Du kommst nirgends durch, und die Kamera zeigt nie etwas außerhalb der Welt.

🧠 **Neu:** Mehrere Atlanten in einem TileSet, Y-Sort bei Tiles, Kollision über eine eigene Layer.
✅ Fertig, wenn die Farm rundum dicht ist und nur die 3 Ausgänge offen sind.

---

## Phase 4 – Gebäude: Kollision jetzt, betreten später

**4.1 Basis-Szene `Gebaeude.tscn`**

```
Gebaeude (StaticBody2D)        Kollisionsebene: Welt, Gruppe: "gebaeude"
├── Sprite (Sprite2D)          Region an, Offset so, dass die Unterkante auf (0,0) liegt
├── Kollision (CollisionShape2D)   Rechteck NUR über die Grundfläche (unterstes Drittel)
├── Tuer (Area2D)              Ebene: Interaktion – macht erstmal nichts
│   └── CollisionShape2D       kleines Rechteck vor der Tür
└── Eingang (Marker2D)         wo der Spieler später rauskommt
```

*Warum Kollision nur unten?* Dann kann der Spieler hinter das Dach laufen, und Y-Sort sortiert ihn richtig ein.

**4.2 Geerbte Szenen bauen:** Rechtsklick auf `Gebaeude.tscn` → **Neue geerbte Szene**. Darin nur Sprite-Region, Kollisionsgröße und Türposition anpassen.

*Warum erben statt duplizieren?* Änderst du später die Basis, zum Beispiel mit einem Interaktions-Script, bekommen alle Gebäude das automatisch.

| Szene | Sprite | Hinweis |
|---|---|---|
| `Farmhaus.tscn` | Stufe 1: `S1 · 0, 39 · 63×73` | Stufe 2 fürs Upgrade: `S1 · 976, 39 · 94×73` |
| `Gewaechshaus_Ruine.tscn` | `S1 · 384, 324 · 112×76` | Sprite → Modulate grau (z. B. `#8c8c8c`), wirkt verfallen |

**4.3 Platzieren** in `Objekte` laut Bauplan.

**4.4 Merken für später:**
- Farmhaus-Upgrade = Sprite-Region + Kollisionsgröße tauschen.
- Ruine: Die Tür-Area ist schon da. Die Logik („Hier ist nichts …“ bzw. später reparieren) kommt in der Interaktions-Phase.

🧠 **Neu:** StaticBody2D, Szenen-Vererbung, Gruppen, Area2D, Marker2D, Y-Sort mit Offset.
💼 **Im Job:** Vererbung, Wiederverwendung, „einmal ändern, überall wirksam“.
✅ Fertig, wenn du um beide Gebäude herum und hinter das Dach laufen kannst, aber nicht hindurch.

---

## Phase 5 – Landmarken & feste Deko

**5.1 Familiengrab**
- Zaun-Viereck: Zaun wie in 3.4, mit Tor-Lücke unten
- darin Grabsteine: `S7 · 80, 512` · `S7 · 16, 512` · `S7 · 48, 512` · `S7 · 96, 512`, je 16×16
- toter Baum: `S4 · 128, 32 · 32×32`

**5.2 Alter Baum** (Landmarke, **nicht** abbaubar): `S4 · 0, 32 · 32×32`

**5.3 Vogelscheuchen:** `S3 · 432, 512 · 16×32` und die leuchtende Variante `S3 · 448, 512 · 16×32`

**5.4 Teich-Deko:** Seerosen `S3 · 80, 240 · 16×16` (+3 Varianten rechts daneben), Schilf `S4 · 144, 112 · 16×16`

**5.5 Wie bauen?**
- Kleine Dinge (Grabsteine, Zaun) als **Tiles mit Kollision** im TileSet.
- Große Dinge (Bäume, Vogelscheuchen) als kleine Szene `Deko_Fest.tscn` (StaticBody2D + Sprite2D + kleine Kollision am Fuß), davon erben.

✅ Fertig, wenn alle Landmarken stehen und nichts davon in der Gruppe `abbaubar` ist.

---

## Phase 6 – Abbaubare Objekte (erstmal nur Optik + Kollision)

**6.1 Basis-Szene `Abbaubar.tscn`:** StaticBody2D (Ebene `Abbaubar` + `Welt`), Sprite2D, CollisionShape2D (1 Tile am Fuß), Gruppe **`abbaubar`**.

**6.2 Geerbte Varianten**

| Szene | Sprite(s) |
|---|---|
| `Stein_0` bis `Stein_3` | `S3 · 144, 448` · `160, 448` · `176, 448` · `192, 448` (je 16×16) |
| `Kiesel_0/6/7` | `S4 · 0, 144` · `96, 144` · `112, 144` (je 16×16) |
| `Busch_0` bis `Busch_3` | `S3 · 96, 416` · `112, 416` · `128, 416` · `144, 416` (je 16×16) |
| `Unkraut_0/1/5` | `S4 · 0, 96` · `16, 96` · `80, 96` (je 16×16) |
| `Baum_Eiche`, `Baum_Eiche2`, `Baum_Tanne` | `S3 · 0, 384` · `32, 384` · `64, 384` (je 32×32) |

**6.3 Beispiel-Cluster von Hand platzieren**, so wie auf der Map:
- Unkraut ums Haus
- Steinfeld im Norden
- junge Bäume an den Rändern

Lass einen Weg vom Haus zu den Ausgängen frei. Später ersetzt ein Zufalls-Spawner das Platzieren von Hand, dann löschst du die Beispiele einfach.

🧠 **Neu:** Gruppen als Markierung, erben im großen Stil.
✅ Fertig, wenn die Farm „verwildert“ aussieht und alles Abbaubare in der Gruppe `abbaubar` ist.

---

## Phase 7 – Übergänge vorbereiten (noch ohne Logik)

**7.1** Pro Ausgang eine `Area2D` in `Uebergaenge`, benannt nach dem Ziel: `Ausgang_Dorf`, `Ausgang_See`, `Ausgang_Strand`.
- Sie liegt an den Koordinaten aus der Tabelle und hat ein Rechteck quer über den Korridor.
- Ebene: `Interaktion`

**7.2** Pro Eingang ein `Marker2D` als Spawnpunkt: `Spawn_vonDorf`, `Spawn_vonSee`, `Spawn_vonStrand`.
- Er liegt 2–3 Tiles **innerhalb** vom Ausgang, sonst landet der Spieler direkt wieder im Trigger.

✅ Fertig, wenn alle Ausgänge und Spawnpunkte benannt an der richtigen Stelle liegen.

---

## Phase 8 – Farm abnehmen & committen

- [ ] Boden, Wege, Teich wie im Bauplan
- [ ] Rundum dicht, nur 3 Ausgänge offen
- [ ] Kamera zeigt nie etwas außerhalb der Welt
- [ ] An den Rändern bleibt der Spieler mittig, an den Ausgängen läuft er aus der Mitte heraus
- [ ] Hinter Dächer und Baumkronen laufen klappt (Y-Sort)
- [ ] Farmhaus + Ruine haben Kollision und eine Tür-Area
- [ ] Abbaubares ist in der Gruppe `abbaubar`, Grenzbäume sind Tiles
- [ ] Commit + Push, dann Screenshot fürs README

---

## Phase 9 – Nebelsee (`See.tscn`)

Rezept wie Phase 1, 3, 4, 5 und 7: Das TileSet hast du schon.

**Boden:** Gras, großer See (Wasser), Fluss-Anfang, Sandwege.
Für den **Holzsteg am Ufer** kommt ein Holz-Terrain oder einfach Holz-Tiles dazu: `S3 · 0, 672 · 64×48`.

**Gebäude:** `Haus_am_See.tscn` = `S1 · 674, 448 · 77×80`

**Steg:** `S7 · 0, 627 · 80×124`, als Deko-Szene.
Damit man darauf laufen kann, bekommen die Wasser-Tiles unter dem Steg **keine** Kollision. Dafür legst du eine alternative Wasser-Kachel ohne Kollision an.

**Insel:** toter Baum `S4 · 128, 32 · 32×32` und Schrein `S7 · 80, 512 · 16×16`. Die Insel ist absichtlich unerreichbar, Wasser rundherum.

**Deko:** Bank `S3 · 144, 432 · 32×16`, Laterne `S7 · 289, 259 · 15×29`, Seerosen, Schilf.

**Grenzwald:** wie auf der Farm, mit den gleichen Grenzbäumen.

**Kamera:** 0, 0, 1408, 928 · **Übergänge** laut Tabelle.

---

## Phase 10 – Spukwald (`Wald.tscn`)

**Boden:** Gras.
Der Wald soll dunkler wirken: `Boden` → Modulate leicht dunkel/bläulich (z. B. `#b8bcd0`). Dafür brauchst du keine neuen Tiles.

**Pfade:** Sand-Terrain, schmal und verzweigt.

**Innenwald:** Er ist dicht, aber nur Deko. Bäume als Tiles:
- Grenzbäume S2
- Herbst: `S2 · 0, 64` und `S2 · 0, 160`
- `S4 · 128, 0` rot · `S4 · 96, 0` dunkel · tote Bäume `S4 · 128, 32` und `S4 · 96, 32`, je 32×32

**Orte**

| Ort | Sprites |
|---|---|
| Verlassene Hütte (Gebäude, später betretbar) | `S1 · 1, 704 · 93×64` |
| Hexenkreis | 12 Pilze im Ring: `S4 · 0, 128` · `16, 128` · `96, 128` (16×16) + Kristall `S4 · 48, 160` |
| Waldtümpel | Wasser + Seerosen |
| Waldgräber | Grabsteine wie beim Familiengrab |
| Sackgasse im Osten | Lichtung für die spätere Gruft (Dungeon-Eingang) freilassen |

**Kamera:** 0, 0, 1312, 1920 · **Übergänge** laut Tabelle.

---

## Phase 11 – Dorf der Toten (`Dorf.tscn`)

**Boden:** neues Terrain **Kopfstein↔Gras** ins Terrain-Set: `S3 · 0, 560 · 96×48`, wieder 13 Kacheln. Damit malst du den runden Platz und die Gassen, dazu Sand-Trampelpfade zu den Türen.

**Fluss:** Wasser-Terrain am Westrand.

**Brücke:** `S7 · 0, 544 · 80×48` als Deko-Szene, darunter die Wasser-Kachel ohne Kollision (wie beim Steg).

**Gebäude:** alle als geerbte `Gebaeude.tscn`, jeweils mit Tür-Area.

| Gebäude | Sprite |
|---|---|
| Rathaus mit Glockenturm | `S5 · 1, 184 · 190×104` |
| Pub | `S5 · 9, 3 · 82×77` |
| Laden | `S1 · 5, 882 · 133×110` |
| Bibliothek | `S5 · 6, 651 · 149×101` |
| Trankladen | `S5 · 2, 1074 · 92×78` |
| Markt | `S1 · 2, 997 · 91×75` |
| Café | `S5 · 5, 292 · 85×92` |
| Arzt | `S5 · 3, 759 · 88×89` |
| Bürgermeister | `S5 · 2, 395 · 156×117` |
| Totengräber | `S1 · 235, 39 · 68×73` |
| Wohnhaus blau | `S1 · 0, 613 · 96×75` |
| Wohnhaus rot | `S5 · 4, 1154 · 88×62` |
| Wohnhaus Stroh | `S1 · 4, 534 · 72×74` |
| Wohnhaus Ziegel | `S1 · 737, 704 · 93×64` |

**Friedhof:** Eisenzaun-Set `S3 · 0, 512 · 128×48` (Tor, Pfosten, Gitter), Grabsteine `S7 · 0–144, 512` (16×16), toter Baum.

**Deko:**
- Brunnen `S7 · 4, 260 · 38×28`
- Laternen
- Bänke
- Kürbisse `S3 · 432, 512` und `448, 512` (16×16)
- Kürbisgarten und Kräutergarten mit Zaun
- Bäume `S4 · 64, 32` (rosa) und `S4 · 64, 0` (Birke)

**Wichtig:** Im Dorf kommt **nichts** in die Gruppe `abbaubar`.

**Kamera:** 0, 0, 1408, 1152 · **Übergänge** laut Tabelle.

---

## Phase 12 – Strand (`Strand.tscn`)

**Boden:** Sand-Terrain. Das Meer ist Wasser mit Kollision. Die Brandungskante ist im Pack nicht drin, darum erstmal eine gerade oder grob gezackte Wasserkante. Die schöne Brandung malen wir später selbst.

**Küstenwald** an beiden Enden bis ans Wasser, mit Grenzbäumen.

**Gebäude:** Fischerhütte = `S1 · 0, 613 · 96×75` (wie das blaue Wohnhaus; später eigenes Sprite).

**Pier:**
- Steg `S7 · 0, 627 · 80×124` verlängern: Mittelteil mehrfach untereinander setzen
- darunter Wasser ohne Kollision

**Brücke** über die Flussmündung (wie im Dorf).

**Deko:** Felsen `S3 · 160, 448` · `192, 448`, Muscheln/Kristalle, Dünengras (Busch-Sprites).

**Kamera:** 0, 0, 3008, 736 · **Übergänge** laut Tabelle.

---

## Phase 13 – Szenen verbinden (Code)

**13.1 `SceneManager`** als Autoload (Singleton), also global erreichbar:
- merkt sich den Ziel-Spawnpunkt
- wechselt die Szene
- setzt den Spieler an den richtigen Marker2D

**13.2 Ausgangs-Script** für die `Area2D`s:
- `[Export]` Zielszene + Spawnname
- bei `BodyEntered` → SceneManager

**13.3 Kurzer Fade** (schwarz ein/aus), damit der Wechsel nicht hart wirkt.

**13.4 Test:** Einmal im Kreis durch alle 5 Szenen laufen, jeder Übergang in beide Richtungen.

🧠 **Neu:** Autoload/Singleton, `[Export]`, Signale, `ChangeSceneToFile`.
💼 **Im Job:** Singletons, Events/Signale, Konfiguration über Properties.
✅ Fertig, wenn du die ganze Welt ohne Fehler ablaufen kannst. Dann steht **die Map**. 🎉

---

## Danach (eigene Pläne, Stück für Stück)

1. **Spieler-Animationen:** Laufen in 4 Richtungen, AnimatedSprite2D / AnimationPlayer
2. **Interaktion:**
   - Taste „E“ an Tür-Areas
   - Ruine: „Hier ist nichts …“
   - Farmhaus: noch zu
3. **Werkzeuge & Abbauen:**
   - Axt/Spitzhacke/Sense
   - `Abbaubar` bekommt ein C#-Script (Lebenspunkte, Treffer, Zerstören)
4. **Items & Drops:** Item-Ressourcen (Holz, Stein, Faser), Drop beim Abbauen, Aufsammeln
5. **Inventar:** Datenmodell getrennt von der UI (wichtig für späteren Koop), Hotbar
6. **Zufalls-Spawner Farm:** ersetzt die Beispiel-Cluster; spawnt über Nacht neu, nur auf freien Gras-Tiles
7. **Hacken & Säen:** Acker entsteht erst durch den Spieler, verfluchte Crops wachsen
8. **Tag/Nacht & Mondphasen:** Nachtfilter, Nebel, Irrlichter
9. **Farmgebäude kaufen & bauen:** Scheune, Silo, Hühnerstall, Mühle (Sprites stehen in der Referenz)
10. **Farmhaus-Upgrade:** Stufe 1 → 2 (→ 3)
11. **Speichern/Laden**
12. **NPCs im Dorf der Toten**
13. **Dungeon (Gruft im Spukwald)**

---

### Dateien zu diesem Plan
- `PLAN.md`: dieser Plan
- `sprite_referenz.png`: alle Sprites mit Sheet und Region
- `map_v3_szenen.png` / `map_v3_kollision.png` / `map_v3_clean.png`: Gesamtkarte
- `bauplan_farm_v3.png`, `bauplan_see_v3.png`, `bauplan_wald_v3.png`, `bauplan_dorf_v3.png`, `bauplan_strand_v3.png`: Baupläne mit Raster

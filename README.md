# La Ruta del Ter

Projecte Unity 3D d’un joc de taula digital basat en La Ruta del Ter. Desenvolupat per al Consorci del Ter dins l’equip Artilogics, amb suport formatiu de l’Escola CIFOG. Proposta lúdica i educativa per explorar el territori, el patrimoni natural i cultural, i promoure la sostenibilitat al llarg del riu Ter.

## 🎮 Resum del Joc

- **Gènere**: Joc de taula 3D / Party Game
- **Jugadors**: Multijugador Local (2 o més jugadors)
- **Motor**: Unity 6 (6000.3.11f1)
- **Estat**: Prototip / En Desenvolupament

## ✨ Funcionalitats Principals

- **Sistema de Trivia Interactiu**:
  - Preguntes sobre la natura, el patrimoni i la cultura del territori.
  - Respostes encertades permeten tornar a tirar el dau.
- **Daus 3D Físics**: Simulació de llançament amb detecció de resultats en temps real.
- **Taulell Dinàmic**: El taulell gira automàticament per enfocar el jugador actiu, millorant la immersió.
- **Configuració de Jugadors Personalitzada**:
  - Pantalla de selecció de personatges i colors.
  - Suport per a models 3D i sprites 2D.
- **Caselles Especials**:
  - 🟢 **Drecera**: Permet saltar a punts avançats del camí (amb opció d'elecció).
  - 🔴 **Pèrdua de Torn**: El jugador es queda una ronda sense jugar.
  - ⭐ **Tirada Extra**: Permet un segon llançament immediat.
  - ❓ **Casella de Pregunta**: Activa el sistema de trivia.
- **Animacions i "Feel" (Juice)**:
  - Moviment suau entre punts (waypoint navigation).
  - Efectes visuals de rebot ("squash and stretch") en aterrar i interactuar.

## 🛠️ Detalls Tècnics & Configuració

- **Scripts Principals**:
  - `GameControl.cs`: Gestiona l'estat del joc, els torns, les condicions de victòria i l'interfície.
  - `BoardController.cs`: Controla la rotació i el moviment físic del taulell.
  - `QuestionManager.cs`: Administra la càrrega de preguntes des de fitxers CSV i la lògica de trivia.
  - `FollowThePath.cs`: Gestiona el moviment dels jugadors i les animacions de salt.
  - `Dice3D.cs`: Controla la física dels daus en l'espai 3D.
- **Interfície (UI)**: Panells de control dinàmics que mostren l'estat de cada jugador i l'indicador de torn.
- **Àudio**: Sistema de so integrat per a accions com llançaments de dau, encerts i canvis de torn.

## 🚀 Començar a Jugar

1. **Clona el repositori**:
   ```bash
   git clone https://github.com/artilogics/larutadelter.git
   ```
2. **Obre el projecte a Unity**:
   - Utilitza **Unity Hub**.
   - Afegeix la carpeta del repositori.
   - Recomanat utilitzar la versió **Unity 6 (6000.3.11f1)**.
3. **Execució**:
   - Obre l'escena principal a `Assets/Scenes`.
   - Prem el botó **Play** de l'editor.

## 📝 Full de Ruta (Roadmap)

- [ ] Afegir més tipus de caselles interactives.
- [ ] Implementar intel·ligència artificial (IA) per a mode d'un sol jugador.
- [ ] Ampliar la base de dades de preguntes sobre el patrimoni del Ter.
- [ ] Millorar els efectes visuals i sonors per a una experiència més premium.

## 📄 Llicència

Aquest projecte està sota la llicència MIT - vegeu el fitxer [LICENSE](LICENSE) per a més detalls.

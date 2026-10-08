# Explicación del juego Ping-Pong 2D

Esta carpeta contiene la explicación de cómo se armó el juego y cómo funcionan sus partes.
Está basada en lo que hay en el proyecto, no en teoría externa.

 무면허
- Carpeta del menú y escenas: `Assets/Scenes/`
- Scripts del juego: `Assets/Scenes/Scripts/`
- Sonidos: `Assets/Resources/Sonidos/`
- Arte (sprites): `Assets/Sprites/arts/`
- Builder de Unity (solo para generar el `.exe`): `Assets/Editor/CommandLineBuild.cs`

---

## 1. Menú principal

El menú vive en la escena **Menu**:

- Archivo de escena: `Assets/Scenes/Menu/Menu.unity`
- Prefab del menú: `Assets/Scenes/Scripts/MainMenu.prefab`
- Script del menú: `Assets/Scenes/Scripts/MainMenu.cs`

### Qué tiene el menú

Según el archivo de escena, el menú contiene objetos como:

- `MainMenuPanel` (panel principal del menú)
- `BtnSalir` (botón salir)
- `BtnIniciar` (botón iniciar/jugar)

Esos botones están conectados alscript `MainMenu`, que tiene dos funciones públicas:

- `Jugar()`
- `Salir()`

Queda asi: el player hace clic en **Jugar** → se ejecuta `MainMenu.Jugar()` → se reproduce el sonido de clic → se carga la escena del juego.

### Cómo funciona `MainMenu.Jugar()`

```csharp
public void Jugar()
{
    GestorSonido.Instancia.SonarClic();
    SceneManager.LoadScene("SampleScene");
}
```

En Unity, esto se conecta así en el editor:

1. Se tiene un GameObject con el script `MainMenu` puesto.
2. En el botón del UI, en el componente `Button`, se arrastra ese GameObject al campo **On Click()**.
3. Se selecciona la función `MainMenu → Jugar()`.

Esto es lo que probablemente ya estaba en la escena `Menu` cuando se abrió el proyecto.

### Cómo funciona `MainMenu.Salir()`

```csharp
public void Salir()
{
    GestorSonido.Instancia.SonarClic();
    Debug.Log("Saliendo del juego...");
    Application.Quit();
}
```

En el editor esto tambien suena el clic y luego sale de la aplicación.

---

## 2. Cómo se unieron las dos escenas

El juego tiene dos escenas:

- **Menu** → escena del menú
- **SampleScene** → escena del juego

### Reglas de conexión

Las dos escenas están unidas por **SceneManager.LoadScene(...)**`:

- Menú → Juego: `MainMenu.Jugar()` hace `SceneManager.LoadScene("SampleScene")`
- Juego → Menú: `ControladorJuego.VolverAlMenu()` hace `SceneManager.LoadScene("Menu")`

Además el juego usa un **patrón singleton** en `ControladorJuego`:

- `ControladorJuego.Instancia` se asigna en `Awake()`
- Desde otros objetos se accede a `ControladorJuego.Instancia` para anotar goles y leer el estado

Esto permite que objetos distintos (la pelota, la UI, el sistema de goles) compartan un solo controlador de partida sin tener que buscarlo con `FindObjectOfType` cada vez.

### Flujo completo de una partida

1. Se abre la escena `Menu`.
2. El player presiona **Jugar**.
3. Se carga `SampleScene`.
4. En `SampleScene` se ejecuta `ControladorJuego.Start()` → actualiza la UI de puntuación.
5. La pelota se lanza con `Pelota.LanzarPelota()`.
6. Cuando alguien mete gol, se llama a `ControladorJuego.AnotarGol(jugador)`.
7. Si alguien llega a `puntosParaGanar`, se muestra el mensaje de ganador, suena el sonido de victoria y, tras 2.5 segundos, se va al menú con `VolverAlMenu()`.

---

## 3. Sistema de sonido

El sonido está manejado por un script único: `GestorSonido.cs`.

### Filosofía del sistema

El `GestorSonido`:

- Es un **singleton accesible desde cualquier lado** con `GestorSonido.Instancia`
- Se crea solo en runtime si no existe (`Instancia` lo crea con `new GameObject` + `AddComponent`)
- No requiere que se le ponga un AudioSource en la escena, porque él mismo lo crea con `gameObject.AddComponent<AudioSource>()`

### Cómo suena cada efecto

Cada sonido se carga desde la carpeta `Resources/Sonidos/`:

```csharp
AudioClip clip = Resources.Load<AudioClip>("Sonidos/" + nombre);
```

Luego se reproduce con:

```csharp
fuente.PlayOneShot(clip);
```

Los efectos disponibles son:

- `clic` → para botones del menú (`SonarClic`)
- `gol` → cuando alguien anota (`SonarGol`)
- `rebote` → cuando la pelota choca con pala o pared (`SonarRebote`)
- `ganar` → cuando alguien gana la partida (`SonarGanar`)

### De dónde se casó el sonido

Los archivos de audio están en:

- `Assets/Resources/Sonidos/clic.wav`
- `Assets/Resources/Sonidos/gol.wav`
- `Assets/Resources/Sonidos/rebote.wav`
- `Assets/Resources/Sonidos/ganar.wav`

Cada uno tiene su `.meta`, que es el archivo que Unity usa para identificar el asset internamente.

La forma de usarlos es **Resources.Load**, que es una forma clásica de cargar assets pornombre sin tener que referenciarlos en el inspector.

---

## 4. Pelota

Archivo: `Assets/Scenes/Scripts/Pelota.cs`

La pelota tiene:

- `velocidadInicial`
- Un `Rigidbody2D` que obtiene en `Start()`
- Un método `LanzarPelota()` que:
  - Pone la pelota en `(0,0)`
  - Le da velocidad cero
  - Elige una dirección aleatoria
  - Le aplica un impulso con `AddForce`

### Comportamiento al chocar

- `OnCollisionEnter2D` → suena `rebote`
- `OnTriggerEnter2D` → detecta si entró en `GolIzquierda` o `GolDerecha`
  - Si entró en `GolIzquierda` → `AnotarGol(2)` y lanza otra vez
  - Si entró en `GolDerecha` → `AnotarGol(1)` y lanza otra vez

Esto significa que los "goles" no son collissions normales, sino **triggers** con nombres fijos:
- `GolIzquierda`
- `GolDerecha`

Esos triggers deben estar puestos en la escena del juego, con los nombres exactos.

---

## 5. Paletas

Archivo: `Assets/Scenes/Scripts/Paleta.cs`

Una misma clase sirve para la paleta del jugador 1 y para la del otro jugador. El comportamiento se decide con:

- `esJugador1`
- `Input` diferenciado:
  - Jugador 1: W / S
  - Jugador 2: UpArrow / DownArrow

La paleta se mueve con:

```csharp
transform.Translate(Vector3.up * movimiento * velocidad * Time.deltaTime);
```

Y se limita con:

```csharp
pos.y = Mathf.Clamp(pos.y, -limiteY, limiteY);
```

Esto evita que salga de la zona permitida.

---

## 6. Controlador de juego

Archivo: `Assets/Scenes/Scripts/ControladorJuego.cs`

Este script maneja:

- Puntos de cada jugador (`puntosJ1`, `puntosJ2`)
- Puntos necesarios para ganar (`puntosParaGanar = 5`)
- Texto de puntuación (`textoPuntosJ1`, `textoPuntosJ2`)
- Texto del ganador (`textoGanador`)
- Bandera `partidaTerminada`

### Qué pasa cuando alguien gana

- Se marca `partidaTerminada = true`
- Se escribe en `textoGanador` quién ganó
- Suena `SonarGanar()`
- Se agenda `VolverAlMenu()` con `Invoke(..., 2.5f)`
- Al terminar ese retraso, se vuelve al menú

### Por qué es importante este script

Sirve de **centro de la reglas del juego**:

- Evita que siga contando goles después de terminar
- Centraliza la lógica de victoria
- Es el puente entre la pelota, la UI y el menú

---

## 7. De dónde se casó cada cosa (resumen)

### Escenas
- `Assets/Scenes/Menu/Menu.unity` → menú
- `Assets/Scenes/SampleScene.unity` → juego

### Scripts del juego
- `Assets/Scenes/Scripts/MainMenu.cs` → menú
- `Assets/Scenes/Scripts/ControladorJuego.cs` → reglas y flujo
- `Assets/Scenes/Scripts/Pelota.cs` → pelota y detección de gol
- `Assets/Scenes/Scripts/Paleta.cs` → movimiento de paletas
- `Assets/Scenes/Scripts/GestorSonido.cs` → reproductor de audio central

### Prefab del menú
- `Assets/Scenes/Scripts/MainMenu.prefab`

Este prefab probablemente agrupa el panel del menú y los botones, y se usa en la escena `Menu`.

### Sonidos
- `Assets/Resources/Sonidos/clic.wav`
- `Assets/Resources/Sonidos/gol.wav`
- `Assets/Resources/Sonidos/rebote.wav`
- `Assets/Resources/Sonidos/ganar.wav`

### Arte
- `Assets/Sprites/arts/Ball.png`
- `Assets/Sprites/arts/BallMotion.png`
- `Assets/Sprites/arts/Board.png`
- `Assets/Sprites/arts/Computer.png`
- `Assets/Sprites/arts/Frame.png`
- `Assets/Sprites/arts/Player.png`
- `Assets/Sprites/arts/ScoreBar.png`

### Material físico
- `Assets/PelotaFisica.physicsMaterial2D` → define cómo rebota la pelota físicamente

### Font / UI de texto
- `Assets/TextMesh Pro/Fonts/LiberationSans.ttf`
- Configuración TMP en `Assets/TextMesh Pro/Resources/TMP Settings.asset`

### Builder
- `Assets/Editor/CommandLineBuild.cs` → script de editor usado para generar el ejecutable fuera del editor

---

## 8. Cómo se construyó el ejecutable

Se usó un script de editor `CommandLineBuild.Build()` que hace:

1. Define la carpeta de salida como `ejecutable/` junto al proyecto
2. Construye para `StandaloneWindows64`
3. Empaqueta las escenas:
   - `Assets/Scenes/Menu/Menu.unity`
   - `Assets/Scenes/SampleScene.unity`
4. Escribe `ping-pong.exe` dentro de `ejecutable/`

Esto es lo que generó la carpeta `ejecutable` que está junto al proyecto.

---

## 9. Cómo leyó el proyecto para entenderlo

Este README viene de leer:

- Los 5 scripts del juego
- El prefab del menú
- La estructura de carpetas de `Assets`
- Los archivos de audio y sus metadatos
- Los archivos de escena y los nombres de objetos clave
- El script `CommandLineBuild.cs` usado para crear el `.exe`

No se inventó nada: lo que está aquí describe lo que hay en el proyecto actual.

---

## 10. Puntos clave para modificarlo

- Si quieres cambiar el nombre de la escena del juego, cambia `"SampleScene"` en `MainMenu.Jugar()` y en `ControladorJuego.VolverAlMenu()` si se usa desde ahí.
- Si quieres cambiar los goles necesarios para ganar, cambia `puntosParaGanar`.
- Si quieres añadir un nuevo sonido, pon el `.wav` en `Assets/Resources/Sonidos/` y añade un método en `GestorSonido` que llame a `Sonar("nombre")`.
- Si quieres cambiar qué pasa al hacer gol, modifica `Pelota.OnTriggerEnter2D` o `ControladorJuego.AnotarGol()`.
- Si quieres cambiar el movimiento de las paletas, modificas `Paleta.cs` o los valores del inspector en la escena.

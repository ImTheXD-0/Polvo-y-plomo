//---------------------------------------------------------
// Contiene el componente GameManager
// Guillermo Jiménez Díaz, Pedro P. Gómez Martín
// Marco A. Gómez Martín
// Template-P1
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Componente responsable de la gestión global del juego. Es un singleton
/// que orquesta el funcionamiento general de la aplicación,
/// sirviendo de comunicación entre las escenas.
///
/// El GameManager ha de sobrevivir entre escenas por lo que hace uso del
/// DontDestroyOnLoad. En caso de usarlo, cada escena debería tener su propio
/// GameManager para evitar problemas al usarlo. Además, se debería producir
/// un intercambio de información entre los GameManager de distintas escenas.
/// Generalmente, esta información debería estar en un LevelManager o similar.
/// 
/// Funcionalidades implementadas:
/// +++
/// Manejo del HUD: Se le pueden asignar distintos elementos del HUD para
/// que se vayan modificando con los datos del juego. Funcionará sin problemas
/// si no son asignados.
/// 
/// +++
/// Manejo entre escenas: Se le pueden asignar distintos tiempos de espera antes de
/// reiniciar el nivel o cargar la siguiente escena. Se encarga de llevar la lógica 
/// de reinicio de escena (Respawn()); reiniciar atributos del jugador, desactivar 
/// el input, realiza la animación de pantalla negra e inicia un contador con el Update(). 
/// También el de victoria de nivel (LevelEnds()) poniendo la pantalla negra, música 
/// de victoria y iniciando un contador distinto en el Update().
/// Las esperas se llevan en el Update, usando un parámetro para distinguir entre jugador
/// derrotado y victoria.
/// Por último tiene un método para reiniciar la escena y otro para cargar una distinta (usado
/// por el componente ChangeScene, para indicarle que a que escena cambiar).
/// 
/// +++
/// Tranferencia de información: permite configurarle a cada GameManager un
/// HUD en cada escena, que se transfiere en Awake() cuando la nueva instancia se da cuenta de que 
/// ya existe otro.
/// Incluye llevar la cuenta de la vida del jugador, su munición, cantidad de kills y cantidad de puntos.
/// Además el GameManager, al estar en el DontDestroyOnLoad, acarrea información entre escenas;
/// la vida del jugador, su puntaje y su cantidad de muertes. Este acarreo es intencionado y si el jugador
/// muriese estos datos se reinician a los que había al inicio de la escena (si hay LevelManager), o a 0.
/// Al cargarse en una escena, si había otro GameManager, es llamado el método NewSceneUpdate(), desde el
/// Start(). En esta llamada se hacen cosas necesarias al cargarse una escena; actualizar la vida del jugador,
/// el HUD, su puntaje, sus niveles de habilidad...
/// Si no hay otro GameManager se asume que la transferencia de datos es innecesaria.
/// +
/// Añadida a esta funcionalidad el método MatchEnded() que reinicia las estadísticas a cero. Será útil después
/// para añadir la funcionalidad de guardar el Highscore
/// 
/// +++
/// Lógica para la habilidad "SlowShot" del jugador, "ralentizando" el juego: todos los otros componentes que
/// tengan comportamientos relacionados con el tiempo usaran, si existe GameManager Instance, el parámetro
/// _slowMultiplier para simular la relantización del tiempo.
/// En caso de que no exista Instance, usan 1.
/// 
/// +++
/// Funcionalidad para almacenar la sensibilidad ajustada por el jugador.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----

    #region Atributos del Inspector (serialized fields)

    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Componente de audio que almacena la canción de victoria que sonará
    /// tras ganar un nivel.
    /// </summary>
    [SerializeField]
    private AudioClip VictoryMusic;

    /// <summary>
    /// Indice de la escena que contiene el nivel al que se pasará tras
    /// ganar la partida.
    /// </summary>
    [SerializeField]
    private int NextLevel = 0;

    /// <summary>
    /// Tiempo que tardara la escena en reiniciarse
    /// Ha de estar configurado de forma en la que funcione bien con los fadein y fadeout de la blackscreen
    /// </summary>
    [SerializeField]
    private float TiempoEsperaRespawn = 3f;

    /// <summary>
    /// Almacena cuanto tiempo se tardará en cargar la siguiente escena tras
    /// Ha de estar configurado de forma en la que funcione bien con los fadein y fadeout de la blackscreen
    /// la victoria del jugador.
    /// </summary>
    [SerializeField]
    private float TiempoEsperaSiguienteNivel = 5f;



    #endregion

    // ---- ATRIBUTOS PRIVADOS ----

    #region Atributos Privados (private fields)

    /// <summary>
    /// Constante que guarda la vida máxima del jugador.
    /// </summary>
    private const int VIDABASEJUGADOR = 10;

    /// <summary>
    /// Instancia única de la clase (singleton).
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// Esta es la vida actual del jugador.
    /// Inicializada en 10 por ser con la que empieza.
    /// </summary>
    private int _vidaJugador = VIDABASEJUGADOR;


    /// <summary>
    /// Este es el contador total de muertes.
    /// </summary>
    private int _totalDeaths = 0;
    
    /// <summary>
    /// Este es el contador total de puntos.
    /// </summary>
    private int _totalPoints = 0;

    /// <summary>
    /// Guarda un tiempo concreto. Usado para esperar en el Update().
    /// </summary>
    private float _t;

    /// <summary>
    /// Variable que guarda la velocidad de ralentización, distinta de 1,00 cuando
    /// la habilidad del jugador está activa. 
    /// Tiene una variable de acceso con un get para que aquellos scripts que la necesitan para 
    /// modificar su velocidad, tengan acceso a ella.
    /// </summary>
    private static float _slowMultiplier = 1f;

    /// <summary>
    /// Indica si el jugador ha muerto.
    /// Se usa para diferenciar que contador usar en el Update().
    /// </summary>
    private bool _playerDied = false;

    /// <summary>
    /// Almacena el componente de control del jugador para cambiarle
    /// la sensibilidad con los settings
    /// </summary>
    private playerControlledCursor _playerCursor;

    /// <summary>
    /// Almacena la sensibilidad del jugador, en un intervalo del [0,10].
    /// </summary>
    private float _cursorSensibility = 5f;

    /// <summary>
    /// Variable booleana para conocer si la habilidad del jugador esta activa o no.
    /// Evita bugs en el método ResumeGame().
    /// </summary>
    private bool _playerSlowShotOn = false;

    /// <summary>
    /// Variable booleana para conocer si el juego debe estar parado (por la muerte de Suzie, para evitar
    /// que el juego se reaunude si se pausa la partida después de matar al jefe)
    /// </summary>
    private bool _gameMustBePaused = false;

    /// <summary>
    /// Variable constante que indica el multiplicador del tiempo al estar activa la habilidad de SlowShot.
    /// </summary>
    private const float SLOWSHOT_TIMEMULTIPLIER = 0.25f;



    /// <summary>
    /// Variable booleana para saber si el jugador tiene activados el rexibir daño
    /// </summary>
    private bool _cheatInmortalJugador = false;

    /// <summary>
    /// Variable booleana para determinar si el jugador ha activado el cheat de nivel maximo de habilidad
    /// </summary>
    private bool _cheatMaxLVLJugador = false;


    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    #region Métodos de MonoBehaviour

    /// <summary>
    /// Se ejecuta al instaciar el componente.
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            // Somos el primer GameManager.
            // Queremos sobrevivir a cambios de escena.
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else
        {
            // Transferencia de configuración del HUD
            GameManager.Instance.TransferManagerSetup(VictoryMusic, NextLevel, TiempoEsperaRespawn, TiempoEsperaSiguienteNivel);
        }
    }

    /// <summary>
    /// Si hay un highscore, lo muestra en pantalla y de lo contrario lo marca como cero
    /// Método llamado una vez después del Awake(), una vez otros componentes ya
    /// se han inicializado correctamente.
    /// En el momento de la carga, si ya hay otra instancia creada,
    /// nos destruimos (al GameObject completo)
    /// Desactiva el componente para evitar que se corra updates.
    /// </summary>
    private void Start()
    {
        if (this != _instance)
        {
            // No somos la primera instancia. Se supone que somos un
            // GameManager de una escena que acaba de cargarse, pero
            // ya había otro en DontDestroyOnLoad que se ha registrado
            // como la única instancia.
            // Si es necesario, transferimos la configuración que es
            // dependiente de este manager al que ya existe.
            // Esto permitirá al GameManager real mantener su estado interno
            // pero acceder a los elementos de la nueva escena
            // o bien olvidar los de la escena previa de la que venimos
            
            // Mensaje de actualización de escena para la carga inicial
            GameManager.Instance.NewSceneUpdate();

            DestroyImmediate(this.gameObject);
        }
        else
        {
            // Se desactiva el componente para no usar el Update() hasta que sea necesario un contador.
            this.enabled = false;
            Init();
        } // if-else somos instancia nueva o no.
    }

    /// <summary>
    /// Método llamado cuando se destruye el componente.
    /// </summary>
    protected void OnDestroy()
    {
        if (this == _instance)
        {
            // Éramos la instancia de verdad, no un clon.
            _instance = null;
        } // if somos la instancia principal
    }

    /// <summary>
    /// Se llama cada frame si el componente está activo.
    /// Realiza distintas esperas en función de _playerDied para los métodos de Respawn() y LevelEnds().
    /// </summary>
    private void Update()
    {
        if (_playerDied) // proviene de Respawn()
        {
            if (Time.time - _t > TiempoEsperaRespawn)
            {
                this.enabled = false;
                ReinicioEscena();
            }
        }
        else // proviene de LevelEnds().
        {
            if (Time.time - _t > TiempoEsperaSiguienteNivel)
            {
                if (InputManager.HasInstance()) InputManager.Instance.ActivarInput();
                this.enabled = false;
                ChangeScene(NextLevel);
            }
        }
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----

    #region Métodos públicos
    #region Propiedades de acceso
    /// <summary>
    /// Propiedad para acceder a la única instancia de la clase.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            Debug.Assert(_instance != null);
            return _instance;
        }
    }

    /// <summary>
    /// Devuelve cierto si la instancia del singleton está creada y
    /// falso en otro caso.
    /// Lo normal es que esté creada, pero puede ser útil durante el
    /// cierre para evitar usar el GameManager que podría haber sido
    /// destruído antes de tiempo.
    /// </summary>
    /// <returns>Cierto si hay instancia creada.</returns>
    public static bool HasInstance()
    {
        return _instance != null;
    }
    #endregion

    #region Funcionalidad manejo de escenas publicos

    /// <summary>
    /// Método que cambia la escena actual por la indicada en el parámetro.
    /// </summary>
    /// <param name="index">Índice de la escena (en el build settings)
    /// que se cargará.</param>
    public void ChangeScene(int index)
    {
        // Antes y después de la carga fuerza la recolección de basura, por eficiencia,
        // dado que se espera que la carga tarde un tiempo, y dado que tenemos al
        // usuario esperando podemos aprovechar para hacer limpieza y ahorrarnos algún
        // tirón en otro momento.
        // De Unity Configuration Tips: Memory, Audio, and Textures
        // https://software.intel.com/en-us/blogs/2015/02/05/fix-memory-audio-texture-issues-in-unity
        //
        // "Since Unity's Auto Garbage Collection is usually only called when the heap is full
        // or there is not a large enough freeblock, consider calling (System.GC..Collect) before
        // and after loading a level (or put it on a timer) or otherwise cleanup at transition times."
        //
        // En realidad... todo esto es algo antiguo por lo que lo mismo ya está resuelto)
        System.GC.Collect();
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        System.GC.Collect();
    } // ChangeScene

    /// <summary>
    /// Método que se encarga de llevar los procesos tras la muerte del jugador.
    /// Desactiva el input del jugador, inicia un FadeIn de pantalla negra y activa este componente para que en
    /// el Update() se lleve un contador para esperar al FadeIn y luego reiniciar la escena.
    /// Se llama desde HealthChanger, cuando muere el jugador.
    /// </summary>
    public void Respawn()
    {
        // Reinicio de las stats del jugador para que empiecen completas tras reiniciarse la escena.
        _vidaJugador = VIDABASEJUGADOR;

        if (LevelManager.HasInstance())
        {
            _totalPoints = LevelManager.Instance.GetPointsAtStartOfLevel();
            _totalDeaths = LevelManager.Instance.GetKillsAtStartOfLevel();
        }
        else
        {
            _totalPoints = 0;
            _totalDeaths = 0;
        }

        if (InputManager.HasInstance()) InputManager.Instance.DesactivarInput();

        // Animación de pantalla negra.
        if (HUDManager.HasInstance()) HUDManager.Instance.StartFadeInBlackScreen();



        _t = Time.time;
        _playerDied = true;
        this.enabled = true; // comienza el temporizador en el update
    }

    /// <summary>
    /// Este metodo gestiona el final de un nivel.
    /// Se llama cuando hay una victoria de nivel (se mantiene vida y puntaje)
    /// </summary>
    public void LevelEnds()
    {
        _gameMustBePaused = true;
        PauseGame();

        // Feedback de victoria
        if (InputManager.HasInstance()) InputManager.Instance.DesactivarInput();
        if (HUDManager.HasInstance()) HUDManager.Instance.StartFadeInBlackScreen();
        if (AudioManager.HasInstance()) AudioManager.Instance.PlayMusic(VictoryMusic);

        _t = Time.time;
        _playerDied = false;
        this.enabled = true; // inicia contador en Update().

        SaveScore(_totalPoints);
    }

    /// <summary>
    /// Se llama al ganar el juego, tras derrotar a Suzie.
    /// Igual que LevelEnds pero reinicia las estadisticas.
    /// </summary>
    public void GameEnds()
    {
        LevelEnds(); // inicia el fin de nivel y guarda puntos
        ResetStats(); // reset de stats

        // Desactivar cheats si estan activos para que no haya errores.
        ResetCheats();
    }

    /// <summary>
    /// Método para reiniciar los cheats y desactivarlos desde fuera.
    /// Necesario para salir del juego desde PauseMenuManager sin que haya problemas.
    /// </summary>
    public void ResetCheats()
    {
        if (_cheatMaxLVLJugador) MaxLvlCheats();
        if (_cheatInmortalJugador) InmortalCheats();
    }

    #endregion

    #region Metodos transferencia de informacion

    /// <summary>
    /// Transfiere datos importantes de un GameManager que ha de destruirse al activo.
    /// Reconfigura el HUD para incluir el de la escena actual.
    /// </summary>
    public void TransferManagerSetup(AudioClip VictoryMusic,
        int NextLevel, float TiempoEsperaRespawn, float TiempoEsperaSiguienteNivel)
    {
        this.VictoryMusic = VictoryMusic;
        this.NextLevel = NextLevel;
        this.TiempoEsperaRespawn = TiempoEsperaRespawn;
        this.TiempoEsperaSiguienteNivel = TiempoEsperaSiguienteNivel;
    }

    /// <summary>
    /// Cuando un GameManager ya existe, esta función es llamada para actualizar
    /// cosas necesarias en la nueva escena desde el GameManager original.
    /// Sirve como indicador de que se ha cargado una nueva escena (que se llama solo
    /// cuando al cargarla hay otro GameManager).
    /// </summary>
    public void NewSceneUpdate()
    {
        if (HUDManager.HasInstance())
        {
            // Actualizar HUD del jugador
            HUDManager.Instance.UpdateHealthHUD(_vidaJugador);
            HUDManager.Instance.UpdateScoreHUD(_totalPoints);

            // Realizar el FadeOut de la pantalla negra al inicio de la escena solo si estaba activo (valor 1).
            HUDManager.Instance.StartFadeOutBlackScreen();
        }

        // Reiniciar flujo del tiempo (es posible salir de una escena con la habilidad activada, si no se reinicia,
        // se podría mantener la habilidad siempre activa.
        _playerSlowShotOn = false;
        _gameMustBePaused = false;
        ResumeGame();
        if (AudioManager.HasInstance())
            AudioManager.Instance.SetSlowMotionAudio(false);

        this.enabled = false;

        if (InputManager.HasInstance()) InputManager.Instance.ActivarInput();

        if (_cheatInmortalJugador)
        {
            if (LevelManager.HasInstance())
            {
                Transform playerTransform = LevelManager.Instance.PlayerTransform();
                if (playerTransform != null)
                {
                    Health healthChanger = playerTransform.GetComponent<Health>();

                    if (healthChanger != null)
                    {
                        healthChanger.BlockDamage();
                    }
                }
            }
        }

        Init();
        if (_playerCursor != null)
        {
            _playerCursor.SetCursorSpeed(_cursorSensibility);
        }
    }

    /// <summary>
    /// Método llamado para reiniciar las estadisticas del jugador.
    /// </summary>
    public void ResetStats()
    {
        _vidaJugador = VIDABASEJUGADOR;
        _totalDeaths = 0;
        _totalPoints = 0;
    }

    /// <summary>
    /// Este metodo actualiza la cantidad de muertes y los niveles de habilidad del jugador.
    /// </summary>
    public void AnEnemyDied()
    {
        _totalDeaths += 1;
        if (LevelManager.HasInstance() && LevelManager.Instance.PlayerTransform() != null)
        {
            playerSlowShot playerSlSh = LevelManager.Instance.PlayerTransform().GetComponent<playerSlowShot>();
            if (playerSlSh != null) playerSlSh.PlayerKill(_totalDeaths);
        }
    }

    /// <summary>
    /// Este metodo devuelve el valor int almacenado en _levelPoints, entendido como puntos iniciales
    /// </summary>
    public int TransferInitialPoints()
    {
        return _totalPoints;
    }

    /// <summary>
    /// Este método devuelve el número de muertes almacenado en el GameManager.
    /// </summary>
    public int TransferTotalDeaths()
    {
        return _totalDeaths;
    }

    /// <summary>
    /// Método para preguntarle al GameManager la vida a la que debe aparecer el jugador.
    /// Lo llama el HealthChanger del jugador al inicializarse.
    /// </summary>
    /// <returns></returns>
    public int InitHealthChanger()
    {
        return _vidaJugador;
    }

    /// <summary>
    /// Método para actualizar el puntaje del jugador.
    /// </summary>
    /// <param name="cambioDePuntos"></param>
    public void UpdateScore(int cambioDePuntos)
    {
        _totalPoints += cambioDePuntos;
        if (HUDManager.HasInstance()) HUDManager.Instance.UpdateScoreHUD(_totalPoints);
    }

    public int GetPlayerHealth()
    {
        return _vidaJugador;
    }

    public void UpdatePlayerHealth(int newPlayerHealth)
    {
        // IMPORTANTE: Primero la llamada y luego el cambio de vida. Así HUDManager puede calcular la diferencia.
        if (HUDManager.HasInstance()) HUDManager.Instance.UpdateHealthHUD(newPlayerHealth);
        _vidaJugador = newPlayerHealth;
    }

    #endregion

    #region Funcionalidad SlowShot y Pausa
    
    /// <summary>
    /// Delegado de tipo evento (solo permite += y -=, por lo que hace que sea seguro)
    /// que llamará a todas las funciones añadidas al cambiar la escala de tiempo.
    /// 
    /// Servirá para que los distintos componentes que usan la escala de SlowShot para su
    /// flujo de tiempo no tengan que estar preguntando constantemente en el Update() por el valor
    /// en el GameManager.
    /// 
    /// (!!!) Siempre que se añada un método a este delegado, asegurarse de que se elimina en el OnDestroy() o cuando sea apropiado.
    /// </summary>
    public event Action<float> OnTimeScaleChanged;

    /// <summary>
    /// Método público que modifica la velocidad de ralentización consecuencia de la activación de la habilidad del jugador
    /// </summary>
    public void SlowShotOn()
    {
        _playerSlowShotOn = true;
        ChangeTimeScale(SLOWSHOT_TIMEMULTIPLIER);
        if (HUDManager.HasInstance()) HUDManager.Instance.StartSlowshot();

        if (AudioManager.HasInstance())
            AudioManager.Instance.SetSlowMotionAudio(true);
    }

    /// <summary>
    /// Método público que modifica la velocidad de ralentización consecuencia de la desactivación de la habilidad del jugador
    /// </summary>
    public void SlowShotOff()
    {
        _playerSlowShotOn = false;
        ResumeGame();
        if (HUDManager.HasInstance()) HUDManager.Instance.EndSlowShot();

        if (AudioManager.HasInstance())
            AudioManager.Instance.SetSlowMotionAudio(false);
    }

    /// <summary>
    /// Método para pausar el juego, haciendo que su flujo de tiempo sea 0
    /// </summary>
    public void PauseGame()
    {
        ChangeTimeScale(0);

        if (AudioManager.HasInstance())
        {
            // Activa el estado de pausa musical. El AudioManager se encarga de subir el volumen de la pista pausada.
            AudioManager.Instance.SetPauseMusicStatus(true);
        }
    }

    /// <summary>
    /// Método para resumir el juego, cambiando el flujo del tiempo a 1 o al de la habilidad según corresponda.
    /// </summary
    public void ResumeGame()
    {
        if (_gameMustBePaused) ChangeTimeScale(0);
        else if (_playerSlowShotOn) ChangeTimeScale(SLOWSHOT_TIMEMULTIPLIER);
        else ChangeTimeScale(1.00f);

        if (AudioManager.HasInstance())
        {
            // Desactiva el estado de pausa musical. El AudioManager volverá a la fase (1 o 2) en la que estuviera.
            AudioManager.Instance.SetPauseMusicStatus(false);
        }
    }


    // NOTA: Los niveles de habilidad del jugador se actualizan también en AnEnemyDied(), en la región de Transferencia de información.
    #endregion

    #region Funcionalidad settings

    /// <summary>
    /// Método público para aumentar (en 0.1) la sensibilidad del cursor del jugador.
    /// </summary>
    public void LitSensIncrease()
    {
        _cursorSensibility += 0.1f;
        if (_cursorSensibility > 10f) _cursorSensibility = 10f;
        if (_playerCursor != null)
        {
            _playerCursor.SetCursorSpeed(_cursorSensibility);
        }
    }

    /// <summary>
    /// Método público para aumentar (en 1) la sensibilidad del cursor del jugador.
    /// </summary>
    public void BigSensIncrease()
    {
        _cursorSensibility += 1f;
        if (_cursorSensibility > 10f) _cursorSensibility = 10f;
        if (_playerCursor != null)
        {
            _playerCursor.SetCursorSpeed(_cursorSensibility);
        }
    }

    /// <summary>
    /// Método público para disminuir (en 0.1) la sensibilidad del cursor del jugador.
    /// </summary>
    public void LitSensDecrease()
    {
        _cursorSensibility -= 0.1f;
        if (_cursorSensibility < 0) _cursorSensibility = 0;
        if (_playerCursor != null)
        {
            _playerCursor.SetCursorSpeed(_cursorSensibility);
        }
    }

    /// <summary>
    /// Método público para disminuir (en 1) la sensibilidad del cursor del jugador.
    /// </summary>
    public void BigSensDecrease()
    {
        _cursorSensibility -= 1f;
        if (_cursorSensibility < 0) _cursorSensibility = 0;
        if (_playerCursor != null)
        {
            _playerCursor.SetCursorSpeed(_cursorSensibility);
        }
    }

    /// <summary>
    /// Método para leer la sensibilidad actual para el cursor del GameManager.
    /// </summary>
    /// <returns></returns>
    public float GetSens()
    {
        return _cursorSensibility;
    }

    #endregion

    #region Cheats

    /// <summary>
    /// Un método para indicar que se activan y desactican los trucos
    /// </summary>
    public void InmortalCheats()
    {
        _cheatInmortalJugador = !_cheatInmortalJugador;
    }

    /// <summary>
    /// Método para saber si los cheats de inmortalidad del jugador estan activos
    /// </summary>
    /// <returns></returns>
    public bool AreInmortalCheatsOn()
    {
        return _cheatInmortalJugador;
    }


    /// <summary>
    /// Un método para indicar que se activan y desactican los trucos
    /// </summary>
    public void MaxLvlCheats()
    {
        if (_cheatMaxLVLJugador)
        {
            _cheatMaxLVLJugador = false;
            _totalDeaths = 0;
        }
        else
        {
            _cheatMaxLVLJugador = true;
            _totalDeaths = 1000;
        }
    }

    /// <summary>
    /// Método para saber si los cheats de max lvl del jugador estan activos
    /// </summary>
    /// <returns></returns>
    public bool AreMaxLvlCheatsOn()
    {
        return _cheatMaxLVLJugador;
    }


    #endregion

    /// <summary>
    /// Método que se encarga de guardar el highscore de puntos que has obtenido en el juego
    /// </summary>
    public void SaveScore(int score)
    {
        if (!_cheatInmortalJugador && !_cheatMaxLVLJugador)
        {
            string data = score.ToString();
            string path = Application.persistentDataPath + "/Score.txt";
            
            if (File.Exists(path)) //Si existe el path se leera el número almacenado
            {
                string contenido = File.ReadAllText(path);
                int savedScore = int.Parse(contenido);

                if (savedScore >= score) // Si la puntuación no es mayor a la puntuación del archivo se corta
                {
                    return;
                }
            }
            File.WriteAllText(path, data); // Se guarda el puntaje en el path indicado
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----

    #region Métodos Privados

    /// <summary>
    /// Dispara la inicialización.
    /// Se llama desde el Start() y cada vez que se carga una escena nueva.
    /// </summary>
    private void Init()
    {
        if (LevelManager.HasInstance())
        {
            Transform _player = LevelManager.Instance.PlayerTransform();
            if (_player != null)
            {
                _playerCursor = _player.GetComponentInChildren<playerControlledCursor>();
            }
        }
    }

    /// <summary>
    /// Reinicia la escena actual, activa el FadeOut de la pantalla negra y reactiva el input del jugador.
    /// </summary>
    private void ReinicioEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Método para cambiar la escala de tiempo del juego.
    /// Llama a los métodos del delegado OnTimeScaleChanged.
    /// </summary>
    /// <param name="newScale"></param>
    private void ChangeTimeScale(float newScale)
    {
        _slowMultiplier = newScale;
        if (OnTimeScaleChanged != null) OnTimeScaleChanged.Invoke(newScale);
    }
    #endregion
} // class GameManager 
// namespace
//---------------------------------------------------------
// Singleton para manejar los HUDS de diferentes escenas
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
// Añadir aquí el resto de directivas using
using System.IO;

/// <summary>
/// Singleton encargado de todos los aspectos del HUD del juego.
/// NO se transferira entre escenas (en cada una configurarlo si es necesario)
/// </summary>
public class HUDManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #region Transiciones entre escenas
    [Header("Transiciones entre escenas")]
    /// <summary>
    /// Componente con el FadeIn configurado
    /// Realizará un FadeIn de pantalla negra al morir el jugador.
    /// Se debería configurar para que acabe en 1 de transparencia.
    /// </summary>
    [SerializeField]
    private FadeColor FadeInBlackScreen;

    /// <summary>
    /// Componente con el FadeOut configurado
    /// Realizará un FadeOut de pantalla negra al reaparecer el jugador.
    /// </summary>
    [SerializeField]
    private FadeColor FadeOutBlackScreen;

    /// <summary>
    /// Componente con el FadeIn configurado.
    /// Realizará un FadeIn de pantalla azul al activarse la habilidad de tiempo lento del jugador.
    /// </summary>
    [SerializeField]
    private FadeColor FadeInBlueScreen;

    /// <summary>
    /// Componente con FadeOut configurado.
    /// Realizará un FadeOut de pantalla azul al desactivarse la habilidad de tiempo lento del jugador.
    /// </summary>
    [SerializeField]
    private FadeColor FadeOutBlueScreen;
    #endregion

    #region Elementos habilidad slowshot
    [Header("Elementos habilidad slowshot")]
    /// <summary>
    /// Texto que guarda el nivel actual de la habilidad.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI ActLevelMessage;

    /// <summary>
    /// Texto que saldrá al subir de nivel la habilidad.
    /// </summary>
    [SerializeField]
    private ChangeColorAndHide LevelUpMessage;

    /// <summary>
    /// Componente con el "líquido" de la habilidad (duración restante de esta)
    /// Se podrá llamar al GameManager para modificar su fillAmmount.
    /// </summary>
    [SerializeField]
    private ImageFill HabilityLiquid;

    /// <summary>
    /// Componente con la "sombra" de la habilidad (duración restante del cooldown de la habilidad)
    /// Se podrá llamar al GameManager para modificar su fillAmmount.
    /// </summary>
    [SerializeField]
    private ImageFill HabilityShadow;

    /// <summary>
    /// Componente con el "liquido" de la barra que muestra cuantas muertes
    /// quedan para que la habilidad suba de nivel.
    /// </summary>
    [SerializeField]
    private ImageFill LevelBar;

    /// <summary>
    /// Variable a la que se le debe asignar el Animator del icono de la habilidad.
    /// En concreto, la del icono (reloj y contorno).
    /// Esto hace que se pueda cambiar al "estado activo" durante la habilidad.
    /// </summary>
    [SerializeField]
    private Animator TimeAbilityAnimator;
    #endregion

    #region Elementos puntaje y racha
    [Header("Elementos puntaje y racha")]

    /// <summary>
    /// Texto que muestra los puntos en el HUD.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI ScoreText;

    // <summary>
    /// Array para almacenar todos los gameObjects que se usaran para indicar los puntos que dan los enemigos en pantalla
    /// </summary>
    [SerializeField]
    private GameObject[] PointsPopups;

    /// <summary>
    /// Texto que muestra el multiplicador en el HUD.
    /// También se usará para acceder al componente UIVibration del mismo GameObject.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI StreakMultiplier;

    /// <summary>
    /// Struct para añadir colores al multiplicador y su racha asociada a la que cambiar
    /// </summary>
    [System.Serializable]
    public struct StreakColor
    {
        /// <summary>
        /// Color al que cambiará el texto en este StreakColor
        /// </summary>
        public Color Color;

        /// <summary>
        /// Cantidad para pasar al siguiente nivel configurado de StreakColors.
        /// El del último elemento del Array será ignorado.
        /// </summary>
        public int StreakToChangeColor;

        /// <summary>
        /// Nueva Vibration Intensity para el texto.
        /// </summary>
        public float NewVibration;
    }

    /// <summary>
    /// Array de colores.
    /// </summary>
    [SerializeField]
    private StreakColor[] StreakColors;

    /// <summary>
    /// Componente con el "liquido" de la barra que muestra cuanto porcentaje de tiempo
    /// queda para que la racha baje en 1.
    /// </summary>
    [SerializeField]
    private ImageFill StreakBar;

    #endregion

    #region Elementos vida y municion
    [Header("Elementos vida y municion")]
    /// <summary>
    /// Barril de revólver del HUD 
    /// </summary>
    [SerializeField]
    private GameObject Barrel;

    /// <summary>
    /// Lista de objetos de balas del HUD
    /// </summary>
    [SerializeField]
    private GameObject[] Bullets;

    /// <summary>
    /// Lista de objetos de vida del HUD
    /// </summary>
    [SerializeField]
    private HeartUI[] Lifes;
    #endregion

    #region Elementos ataque melee
    [Header("Elementos ataque melee")]
    /// <summary>
    /// ImageFill de la "sombra" del ataque melee, para representar su cooldown con un sprite que "cambia" de color
    /// </summary>
    [SerializeField]
    private ImageFill MeleeCooldown = null;
    #endregion

    #region Interfaz de menú
    [Header("Interfaz de menú")]

    /// <summary>
    /// Texto en el que se escribe el número del highscore.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI HighScoreTextUI;

    /// <summary>
    /// Variable que almacena el texto que te dice si los cheats de inmortalidad estan activados o no
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI TextoInmortalCheatHUD;

    /// <summary>
    /// Variable que almacena el texto que te dice si los cheats de Max LVL estan activados o no.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI TextoMaxLVLCheatHUD;

    #endregion

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    /// <summary>
    /// Constante que guarda la munición máxima del jugador.
    /// </summary>
    private const int MUNICIONBASEJUGADOR = 6;

    private static HUDManager _instance;

    /// <summary>
    /// Número que lleva la cuenta del gameObject actual con el que indicar los puntos (Cual de los gameObjects dentro del array es el encargado de generarse)
    /// </summary>
    private int actualtext;

    /// <summary>
    /// Indice que indica que color se esta usando actualmente para el StreakColor.
    /// </summary>
    private int _currentStreakColor = 0;

    /// <summary>
    /// Almacena el componente de UIVibration del texto StreakVibration.
    /// </summary>
    private UIVibration _streakVibration;

    /// <summary>
    /// Esta es la munición actual del jugador.
    /// Inicializada en 6 por ser en la que empieza.
    /// </summary>
    private int _municionJugador = MUNICIONBASEJUGADOR;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    private void Awake()
    {
        if ( _instance == null)
        {
            _instance = this;
            foreach (GameObject obj in PointsPopups) // Desactiva los indicadores de puntos 
            {
                if (obj != null) obj.SetActive(false);
            }
            if (SceneManager.GetActiveScene().buildIndex == 0) LoadScore();
        }
        else
        {
            Debug.Log("Dos HUDManager puestos en la misma escena. Uno será destruido");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!GameManager.HasInstance())
        {
            Debug.Log("HUDManager puesto en una escena sin GameManager. Es imprescindible que exista");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (this == Instance)
        {
            _instance = null;
        }
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #region Atributos/métodos de acceso

    public static HUDManager Instance
    {
        get { return _instance; }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }

    #endregion

    #region Transiciones entre escenas

    /// <summary>
    /// Empieza el FadeIn de la pantalla azul.
    /// Se llamará cuando empiece la habilidad del jugador, desde su correspondiente script.
    /// </summary>
    public void StartFadeInBlackScreen()
    {
        if (FadeInBlackScreen != null) FadeInBlackScreen.enabled = true;
    }

    /// <summary>
    /// Empieza el FadeOut de la pantalla azul.
    /// Se llamará cuando acabe la habilidad del jugador, desde su correspondiente script.
    /// </summary>
    public void StartFadeOutBlackScreen()
    {
        if (FadeOutBlackScreen != null) FadeOutBlackScreen.enabled = true;
    }

    #endregion

    #region Habilidad Slowshot

    /// <summary>
    /// Actualiza el fill ammount de la imagen del nivel de la habilidad.
    /// se llamará en 
    /// </summary>
    /// <param name="fillAmmount"></param>
    public void UpdateLevelBar(float fillAmmount)
    {
        if (LevelBar != null)
        {
            LevelBar.UpdateImageFillAmmount(fillAmmount);
        }
    }

    /// <summary>
    /// Actualiza en el HUD el texto que dice el nivel actual de la habilidad.
    /// </summary>
    /// <param name="a">Este sería el nuevo valor del nivel</param>
    public void UpdateActLevelText(int a)
    {
        if (ActLevelMessage != null)
        {
            ActLevelMessage.text = a.ToString();
        }
    }

    /// <summary>
    /// Llama al método ColorChanging de LevelUpMessage (solo hace que se active dicho componente)
    /// </summary>
    public void ActivateLevelUpText()
    {
        if (LevelUpMessage != null)
        {
            LevelUpMessage.ColorChanging();
        }
    }

    /// <summary>
    /// Actualiza el fill ammount de la imagen de la sombra de la habilidad.
    /// Se irá llamando durante el cooldown de la habilidad para indicar cuánto tiempo queda.
    /// </summary>
    /// <param name="fillAmmount"></param>
    public void UpdateTimeHabilityShadow(float fillAmmount)
    {
        if (HabilityShadow != null) HabilityShadow.UpdateImageFillAmmount(fillAmmount);
    }

    /// <summary>
    /// Actualiza el fill ammount de la imagen del liquido de la habilidad.
    /// Se irá llamando durante la duración de la habilidad para indicar cuanto tiempo le queda.
    /// </summary>
    /// <param name="fillAmmount"></param>
    public void UpdateTimeHabilityLiquid(float fillAmmount)
    {
        if (HabilityLiquid != null) HabilityLiquid.UpdateImageFillAmmount(fillAmmount);
    }

    /// <summary>
    /// Empieza el FadeIn de la pantalla azul.
    /// Se llamará cuando empiece la habilidad del jugador, desde su correspondiente script.
    /// </summary>
    public void StartSlowshot()
    {
        if (FadeInBlueScreen != null) FadeInBlueScreen.enabled = true;
        if (TimeAbilityAnimator != null) TimeAbilityAnimator.SetBool("AbilityActive", true);
    }

    /// <summary>
    /// Empieza el FadeOut de la pantalla azul.
    /// Se llamará cuando acabe la habilidad del jugador, desde su correspondiente script.
    /// </summary>
    public void EndSlowShot()
    {
        if (FadeOutBlueScreen != null) FadeOutBlueScreen.enabled = true;
        if (TimeAbilityAnimator != null) TimeAbilityAnimator.SetBool("AbilityActive", false);
    }

    #endregion

    #region Puntaje y racha
    /// <summary>
    /// Este metodo actualiza los puntos, y su HUD.
    /// </summary>
    public void UpdateScoreHUD(int totalPoints)
    {
        if (ScoreText != null) ScoreText.text = totalPoints.ToString();

    }

    /// <summary>
    /// Método que se encarga de generar los puntos que se le pasen en la posición que se le introduzca y de gestionar cual es el siguiente gameObject en encargarse de mostrarlos 
    /// </summary>
    public void SpawnPointIndicator(Vector3 position, int cambioDePuntos)
    {
        if (PointsPopups.Length > 0)
        {
            //Setear el punto
            PointsPopups[actualtext].SetActive(true);
            PointsPopups[actualtext].GetComponent<PointIndicator>().SpawnHere(position, cambioDePuntos);

            //Gestionar lista
            actualtext++;
            if (actualtext >= PointsPopups.Length) actualtext = 0;
        }
    }

    /// <summary>
    /// Este método actualiza la racha de muertes y su HUD.
    /// </summary>
    /// <param name="NuevoScoreJugador"></param>
    public void UpdateStreakMultiplierHUD(int Streak)
    {
        if (StreakMultiplier != null)
        {
            StreakMultiplier.text = "x" + Streak.ToString();
            if (StreakColors.Length > 0)
            {
                // Actualización de color y vibración
                if (_currentStreakColor < StreakColors.Length - 1 && Streak >= StreakColors[_currentStreakColor].StreakToChangeColor)
                {
                    _currentStreakColor++;
                    UpdateStreakMultiplierEffects();
                }
                else if (_currentStreakColor > 0 && Streak < StreakColors[_currentStreakColor - 1].StreakToChangeColor)
                {
                    _currentStreakColor--;
                    UpdateStreakMultiplierEffects();
                }
            }
        }
    }


    /// <summary>
    /// Actualiza el fill ammount de la imagen del tiempo de la racha.
    /// Se llamará en cada comprobación.
    /// </summary>
    /// <param name="fillAmmount"></param>
    public void UpdateStreakBar(float fillAmmount)
    {
        if (StreakBar != null) StreakBar.UpdateImageFillAmmount(fillAmmount);
    }
    #endregion

    #region Vida y municion

    /// <summary>
    /// Este metodo actualiza la vida y su HUD.
    /// </summary>
    public void UpdateHealthHUD(int NuevaVidaJugador)
    {
        int diff = NuevaVidaJugador - GameManager.Instance.GetPlayerHealth();

        for (int i = 0; i < Lifes.Length; i++)
        {
            if (Lifes[i] != null)
            {
                // i recorre corazones enteros
                // cada corazon son 2 PV

                // en cada corazón hay que ver si:
                // 1) hay que rellenarlo completo o dejarlo vacio
                // 2) hay que hacer una animación, de daño o de cura
                // para decidir entre si he recibido daño o me he curado seguramente sea
                // más intuitivo calcular  diff = NuevaVidaJugador - vidaAnterior, si es (+) se ha curado, si es (-) ha perdido vida


                // lo primordal al analizar cada corazón es ver si se tiene que hacer la animacion en este o no
                // verlo es más sencillo si primero verificamos el signo de diff

                // Para diff < 0:
                // si 2i <= NuevaVidaJugador < 2(i+1) el corazon necesita animacion

                // if NuevaVidaJugador >= 2(ì+1) entonces se pinta el corazon lleno y listo
                // else if NuevaVidaJugador < 2i entonces se pinta el corazon vacio y listo
                // else, (hace falta animacion) hago un switch con (NuevaVidaJugador - 2i)
                // case 0: animacion corazon medio a corazon vacio
                // case 1: animacion corazon completo a corazon medio
                // --> Aqui se asume que los cambios son de 1 PV, habría que ver con Suzie como queda la perdida de vida

                // Para diff > 0:
                // si 2i < NuevaVidaJugador <= 2(i+1) el corazon necesita animacion

                // if NuevaVidaJugador > 2(i+1) se pinta el corazon lleno y listo
                // else if NuevaVidaJugador <= 2i se pinta el corazon vacio y listo
                // else, (hace falta animacion) hago un switch con (NuevaVidaJugador - 2i)
                // case 1: animacion de corazon vacio a corazon medio
                // case 2: animacion de corazon medio a corazon lleno
                // --> De nuevo asumimos que los cambios son de 1 PV y podría quedar raro con curas mayores

                // Para diff = 0: (posible en la transición de escenas)
                // if NuevaVidaJugador >= 2(i+1) se pinta el corazón lleno y listo
                // else if NuevaVidaJugador <= 2i se pinta el corazon vacio y listo
                // else -> mitad de corazon
                if (diff < 0)
                {
                    if (NuevaVidaJugador >= 2 * (i + 1)) Lifes[i].FullHeart();
                    else if (NuevaVidaJugador < 2 * i) Lifes[i].EmptyHeart();
                    else
                    {
                        switch (NuevaVidaJugador - 2 * i)
                        {
                            case 0:
                                Lifes[i].HitToEmpty();
                                break;
                            case 1:
                                Lifes[i].HitToHalf();
                                break;
                        }
                    }
                }
                else if (diff > 0)
                {
                    if (NuevaVidaJugador > 2 * (i + 1)) Lifes[i].FullHeart();
                    else if (NuevaVidaJugador <= 2 * i) Lifes[i].EmptyHeart();
                    else
                    {
                        switch (NuevaVidaJugador - 2 * i)
                        {
                            case 1:
                                Lifes[i].HealToHalf();
                                break;
                            case 2:
                                Lifes[i].HealToFull();
                                break;
                        }
                    }
                }
                else
                {
                    if (NuevaVidaJugador >= 2 * (i + 1)) Lifes[i].FullHeart();
                    else if (NuevaVidaJugador <= 2 * i) Lifes[i].EmptyHeart();
                    else Lifes[i].HalfHeart();
                }
            }
        }
    }


    /// <summary>
    /// Este metodo actualiza las balas y su HUD
    /// </summary>
    public void UpdateAmmoHUD(int NuevaMunicionJugador)
    {
        bool recarga = NuevaMunicionJugador - _municionJugador > 0;

        BarrelAnimatorController barrelAnimator = Barrel.GetComponent<BarrelAnimatorController>();

        _municionJugador = NuevaMunicionJugador;
        for (int i = 0; i < Bullets.Length; i++)
        {
            if (Bullets[i] != null)
            {
                Animator bulletAnimator = Bullets[i].GetComponent<Animator>();
                if (bulletAnimator != null)
                {
                    if ((i < _municionJugador)) bulletAnimator.Play("BulletIdle", 0, 0f);
                    else if ((i == _municionJugador) && !recarga) bulletAnimator.Play("Bullet", 0, 0f);
                }
                else Debug.Log("Falta animator en una de las bullets del barril de recarga");
            }
        }

        if (barrelAnimator != null)
        {
            if (recarga)
            {
                barrelAnimator.AddAnticlockwiseRotation();
            }
            else barrelAnimator.AddClockwiseRotation();
        }
    }

    #endregion

    #region Ataque melee

    /// <summary>
    /// Método público que llama al ImageFill que controla la representación del cooldown del ataque melee, para que se actualice al valor que le corresponda.
    /// </summary>
    /// <param name="fillAmount"></param>
    public void UpdateMeleeCooldownShadow(float fillAmount)
    {
        if (MeleeCooldown != null) MeleeCooldown.UpdateImageFillAmmount(fillAmount);
    }

    #endregion

    #region Interfaz del menú

    /// <summary>
    /// Actualiza en pantalla si el jugador tiene o no los cheats
    /// </summary>
    public void UpdateInmortalCheatHUD(bool cheatInmortalJugador)
    {
        if (TextoInmortalCheatHUD != null)
        {
            if (cheatInmortalJugador) TextoInmortalCheatHUD.text = "God Mode: ON";
            else TextoInmortalCheatHUD.text = "God Mode: OFF";
        }
    }

    /// <summary>
    /// Actualiza en pantalla si el jugador tiene o no los cheats
    /// </summary>
    public void UpdateMaxLVLCheatHUD(bool cheatMaxLVLJugador)
    {
        if (TextoMaxLVLCheatHUD != null)
        {
            if (cheatMaxLVLJugador) TextoMaxLVLCheatHUD.text = "Max LVL: ON";
            else TextoMaxLVLCheatHUD.text = "Max LVL: OFF";
        }
    }
    #endregion

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Actualiza el color y la intensidad de vibración del texto del multiplicador de racha.
    /// (!) No verifica si _currentStreakColor es correcta, ha de serlo (se controla en el método
    /// de UpdateStreakMultiplierHUD() ).
    /// </summary>
    private void UpdateStreakMultiplierEffects()
    {
        StreakMultiplier.color = StreakColors[_currentStreakColor].Color;

        // intento de inicializar la vibración (puede ya estar almacenado)
        if (_streakVibration == null)
        {
            _streakVibration = StreakMultiplier.gameObject.GetComponent<UIVibration>();
        }

        // si se consigue o si ya estaba guardada
        if (_streakVibration != null)
        {
            _streakVibration.ChangeIntensity(StreakColors[_currentStreakColor].NewVibration);
        }
    }

    /// <summary>
    /// Método que permite cargar el puntaje desde una ruta de archivo guardada
    /// </summary>
    private void LoadScore()
    {
        if (HighScoreTextUI == null) // Si no existe el archivo el puntaje es 0 y se acaba
        {
            HighScoreTextUI.text = "0";
            return;
        }

        string path = Application.persistentDataPath + "/Score.txt";

        if (File.Exists(path)) // Si existe el archivo se lee y se actualiza el highscore
        {
            string file = File.ReadAllText(path);

            HighScoreTextUI.text = file;

        }
        //Ponerla en la UI

    }

    #endregion

} // class HUDManager 
// namespace

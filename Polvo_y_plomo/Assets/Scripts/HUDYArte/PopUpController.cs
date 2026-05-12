//---------------------------------------------------------
// Script sencillo para incluir la funcionalidad de un Pop-Up al principio de una escena
// Ängel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
// Añadir aquí el resto de directivas using


/// <summary>
/// Queremos que al empezar el nivel 1 aparezcan los controles del juego para llevar un poco
/// de la mano al jugador y que los pueda ver.
/// Este script sirve para pausar el juego al inicio de la partida, y para cerrar el pop-up con
/// ESC o con el boton de cerrar.
/// 
/// Para ello es necesario asignarle el BlockCursor de la escena (liberar el cursor), el PauseMenuManager
/// (para desactivarlo inicialmente y que no se pueda abrir durante el PopUp).
/// Si se le asigna PlayMusic evitara el comienzo del a cancion hasta cerrar el popup.
/// 
/// (!) Este script debe estar en el Canvas del Pop-Up ya que es el que debe ser destruido una vez cerrado el pop-up.
/// El script destruye el objeto en el que se situa.
/// </summary>
public class PopUpController : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// CursorBlocker que debe ser asignado.
    /// </summary>
    [SerializeField]
    private CursorBloqueado CursorBlocker;

    /// <summary>
    /// PauseMenuManager que debe ser asignado.
    /// </summary>
    [SerializeField]
    private PauseMenuManager PauseMenu;

    /// <summary>
    /// Componente que inicia la música al principio de la partida.
    /// Si se asigna, evitará esto hasta que se cierre el PopUp.
    /// </summary>
    [SerializeField]
    private PlaySongOnEnable PlaySong;

    /// <summary>
    /// Boton seleccionado al cargarse el PopUp.
    /// </summary>
    [SerializeField]
    private GameObject FirstButtonSelected;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Se llama al cargarse en la escena.
    /// Hace comprobaciones necesarias para el componente.
    /// </summary>
    private void Awake()
    {
        if (CursorBlocker == null)
        {
            Debug.Log("PopUpController sin CursorBlocker asignado. No funcionará y destruira el PopUp");
            Destroy(gameObject);
        }

        if (PauseMenu == null)
        {
            Debug.Log("PopUpController sin PauseMenu asignado. No funcionará y destruira el PopUp");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Se llama una vez al activarse por primera vez
    /// Inicializa el componente.
    /// </summary>
    private void Start()
    {

        if (FirstButtonSelected != null) EventSystem.current.SetSelectedGameObject(FirstButtonSelected);

        PauseMenu.ResumeGame(); // cerrar todo el menu de pausa si estaba abierto
        PauseMenu.enabled = false; // impide su funcionamiento mientras exista el popup

        CursorBlocker.enabled = false; // impide que se de su Start() y por ende no bloquea el cursor
        CursorBlocker.UnlockCursor(); // me aseguro

        if (PlaySong != null) PlaySong.enabled = false;
    }

    /// <summary>
    /// Se llama cada frame mientras el componente este activo.
    /// Verifica si se ha presionado Exit para cerrar el popup
    /// </summary>
    private void Update()
    {
        // Asegurarme de que todo se pause bien y los controles se apagen (en el Start podría no darse)
        if (GameManager.HasInstance()) GameManager.Instance.PauseGame();
        if (InputManager.HasInstance()) InputManager.Instance.DesactivarInput();

        if (InputManager.HasInstance() && InputManager.Instance.ExitWasPressedThisFrame())
        {
            ClosePopUp();
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

    /// <summary>
    /// Método para cerrar el PopUp
    /// </summary>
    public void ClosePopUp()
    {
        PauseMenu.enabled = true;
        CursorBlocker.enabled = true;
        CursorBlocker.LockCursor(); // por si acaso

        if (PlaySong != null) PlaySong.enabled = true;

        if (GameManager.HasInstance()) GameManager.Instance.ResumeGame();
        if (InputManager.HasInstance()) InputManager.Instance.ActivarInput();
        Destroy(gameObject);
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class PopUpController 
// namespace

//---------------------------------------------------------
// Script utilizado en el tutorial para activar objetos cuando el jugador realice una acción
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.InputSystem;
// Añadir aquí el resto de directivas using


/// <summary>
/// Observa la acción asignada en el editor y activa todos los objetos indicados en cuanto se usa por primera vez.
/// Se desactiva tras observarla.
/// </summary>
public class EnableObjectsOnActionPerformed : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Nombre de la acción que se observará
    /// </summary>
    [Header("Acción a observar")]
    [SerializeField] 
    private string NombreDeLaAccion;

    /// <summary>
    /// Gameobjects que se activarán tras la acción
    /// </summary>
    [Header("Objetos a activar cuando se de la acción")]
    [SerializeField]
    private GameObject[] GameObjects;

    /// <summary>
    /// Scripts concretos que activar tras la acción
    /// </summary>
    [SerializeField]
    private MonoBehaviour[] Scripts;

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
    /// Acción observada
    /// Inicializada en el Start().
    /// </summary>
    private InputAction _action;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    private void Start()
    {
        if (InputManager.HasInstance())
        {
            _action = InputManager.Instance.GetAction(NombreDeLaAccion);

            if (_action == null)
            {
                Debug.Log("Accion no se ha registrado en EnableObjectOnActionPerformed. No funcionará");
                Destroy(this);
            }

            _action.performed += OnStarted;
        }
        else
        {
            Debug.Log("EnableObjectOnActionPerformed puesto en una escena sin InputManager. No funcionará");
            Destroy(this);
        } 
    }

    private void OnEnable()
    {
        if (_action != null) _action.performed += OnStarted;
    }

    private void OnDisable()
    {
        if (_action != null) _action.performed -= OnStarted;
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    private void OnStarted(InputAction.CallbackContext ctx)
    {
        foreach (GameObject GameObject in GameObjects)
        {
            if (GameObject != null) GameObject.SetActive(true);
        }

        foreach(MonoBehaviour script in Scripts)
        {
            if (script != null) script.enabled = true;
        }

        this.gameObject.SetActive(false);
    }

    #endregion

} // class EnableObjectsOnActionPerformed 
// namespace

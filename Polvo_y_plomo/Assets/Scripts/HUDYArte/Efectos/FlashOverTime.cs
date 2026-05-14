//---------------------------------------------------------
// Inicia los flashes del objeto tras pasar cierto tiempo.
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEditorInternal;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Componente para iniciar los flashes de un objeto tras transcurrir cierto tiempo configurable.
/// Después de activar los flashes este script se desactiva.
/// </summary>
public class FlashOverTime : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Variable que indica a partir de cuantos segundos empieza el parpadeo.
    /// </summary>
    [SerializeField]
    private float TiempoParpadeo = 10f;

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
    /// Almacena el tiempo para iniciar el parpadeo.
    /// </summary>
    private float _t;

    /// <summary>
    /// Almacena el componente CanFlash que ha de tener este gameobject.
    /// Inicializado en el Awake().
    /// </summary>
    private CanFlash _canFlash;

    /// <summary>
    /// Almacena el multiplicador de tiempo del GameManager. Se actualiza siempre que cambia con un método delegado.
    /// </summary>
    private float _slowMultiplier = 1f;

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
        _canFlash = GetComponent<CanFlash>();
        if (_canFlash == null)
        {
            Debug.Log("FlashOverTime puesto en un gameobject sin CanFlash. No funcionará");
            Destroy(this);
        }
    }

    /// <summary>
    /// Se llama despues del awake si el componente esta activo.
    /// Se añade al delegado del GameManager para actualizar _slowMultiplier
    /// </summary>
    private void Start()
    {
        if (GameManager.HasInstance()) GameManager.Instance.OnTimeScaleChanged += OnTimeScaleChanged;
    }

    /// <summary>
    /// Reinicia _t al activarse el componente.
    /// </summary>
    private void OnEnable()
    {
        _t = 0;
    }

    /// <summary>
    /// Se llama cada frame si el componente esta activo.
    /// Actualiza el contador de tiempo y si es suficientemnete grande inicia los flashes.
    /// </summary>
    private void Update()
    {
        _t += Time.deltaTime * _slowMultiplier;

        if (_t >= TiempoParpadeo)
        {
            _canFlash.StartFlashes();
            this.enabled = false;
        }
    }

    /// <summary>
    /// Se llama al destruirse el componente.
    /// Elimina su método del delegado del GameManager.
    /// </summary>
    private void OnDestroy()
    {
        if (GameManager.HasInstance()) GameManager.Instance.OnTimeScaleChanged -= OnTimeScaleChanged;
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
    /// <summary>
    /// Método que se delegará al GameManager para actualizar el _slowMultiplier.
    /// </summary>
    /// <param name="newScale"></param>
    private void OnTimeScaleChanged(float newScale)
    {
        _slowMultiplier = newScale;
    }
    #endregion

} // class FlashOverTime 
// namespace

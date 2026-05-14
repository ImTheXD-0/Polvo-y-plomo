//---------------------------------------------------------
// Componente para iniciar un Fade Out despues de un tiempo configurable
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Componente que inicia un FadeOut después de un tiempo configurable desde su aparición.
/// Se le ha de asignar FadeOut.
/// 
/// Una vez activado el FadeOut este componente se desactiva a si mismo.
/// </summary>
public class VanishOverTime : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    /// <summary>
    /// Variable que almacena el tiempo de vida del objeto
    /// </summary>
    [SerializeField]
    private float VanishTime = 5f;

    /// <summary>
    /// Variable que se debe asignar y que debe tener un FadeOut configurado para el vanish.
    /// </summary>
    [SerializeField]
    private FadeColor FadeOut;

    /// <summary>
    /// Almacena el multiplicador de tiempo del GameManager. Se actualiza siempre que cambia con un método delegado.
    /// </summary>
    private float _slowMultiplier = 1f;

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
    /// Almacena el tiempo desde la aparición
    /// Inicializada en el Start()
    /// </summary>
    private float _t = 0;

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
    void Awake()
    {
        if (FadeOut == null)
        {
            Debug.Log("Componente VanishOverTime colocado sin configurarle un FadeOut. No funcionará");
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
    /// Al ser activado se reinicia el contador del tiempo.
    /// </summary>
    private void OnEnable()
    {
        _t = 0;
    }

    /// <summary>
    /// Se llama cada frame si el componente esta activo.
    /// Lleva el contador y si ha pasado suficiente tiempo, activa el FadeOut y desactiva este componente.
    /// </summary>
    void Update()
    {

        _t += Time.deltaTime * _slowMultiplier;

        if (_t >= VanishTime)
        {
            FadeOut.enabled = true;
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

} // class VanishOverTime 
// namespace

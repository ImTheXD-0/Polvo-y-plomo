//---------------------------------------------------------
// Destruye un gameobject al transcurrir un tiempo editable
// CamiloSandovalSánchez
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Componente que se usa para darle a un objeto un "tiempo de vida", que transcurre desde que
/// se carga el objeto. Tras pasar el tiempo, el GameObject es destruido.
/// </summary>
public class DestroyOverTime : MonoBehaviour
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
    private float LifeTime = 0.1f;
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
    /// Variable que el tiempo restante de vida del objeto
    /// Inicializada en el Start()
    /// </summary>
    private float _tLifeSpan;

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
    /// Se llama la primera vez que el componente esta activo, después del Awake
    /// Guarda el tiempo de spawn.
    /// </summary>
    void Start()
    {
        _tLifeSpan = LifeTime;
        if (GameManager.HasInstance()) GameManager.Instance.OnTimeScaleChanged += OnTimeScaleChanged;
    }

    /// <summary>
    /// Se llama cada frame
    /// Elimina al objeto una vez que el tiempo de vida parametrizado se alcanza.
    /// </summary>
    void Update()
    {
         _tLifeSpan -= Time.deltaTime * _slowMultiplier;

        if (_tLifeSpan <= 0)
        {
            Destroy(gameObject);
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

} // class DestroyOverTime 
// namespace

//---------------------------------------------------------
// Script temporizador para activar varios elementos
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Script que sirve como temporizador para que, una vez activado, pase el tiempo y después
/// se activen varios objetos u scripts.
/// Destruye el GameObject con este script tras acabar el temporizador.
/// 
/// Se usa despues de observar una acción en el Tutorial
/// </summary>
public class EnableObjectsOverTime : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Tiempo que ha de pasar para activar los objetos
    /// </summary>
    [SerializeField]
    private float TimeToActivate = 2f;

    /// <summary>
    /// Objetos que serán activados al pasar suficiente tiempo
    /// </summary>
    [SerializeField]
    private GameObject[] ActivateGameObjects;

    /// <summary>
    /// Scripts concretos que serán activados al pasar suficiente tiempo
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
    /// Tiempo transcurrido desde que se activó el objeto
    /// </summary>
    private float _t = 0;

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
    /// Se llama despues del awake si el componente esta activo.
    /// Se añade al delegado del GameManager para actualizar _slowMultiplier
    /// </summary>
    private void Start()
    {
        if (GameManager.HasInstance()) GameManager.Instance.OnTimeScaleChanged += OnTimeScaleChanged;
    }

    /// <summary>
    /// Se llama cada frame.
    /// Lleva el temporizador y activa los elementos.
    /// </summary>
    private void Update()
    {
        _t += Time.deltaTime * _slowMultiplier;

        if (_t > TimeToActivate)
        {
            foreach (GameObject GameObject in ActivateGameObjects)
            {
                if (GameObject != null) GameObject.SetActive(true);
            }

            foreach (MonoBehaviour script in Scripts)
            {
                if (script != null) script.enabled = true;
            }

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

} // class EnableObjectsOverTime 
// namespace

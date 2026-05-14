//---------------------------------------------------------
// Maneja animación de Dany
// Samuel Asensio Torres
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Script modular que únicamente se encarga de ciclar un array de sprites
/// para simular una animación de forma manual, respetando 
/// el multiplicador de tiempo del GameManager (para la habilidad).
/// </summary>
public class DanyAnimator : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Array de sprites que conforman los frames de la animación de Dany tocando el piano.
    /// </summary>
    [SerializeField]
    private Sprite[] SpritesTocando;

    /// <summary>
    /// Tiempo en segundos que transcurre entre cada cambio de frame de la animación.
    /// </summary>
    [SerializeField]
    private float TiempoEntreFrames = 0.5714f;

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
    /// Referencia al componente SpriteRenderer del GameObject.
    /// </summary>
    private SpriteRenderer _sr;

    /// <summary>
    /// Temporizador interno para llevar la cuenta del tiempo transcurrido entre frames.
    /// </summary>
    private float _timer;

    /// <summary>
    /// Índice del frame que se está renderizando actualmente del array SpritesTocando.
    /// </summary>
    private int _frameActual = 0;

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
    /// Se llama al inicializar el objeto.
    /// Obtiene y almacena la referencia al SpriteRenderer para modificar sus sprites posteriormente.
    /// </summary>
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
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
    /// Se llama en cada frame.
    /// Actualiza el temporizador y cambia el sprite cuando se alcanza el tiempo definido,
    /// teniendo en cuenta la habilidad de cámara lenta del GameManager si este existe en la escena.
    /// </summary>
    private void Update()
    {
        if (SpritesTocando == null || SpritesTocando.Length == 0) return;

        // Respeta la habilidad de tiempo
         _timer += Time.deltaTime * _slowMultiplier;
        

        if (_timer >= TiempoEntreFrames)
        {
            _timer = 0f;
            _frameActual = (_frameActual + 1) % SpritesTocando.Length;
            _sr.sprite = SpritesTocando[_frameActual];
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

} // class DanyAnimator 
// namespace

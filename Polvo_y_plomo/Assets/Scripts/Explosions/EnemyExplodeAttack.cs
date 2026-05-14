//---------------------------------------------------------
// Componente para que un enemigo con CanExplode explote al acercarse al jugador.
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Componente para que un enemigo que se acerca al jugador y debe explotar cuando pase de Chasing a Attacking lo haga.
/// Contiene un parámetro de "FuzeTime" que indica cuanto tiempo ha de estar en estado "Attacking" para que explote (evitando que explote en 1 solo frame).
/// El gameobject con este script ha de tener CanExplode y ChasePlayer.
/// </summary>
public class EnemyExplodeAttack : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Tiempo que tardará el enemigo en explotar una vez ha parado
    /// </summary>
    [SerializeField]
    private float FuzeTime = 0.1f;

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
    /// Componente CanExplode que ha de tener este gameobject.
    /// Inicializado en el Awake()
    /// </summary>
    private CanExplode _canExplode;

    /// <summary>
    /// Componente ChasePlayer que ha de tener este gameobject.
    /// Inicializado en el Awake()
    /// </summary>
    private ChasePlayer _chasePlayer;

    /// <summary>
    /// Variable booleana para registrar si actualmente estamos persiguiendo
    /// </summary>
    private bool _isChasing = true;

    /// <summary>
    /// Variable booleana para registrar si antes estabamos persiguiendo
    /// </summary>
    private bool _wasChasing = true;

    /// <summary>
    /// Temporizador que aumenta hasta FuzeTime para determinar si el enemigo explota o no.
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
    /// Se llama al cargarse en escena.
    /// Hace comprobaciones necesarias para el componente.
    /// </summary>
    private void Awake()
    {
        _canExplode = GetComponent<CanExplode>();
        if (_canExplode == null)
        {
            Debug.Log("EnemyExplodeAttack puesto en un gameobject sin CanExplode. No funcionará");
            Destroy(this);
        }

        _chasePlayer = GetComponent<ChasePlayer>();
        if (_chasePlayer == null)
        {
            Debug.Log("EnemyExplodeAttack puesto en un gameobject sin ChasePlayer. No funcionará");
            Destroy(this);
        }
    }

    /// <summary>
    /// Se llama después del awake si el componente esta activo.
    /// Se añade al delegado del GameManager para actualizar _slowMultiplier
    /// </summary>
    private void Start()
    {
        if (GameManager.HasInstance()) GameManager.Instance.OnTimeScaleChanged += OnTimeScaleChanged;
    }

    private void Update()
    {
        _isChasing = _chasePlayer.IsChasing();
        
        if (_wasChasing) // aun persigue
        {
            if (!_isChasing) // iniciar FuzeTime
            {
                _t = 0;
                _wasChasing = false;
            }
        }
        else // estaba parado
        {
            if (_isChasing) // a vuelto a perseguir
            {
                _wasChasing = true;
            }
            else // esta quieto intentando explotar
            {
                _t += Time.deltaTime * _slowMultiplier;

                if (_t >= FuzeTime) // explota
                {
                    ExplodeOnDestroy _explodeOnDestroy = GetComponent<ExplodeOnDestroy>();
                    if (_explodeOnDestroy != null) _explodeOnDestroy.DisableExplosion();
                    _canExplode.Explode();
                }
            }
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

} // class EnemyExplodeAttack 
// namespace

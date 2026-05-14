//---------------------------------------------------------
// Tercer patrón del jefe de Suzie, de tirar un par de dinamitas a un objetivo dependiendo del terreno de juego
// Miguel Gómez García
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
// Añadir aquí el resto de directivas using


/// <summary>
/// Script del tercer patrón del jefe Suzie que gestiona el lanzamiento de dos dinamitas
/// Mediante la tag de barriles comprobará cuantos hay en escena y determinará en base a eso los objetivos de la dinamita
/// Si hay dos o mas barriles, las dinamitas iran a las coberturas. 
/// Si hay solo un barril, una dinamita irá a esa cobertura y la otra al jugador
/// Si no quedan coberturas ambas dinamitas irán siempre al jugador
/// </summary>
public class SuziesThirdPattern : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Un array que almacenará el número de barriles que habrá en escena
    /// </summary>
    [SerializeField]
    GameObject[] Barrels;

    /// <summary>
    /// Contadir hacia cobertura
    /// </summary>
    [SerializeField]
    float Contador1 = 0.5f;

    /// <summary>
    /// Contador hacia jugador
    /// </summary>
    [SerializeField]
    float Contador2 = 2f;

    /// <summary>
    /// Prefab de la dinamita que lanzará Suzie
    /// </summary>
    [SerializeField]
    private GameObject DynamitePrefab;

    /// <summary>
    /// Vector que se añadira de offset al spawn de la dinamita, desde la posición de Suzie.
    /// </summary>
    [SerializeField]
    private Vector3 DynaSpawnOffset;

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
    /// Almacena la posición del primer objetivo de la primera dinamita
    /// </summary>
    private Vector3 _firstTarget;

    /// <summary>
    /// Almacena la posición del segundo objetivo de la segunda dinamita
    /// </summary>
    private Vector3 _secondTarget;

    /// <summary>
    /// Booleano que indica que hay más de dos barriles 
    /// </summary>
    private bool _manyBarrels = false;

    /// <summary>
    /// Booleano que indica que hay menos de dos barriles
    /// </summary>
    private bool _lessThanTwo = false;

    /// <summary>
    /// Prefab de la dinamita que lanzará Suzie
    /// </summary>
    private float _tFirstDyna = 0f;

    /// <summary>
    /// Almacena el HeatlhChanger de Suzie para evitar que reciba daño durante este patrón
    /// </summary>
    private Health _suzieHealthChanger;

    /// <summary>
    /// Variable para determinar a donde se lanzarán las dinamitas
    /// </summary>
    private Transform _player;

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
    /// Inicializa el componente.
    /// </summary>
    private void Awake()
    {
        _suzieHealthChanger = this.GetComponent<Health>();
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
    /// En el update dependiendo de los booleanos que se hayan marcado como true, lanzaremos la segunda dinamita con el tiempo configurable al objetivo
    /// </summary>
    void Update()
    {
        _tFirstDyna += Time.deltaTime * _slowMultiplier;

        if (_manyBarrels && _tFirstDyna > Contador1)
        {
            ThrowSecondGrenade();
            FinalizarPatron();
        }
        else if (_lessThanTwo && _tFirstDyna > Contador2) // Si había menos de dos barriles la segunda dinamita siempre irá al jugador
        {
            _secondTarget = _player.position;
            ThrowSecondGrenade();
            FinalizarPatron();
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

    /// <summary>
    /// Localizaremos los objetos con el tag de barril y los guardaremos en un array
    /// Luego dependiendo de si hay más de una cobertura o no, determianremos a que barriles aleatorios y distintos lanzaremos la dinamita
    /// Si no hay más coberturas esa dinamita ira a la posición del jugador
    /// Se marcaran con booleanos cada situación para posteriormente el tiempo entre lanzamientos y se lanzará la primera dinamita
    /// </summary>
    public void IniciarPatron()
    {
        _suzieHealthChanger.BlockDamage(); // Suzie no podrá recibir daño mientras se ejecuta el patrón
        _manyBarrels = false; 
        _lessThanTwo = false;

        _player = LevelManager.Instance.PlayerTransform();

        Barrels = Barrels.Where(b => b != null).ToArray();

        if (Barrels.Length >= 2) // Si hay más de dos barriles se lanzará una dinamita de manera aleatoria a dos de los barriles sin que se repita
        {
            _manyBarrels = true;

            int r1 = UnityEngine.Random.Range(0, Barrels.Length);
            int r2 = r1;
            while (r2 == r1)
            {
                r2 = UnityEngine.Random.Range(0, Barrels.Length);
            }

            _firstTarget = Barrels[r1].transform.position;
            _secondTarget = Barrels[r2].transform.position;
        }
        else if (Barrels.Length == 1)  // Si hay exactamente un barril, se lanza una dinamita al barril localizado
        {
            _firstTarget = Barrels[0].transform.position;
        }
        else // De lo contrario se lanzan a la posición del jugador
        {
            _firstTarget = _player.position;
        }
        if (!_manyBarrels) _lessThanTwo = true;
        ThrowFirstGrenade();
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)


    /// <summary>
    /// Metodo para lanzar la primera dinamita e iniciar el contador desde que la ha lanzado
    /// </summary>
    private void ThrowFirstGrenade()
    {
        ThrowGrenadeTo(_firstTarget);
        _tFirstDyna = 0;
    }

    /// <summary>
    /// Metodo para lanzar la seguda dinamita
    /// </summary>
    private void ThrowSecondGrenade()
    {
        ThrowGrenadeTo(_secondTarget);
    }

    /// <summary>
    /// Lanza la granada instanciando el prefab de la dinamita
    /// </summary>
    private void ThrowGrenadeTo(Vector3 targetPos)
    {
        if (DynamitePrefab != null)
        {
            GameObject dynamite = Instantiate(DynamitePrefab, transform.position + DynaSpawnOffset, Quaternion.identity);

            MoveToCoordsAndExplode moveScript = dynamite.GetComponent<MoveToCoordsAndExplode>();

            if (moveScript != null)
            {
                moveScript.SetFinalPosition(targetPos);
            }
        }
    }

    /// <summary>
    /// Termina el ataque permitiendo recibir daño nuevamente y lo reporta al Manager
    /// </summary>
    private void FinalizarPatron()
    {
        GetComponent<SuziePhaseManager>().ReportarAtaqueTerminado();
    }

    /// <summary>
    /// Método que se delegará al GameManager para actualizar el _slowMultiplier.
    /// </summary>
    /// <param name="newScale"></param>
    private void OnTimeScaleChanged(float newScale)
    {
        _slowMultiplier = newScale;
    }
    #endregion
}
// class SuziesThirdPattern 
// namespace

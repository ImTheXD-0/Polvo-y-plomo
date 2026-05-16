//---------------------------------------------------------
// Controlador de disparo de enemigo que hace una ráfaga de ataques.
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class EnemyBurstShootingAttack : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Define cuantos disparos se van a disparar
    /// </summary>
    [SerializeField]
    private int BurstShotAmmount = 3;

    /// <summary>
    /// Define el ángulo completo en el que se realizarán los disparos, espaciados equitativamente.
    /// </summary>
    [SerializeField]
    private float CompleteBurstAngle = 60f;

    /// <summary>
    /// Define el tiempo que pasa entre disparo y disparo.
    /// Por ende, y junto a BurstShotAmmount, define que tan rápido se recorre el CompleteBurstAngle.
    /// </summary>
    [SerializeField]
    private float TimeBetweenShots = 0.35f;

    /// <summary>
    /// Define el tiempo que pasa entre rafaga disparada y rafaga disparada.
    /// </summary>
    [SerializeField]
    private float TimeBetweenBursts = 4.5f;

    /// <summary>
    /// Determina el sentido de reloj en el que se realiza el giro de la rafaga
    /// </summary>
    [SerializeField]
    private bool ClockwiseBurst = false;

    /// <summary>
    /// Define cuantas veces se van a hacer las oleadas de "burst"
    /// Es decir, con 1 simplemente hace una pasada de disparos completa.
    /// Con 2, haría dos pasadas, omitiendo disparar en el borde inicial del ángulo barrido.
    /// Para 3 o más haría ese número de pasadas, y siempre entre pasadas evitará el primer disparo (para no solapar disparos con la anterior pasada)
    /// 
    /// Es decir, este parámetro consigue que se hagan más pasadas de disparo, "repitiendo" la rafaga configurada anteriormente.
    /// </summary>
    [SerializeField]
    private int BurstWavesAmmount = 1;

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
    /// Almacena el componente Weapon (pensado para que sea Shoot) Shoot que ha de tener este componente.
    /// </summary>
    private Weapon _shoot;

    /// <summary>
    /// Almacena el componente _chasePlayer que ha de tener uno de los objetos padres del gameobject con el componente.
    /// </summary>
    private ChasePlayer _chasePlayer;

    /// <summary>
    /// Almacena el rotateTowardsObject que ha de tener el gameobject padre de este objeto (su RotateBody).
    /// Tomará momentaneamente el control de este script para hacer el giro de la rafaga.
    /// </summary>
    private rotateTowardsObject _rotateBody;

    /// <summary>
    /// Variable booleana que determina si el enemigo esta persiguiendo actualmente.
    /// </summary>
    private bool _isChasing = true;

    /// <summary>
    /// Variable booleana que determina si el enemigo ya ha empezado una ráfaga.
    /// </summary>
    private bool _isShooting = false;

    /// <summary>
    /// Temporizador que almacena cuanto tiempo lleva de ráfaga. Va aumentando desde que empieza una. 
    /// </summary>
    private float _t = 0;

    /// <summary>
    /// Temporizador que almacena cuanto tiempo ha transcurrido desde que se acabo la última ráfaga
    /// Inicializado en el Awake()
    /// </summary>
    private float _timeSinceLastBurst;

    /// <summary>
    /// Contador de disparos de ráfaga realizados
    /// </summary>
    private int _shotsTaken = 0;

    /// <summary>
    /// Almacena el ángulo inicial al empezar una ráfaga.
    /// </summary>
    private float _initialAngle;

    /// <summary>
    /// Número que variará entre 1 y -1, para realizar las varias pasadas de las rafagas.
    /// </summary>
    private int _actualAngleSignMultiplier = 1;

    /// <summary>
    /// Contador que almacena cuantas pasadas de ráfaga se han realizado en el ataque actual.
    /// </summary>
    private int _totalWavesDone = 0;

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

    private void Awake()
    {
        _shoot = GetComponent<Weapon>();
        if (_shoot == null)
        {
            Debug.Log("EneyBurstShootingAttack puesto en un gameobject sin componente tipo Weapon. No funcionará");
            Destroy(this);
        }

        _chasePlayer = transform.root.GetComponent<ChasePlayer>(); // se revisa en el objeto más "grande", el "mayor padre"
        if (_shoot == null)
        {
            Debug.Log("EneyBurstShootingAttack puesto en un gameobject cuyo mayor padre no tiene el componente ChasePlayer. No funcionará");
            Destroy(this);
        }

        _rotateBody = GetComponentInParent<rotateTowardsObject>();
        if (_shoot == null)
        {
            Debug.Log("EneyBurstShootingAttack puesto en un gameobject cuyo padre directo no tiene componente RotateBody. No funcionará");
            Destroy(this);
        }

        _timeSinceLastBurst = TimeBetweenBursts + 1; // para que pueda empezar a atacar de inmediato; si no se quiere se modifica
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
    /// Se llama cada frame, mientras el componente este activo.
    /// </summary>
    private void Update()
    {
        if (_isShooting) // esta en una rafaga
        {
            _t += Time.deltaTime * _slowMultiplier;

            // Rotación del RotateBody
            // calculo el angulo actual
            // (_t / ( (BurstShotAmmount-1) * TimeBetweenShots)) devuelve un valor entre el 0 y el 1 que indica "porcentaje de angulo realizado"
            // multiplicar este valor por CompleteBurstAngle me pasa ese valor a un angulo de intervalo [0, CompleteBurstAngle]
            // ahora quiero que sea desde el angulo registrado _initialAngle entre [-CompleteBurstAngle/2, CompleteBurstAngle/2] por lo que le resto CompleteBurstAngle/2 y se lo sumo al angulo inicial
            float angulo = (_t / ( (BurstShotAmmount-1) * TimeBetweenShots)) * CompleteBurstAngle - CompleteBurstAngle / 2;
            if (ClockwiseBurst) angulo *= -1;
            angulo *= _actualAngleSignMultiplier;
            angulo += _initialAngle;



             _rotateBody.SetAngle(angulo);

            // Disparo de rafaga
            if (_t >= TimeBetweenShots * _shotsTaken)
            {
                _shotsTaken++;
                _shoot.Use(new Vector2(Mathf.Cos(angulo * Mathf.Deg2Rad), Mathf.Sin(angulo * Mathf.Deg2Rad)));

                // Rafaga acabada
                if (_shotsTaken >= BurstShotAmmount)
                {
                    // Contado de pasadas
                    _totalWavesDone++;

                    // Todas las ráfagas acabadas
                    if (_totalWavesDone >= BurstWavesAmmount)
                    {
                        // Establecer el estado actual para intentar empezar otro ataque
                        _timeSinceLastBurst = 0;
                        _isShooting = false;

                        // Reactivar otros componentes controlados
                        _chasePlayer.enabled = true;
                        _rotateBody.enabled = true;
                    }
                    else // Hay que hacer otra ráfaga
                    {
                        // Reinicio de la rafaga para hacer otra
                        _actualAngleSignMultiplier *= -1;
                        _shotsTaken = 1;
                        _t = 0;
                    }
                }
            }
        }
        else // aun no ha empezado ráfaga
        {
            _timeSinceLastBurst += Time.deltaTime * _slowMultiplier;

            _isChasing = _chasePlayer.IsChasing();
            if (!_isChasing && _timeSinceLastBurst >= TimeBetweenBursts) // si esta atacando
            {
                // Se para el ChasePlayer -> se deja de registrar si el enemigo persigue o no.
                _chasePlayer.enabled = false;

                // Se para el RotateBody para manejarlo desde este script
                _rotateBody.enabled = false;

                // Setup del nuevo ataque
                _isShooting = true;
                _t = 0;
                _shotsTaken = 0;
                _totalWavesDone = 0;
                _actualAngleSignMultiplier = 1;

                // Registrar el angulo inicial del ataque para rotar respecto a este
                _initialAngle = _rotateBody.GetAngle();
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

} // class EnemyBurstShootingAttack 
// namespace

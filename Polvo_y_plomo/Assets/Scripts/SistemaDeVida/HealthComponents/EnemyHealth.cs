//---------------------------------------------------------
// Clase heredada de Health que maneja la vida de los enemigos
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Clase heredada de Health que maneja la vida de los enemigos
/// Este objeto debería tener CanFlash si lo necesita (por tener más de 1 PV).
/// Opcionalmente se le puede añadir un sonido de muerte.
/// </summary>
public class EnemyHealth : Health
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Sonido de muerte opcional para cuando muera el enemigo.
    /// </summary>
    [SerializeField]
    private AudioClip DeathSound;

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
    /// Componente CanFlash que puede tener opcionalmente el enemigo.
    /// </summary>
    private CanFlash _canFlash;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Override del método Awake() de Health.
    /// Inicializa el componente.
    /// </summary>
    protected override void Awake()
    {
        _canFlash = GetComponent<CanFlash>();
        base.Awake();
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
    /// Override del método CambiarVida() de Health.
    /// Intenta hacer los flashes con CanFlash, incluyendo lógica para que pueda hacerlo si es EnemySpawnLogic.
    /// </summary>
    /// <param name="cambio"></param>
    public override void CambiarVida(int cambio = -1)
    {
        if (cambio < 0)
        {
            if (_canFlash != null) _canFlash.StartFlashes();
            else // Para que los enemigos puedan flashear durante la animacion de spawn
            {
                EnemySpawnLogic enemySpawn = GetComponent<EnemySpawnLogic>();
                if (enemySpawn != null)
                {
                    _canFlash = GetComponentInChildren<CanFlash>();
                    if (_canFlash != null) _canFlash.StartFlashes();
                }
            }
        }
        base.CambiarVida(cambio);
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Override del MetodoMuerte() de Health.
    /// Intenta marcar la muerte de un enemigo, dar puntos y ejecutar el sonido opcional de muerte.
    /// </summary>
    protected override void MetodoMuerte()
    {
        // Llamada a EnemyDied para que actualice habilidad y cantidad de enemigos registrada
        IsEnemy isenemy = GetComponent<IsEnemy>();
        if (isenemy != null) isenemy.EnemyDied();

        // Dar puntos
        PointsOnDeath points = GetComponent<PointsOnDeath>();
        if (points != null) points.GivePoints();

        if (DeathSound != null && AudioManager.HasInstance()) AudioManager.Instance.Play(DeathSound, transform.position);

        base.MetodoMuerte();
    }

    #endregion   

} // class EnemyHealth 
// namespace

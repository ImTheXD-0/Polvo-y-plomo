//---------------------------------------------------------
// Clase heredada de Health que maneja la vida de los jefes
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Clase heredada de Health que maneja la vida de los jefes
/// El GameObject que tenga este objeto debería tener IsEnemy, PointsOnDeath, CanFlash...
/// Especialmente ha de tener SuzieHealthBar, componente por ahora generico para las barras de vida de jefe.
/// </summary>
public class BossHealth : Health
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField]
    private bool IsFinalBoss = true;

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
    /// Componente de Barra de vida que ha de tener un jefe.
    /// </summary>
    private SuzieHealthBar _suzieHealthBar;

    /// <summary>
    /// Componente CanFlash opcional que puede tener un jefe.
    /// </summary>
    private CanFlash _canFlash;

    /// <summary>
    /// Almacena la vida maxima configurada en el editor.
    /// ACTUALMENTE sirve solo en el caso de Suzie, ya que su vida cambia dependiendo de la dificultad.
    /// Inicializada en el Awake();
    /// </summary>
    private int _originalMaxHealth;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Override del Awake() de Health.
    /// Hace comprobaciones necesarias para el componente y lo inicializa.
    /// </summary>
    protected override void Awake()
    {
        _originalMaxHealth = VidaMax;
        _suzieHealthBar = GetComponent<SuzieHealthBar>();
        if (_suzieHealthBar == null)
        {
            Debug.Log("BossHealth colocado en componente sin SuzieHealthBar. No funcionará por no poder representar la vida del jefe");
            Destroy(this);
        }
        _canFlash = GetComponent<CanFlash>();
        base.Awake();
    }

    /// <summary>
    /// Se llama al cargarse en escena si el objeto esta activo, o al activarse por primera vez.
    /// Actualiza las Stats que dependen de la dificultad para el jefe.
    /// 
    /// Añade el método al delegado del DifficultyManager.
    /// </summary>
    private void Start()
    {
        if (DifficultyManager.HasInstance()) DifficultyManager.Instance.OnDifficultyChanged += UpdateDifficultyStats;
        UpdateDifficultyStats();
    }

    /// <summary>
    /// Se llama al destruirse el componente.
    /// Elimina el método del delegado del DifficultyManager.
    /// </summary>
    private void OnDestroy()
    {
        if (DifficultyManager.HasInstance()) DifficultyManager.Instance.OnDifficultyChanged -= UpdateDifficultyStats;
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
    /// Inicia flashes si es posible y actualiza la barra de vida del HUD.
    /// </summary>
    /// <param name="cambio"></param>
    public override void CambiarVida(int cambio = -1)
    {
        if (_canFlash != null && cambio < 0) _canFlash.StartFlashes();

        base.CambiarVida(cambio);
        _suzieHealthBar.UpdateHealthBar(VidaMax, _vida);
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
    /// Intenta marcar que ha muerto un enemigo, dar puntos e informar al GameManager del final del juego.
    /// </summary>
    protected override void MetodoMuerte()
    {
        // Llamada a EnemyDied para que actualice habilidad y cantidad de enemigos registrada
        IsEnemy isenemy = GetComponent<IsEnemy>();
        if (isenemy != null) isenemy.EnemyDied();

        // Dar puntos
        PointsOnDeath points = GetComponent<PointsOnDeath>();
        if (points != null) points.GivePoints();

        if (GameManager.HasInstance())
        {
            if (IsFinalBoss) GameManager.Instance.GameEnds();
            else
            {
                GameManager.Instance.LevelEnds();
            }
        }

        base.MetodoMuerte();
    }

    /// <summary>
    /// Método para actualizar las stats de este componente que dependan de la dificultad.
    /// Actualmente solo incluye los cambios para Suzie.
    /// </summary>
    private void UpdateDifficultyStats()
    {
        if (DifficultyManager.HasInstance())
        {
            // la vida actual se cambia a la que se tendria sin modificadores de dificultad
            int difference = VidaMax - _originalMaxHealth;
            _vida -= difference;

            // cambiamos VidaMax y vida para incluir la vida de esta dificultad
            VidaMax = _originalMaxHealth + DifficultyManager.Instance.GetSuzieHealthAdded();
            _vida += DifficultyManager.Instance.GetSuzieHealthAdded();

            // actualizar el hud de la vida
            SuzieHealthBar healthBar = GetComponent<SuzieHealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(VidaMax, _vida);
            }

            // en el cambio la vida puede disminuir y volverse menor que 0, será necesario actualizar 
            if (_vida <= 0)
            {
                MetodoMuerte();
                if (GameManager.HasInstance())
                {
                    GameManager.Instance.GameEnds();
                }
            }
        }
    }

    #endregion

} // class BossHealth 
// namespace

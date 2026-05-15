//---------------------------------------------------------
// Clase heredada de Health que maneja la vida para el jugador
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Clase heredada de Health que maneja la vida del jugador.
/// Se le deberían configurar sonidos de daño y curación para su correcto
/// funcionamiento.
/// 
/// </summary>
public class PlayerHealth : Health
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Sonido que sonará cuando el jugador reciba daño.
    /// </summary>
    [SerializeField]
    private AudioClip SonidoDanyo;

    /// <summary>
    /// Sonido que sonará cuando el jugador se cure
    /// </summary>
    [SerializeField]
    private AudioClip SonidoCura;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Override del Awake() de Health.
    /// Hace comprobaciones necesarias para este componente.
    /// </summary>
    protected override void Awake()
    {
        if (SonidoDanyo == null) Debug.Log("PlayerHealth colocado sin sonido de daño. No sonará");
        if (SonidoCura == null) Debug.Log("PlayerHealth colocado sin sonido de cura. No sonará");

        base.Awake();
    }

    /// <summary>
    /// Se llama al cargarse en la escnea, si esta activo, o al activarse por primera vez.
    /// Inicializa la vida del jugador.
    /// </summary>
    private void Start()
    {
        if (GameManager.HasInstance()) _vida = GameManager.Instance.InitHealthChanger();
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
    /// Override del método para CambiarVida() de Health.
    /// Este lo remplaza casi por completo por conflictos en el orden de la llamada de los métodos.
    /// Su intención es solo actualizar el HUD del jugador.
    /// </summary>
    /// <param name="cambio"></param>
    public override void CambiarVida(int cambio = -1)
    {
        // Evito llamar a la base ya que se llamaria antes de tiempo al método de muerte
        if (!_canRecieveDamage && cambio < 0) return;

        _vida += cambio;
        if (_vida > VidaMax) // si se da curación y se excede el máximo de vida
            _vida = VidaMax;

        // Actualizar HUD del jugador o sonido de bloqueo de cobertura
        if (GameManager.HasInstance())
        {
            GameManager.Instance.UpdatePlayerHealth(_vida);
        }

        if (AudioManager.HasInstance() && SonidoDanyo && cambio < 0) AudioManager.Instance.Play(SonidoDanyo, transform.position);
        else if (AudioManager.HasInstance() && SonidoCura && cambio > 0) AudioManager.Instance.Play(SonidoCura, transform.position);

        // Muerte del objeto
        if (_vida <= 0)
        {
            MetodoMuerte();
        }
    }

    /// <summary>
    /// Con este metodo podremos saber si la vida del jugador es igual o mayor a la vida máxima
    /// Con eso podremos determinar si puede ser curado por objetos o no
    /// </summary>
    public bool CuracionPermitida()
    {
        if (_vida < VidaMax) return true;
        else return false;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Override del MetodoMuerte() de Health para el jugador.
    /// Lo reemplaza por completo, solo destruyendo el componente y no el GameObject, e intentando Respawnear al jugador.
    /// </summary>
    protected override void MetodoMuerte()
    {
        if (GameManager.HasInstance()) GameManager.Instance.Respawn();

        InvokeOnDeath();

        Destroy(this);
    }
    #endregion

} // class PlayerHealth 
// namespace

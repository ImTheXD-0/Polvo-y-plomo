//---------------------------------------------------------
// Clase heredada de Health que maneja la vida de las coberturas
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Clase heredada de Health que maneja la vida de las coberturas
/// Se le deberían de asignar sonidos de daño a cobertura y de cobertura rota.
/// Opcionalmente puede tener CanFlash el gameobject con este script.
/// 
/// </summary>
public class CoverHealth : Health
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Sonido que sonará al ser dañada la cobertura
    /// </summary>
    [SerializeField]
    private AudioClip SoundCoberturaDanyo;

    /// <summary>
    /// Sonido que sonará al ser rota la cobertura.
    /// </summary>
    [SerializeField]
    private AudioClip SoundCoberturaRota;

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
    /// Componente CanFlash que podría tener el gameobject de cobertura.
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
    /// Hace comprobaciones necesarias para el componente y lo inicializa.
    /// </summary>
    protected override void Awake()
    {
        if (SoundCoberturaDanyo == null) Debug.Log("CoverHealth colocado sin sonido de daño. No sonará");
        if (SoundCoberturaRota == null) Debug.Log("CoverHealth colocado sin sonido de destrucción. No sonará");

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
    /// Intenta ejecutar el sonido de daño a cobertura e iniciar los flashes.
    /// </summary>
    /// <param name="cambio"></param>
    public override void CambiarVida(int cambio = -1)
    {
        if (cambio < 0)
        {
            if (SoundCoberturaDanyo && _vida > 0) AudioManager.Instance.Play(SoundCoberturaDanyo, transform.position);
            if (_canFlash != null) _canFlash.StartFlashes();
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
    /// Intenta hacer sonar el audio de cobertura rota.
    /// </summary>
    protected override void MetodoMuerte()
    {
        if (SoundCoberturaRota) AudioManager.Instance.Play(SoundCoberturaRota, transform.position);
        base.MetodoMuerte();
    }

    #endregion

} // class CoverHealth 
// namespace

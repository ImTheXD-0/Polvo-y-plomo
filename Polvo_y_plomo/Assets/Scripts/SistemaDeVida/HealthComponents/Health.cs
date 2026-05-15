//---------------------------------------------------------
// Gestor de vida
// Miguel Gómez García
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System;
using DG.Tweening.Core.Easing;
using UnityEngine;

/// <summary>
/// Script que permite gestionar la vida de los gameObjets
/// Permite sumar y restar vida mediante un metodo general que pide la cantidad a restar/sumar
/// También permite establecer la vida inicial del gameObject y bloquear si puede o no recibir daño
/// Mediante el método de muerte se puede controlar lo que sucedera con el gameObject cuando su vida llegue a cero o menos
/// </summary>
public class Health : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Esta será la variable de la vida con la que iniciarán los gameObject. Debe ser configurable para ajustarse a cada caso especificó y no variará una vez establecida
    /// </summary>
    [SerializeField]
    protected int VidaMax = 10;

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
    /// Esta será la variable de la vida que tendrán los game objects (irá variando)
    /// </summary>
    protected int _vida;


    /// <summary>
    /// Un booleano que determinará si podemos recibir daño o no 
    /// </summary
    protected bool _canRecieveDamage = true;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Se llama al cargarse en la escena de inmediato.
    /// Establece la vida.
    /// </summary>
    protected virtual void Awake()
    {
        _vida = VidaMax;
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
    /// Metodo que permitirá que no nos hagan daño
    /// </summary
    public void BlockDamage()
    {
        _canRecieveDamage = false;
    }

    /// <summary>
    /// Metodo que permitirá que nos hagan daño
    /// </summary
    public void AllowDamage()
    {
        _canRecieveDamage = true;
    }

    /// <summary>
    /// Por lo general todos los ataques harán uno de daño, pero si te curas, no te puedes curar más del maximo de lo que se te permite
    /// Este metodo permitirá curarse (teniendo como tope la vida con la que empiezas) y hacer daño hasta quedarte sin vida
    /// Si te quedas sin vida llamara al metodo para matar
    /// </summary>
    public virtual void CambiarVida(int cambio = -1)      
    {
        if (!_canRecieveDamage && cambio < 0) return;

        _vida += cambio;
        if (_vida > VidaMax) // si se da curación y se excede el máximo de vida
            _vida = VidaMax;


        // Muerte del objeto
        if (_vida <= 0)
        {
            MetodoMuerte();
        }
    }

    /// <summary>
    /// Método público que devuelve la vida actual del GameObject. Utilizado principalmente para transicionar fases en enemigos grandes (Suzie).
    /// </summary>
    public int GetCurrentHealth()
    {
        return _vida;
    }

    /// <summary>
    /// Método público que devuelve la vida maxima del GameObject. Utilizado principalmente para transicionar fases en enemigos grandes (Suzie).
    /// </summary>
    public int GetMaxHealth()
    {
        return VidaMax;
    }

    public event Action OnDeath;

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Comprueba si el gameObject tiene cadaver, si lo tiene lo genera el cadaver.
    /// Después destruye el objeto.
    /// </summary>
    protected virtual void MetodoMuerte()
    {
        // spawn del cadaver
        if (GetComponent<GeneraCadaver>() != null)
        {
            GeneraCadaver genCad = GetComponent<GeneraCadaver>();
            genCad.PonCadaver();
        }
        else Debug.Log("Este Objeto no tiene un componente GeneraCadaver");

        // llamadas del OnDeath
        InvokeOnDeath();

        Destroy(gameObject);
    }

    /// <summary>
    /// Método necesario para llamar a los métodos del OnDeath.
    /// Al ser un evento público no se puede usar en otras clases, es necesario invocarlo así.
    /// </summary>
    protected void InvokeOnDeath()
    {
        if (OnDeath != null) OnDeath.Invoke();
    }
    #endregion
}

//---------------------------------------------------------
// Script sencillo para explotar un CanExplode al morir el objeto
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Hace explotar el CanExplode, que ha de tener el gameobject en el que se pone este componente, al
/// morir el objeto.
/// 
/// +++
/// Añadida funcionalidad para que se incluya un método que evite que explote. Esto es ya que
/// otros componentes pueden querer hacer que CanExplode se active pero que el ExplodeOnDestroy no lo haga,
/// ya que generaria 2 explosiones.
/// </summary>
public class ExplodeOnDeath : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

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
    /// Almacena el componente CanExplode que ha de tener el gameobject.
    /// Inicializado en el Awake()
    /// </summary>
    private CanExplode _canExplode;

    /// <summary>
    /// Almacena el componente tipo Health que ha de tener el gameobject.
    /// Inicializado en el awake().
    /// </summary>
    private Health _health;

    /// <summary>
    /// Variable booleana que maneja si el objeto debe explotar en el OnDeath.
    /// </summary>
    private bool _explode = true;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Se llama al cargarse en la escena.
    /// Hace comprobaciones necesarias para el componente.
    /// Se añade el método DoExplosionOnDeath al delegado OnDeath del componente Heatlh
    /// </summary>
    private void Awake()
    {
        _canExplode = GetComponent<CanExplode>();
        if (_canExplode == null)
        {
            Debug.Log("ExplodeOnDeath puesto en un gameobject sin CanExplode. No funcionará");
            Destroy(this);

        }
        else
        {
            _health = GetComponent<Health>();
            if (_health == null)
            {
                Debug.Log("ExplodeOnDeath puesto en un gameobject sin compotente tipo Health. No funcionará");
                Destroy(this);
            }
            else _health.OnDeath += DoExplosionOnDeath;
        }
    }

    /// <summary>
    /// Al destruirse el objeto elimina su método del OnDeath del health.
    /// </summary>
    private void OnDestroy()
    {
        if (_health != null) _health.OnDeath -= DoExplosionOnDeath;
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
    /// Activa la explosión del OnDestroy.
    /// </summary>
    public void EnableExplosion()
    {
        _explode = true;
    }

    /// <summary>
    /// Desactiva la explosion del OnDestroy.
    /// </summary>
    public void DisableExplosion()
    {
        _explode = false;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Realiza la explosión si es posible
    /// </summary>
    private void DoExplosionOnDeath()
    {
        if (_explode) _canExplode.Explode();
    }
    #endregion

} // class ExplodeOnDestroy 
// namespace

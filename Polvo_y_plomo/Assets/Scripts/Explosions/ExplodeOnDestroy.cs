//---------------------------------------------------------
// Script sencillo para explotar un CanExplode al destruirse el objeto
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Hace explotar el CanExplode, que ha de tener el gameobject en el que se pone este componente, al
/// ser destruido el objeto.
/// 
/// +++
/// Añadida funcionalidad para que se incluya un método que evite que explote. Esto es ya que
/// otros componentes pueden querer hacer que CanExplode se active pero que el ExplodeOnDestroy no lo haga,
/// ya que generaria 2 explosiones.
/// </summary>
public class ExplodeOnDestroy : MonoBehaviour
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
    /// Variable booleana que maneja si el objeto debe explotar en el OnDestroy.
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
    /// </summary>
    private void Awake()
    {
        _canExplode = GetComponent<CanExplode>();
        if (_canExplode == null)
        {
            Debug.Log("ExplodeOnDestroy puesto en un componente sin CanExplode. No funcionará");
            Destroy(this);

        }
    }

    /// <summary>
    /// Al destruirse el objeto (normalmente interpretado como morir) explota.
    /// 
    /// +++
    /// Ahora solo explota si _explode es true.
    /// </summary>
    private void OnDestroy()
    {
        if (_explode && _canExplode != null ) _canExplode.Explode();
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

    #endregion

} // class ExplodeOnDestroy 
// namespace

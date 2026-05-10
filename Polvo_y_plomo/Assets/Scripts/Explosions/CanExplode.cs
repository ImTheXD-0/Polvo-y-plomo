//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class CanExplode : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>
    /// Este es el GameObject que se instanciará pasado el tiempo. Hará daño al colisionar con objetos.
    /// </summary>
    [SerializeField]
    private onCollisionDealDamage Explosion;

    /// <summary>
    /// Sonido que se dará al iniciarse la explosion
    /// </summary>
    [SerializeField]
    private AudioClip ExplosionSound;

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
    /// Se llama al cargarse en escena.
    /// Realiza comprobaciones necesarias para el componente.
    /// </summary>
    private void Awake()
    {
        if (Explosion == null)
        {
            Debug.Log("Componente CanExplode puesto sin asignarle objeto de explosion. No funcionará");
            Destroy(this);
        }
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
    /// Método público para iniciar la explosión.
    /// Causará la destrucción de este objeto, y se instanciará el objeto asignado de explosion en la misma posicion.
    /// </summary>
    public void Explode()
    {
        Instantiate(Explosion, transform.position, transform.rotation);
        if (AudioManager.HasInstance()) AudioManager.Instance.Play(ExplosionSound, transform.position);
        Destroy(gameObject);
    }

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion   

} // class CanExplode 
// namespace

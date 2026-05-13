//---------------------------------------------------------
// Breve script que verifica si se ha liberado la acción de melee para destruir el GameObject.
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Breve script que verifica si se ha liberado la acción de melee para destruir el GameObject.
/// También tiene en cuenta si se esta presionando la acción de "Exit", para poder borrarlo en caso de cancelar la acción.
/// 
/// Usado para destruir la sombra del ataque a melee.
/// </summary>
public class DestroyOnMeleeRelease : MonoBehaviour
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

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    private void Start()
    {
        if (!InputManager.HasInstance())
        {
            Debug.Log("DestroyOnMeleeRelease puesto en una escena sin InputManager. Se destruirá el objeto de inmediato");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Llamado cada frame si el componente esta activo.
    /// </summary>
    private void Update()
    {
        if (InputManager.Instance.MeleeWasReleasedThisFrame() || InputManager.Instance.ExitWasPressedThisFrame()) Destroy(gameObject);
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

    #endregion   

} // class DestroyOnMeleeRelease 
// namespace

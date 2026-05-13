//---------------------------------------------------------
// Controlador de la rotación del barril del HUD
// Ángel Seijas de Ema
// Polvo y plomo
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Threading;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Controlador de la rotación del barril del HUD.
/// Permite acumular rotaciones para realizarlas con el tiempo, evitando que las rotaciones se queden a medias
/// </summary>
public class BarrelAnimatorController : MonoBehaviour
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
    /// Almacena el animator del barril, que debe estar situado en este mismo GameObject.
    /// Inicializado en el Awake().
    /// </summary>
    private Animator barrelAnimator;

    /// <summary>
    /// Contador para saber cuantas rotaciones Anticlockwise faltan por hacer
    /// </summary>
    private int AnticlockwiseRotations = 0;

    /// <summary>
    /// Contador para saber cuantas rotaciones Clockwise faltan por hacer
    /// </summary>
    private int ClockwiseRotations = 0;

    /// <summary>
    /// Variable booleana para registrar que tipo de rotación ha sido el añadido.
    /// De esta forma se decidirá que tipo de rotación se realizará primero, en caso de que haya de ambos tipos en la cola.
    private bool LastUpdateWasAnticlockwise = true;

    /// <summary>
    /// Variable booleana que registra si es posible realizar una animación actualmente.
    /// </summary>
    private bool FreeToDoRotations = true;
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Se llama al cargarse en escena.
    /// Hace comprobaciones necesarias para el componente.
    /// </summary>
    private void Awake()
    {
        barrelAnimator = GetComponent<Animator>();
        if (barrelAnimator == null)
        {
            Debug.Log("BarrelAinmatorController puesto en un gameobject sin Animator. No podrá realizar las animaciones");
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
    /// Método para añadir una rotacion Anticlockwise a la cola de animaciones por realizar
    /// Intentará realizarla si es posible.
    /// </summary>
    public void AddAnticlockwiseRotation()
    {
        AnticlockwiseRotations++;
        LastUpdateWasAnticlockwise = true;
        CheckToDoRotations();
    }

    /// <summary>
    /// Método para añadir una rotacion Clockwise a la cola de animaciones por realizar
    /// Intentará realizarla si es posible.
    /// </summary>
    public void AddClockwiseRotation()
    {
        ClockwiseRotations++;
        LastUpdateWasAnticlockwise = false;
        CheckToDoRotations();
    }

    /// <summary>
    /// Intenta iniciar rotaciones según los booleanos y cantidad de rotaciones restantes por hacer.
    /// </summary>
    public void CheckToDoRotations()
    {
        if (FreeToDoRotations)
        {
            if (LastUpdateWasAnticlockwise) // revisar primero Anticlockwise
            {
                if (AnticlockwiseRotations > 0)
                {
                    PlayAnticlockwiseRotation();
                }
                else if (ClockwiseRotations > 0)
                {
                    PlayClockwiseRotation();
                }
            }
            else // revisar primero Clockwise
            {
                if (ClockwiseRotations > 0)
                {
                    PlayClockwiseRotation();
                }
                else if (AnticlockwiseRotations > 0)
                {
                    PlayAnticlockwiseRotation();
                }
            }
        }
    }

    /// <summary>
    /// Método que se ha de llamar siempre que se salga de un estado de animación para
    /// que se puedan realizar más.
    /// </summary>
    public void ExitDetected()
    {
        FreeToDoRotations = true;
        CheckToDoRotations();
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Método para iniciar una rotación Anticlockwise.
    /// </summary>
    private void PlayAnticlockwiseRotation()
    {
        FreeToDoRotations = false;
        AnticlockwiseRotations--;
        barrelAnimator.Play("RevolverAntiClock");
    }

    /// <summary>
    /// Método para iniciar una rotación Clockwise.
    /// </summary>
    private void PlayClockwiseRotation()
    {
        FreeToDoRotations = false;
        ClockwiseRotations--;
        barrelAnimator.Play("RevolverClock");
    }

    #endregion

} // class BarrelAnimatorController 
// namespace

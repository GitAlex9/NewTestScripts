using UnityEngine;
using System.Collections;

public abstract class InterfaceState : MonoBehaviour
{
    /// <summary>
    /// Chamado quando a interface entra neste estado
    /// </summary>
    public virtual void Enter(InterfaceManager manager)
    {
        // Override em classes derivadas
    }

    /// <summary>
    /// Chamado todo frame enquanto a interface está neste estado
    /// </summary>
    public virtual void UpdateState(InterfaceManager manager)
    {
        // Override em classes derivadas
    }

    /// <summary>
    /// Chamado quando a interface sai deste estado.
    /// Use coroutine para animações de saída.
    /// </summary>
    public virtual IEnumerator Exit(InterfaceManager manager)
    {
        // Override em classes derivadas
        yield return null;
    }
}

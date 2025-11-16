using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Classe base para qualquer estado de UI que necessite de navegação por teclado/gamepad.
/// Gerencia a seleção inicial de um botão e a ação de "submit" (clique) com a tecla de interação.
/// </summary>
public abstract class InterfaceStateNavegavel : InterfaceState
{
    [Header("Configuração de Navegação")]
    [Tooltip("O primeiro botão ou elemento de UI que será selecionado quando esta interface for ativada.")]
    [SerializeField] protected Selectable primeiroElementoSelecionado;

    /// <summary>
    /// Chamado quando a interface entra neste estado.
    /// Seleciona o primeiro elemento, muda o contexto de input para UI e se inscreve no evento de interação.
    /// </summary>
    public override void Enter(InterfaceManager manager)
    {
        base.Enter(manager);

        if (primeiroElementoSelecionado != null)
        {
            primeiroElementoSelecionado.Select();
        }

        GameManager.Instance.ChangeInputEventContext(InputEventContext.UI);
        GameManager.Instance.OnInteractPress += HandleSubmit;
    }

    /// <summary>
    /// Chamado quando a interface sai deste estado.
    /// Limpa a seleção, retorna o contexto de input para o padrão e cancela a inscrição no evento.
    /// </summary>
    public override IEnumerator Exit(InterfaceManager manager)
    {

        GameManager.Instance.ChangeInputEventContext(InputEventContext.DEFAULT);
        GameManager.Instance.OnInteractPress -= HandleSubmit;
        
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        yield return base.Exit(manager);
    }

    /// <summary>
    /// Lida com o evento de 'submit' (ex: tecla E).
    /// Simula um clique no botão ou elemento de UI atualmente selecionado pelo EventSystem.
    /// </summary>
    protected virtual void HandleSubmit(InputEventContext context)
    {
        if (context != InputEventContext.UI) return;

        GameObject objetoSelecionado = EventSystem.current.currentSelectedGameObject;
        if (objetoSelecionado != null)
        {
            Button botao = objetoSelecionado.GetComponent<Button>();
            if (botao != null)
            {
                botao.onClick.Invoke();
            }
        }
    }
}
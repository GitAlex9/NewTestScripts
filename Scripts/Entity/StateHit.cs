using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "EntityState/StateHit", fileName = "StateHit")]
public class StateHit : EntityState
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDistance = 1.5f;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float knockbackUpForce = 5f; // Força vertical para jogar para cima
    
    [Header("Juice Settings")]
    [SerializeField] private bool enableScalePunch = true;
    [SerializeField] private float punchScale = 0.5f;
    [SerializeField] private float punchDuration = 5f;
    
    [SerializeField] private bool enableMaterialFlash = true;
    [SerializeField] private Color flashColor = new Color(1f, 0.3f, 0.3f, 1f); // Vermelho
    [SerializeField] private float flashDuration = 0.15f;
    
    [SerializeField] private bool enableRotationShake = false;
    [SerializeField] private float shakeStrength = 5f;
    [SerializeField] private float shakeDuration = 0.1f;
    
    public override void Enter(Entity entity)
    {
        base.Enter(entity);
        
        entity.animationController?.TriggerAnimation("GetHit");

        // Aplicar knockback
        ApplyKnockback(entity);
        
        // Aplicar efeitos de juice
        ApplyHitJuice(entity);

        // Desbloquear e sair do state após o knockback terminar
        if (entity.stats.Health > 0) entity.PopStateAfter(knockbackDuration);
        else entity.TransitionToStateAfter(entity.deadState, knockbackDuration * 0.8f);
    }
    
    private void ApplyKnockback(Entity entity)
    {
        // Calcular direção do knockback (contrária ao atacante)
        Vector3 knockbackDirection = (entity.transform.position - entity.lastDamageTaken.damageOwner.transform.position).normalized;
        
        // Knockback horizontal usando DOTween (apenas X e Z, preservando Y)
        Vector3 currentPosition = entity.transform.position;
        Vector3 horizontalKnockback = new Vector3(
            knockbackDirection.x * knockbackDistance,
            0, // Não mexer no Y
            knockbackDirection.z * knockbackDistance
        );
        
        // Verificar se há colisão no caminho do knockback
        float actualKnockbackDistance = knockbackDistance;
        RaycastHit hit;
        
        // Raycast do centro da entidade na direção do knockback
        Vector3 rayOrigin = currentPosition + Vector3.up * 0.5f; // Meio do personagem
        Vector3 rayDirection = new Vector3(knockbackDirection.x, 0, knockbackDirection.z).normalized;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, knockbackDistance, LayerMask.GetMask("Default", "Ground", "Wall")))
        {
            // Se há uma parede no caminho, reduzir a distância do knockback
            actualKnockbackDistance = Mathf.Max(0.1f, hit.distance - 0.2f); // Deixar um espaço de 0.2 da parede
            
            horizontalKnockback = new Vector3(
                knockbackDirection.x * actualKnockbackDistance,
                0,
                knockbackDirection.z * actualKnockbackDistance
            );
        }
        
        Vector3 targetPosition = currentPosition + horizontalKnockback;
        
        // Animar apenas X e Z usando DOTween
        entity.transform.DOMoveX(targetPosition.x, knockbackDuration).SetEase(Ease.OutQuad);
        entity.transform.DOMoveZ(targetPosition.z, knockbackDuration).SetEase(Ease.OutQuad);
        
        // Knockback vertical usando ApplyJumpForce do EntityMovement
        if (entity.movementComponent != null && knockbackUpForce > 0)
        {
            entity.movementComponent.ApplyJumpForce(knockbackUpForce);
        }
    }

    public override void UpdateState(Entity entity)
    {
        base.UpdateState(entity);
    }

    public override void Exit(Entity entity)
    {
        base.Exit(entity);
    }
    
    private void ApplyHitJuice(Entity entity)
    {
        // Scale Punch - Efeito de "squeeze" quando leva hit
        if (enableScalePunch)
        {
            // Matar apenas tweens de escala usando SetId
            DOTween.Kill("scalePunch_" + entity.GetInstanceID());
            entity.transform.localScale = Vector3.one;
            
            entity.transform.DOPunchScale(Vector3.one * punchScale, punchDuration, 5, 0.5f)
                .SetId("scalePunch_" + entity.GetInstanceID()) // ID único por entidade
                .SetEase(Ease.InOutElastic)
                .OnComplete(() => {
                    // Garantir que volta para escala 1,1,1
                    entity.transform.localScale = Vector3.one;
                });
        }
        
        // Material Flash - Flash de cor quando leva hit
        if (enableMaterialFlash)
        {
            ApplyMaterialFlash(entity);
        }
        
        // Rotation Shake - Tremida na rotação (opcional)
        if (enableRotationShake)
        {
            entity.transform.DOShakeRotation(shakeDuration, new Vector3(0, shakeStrength, 0), 10, 90f);
        }
    }
    
    private void ApplyMaterialFlash(Entity entity)
    {
        // Procurar todos os Renderers na entidade
        Renderer[] renderers = entity.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"[StateHit] Nenhum Renderer encontrado em {entity.name}");
            return;
        }
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            
            foreach (Material mat in materials)
            {
                // Verificar se o material tem a propriedade _Color
                if (mat.HasProperty("_Color"))
                {
                    Color originalColor = mat.color;
                    
                    // Flash para a cor configurada
                    mat.DOColor(flashColor, flashDuration * 0.5f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            // Voltar para a cor original
                            mat.DOColor(originalColor, flashDuration * 0.5f)
                                .SetEase(Ease.InQuad);
                        });
                }
                // Tentar propriedade alternativa _BaseColor (URP)
                else if (mat.HasProperty("_BaseColor"))
                {
                    Color originalColor = mat.GetColor("_BaseColor");
                    
                    mat.DOColor(flashColor, "_BaseColor", flashDuration * 0.5f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            mat.DOColor(originalColor, "_BaseColor", flashDuration * 0.5f)
                                .SetEase(Ease.InQuad);
                        });
                }
            }
        }
    }
}

using UnityEngine;

public class Card : MonoBehaviour
{
    [Tooltip("ID da carta (p.ex. 'cat', 'dog'). Use igual nas duas cartas do par ou use Tags.")]
    public string cardId; // opcional, mais robusto que depender só de Tag

    [Tooltip("Se true, esta carta já foi casada e não pode ser clicada.")]
    public bool matched = false;

    // Helper: retorna todos os renderers filhos (inclui o próprio)
    public Renderer[] GetRenderers()
    {
        return GetComponentsInChildren<Renderer>();
    }

    // Marca visualmente como matched (verde) e desativa colisores
    public void MarkMatched()
    {
        
        matched = true;

        // pinta todos os renderers — usa material (instância) para não afetar outros objetos
        var rends = GetRenderers();
        foreach (var r in rends)
        {
            if (r != null)
            {
                // garante instância do material para essa renderer
                r.material.color = Color.green;
            }
        }

        // desativa todos os colliders (filhos também)
        var cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols)
            c.enabled = false;
    }

    private void Update()
    {
        
    }
}

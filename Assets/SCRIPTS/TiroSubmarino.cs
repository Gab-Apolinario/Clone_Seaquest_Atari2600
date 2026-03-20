using UnityEngine;

public class TiroSubmarino : MonoBehaviour
{
    [SerializeField] private float velocidadeTiro;
    [SerializeField] private float tempoVidaTiro;
    [SerializeField] private bool irParaEsquerda;
    
     void Start()
    {
        Destroy(gameObject, tempoVidaTiro);
    }
    private void Update()
    {        
        if (irParaEsquerda)
        {
            transform.Translate(Vector2.left * velocidadeTiro * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * velocidadeTiro * Time.deltaTime);
        }
    }

    public void AtirarEsquerda()
    {
        irParaEsquerda = true;
    }
}

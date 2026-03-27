using UnityEngine;

public class Peixe : BaseInimigo
{

    public enum TipoPeixe
    {
        Dificuldade1,
        Dificuldade2,
        Dificuldade3
    }

    private TipoPeixe tipoPeixe;

    protected override void Start()
    {
        Seguranças();
        TipoPeixeConfigurar(tipoPeixe);
        
        base.Start(); // Chama o Start da classe base (BaseInimigo) para 
                      // garantir que a lógica de movimento e destruição funcione corretamente.
        pontos = 20; // Define os pontos específicos para o Peixe
    }

    void TipoPeixeConfigurar(TipoPeixe tipo)
    {
        if (multiplicadorVelocidade > 2f) //segunda rodada com sucesso
        {
            tipo = Random.Range(0, 3) switch
            {
                0 => TipoPeixe.Dificuldade1,
                1 => TipoPeixe.Dificuldade2,
                2 => TipoPeixe.Dificuldade3,
            };
        }
        else if (multiplicadorVelocidade >= 1f) //primeira rodada com sucesso
        {
            tipo = Random.Range(0, 3) switch
            {
                0 => TipoPeixe.Dificuldade1,
                1 => TipoPeixe.Dificuldade2,
            };
        }
        else //primeira
        {
            tipo = TipoPeixe.Dificuldade1;
        }

        switch (tipo)
        {
            case TipoPeixe.Dificuldade1:
                velocidade = 2f;
                spriteRenderer.color = Color.yellow;
                break;
            case TipoPeixe.Dificuldade2:
                velocidade = 2.5f;
                spriteRenderer.color = Color.green;
                break;
            case TipoPeixe.Dificuldade3:
                velocidade = 3f;
                spriteRenderer.color = Color.red;
                break;
        }
    }

}

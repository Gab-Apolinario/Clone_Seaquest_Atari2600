using UnityEngine;

public class Submarino : BaseInimigo
{
    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base (BaseInimigo) para 
                      // garantir que a lógica de movimento e destruição funcione corretamente.
        pontos = 20; // Define os pontos específicos para o Peixe
    }
}

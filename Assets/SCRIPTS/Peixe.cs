using UnityEngine;

public class Peixe : BaseInimigo
{
    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base (BaseInimigo) para 
                      // garantir que a lógica de movimento e destruição funcione corretamente.
    }

}

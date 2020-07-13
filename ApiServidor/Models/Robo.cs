namespace InterfacexD.Models
{
    public class Robo : Objeto
    {
        public override string RepresentacaoVisual { get { return "R"; } }
        
        public Robo(int id, int posicaoX, int posicaoY) : base(id, posicaoX, posicaoY) { }
    }
}

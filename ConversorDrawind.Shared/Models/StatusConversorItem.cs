namespace ConversorDrawind
{
    public class StatusConversorItem
    {
        string nome;
        string pasta;

        public StatusConversorItem(string nome, string pasta)
        {
            Nome = nome;
            Pasta = pasta;
        }

        public string Nome { get => nome; set => nome = value; }
        public string Pasta { get => pasta; set => pasta = value; }
    }
}




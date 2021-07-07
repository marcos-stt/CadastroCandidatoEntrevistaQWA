using System;

#nullable enable
namespace CadastroCandidato
{
    public class Pessoa
    {
        private DateTime _DataNasc;

        public string Nome { get; private set; }
        public string Sobrenome { get; private set; }
        public string CPF { get; private set; }

        public DateTime DataDeNascimento {
            get { return _DataNasc.Date; }
            private set { _DataNasc = value.Date; }
        }

        public int Idade { get => (int)(DateTime.Now.Date - _DataNasc.Date).TotalDays / 365; }
        public bool Maior { get => Idade >= 18; }

        public Pessoa(string nome, string sobrenome, string cpf, DateTime dataDeNascimento) {
            Nome = nome;
            Sobrenome = sobrenome;
            CPF = cpf;
            DataDeNascimento = dataDeNascimento;
        }
    }
}
#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FluentValidation;


namespace CadastroCandidato
{
    public class Pessoa
    {
        private DateTime _DataNasc;

        public String Nome { get; set; }
        public String Sobrenome { get; set; }
        public String CPF { get; set; }

        public DateTime DataDeNascimento {
            get { return this._DataNasc; }
            set { this._DataNasc = value.Date; }
        }

        public int Idade { get { return (int)(DateTime.Now.Date - this._DataNasc).TotalDays / 365; }}
        public bool Maior { get { return this.Idade >= 18; } }
    }

    public class PessoaValidator : AbstractValidator<Pessoa>
    {
        private static Regex RegexDigitos = new Regex(@"\D*");
        public PessoaValidator() {
            RuleFor(pessoa => pessoa.Nome).NotEmpty();
            RuleFor(pessoa => pessoa.Sobrenome).NotEmpty();
            RuleFor(pessoa => pessoa.CPF).NotEmpty().Matches(@"\D*(?:\d\D*){11}")
                .Must(cpf => CpfValidator(cpf)).WithMessage("CPF inválido");
            RuleFor(pessoa => pessoa.DataDeNascimento).NotEmpty();
            RuleFor(pessoa => pessoa.Idade).GreaterThan(0);
        }

        static bool CpfValidator(String cpf) {
            cpf = RegexDigitos.Replace(cpf, "");

            // TODO: validação de CPF
            return true;
        }
    }
}
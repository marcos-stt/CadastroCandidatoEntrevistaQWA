using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;

#nullable enable
namespace CadastroCandidato
{
    public class PessoaForm
    {
        public String? Nome { get; set; }
        public String? Sobrenome { get; set; }
        public String? CPF { get; set; }
        public DateTime? DataDeNascimento { get; set; }

        public IEnumerable<ValidationFailure>? Validar(out Pessoa? validado) {
            var validador = new PessoaFormValidator();
            var resultado = validador.Validate(this);

            if(resultado.IsValid) {
                validado = new Pessoa(
                    nome: Nome!,
                    sobrenome: Sobrenome!,
                    cpf: CPF!,
                    dataDeNascimento: (DateTime)DataDeNascimento!
                );
                return null;
            } else {
                validado = null;
                return resultado.Errors;
            }
        }
    }

    public class PessoaFormValidator : AbstractValidator<PessoaForm>
    {
        private readonly static Regex RegexDigitos = new(@"\D*");
        public PessoaFormValidator()
        {
            RuleFor(pessoa => pessoa.Nome).NotEmpty();
            RuleFor(pessoa => pessoa.Sobrenome).NotEmpty();
            RuleFor(pessoa => pessoa.CPF).NotEmpty().Matches(@"\D*(?:\d\D*){11}")
                .Must(cpf => CpfValidator(cpf))
                .WithMessage("CPF inválido");
            RuleFor(pessoa => pessoa.DataDeNascimento).NotEmpty()
                .Must(data => (DateTime.Now.Date - data)?.TotalDays > 0)
                .WithMessage("Data de nascimento no futuro");
        }

        static bool CpfValidator(String? cpf)
        {
            if(cpf is null) {
                return true;
            }
            
            cpf = RegexDigitos.Replace(cpf, "");

            // TODO: validação de CPF
            return true;
        }
    }
}
#nullable disable
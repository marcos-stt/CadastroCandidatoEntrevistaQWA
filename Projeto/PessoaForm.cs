using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;

#nullable enable
namespace CadastroCandidato
{
    public class PessoaForm
    {
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? CPF { get; set; }
        public DateTime? DataDeNascimento { get; set; }

        public IEnumerable<ValidationFailure>? Validar(out Pessoa? validado) {
            var validador = new PessoaFormValidator();
            var resultado = validador.Validate(this);

            if(resultado.IsValid) {
                validado = new Pessoa(
                    nome: Nome!,
                    sobrenome: Sobrenome!,
                    cpf: PessoaFormValidator.RegexDigitos.Replace(CPF!, ""),
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
        public readonly static Regex RegexDigitos = new(@"\D*");

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

        static bool CpfValidator(string? cpf)
        {
            if(cpf is null) {
                return true;
            }

            // Fontes: https://www.geradorcpf.com/algoritmo_do_cpf.htm https://www.somatematica.com.br/faq/cpf.php
            cpf = RegexDigitos.Replace(cpf, "");

            if(cpf.Length != 11) {
                return false;
            }

            var digitos = cpf.ToCharArray().Select(digito => (int)char.GetNumericValue(digito)).ToList();

            var verificacao = new int[] { 0, 0 };
            
            for(int i = 0; i < 9; i ++) {
                verificacao[0] += digitos[i] * (10 - i);
            };
            for(int i = 0; i < 10; i ++) {
                verificacao[1] += digitos[i] * (11 - i);
            };

            for(int i = 0; i < 2; i++) {
                verificacao[i] %= 11;
                if(verificacao[i] < 2) {
                    verificacao[i] = 0;
                } else {
                    verificacao[i] = 11 - verificacao[i];
                }
            }

            for(int i = 0; i < 2; i++) {
                if (verificacao[i] != digitos[9 + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#nullable disable
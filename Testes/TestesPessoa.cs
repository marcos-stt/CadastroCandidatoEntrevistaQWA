using System;
using System.Collections.Generic;
using NUnit.Framework;
using CadastroCandidato;

namespace Testes
{
    public class TestesPessoa
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValidacaoCamposObrigatorios()
        {
            var pessoa = new PessoaForm
            {
                Nome = null,
                Sobrenome = null,
                CPF = null,
                DataDeNascimento = null,
            };

            var resultado = pessoa.Validar(out var validado);

            Assert.IsNotNull(resultado);
            Assert.IsNull(validado);

            var camposIncorretos = new HashSet<string>();

            foreach(var erro in resultado) {
                camposIncorretos.Add(erro.PropertyName);
            }

            Assert.IsTrue(camposIncorretos.SetEquals(new HashSet<string> {
                "Nome",
                "Sobrenome",
                "CPF",
                "DataDeNascimento",
            }));
        }

        [Test]
        public void ValidacaoCpfFormatoIncorreto()
        {
            var pessoa = new PessoaForm
            {
                Nome = "João",
                Sobrenome = "Silva",
                CPF = "TESTE",
                DataDeNascimento = DateTime.Now.AddYears(-18),
            };

            var resultado = pessoa.Validar(out var validado);

            Assert.IsNotNull(resultado);
            Assert.IsNull(validado);

            var camposIncorretos = new HashSet<string>();

            foreach (var erro in resultado)
            {
                camposIncorretos.Add(erro.PropertyName);
            }

            Assert.IsTrue(camposIncorretos.Contains("CPF"));
        }

        [Test]
        public void ValidacaoDtNascNoFuturo()
        {
            var pessoa = new PessoaForm
            {
                Nome = "João",
                Sobrenome = "Silva",
                CPF = "220.074.270-36", // CPF gerado por https://www.4devs.com.br/gerador_de_cpf
                DataDeNascimento = DateTime.Now.AddYears(1),
            };

            var resultado = pessoa.Validar(out var validado);

            Assert.IsNotNull(resultado);
            Assert.IsNull(validado);

            var camposIncorretos = new HashSet<string>();

            foreach (var erro in resultado)
            {
                camposIncorretos.Add(erro.PropertyName);
            }

            Assert.IsTrue(camposIncorretos.Contains("DataDeNascimento"));
        }

        [Test]
        public void ValidacaoOk()
        {
            var dtNasc18 = DateTime.Now.AddYears(-18);
            var pessoa = new PessoaForm
            {
                Nome = "João",
                Sobrenome = "Silva",
                CPF = "220.074.270-36",
                DataDeNascimento = dtNasc18,
            };

            var resultado = pessoa.Validar(out var validado);
            var esperado = new Pessoa(
                nome: "João",
                sobrenome: "Silva",
                cpf: "220.074.270-36",
                dataDeNascimento: dtNasc18
            );

            Assert.IsNull(resultado);
            Assert.IsNotNull(validado);

            Assert.AreEqual(esperado.Nome, validado.Nome);
            Assert.AreEqual(esperado.Sobrenome, validado.Sobrenome);
            Assert.AreEqual(esperado.CPF, validado.CPF);
            Assert.AreEqual(esperado.DataDeNascimento, validado.DataDeNascimento);
        }

        [Test]
        public void IdadeMaiorMenor()
        {
            var pessoaMaior = new Pessoa(
                nome: "João",
                sobrenome: "Silva",
                cpf: "220.074.270-36",
                dataDeNascimento: DateTime.Now.AddYears(-18).AddMonths(-1)
            );
            var pessoaMenor = new Pessoa(
                nome: "João",
                sobrenome: "Silva",
                cpf: "220.074.270-36",
                dataDeNascimento: DateTime.Now.AddYears(-18).AddMonths(1)
            );

            Assert.AreEqual(18, pessoaMaior.Idade);
            Assert.AreEqual(17, pessoaMenor.Idade);
            Assert.IsTrue(pessoaMaior.Maior);
            Assert.IsFalse(pessoaMenor.Maior);
        }
    }
}
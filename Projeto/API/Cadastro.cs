using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace CadastroCandidato.API
{
    [Route("api/cadastro")]
    public class CadastroController: Controller
    {
        public int Limite { get; set; }
        public int LimiteTotal { get; set; }
        private NpgsqlConnection Connection { get; set; }

        public CadastroController(NpgsqlConnection conn)
        {
            Connection = conn;
        }

        private async Task AtualizarLimite()
        {
            LimiteTotal = 10;

            await using var cmd = new NpgsqlCommand("SELECT COUNT(id) FROM candidato", Connection);
            var cadastrado = (int)(long)await cmd.ExecuteScalarAsync();
            Limite = Math.Max(0, LimiteTotal - cadastrado);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] IEnumerable<PessoaForm> candidatos)
        {
            await AtualizarLimite();

            if(Limite < candidatos.Count()) {
                return "LIMITE";
            }

            var validados = new List<Pessoa>();

            foreach(var candidato in candidatos) {
                if(candidato.Validar(out var validado) is null && validado is not null) {
                    validados.Add(validado);
                } else {
                    return "VALIDACAO";
                }
            }

            var cpfsDuplicados = new List<String>();
            await using (var transaction = await Connection.BeginTransactionAsync()) {
                try {
                    foreach(var validado in validados) {
                        await using var cmd = new NpgsqlCommand(@"
                            WITH Inserted AS (
                                INSERT INTO candidato(nome, sobrenome, cpf, dtnasc)
                                VALUES (@nome, @sobrenome, @cpf, @dtnasc)
                                ON CONFLICT DO NOTHING
                                RETURNING id
                            )
                            SELECT COUNT(*) <> 0 FROM Inserted
                        ", Connection);
                        cmd.Parameters.Add(new NpgsqlParameter("nome", validado.Nome));
                        cmd.Parameters.Add(new NpgsqlParameter("sobrenome", validado.Sobrenome));
                        cmd.Parameters.Add(new NpgsqlParameter("cpf", validado.CPF));
                        cmd.Parameters.Add(new NpgsqlParameter("dtnasc", validado.DataDeNascimento.Date));
                        var cadastrado = (bool)await cmd.ExecuteScalarAsync();
                        if(!cadastrado) {
                            cpfsDuplicados.Add(validado.CPFFormatado);
                        }
                    }
                    if(cpfsDuplicados.Count != 0) {
                        await transaction.RollbackAsync();
                        return $"DUPLICADO: {string.Join(',', cpfsDuplicados)}";
                    }
                    await transaction.CommitAsync();
                } catch {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return "OK";
        }
    }
}

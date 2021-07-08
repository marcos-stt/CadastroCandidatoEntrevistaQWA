using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace CadastroCandidato.Web
{
    public class ListaModel : PageModel
    {
        public List<Pessoa> Candidatos {get; set;} = new List<Pessoa>();
        private NpgsqlConnection Connection { get; set; }

        public ListaModel(NpgsqlConnection conn) {
            Connection = conn;
        }
        public async Task OnGet()
        {
            await using var cmd = new NpgsqlCommand("SELECT nome, sobrenome, cpf, dtnasc FROM candidato", Connection);
            await using var reader = await cmd.ExecuteReaderAsync(); while (await reader.ReadAsync())
            {
                Candidatos.Add(new Pessoa(
                    nome: reader.GetString(0),
                    sobrenome: reader.GetString(1),
                    cpf: reader.GetString(2),
                    dataDeNascimento: reader.GetDateTime(3).Date
                ));
            }
        }
    }
}

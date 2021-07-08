using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace CadastroCandidato.Web
{
    public class CadastroModel : PageModel
    {
        public int Limite {get; set;}
        public int LimiteTotal { get; set; }
        private NpgsqlConnection Connection { get; set; }

        public CadastroModel(NpgsqlConnection conn)
        {
            Connection = conn;
        }

        private async Task AtualizarLimite() {
            LimiteTotal = 15;

            await using var cmd = new NpgsqlCommand("SELECT COUNT(id) FROM candidato", Connection);
            var cadastrado = (int)(long)await cmd.ExecuteScalarAsync();
            Limite = Math.Max(0, LimiteTotal - cadastrado);
        }

        public async Task OnGet()
        {
            await AtualizarLimite();
        }
    }
}

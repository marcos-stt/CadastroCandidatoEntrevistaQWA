using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CadastroCandidato.Web
{
    public class CadastroModel : PageModel
    {
        public int Limite {get; set;}
        public int LimiteTotal { get; set; }

        private void AtualizarLimite() {
            LimiteTotal = 15;
            Limite = 15;
        }
        
        public void OnGet()
        {
            AtualizarLimite();
        }
        public void OnPost()
        {
            AtualizarLimite();
        }
    }
}

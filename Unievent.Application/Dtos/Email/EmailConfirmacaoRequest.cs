using System;
using System.Collections.Generic;
using System.Text;

namespace Unievent.Application.Dtos.Email
{
    public class EmailConfirmacaoRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Chave { get; set; } = string.Empty;
    }
}

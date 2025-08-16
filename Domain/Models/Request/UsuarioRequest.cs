using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Request
{
    public class UsuarioRequest
    {
        public string Cpf { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Senha { get; set; } = default!;
    }
}
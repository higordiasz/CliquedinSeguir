using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliquedinSeguir.Models.Retorno
{
    public class Retorno
    {
        public int Status { get; set; }
        public string Response { get; set; }
    }

    public class RetornoData
    {
        public int Status { get; set; }
        public string Response { get; set; }
        public string Gender { get; set; }
    }
}

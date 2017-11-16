using System;
using System.ComponentModel.DataAnnotations;

namespace EscolaModelo.Models
{
    public partial class Aluno
    {
        [Key]
        public int Id { get; set; }

        public string CPF { get; set; }

        public string Nome { get; set; }

        public DateTime DataNasc { get; set; }

        public string NomeMae { get; set; }

        public string Bairro { get; set; }

        public string Logradouro { get; set; }

        public int Nro { get; set; }

        public string Complemento { get; set; }
    }
}
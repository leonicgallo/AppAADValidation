using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIDoctores.Models
{
    public class Doctor
    {
        public IEnumerable<string> Telefonos { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string DoctorId { get; set; }
        public string Titulo { get; set; }
        public string Celular { get; set; }
    }
}

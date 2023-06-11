using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.ErrorModels
{
    public class Result<T>
    {
        public T Value { get; set; }
        public bool Succeeded => !Errors.Any();
        public List<ErrorClass> Errors { get; set; } = new List<ErrorClass>();
    }
}

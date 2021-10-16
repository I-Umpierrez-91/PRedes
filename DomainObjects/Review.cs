using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects
{
    public class Review
    {
        private int _nota;
        public string Titulo { get; set; }
        public int Nota { 
            get {
                return _nota;
            } set
            {
                if (value > 5)
                {
                    _nota = 5;
                }
                else
                {
                    if (value < 1)
                    {
                        _nota = 1;
                    }
                    else
                    {
                        _nota = value;
                    }
                }
            }
        }

    }
}

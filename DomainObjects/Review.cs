using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects
{
    public class Review
    {
        public static int minNota = 1;
        public static int maxNota = 5;
        private int _nota;
        public string Titulo { get; set; }
        public int Nota { 
            get {
                return _nota;
            } set
            {
                if (value > maxNota)
                {
                    _nota = maxNota;
                }
                else
                {
                    if (value < minNota)
                    {
                        _nota = minNota;
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

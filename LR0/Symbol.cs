using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR0
{
    class Symbol
    {
        public string id;
        public string stringRepresentation;
        public bool isTerminal;

        public Symbol(string theId, string theStringRepresentation)
        {
            id = theId;
            stringRepresentation = theStringRepresentation;
            isTerminal = true;
        }

        public Symbol(string theId)
        {
            id = theId;
            stringRepresentation = theId;
            isTerminal = false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Symbol s = obj as Symbol;
            if ((System.Object)s == null)
            {
                return false;
            }

            return (id == s.id) && (stringRepresentation == s.stringRepresentation);
        }

        public bool Equals(Symbol s)
        {
            if ((System.Object)s == null)
            {
                return false;
            }

            return (id == s.id) && (stringRepresentation == s.stringRepresentation);
        }

        public override string ToString()
        {
            return stringRepresentation;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ stringRepresentation.GetHashCode();
        }
    }
}

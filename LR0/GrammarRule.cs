using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR0
{
    class GrammarRule
    {
        public String Name;
        public List<Symbol> Expression;

        public GrammarRule(string theName, Symbol[] theExpression)
        {
            Name = theName;
            Expression = new List<Symbol>(theExpression);
        }
        
        public Symbol GetSymbol(int Position)
        {
            return Expression[Position];
        }

        public override string ToString()
        {
            string result = "";
            result += Name;
            result += "->";
            foreach (Symbol sym in Expression)
            {
                result += sym.ToString();
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            GrammarRule s = obj as GrammarRule;
            if ((System.Object)s == null)
            {
                return false;
            }

            if (Name != s.Name)
                return false;

            if (Expression.Count != s.Expression.Count)
                return false;

            bool result = true;

            foreach (var item in s.Expression)
            {
                result = result && Expression.Contains(item);
            }

            return result;
        }

        public bool Equals(GrammarRule g)
        {
            if ((System.Object)g == null)
            {
                return false;
            }

            if (Name != g.Name)
                return false;

            if (Expression.Count != g.Expression.Count)
                return false;

            bool result = true;

            foreach (var item in g.Expression)
            {
                result = result && Expression.Contains(item);
            }

            return result;
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (Symbol sym in Expression)
            {
                result += sym.GetHashCode();
            }
            return Name.GetHashCode() ^ result;
        }
    }
}

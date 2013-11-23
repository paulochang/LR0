namespace LR0
{
    class Item
    {
        public GrammarRule Rule;
        public int Position;

        public bool isFinal()
        {
            return Position == Rule.Expression.Count; //ie: Rule with 2 symbols has a last position of 1 ... S' -> S$
        }

        public Symbol getCurrentSymbol()
        {
            if (!this.isFinal())
            {
                return Rule.GetSymbol(Position);
            }

            return null;
        }

        public Symbol getNextSymbol()
        {
            if (Rule.Expression.Count-Position>1)
            {
                return Rule.GetSymbol(Position+1);
            }

            return null;
        }

        public Item(GrammarRule theRule, int thePosition)
        {
            Rule = theRule;
            Position = thePosition;
        }

        public Item(GrammarRule theRule)
        {
            Rule = theRule;
            Position = 0;
        }

        public override string ToString()
        {
            string result = "";
            result += Rule.RuleSymbol.id;
            result += "->";
            for (int i = 0; i < Rule.Expression.Count; i++)
            {
                if (i == Position)
                {
                    result += "●";
                }
                result += Rule.Expression[i].ToString();
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Item s = obj as Item;
            if ((System.Object)s == null)
            {
                return false;
            }

            return (Rule.Equals(s.Rule)) && (Position == s.Position);
        }

        public bool Equals(Item i)
        {
            if ((System.Object)i == null)
            {
                return false;
            }

            return (Rule.Equals(i.Rule)) && (Position == i.Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Rule.GetHashCode();
        }
    }
}

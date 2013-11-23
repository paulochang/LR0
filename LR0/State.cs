using System.Collections.Generic;

namespace LR0
{
    class State
    {
        public List<Item> itemSet;

        public State()
        {
            itemSet = new List<Item>();
        }

        public State(Item[] theItems)
        {
            itemSet = new List<Item>(theItems);
        }

        public State(List<Item> theItems)
        {
            itemSet = theItems;
        }

        public bool Add(Item theItem)
        {
            if (!itemSet.Contains(theItem))
            {
                itemSet.Add(theItem);
                return true;
            }
            else return false;

        }

        public override string ToString()
        {
            string result = "";
            foreach (Item theItem in itemSet)
            {
                result += theItem.ToString() + " \n";
            }
            return result;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            State i = obj as State;
            if ((System.Object)i == null)
            {
                return false;
            }

            if (itemSet.Count != i.itemSet.Count)
                return false;

            bool result = true;

            foreach (var item in i.itemSet)
            {
                result = result && itemSet.Contains(item);
            }

            return result;
        }

        public bool Equals(State i)
        {
            if ((System.Object)i == null)
            {
                return false;
            }

            if (itemSet.Count != i.itemSet.Count)
                return false;

            bool result = true;

            foreach (var item in i.itemSet)
            {
                result = result && itemSet.Contains(item);
            }

            return result;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (Item theItem in itemSet)
            {
                hashCode += theItem.GetHashCode();
            }
            return hashCode;
        }

        public List<Item> GetForwardItems()
        {
            List<Item> ResultSet = new List<Item>();
            foreach (Item item in itemSet)
            {
                if (!item.isFinal())
                {
                    ResultSet.Add(item);
                }
            }
            return ResultSet;
        }

        public List<Item> GetGotoItems(Symbol theSymbol)
        {
            List<Item> ResultSet = new List<Item>();
            foreach (Item Production in itemSet)
            {
                if (!Production.isFinal())
                {
                    if (Production.getCurrentSymbol().Equals(theSymbol))
                    {
                        Item ItemToAdd = new Item(Production.Rule, Production.Position + 1);
                        ResultSet.Add(ItemToAdd);
                    }
                }
            }

            return ResultSet;
        }
    }
}

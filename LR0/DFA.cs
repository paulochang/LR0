using System;
using System.Collections.Generic;

using state = System.Int32;
using input = System.String;


// class based on 
//  Regular Expression Engine C# Sample Application
//  2006, by Leniel Braz de Oliveira Macaferi & Wellington Magalhães Leite.
// [[http://www.leniel.net/2009/05/regex-engine-in-csharp-dfa.html]]

namespace LR0
{
    class DFA
    {
        public state start;
        public HashSet<state> final;
        public SortedList<KeyValuePair<state, input>, state> transitionTable;

        public DFA()
        {
            start = 0;
            final = new HashSet<state>();
            transitionTable = new SortedList<KeyValuePair<state, input>, state>(new transitionComparer());
        }

        public bool simulate(List<string> @in)
        {
            state currentState = start;
            List<string>.Enumerator myEnumerator = @in.GetEnumerator();

            while (myEnumerator.MoveNext())
            {
                KeyValuePair<state, input> transition = new KeyValuePair<state, input>(currentState, myEnumerator.Current);

                if (!transitionTable.TryGetValue(transition, out currentState))
                {
                    return false;
                }
            }

            if (final.Contains(currentState))
                return true;
            else
                return false;
        }
    }

    public class transitionComparer : IComparer<KeyValuePair<state, input>>
    {
        public state Compare(KeyValuePair<state, input> x, KeyValuePair<state, input> y)
        {
            if (x.Key == y.Key)
                return x.Value.CompareTo(y.Value);
            else
                return x.Key.CompareTo(y.Key);
        }
    }
}

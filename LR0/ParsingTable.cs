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
    class ParsingTable
    {
        public state start;
        public HashSet<state> final;
        public SortedList<KeyValuePair<state, input>, Action> transitionTable;

        public ParsingTable()
        {
            start = 0;
            final = new HashSet<state>();
            transitionTable = new SortedList<KeyValuePair<state, input>, Action>(new transitionComparer());
        }

        //public void parse(List<input> theInput)
        //{
        //    theInput.Add("úEndSymbol"); //add ending signal
        //    state actualState = 0;
        //    Stack<input> theParserStack = new Stack<input>();
        //}

        //public bool simulate(List<string> @in)
        //{
        //    state currentState = start;
        //    List<string>.Enumerator myEnumerator = @in.GetEnumerator();

        //    while (myEnumerator.MoveNext())
        //    {
        //        KeyValuePair<state, input> transition = new KeyValuePair<state, input>(currentState, myEnumerator.Current);

        //        if (!transitionTable.TryGetValue(transition, out currentState))
        //        {
        //            return false;
        //        }
        //    }

        //    if (final.Contains(currentState))
        //        return true;
        //    else
        //        return false;
        //}
    }
}

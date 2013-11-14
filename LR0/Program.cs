using System;
using System.Collections.Generic;

using state = System.Int32;
using input = System.String;

namespace LR0
{
    class Program
    {

        public static int stateId = 1;

        public static int getNextStateId()
        {
            return stateId++;
        }

        public static List<GrammarRule> AugmentGrammar(List<GrammarRule> theGrammar)
        {
            List<GrammarRule> AugmentedGrammar = new List<GrammarRule>();
            GrammarRule FirstRule = theGrammar[0];
            String ExtraRuleName = FirstRule.Name + "'";
            Symbol[] ExtraRuleExpression = new Symbol[] { new Symbol(FirstRule.Name), new Symbol("úEndSymbol", "$") };

            GrammarRule ExtraRule = new GrammarRule(ExtraRuleName, ExtraRuleExpression);
            AugmentedGrammar.Add(ExtraRule);

            foreach (GrammarRule rule in theGrammar)
            {
                AugmentedGrammar.Add(rule);
            }

            return AugmentedGrammar;
        }

        public static State Closure(State theState, List<GrammarRule> theGrammar)
        {
            State ResultState = new State(theState.itemSet);
            bool hasChanged = false;
            do
            {
                hasChanged = false;
                List<Item> forwardItems = theState.GetForwardItems();
                foreach (Item theItem in forwardItems)
                {
                    Symbol currentSymbol = theItem.getCurrentSymbol();
                    List<Item> theProductions = getItemsById(currentSymbol.id, theGrammar);
                    foreach (var item in theProductions)
                    {
                        hasChanged = theState.Add(item) || hasChanged;
                    }
                }
            } while (hasChanged);
            return ResultState;
        }

        public static State Goto(State theState, Symbol inputSymbol, List<GrammarRule> theGrammar)
        {
            List<Item> generatedItems = theState.GetGotoItems(inputSymbol);
            State resultState = new State(generatedItems);

            return Closure(resultState, theGrammar);

        }
        public static List<Item> getItemsById(string theId, List<GrammarRule> theGrammar)
        {
            List<Item> resultList = new List<Item>();

            foreach (GrammarRule rule in theGrammar)
            {
                if (rule.Name == theId)
                {
                    resultList.Add(new Item(rule));
                }
            }

            return resultList;
        }

        public static DFA BuildLr0(List<GrammarRule> AugmentedGrammar)
        {
            DFA E = new DFA();
            E.start = 0;
            List<State> T = new List<State>();

            GrammarRule FirstProduction = AugmentedGrammar[0];
            State InitialState = new State();
            InitialState.Add(new Item(FirstProduction));

            InitialState = Closure(InitialState, AugmentedGrammar);
            T.Add(InitialState);
            bool hasChanged = false;

            do
            {
                hasChanged = false;
                List<State> tempState = new List<State>();
                foreach (State s in T)
                {
                    tempState.Add(s);
                }
                foreach (State I in tempState)
                {
                    foreach (Item item in I.itemSet)
                    {
                        if (!item.isFinal())
                        {
                            Symbol X = item.getCurrentSymbol();
                            State J = Goto(I, X, AugmentedGrammar);
                            int JIndex = -1;
                            if (T.Contains(J))
                            {
                                JIndex = T.IndexOf(J);
                            }
                            else
                            {
                                hasChanged = true;
                                T.Add(J);
                                JIndex = T.IndexOf(J);
                            }
                            if (X.id == "úEndSymbol")
                                E.final.Add(JIndex);
                            int Iindex = T.IndexOf(I);
                            string TransitionSymbol = X.id;
                            if (!E.transitionTable.ContainsKey(new KeyValuePair<int, string>(Iindex, TransitionSymbol)))
                                E.transitionTable.Add(new KeyValuePair<int, string>(Iindex, TransitionSymbol), JIndex);
                        }
                    }
                }
            } while (hasChanged);

            return E;
        }

        private static void printGraph(DFA theAutomaton, System.IO.StreamWriter writer)
        {
            //public SortedList<KeyValuePair<state, input>, state> transitionTable;
            foreach (KeyValuePair<KeyValuePair<state, input>, state> pair in theAutomaton.transitionTable)
            {
                writer.Write(pair.Key.Key.ToString() + " -> " + pair.Value.ToString() + " [label=" + pair.Key.Value + "];");
                if (theAutomaton.final.Contains(pair.Value)) writer.Write(pair.Value.ToString() + " [  style=filled color=\"dodgerblue\" fillcolor=\"lightyellow\" ]");
            }
        }

        static void Main(string[] args)
        {
            List<GrammarRule> Grammar = new List<GrammarRule>();
            GrammarRule Rule1 = new GrammarRule("S", new Symbol[] { new Symbol("leftPar", "("), new Symbol("L"), new Symbol("rightPar", ")") });
            GrammarRule Rule2 = new GrammarRule("S", new Symbol[] { new Symbol("X", "X")});
            GrammarRule Rule3 = new GrammarRule("L", new Symbol[] { new Symbol("S") });
            GrammarRule Rule4 = new GrammarRule("L", new Symbol[] { new Symbol("L"), new Symbol("comma", ","), new Symbol("S") });
            
            Grammar.Add(Rule1);
            Grammar.Add(Rule2);
            Grammar.Add(Rule3);
            Grammar.Add(Rule4);
            Grammar = AugmentGrammar(Grammar);

            DFA theAutomaton = BuildLr0(Grammar);

#if (DEBUG)
                using (System.IO.StreamWriter writer =
                    new System.IO.StreamWriter("grafo.dot"))
                {
                    writer.Write("digraph G {");
                    printGraph(theAutomaton, writer);
                    writer.Write("rankdir=LR; }");
                }
#endif
        }
    }
}

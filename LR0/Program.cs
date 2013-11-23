using System;
using System.Collections.Generic;

using dfaState = System.Int32;
using input = System.String;

namespace LR0
{
    class Program
    {

        public static List<GrammarRule> AugmentGrammar(List<GrammarRule> theGrammar)
        {
            List<GrammarRule> AugmentedGrammar = new List<GrammarRule>();
            GrammarRule FirstRule = theGrammar[0];

            if (!FirstRule.Expression[FirstRule.Expression.Count - 1].Equals(getEndSymbol()))
            {
                String ExtraRuleName = FirstRule.RuleSymbol.id + "'";
                Symbol[] ExtraRuleExpression = new Symbol[] { new Symbol(FirstRule.RuleSymbol.id), getEndSymbol() };

                GrammarRule ExtraRule = new GrammarRule(ExtraRuleName, ExtraRuleExpression);
                AugmentedGrammar.Add(ExtraRule);
            }
            foreach (GrammarRule rule in theGrammar)
            {
                AugmentedGrammar.Add(rule);
            }
            return AugmentedGrammar;
        }


        public static Symbol getEpsylonSymbol()
        {
            return new Symbol(LexerGenerationHelper.getEquivalent('3'), "3");
        }

        public static Symbol getEndSymbol()
        {
            return new Symbol("úEndSymbol", "$");
        }

       
        public static List<Item> getItemsById(string theId, List<GrammarRule> theGrammar)
        {
            List<Item> resultList = new List<Item>();

            foreach (GrammarRule rule in theGrammar)
            {
                if (rule.RuleSymbol.id == theId)
                {
                    resultList.Add(new Item(rule));
                }
            }

            return resultList;
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
                
        public static DFA BuildLr0(List<GrammarRule> AugmentedGrammar, out HashSet<Tuple<dfaState, Symbol, Int32>> ReduceStates, out HashSet<input> Tokens)
        {
            Tokens = new HashSet<input>();
            ReduceStates = new HashSet<Tuple<dfaState, Symbol, Int32>>();
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
                            if (X.id == "úEndSymbol")
                            {
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
                                if (item.getNextSymbol() != null)
                                    if (item.getNextSymbol().id == "úEndSymbol")
                                        E.final.Add(JIndex);

                                int Iindex = T.IndexOf(I);
                                string TransitionSymbol = X.id;
                                Tokens.Add(TransitionSymbol);

                                if (!E.transitionTable.ContainsKey(new KeyValuePair<int, string>(Iindex, TransitionSymbol)))
                                    E.transitionTable.Add(new KeyValuePair<int, string>(Iindex, TransitionSymbol), JIndex);
                            }
                        }
                        else
                        {
                            HashSet<Symbol> followSet = Follow(item.Rule.RuleSymbol, AugmentedGrammar);
                            foreach (Symbol x in followSet)
                            {
                                ReduceStates.Add(new Tuple<dfaState, Symbol, Int32>(T.IndexOf(I), x, AugmentedGrammar.IndexOf(item.Rule)));
                            }

                        }
                    }
                }
            } while (hasChanged);

            return E;
        }

        private static void printGraph(DFA theAutomaton, System.IO.StreamWriter writer)
        {
            //public SortedList<KeyValuePair<state, input>, state> transitionTable;
            foreach (KeyValuePair<KeyValuePair<dfaState, input>, dfaState> pair in theAutomaton.transitionTable)
            {
                writer.Write(pair.Key.Key.ToString() + " -> " + pair.Value.ToString() + " [label=" + pair.Key.Value + "];");
                if (theAutomaton.final.Contains(pair.Value)) writer.Write(pair.Value.ToString() + " [  style=filled color=\"dodgerblue\" fillcolor=\"lightyellow\" ]");
            }
        }

        private static HashSet<Symbol> Follow(Symbol B, List<GrammarRule> theGrammar)
        {
            HashSet<Symbol> result = new HashSet<Symbol>();
            foreach (GrammarRule rule in theGrammar)
            {
                if (rule.Expression.Contains(B))
                {
                    for (int i = 0; i < rule.Expression.Count; i++) // iterate through every expression member
                    {
                        if (rule.Expression[i].Equals(B)) // if it's equal to our symbol...
                        {
                            bool isEpsylon = true;
                            for (int j = i + 1; j < rule.Expression.Count; j++) // iterate through reamining members
                            { // check if they are all epsylon
                                isEpsylon = isEpsylon && rule.Expression[j].Equals(getEpsylonSymbol());
                            }
                            if (!isEpsylon)
                            { // if rest is not epsylon
                                List<Symbol> RemainingSymbols = rule.Expression.GetRange(i + 1, rule.Expression.Count - (i + 1));
                                HashSet<Symbol> FirstSet = First(RemainingSymbols, theGrammar);
                                FirstSet.Remove(getEpsylonSymbol());
                                result.UnionWith(FirstSet);
                            }
                            if (i == rule.Expression.Count - 1) // if it's the final production
                            {
                                if (!rule.RuleSymbol.Equals(B))
                                    result.UnionWith(Follow(rule.RuleSymbol, theGrammar));
                            }
                            else
                            {
                                List<Symbol> RemainingSymbols = rule.Expression.GetRange(i + 1, rule.Expression.Count - (i + 1));
                                if (First(RemainingSymbols, theGrammar).Contains(getEpsylonSymbol()))
                                {
                                    if (!rule.RuleSymbol.Equals(B))
                                        result.UnionWith(Follow(rule.RuleSymbol, theGrammar));
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static HashSet<Symbol> First(List<Symbol> Ys, List<GrammarRule> theGrammar)
        {
            HashSet<Symbol> result = new HashSet<Symbol>();
            int index = 0;
            bool containsEpsylon;
            do
            {
                HashSet<Symbol> y1result = First(Ys[index], theGrammar);
                containsEpsylon = y1result.Contains(getEpsylonSymbol());
                y1result.Remove(getEpsylonSymbol());
                result.UnionWith(y1result);
                index++;
            } while (index < Ys.Count && containsEpsylon);
            if (index == Ys.Count && containsEpsylon) result.Add(getEpsylonSymbol());
            return result;
        }

        private static HashSet<Symbol> First(Symbol X, List<GrammarRule> theGrammar)
        {
            HashSet<Symbol> result = new HashSet<Symbol>();
            if (X.isTerminal) result.Add(X); //If it's terminal.. just add it
            else
            {
                foreach (GrammarRule rule in theGrammar)
                {
                    if (X.Equals(rule.RuleSymbol)) // if productions looks like X --> _______
                    {
                        if (rule.Expression[0].isTerminal)
                        {
                            result.Add(rule.Expression[0]);
                        }
                        else
                        {
                            HashSet<Symbol> YsFirst = First(rule.Expression, theGrammar);
                            result.UnionWith(YsFirst);
                        }
                    }
                }
            }
            return result;
        }

        private static ParsingTable GetParsingTable(DFA theAutomaton, HashSet<input> theTokens, HashSet<input> NonTerminals, HashSet<Tuple<dfaState, Symbol, Int32>> ReduceStates, List<GrammarRule> theGrammar)
        {
            ParsingTable Table = new ParsingTable();

            foreach (KeyValuePair<KeyValuePair<dfaState, input>, dfaState> item in theAutomaton.transitionTable)
            {
                if (NonTerminals.Contains(item.Key.Value))
                {
                    Table.transitionTable.Add(item.Key, new Action(item.Value, ActionType.Goto));
                }
                else
                {
                    Table.transitionTable.Add(item.Key, new Action(item.Value, ActionType.Shift));
                }
            }

            foreach (dfaState theState in theAutomaton.final)
            {
                Table.transitionTable.Add(
                    new KeyValuePair<dfaState, input>(theState, "úEndSymbol"),
                    new Action(-1, ActionType.Accept));
            }

            foreach (Tuple<dfaState, Symbol, Int32> item in ReduceStates)
            {
                if (!NonTerminals.Contains(item.Item2.id))
                    Table.transitionTable.Add(
                    new KeyValuePair<dfaState, input>(item.Item1, item.Item2.id),
                    new Action(item.Item3, ActionType.Reduce));
            }

            return Table;
        }
        
        static void Main(string[] args)
        {
            List<GrammarRule> originalGrammar = new List<GrammarRule>();
            List<GrammarRule> Grammar = new List<GrammarRule>();
            //GrammarRule Rule1 = new GrammarRule("S", new Symbol[] { new Symbol("leftPar", "("), new Symbol("L"), new Symbol("rightPar", ")") });
            //GrammarRule Rule2 = new GrammarRule("S", new Symbol[] { new Symbol("X", "X") });
            //GrammarRule Rule3 = new GrammarRule("L", new Symbol[] { new Symbol("S") });
            //GrammarRule Rule4 = new GrammarRule("L", new Symbol[] { new Symbol("L"), new Symbol("comma", ","), new Symbol("S") });

            //GrammarRule Rule1 = new GrammarRule("S", new Symbol[] { new Symbol("A") });
            //GrammarRule Rule2 = new GrammarRule("A", new Symbol[] { new Symbol("a", "a"), new Symbol("A"), new Symbol("d", "d") });
            //GrammarRule Rule3 = new GrammarRule("A", new Symbol[] { new Symbol("B"), new Symbol("C") });
            //GrammarRule Rule4 = new GrammarRule("B", new Symbol[] { new Symbol("b", "b"), new Symbol("B"), new Symbol("c", "c") });
            //GrammarRule Rule5 = new GrammarRule("B", new Symbol[] { Lr0GenerationHelper.getEpsylonSymbol() });
            //GrammarRule Rule6 = new GrammarRule("C", new Symbol[] { new Symbol("a", "a"), new Symbol("c", "c"), new Symbol("C") });
            //GrammarRule Rule7 = new GrammarRule("C", new Symbol[] { new Symbol("a", "a"), new Symbol("d", "d") });

            //GrammarRule Rule1 = new GrammarRule("E", new Symbol[] { new Symbol("T"), new Symbol("Ex") });
            //GrammarRule Rule2 = new GrammarRule("Ex", new Symbol[] { new Symbol("+", "+"), new Symbol("T"), new Symbol("Ex") });
            //GrammarRule Rule3 = new GrammarRule("Ex", new Symbol[] { Lr0GenerationHelper.getEpsylonSymbol() });
            //GrammarRule Rule4 = new GrammarRule("T", new Symbol[] { new Symbol("F"), new Symbol("Tx") });
            //GrammarRule Rule5 = new GrammarRule("Tx", new Symbol[] { new Symbol("*", "*"), new Symbol("F"), new Symbol("Tx") });
            //GrammarRule Rule6 = new GrammarRule("Tx", new Symbol[] { Lr0GenerationHelper.getEpsylonSymbol() });
            //GrammarRule Rule7 = new GrammarRule("F", new Symbol[] { new Symbol("(", "("), new Symbol("E"), new Symbol(")", ")") });
            //GrammarRule Rule8 = new GrammarRule("F", new Symbol[] { new Symbol("id", "id") });

            GrammarRule Rule0 = new GrammarRule("S", new Symbol[] { new Symbol("E"), getEndSymbol() });
            GrammarRule Rule1 = new GrammarRule("E", new Symbol[] { new Symbol("T"), new Symbol("plus", "+"), new Symbol("E") });
            GrammarRule Rule2 = new GrammarRule("E", new Symbol[] { new Symbol("T") });
            GrammarRule Rule3 = new GrammarRule("T", new Symbol[] { new Symbol("X", "x") });

            originalGrammar.Add(Rule1);
            originalGrammar.Add(Rule2);
            originalGrammar.Add(Rule3);
            //originalGrammar.Add(Rule4);
            //originalGrammar.Add(Rule5);
            //originalGrammar.Add(Rule6);
            //originalGrammar.Add(Rule7);
            //originalGrammar.Add(Rule8);

            Grammar = AugmentGrammar(originalGrammar);

            HashSet<input> nonTerminals = new HashSet<input>();
            HashSet<Symbol> terminals = new HashSet<Symbol>();

            foreach (GrammarRule rule in Grammar)
            {
                nonTerminals.Add(rule.RuleSymbol.id);
            }

            foreach (GrammarRule rule in Grammar)
            {
                foreach (Symbol sym in rule.Expression)
                {
                    if (sym.isTerminal)
                    {
                        terminals.Add(sym);
                    }
                }
            }

            HashSet<Tuple<dfaState, Symbol, Int32>> reduceStates = new HashSet<Tuple<dfaState, Symbol, Int32>>();
            HashSet<input> tokens = new HashSet<input>();
            DFA theAutomaton = BuildLr0(Grammar, out reduceStates, out tokens);
            ParsingTable theParsingTable = GetParsingTable(theAutomaton, tokens, nonTerminals, reduceStates, Grammar);

            HashSet<Symbol> theInputs = new HashSet<Symbol>();

            foreach (GrammarRule rule in Grammar)
            {
                foreach (Symbol sym in rule.Expression)
                {
                    if (sym.isTerminal)
                    {
                        theInputs.Add(sym);
                    }
                }
            }
            foreach (GrammarRule rule in Grammar)
            {
                foreach (Symbol sym in rule.Expression)
                {
                    if (!sym.isTerminal)
                    {
                        theInputs.Add(sym);
                    }
                }
            }
            HashSet<dfaState> theStates = new HashSet<dfaState>();
            foreach (KeyValuePair<dfaState, input> key in theParsingTable.transitionTable.Keys)
            {
                theStates.Add(key.Key);
            }

            foreach (GrammarRule rule in originalGrammar)
            {
                Console.WriteLine(rule);
            }

            foreach (Symbol item in theInputs)
            {
                Console.Write("| " + item.stringRepresentation + "|");
            }
            Console.WriteLine();
            foreach (dfaState state in theStates)
            {
                foreach (Symbol key in theInputs)
                {
                    Action tmpAction;
                    if (theParsingTable.transitionTable.TryGetValue(new KeyValuePair<dfaState, input>(state, key.id), out tmpAction))
                        Console.Write("|" + tmpAction + "|");
                    else
                        Console.Write("|  |");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            HashSet<Symbol> AFirst = First(new Symbol("E"), Grammar);
            foreach (Symbol sym in AFirst)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFirst = First(new Symbol("Ex"), Grammar);
            foreach (Symbol sym in AFirst)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFirst = First(new Symbol("T"), Grammar);
            foreach (Symbol sym in AFirst)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFirst = First(new Symbol("Tx"), Grammar);
            foreach (Symbol sym in AFirst)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFirst = First(new Symbol("F"), Grammar);
            foreach (Symbol sym in AFirst)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            Console.ReadLine();
            //-------------------
            Console.WriteLine();
            HashSet<Symbol> AFollow = Follow(new Symbol("E"), Grammar);
            foreach (Symbol sym in AFollow)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFollow = Follow(new Symbol("Ex"), Grammar);
            foreach (Symbol sym in AFollow)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFollow = Follow(new Symbol("T"), Grammar);
            foreach (Symbol sym in AFollow)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFollow = Follow(new Symbol("Tx"), Grammar);
            foreach (Symbol sym in AFollow)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            AFollow = Follow(new Symbol("F"), Grammar);
            foreach (Symbol sym in AFollow)
            {
                Console.Write(sym + ",");
            }
            Console.WriteLine();
            Console.ReadLine();

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

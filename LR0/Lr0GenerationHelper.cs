using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR0
{
    static class Lr0GenerationHelper
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
    }
}

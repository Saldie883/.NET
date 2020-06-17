using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3
{
    /*
       * В цьому завданні потрібно було опрацювати вираз логічними функціями. Оскільки в нас є багато варіацій обчислення логічних функцій з одним обєктом то було
       * вибрано патерн Стратегія - Strategy. Він нам дозволяє зосереджувати різні реалізації одного алгоритму в декількох класах і потім взаємозаміняти цей функціонал клієнту.
       * В цьому файлі зібрано різні типи "Стратегій" для реалізації алгоритму провірки логічними функціями.
       * Створено базовий інтерфейс ILogicStrategyCalculate який має метод CalculateFunction для проведення логічної функції.
       * Його імплементують різні класи(типи алгоритму), такі як StrategyAnd, StrategyOr, StrategyComparisonHighter, StrategyEquals та інші.
       * Кожен з цих класів має свою реалізацію логічної функції, тобто StrategyAnd порівнює через оператор & а StrategyEquals через =.
       * Для доступу до цих класів створено основний клас StrategyClass який містить в собі обєкт ILogicStrategyCalculate, і це дозволяє нам підставляти в цей обєкт любий клас
       * який реалізує цей інтерфейс. Тобто ми можемо під час виконання програми передавати сюди різні класи StrategyAnd або StrategyOr або інші.
       * Також є конструктор для того щоб встановити алгоритм та метод SetStrategy з цією самою метою, щоб можна було впродовж програми це міняти.
       *
       */

    internal class StrategyClass
    {
        public ILogicStrategyCalculate _strategy { get; set; }

        public StrategyClass()
        {
        }

        public StrategyClass(ILogicStrategyCalculate strategy)
        {
            this._strategy = strategy;
        }

        public void SetStrategy(ILogicStrategyCalculate strategy)
        {
            this._strategy = strategy;
        }

        public bool Calculate(object f1, object f2)
        {
            return this._strategy.CalculateFunction(f1, f2);
        }
    }

    internal class StrategyAnd : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object second)
        {
            return (bool)first & (bool)second;
        }
    }

    internal class StrategyOr : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object second)
        {
            return (bool)first | (bool)second;
        }
    }

    internal class StrategyNot : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object s)
        {
            return !(bool)first;
        }
    }

    internal class StrategyEquals : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object second)
        {
            if (int.TryParse(first.ToString(), out var res) && int.TryParse(second.ToString(), out var res2))
            {
                int f1 = res;
                int f2 = res2;
                return f1 == f2;
            }
            return (bool)first | (bool)second;
        }
    }

    internal class StrategyContains : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object second)
        {
            var firstStr = first as string;
            var secondStr = second as string;
            if (firstStr == null || secondStr == null)
            {
                return false;
            }
            return firstStr.Contains(secondStr);
        }
    }

    internal class StrategyComparisonLess : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object second)
        {
            if (int.TryParse(first.ToString(), out var res) && int.TryParse(second.ToString(), out var res2))
            {
                int f1 = res;
                int f2 = res2;
                return f1 < f2;
            }
            return false;
        }
    }

    internal class StrategyComparisonHighter : ILogicStrategyCalculate
    {
        public bool CalculateFunction(object first, object second)
        {
            if (int.TryParse(first.ToString(), out var res) && int.TryParse(second.ToString(), out var res2))
            {
                int f1 = res;
                int f2 = res2;
                return f1 > f2;
            }
            return false;
        }
    }

    internal class StrategyCompound : ISetStrategyCalculate
    {
        public object CalculateFunction(object first, object second)
        {
            var firstList = first as List<int>;
            var secondList = second as List<int>;
            if (firstList == null || secondList == null)
            {
                return null;
            }
            var compound = firstList.Union(secondList);
            return compound;
        }
    }

    internal interface ILogicStrategyCalculate
    {
        bool CalculateFunction(object first, object second);
    }

    internal interface ISetStrategyCalculate
    {
        object CalculateFunction(object first, object second);
    }
}
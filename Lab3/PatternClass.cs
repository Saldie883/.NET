using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3
{
    /*
     * В цьому файлі зосереджено усю логіку обробки виразів.
     * тут є клас PatternClass через який і ведеться робота. Він містить в собі обєкт класу StrategyClass з файлу (StrategyClass.cs) для того щоб опреділити певний
     * алгоритм обробки виразу. (Патерн стратегія)
     * Виконання починається з методу OnMain, в ньому виводиться підказка для користувача та приклад введення, після цього рядок зчитується  з консолі та
     * передається у метод ConvertToPref, цей метод конвертує вираз у польський запис (https://en.wikipedia.org/wiki/Reverse_Polish_notation) згідно параметрів
     * які я там визначив, він проходиться по виразу і згідно пріоритету операцій будує польський запис і повертає нам масив елементів виразу.
     * Після цього викликається метод ParseExpression який проводить обрахування нашого виразу.
     * В цьому методі створюється стек обєктів який буде містити тимчасове сховище для даних. (теорія обрахування польського запису)
     * В ньому проходиться по списку елементів та якщо находить число то закидує його у стек, а якщо оператор то витягує два останні числа зі стеку,
     * і в залежності від того який це оператор, він створює обєкт нашого класу StrategyClass і передає туди потрібну стратегію обробки даних.
     * Після цього викликається в цього обєкта метод Calculate і туди передаються два останні записи зі стеку, а після цього виводиться повідомлення на екран про результат операції.
     * Після обрахування цей результат заноситься в стек і надалі враховується при обрахуваннях.
     * Такий підхід є реалізацією шаблону Стратегія і дозволяє нам підставляти різні типи алгоритму обрахування логічних функцій.
     */

    internal class PatternClass
    {
        private static StrategyClass strategyContext;

        private static List<string> ConvertToPref(string exp)
        {
            string infix = exp;
            string[] tokens = infix.Split(' ');

            Stack<char> s = new Stack<char>();
            List<string> outputList = new List<string>();
            try
            {
                int n;
                for (int i = 0; i < infix.Length; i++)
                {
                    if (int.TryParse(infix[i].ToString(), out n) || ((int)infix[i] >= 65 && (int)infix[i] <= 90) || ((int)infix[i] >= 97 && (int)infix[i] <= 122)) // DIGIT
                    {
                        string digit = infix[i].ToString();
                        i++;
                        while (i < infix.Length && "1234567890".IndexOf(infix[i]) != -1)
                        {
                            digit += infix[i];
                            i++;
                        }
                        outputList.Add(digit);
                        i--;
                    }
                    else if (isOperator(infix[i].ToString()))
                    {
                        while (s.Count != 0 && Priority(s.Peek().ToString()) >= Priority(infix[i].ToString()))
                        {
                            outputList.Add(s.Pop().ToString());
                        }
                        s.Push(infix[i]);
                    }
                }
                while (s.Count != 0)//if any operators remain in the stack, pop all & add to output list until stack is empty
                {
                    outputList.Add(s.Pop().ToString());
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine("Reading formula error!");
                Console.ResetColor();
            }

            return outputList;
        }

        public static int Priority(string c)
        {
            if (c == "!")
            {
                return 5;
            }
            else if (c == "<" || c == ">" || c == "<=" || c == ">=")
            {
                return 4;
            }
            else if (c == "=" || c == "!=")
            {
                return 3;
            }
            else if (c == "&")
            {
                return 2;
            }
            else if (c == "|")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static bool isOperator(string c)
        {
            if (("&|=!><".IndexOf(c) != -1))
                return true;
            return false;
        }

        public static bool isConstant(string c)
        {
            return (int.TryParse(c.ToString(), out int n));
        }

        public static void OnMain()
        {
            Console.WriteLine("Введiть вираз:\nПриклад: 3=4&5<10\n[&] - And\n[|] - Or\n[!] - Not\n[!=] - Not equal\n[=] - Equal\n[<] - Less than\n[>] More than");
            string formula = Console.ReadLine();
            //string formula = "3=4&5<10";
            var list = ConvertToPref(formula);

            var tree = ParseExpression(list);
            Console.WriteLine("------------------");
        }

        private static object ParseExpression(List<string> list)
        {
            bool state = false;

            Stack<object> s = new Stack<object>();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (double.TryParse(list[i].ToString(), out double n))
                    {
                        s.Push(n);
                    }
                    else if (isOperator(list[i]))
                    {
                        object arg1 = 0;
                        object arg2 = 0;
                        switch (list[i])
                        {
                            case "&":
                                strategyContext = new StrategyClass(new StrategyAnd());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation {arg2}&{arg1}: {state}");
                                break;

                            case "|":
                                strategyContext = new StrategyClass(new StrategyOr());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation {arg2}|{arg1}: {state}");
                                break;

                            case "=":
                                strategyContext = new StrategyClass(new StrategyEquals());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation {arg2}={arg1}: {state}");
                                break;

                            case "!":
                                strategyContext = new StrategyClass(new StrategyNot());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation !{arg2}: {state}");
                                break;

                            case "!=":
                                strategyContext = new StrategyClass(new StrategyEquals());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation {arg2}!={arg1}: {state}");
                                break;

                            case ">":
                                strategyContext = new StrategyClass(new StrategyComparisonHighter());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation {arg2}>{arg1}: {state}");
                                break;

                            case "<":
                                strategyContext = new StrategyClass(new StrategyComparisonLess());
                                arg1 = s.Pop();
                                arg2 = s.Pop();
                                state = strategyContext.Calculate(arg2, arg1);
                                Console.WriteLine($"Operation {arg2}<{arg1}: {state}");
                                break;

                            default:
                                break;
                        }
                        s.Push(state);
                    }
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine("Reading formula error!");
                Console.ResetColor();
            }

            return null;
        }
    }
}
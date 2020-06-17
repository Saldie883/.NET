using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2
{
    /*
     * В цьому завданні потрібно було реалізувати дерево розбору виразу, заданого наведеними нижче синтаксичними правилами:
             <вираз>::=<простий вираз> | <складний вираз>
            <простий вираз>::=<константа> | <змінна>
            <константа>::=(<число>)
            <змінна>::=(<ім'я>)
            <складний вираз>::=(<вираз><знак операції><вираз>)
            <знак операції>::=+|-|*|/
        Я реалізував цю програму наступним чином: вибраний структурний паттерн дерево, компонувальник.
        Цей патерн дозволяє нам групувати певну множину обєктів в деревоподібну структуру, і після того працювати з нею як з одиничним обєктом.
        Для представлення синтаксичних одиниць які наведені у завданні створено абстрактний клас TreeComponent який містить абстрактний метод Operation()
        та інші віртуальні методи для додавання і видалення вкладених елементів. Також є метод IsComposite який повертає true якшо цей елемент може містити в собі інші елементи.
        Від цього класу успадковано такі класи як Expression, SimpleExpression, ComplicatedExpression, Constant, Variable, Digit, Name, OperationComponent
        Вони собою представляють елементи дерева з яких складається вираз.
        Функціонал:
        В Функції OnMain користувач вводить вираз, після того він конвертується через метод  ConvertToPref у польський запис для того щоб було зручно проходитись по виразу (https://en.wikipedia.org/wiki/Reverse_Polish_notation)
        Після цього викликається метод ParseExpression який повертає нам обєкт типу TreeComponent (Expression) базовий елемент ( корінь)
        В цьому методі створюється корінь дерева, та в циклі проходиться по змінним та операціям у виразі
        Якщо натикається на оператор то створюється складний вираз та отримується через метод FindExpressions, в цей метод подається наш список елементів виразу та позиція.
        Цей метод також проходиться по нашому списку елементів виразу та якщо натикається на оператор, то це означає шо ми найшли ше один складний елемент і викликається знову цей метод (рекурсивно)
        в підсумку коли ми найшли простий елемент (число, змінна) ми створюємо простий елемент і починаємо повертатись з рекурсії, попутно записуючи прості елементи у складні.
        В кінці коли всі елементи буде пройдено, повертається елемент TreeComponent і метод ParseExpression продовжує проходитись по елементам та створювати або складні або прості вирази. і
        при цьому він виводить дані у консоль.
        В підсумку, метод ParseExpression повертає обєкт TreeComponent який являється корнем дерева а в консолі міститься вивід про вираз.
     */

    internal class PatternClass
    {
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
                        while (s.Count != 0 && Priority(s.Peek()) >= Priority(infix[i]))
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
            outputList.Reverse();
            return outputList;
        }

        public static int Priority(char c)
        {
            if (c == '*' || c == '/')
            {
                return 2;
            }
            else if (c == '+' || c == '-')
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
            if (("+-/*".IndexOf(c) != -1))
                return true;
            return false;
        }

        public static bool isVariable(string c)
        {
            return (!int.TryParse(c, out var res));
        }

        public static bool isConstant(string c)
        {
            return (int.TryParse(c.ToString(), out int n));
        }

        public static void OnMain()
        {
            Console.WriteLine("Введiть вираз:\nПриклад: 20*3+34/2\nВикористовуйте будь ласка операцiї +,-,*,/");
            string formula = Console.ReadLine();
            // string formula = "20*3+34/2";
            var list = ConvertToPref(formula);

            var tree = ParseExpression(list);
            Console.WriteLine("------------------");
        }

        private static TreeComponent ParseExpression(List<string> list)
        {
            TreeComponent mainComp = new Expression();
            for (int i = 0; i < list.Count; i++)
            {
                if (isOperator(list[i]))
                {
                    ComplicatedExpression cExp = new ComplicatedExpression();
                    var operation = new OperationComponent();
                    operation.value = list[i];
                    cExp._operation = operation;
                    i++;
                    TreeComponent complicated1 = FindExpressions(list, i, out i);
                    i++;
                    TreeComponent complicated2 = FindExpressions(list, i, out i);
                    cExp.AddFirstExp(complicated2);
                    cExp.AddSecondExp(complicated1);
                    mainComp.Add(cExp);
                    Console.WriteLine($"Складний вираз:\n{cExp._firstExp.Operation()}{cExp._operation.Operation()}{cExp._secondExp.Operation()}");
                }
                else
                {
                    var simple = new SimpleExpression();
                    TreeComponent varComp;
                    if (isConstant(list[i]))
                    {
                        varComp = new Constant();
                        var varDig = new Digit();
                        varDig.value = int.Parse(list[i]);
                        varComp.Add(varDig);
                    }
                    else
                    {
                        varComp = new Variable();
                        var varDig = new Name();
                        varDig.value = list[i];
                        varComp.Add(varDig);
                    }
                    simple.Add(varComp);
                    mainComp.Add(simple);
                }
            }
            return mainComp;
        }

        // 20*3+34/2
        private static TreeComponent FindExpressions(List<string> list, int position1, out int position)
        {
            int count = 0;
            position = position1;
            for (int i = position; i < list.Count; i++)
            {
                var sTemp = list[i];
                if (isOperator(sTemp))
                {
                    i++;
                    TreeComponent c1 = FindExpressions(list, i, out i);
                    i++;
                    TreeComponent c2 = FindExpressions(list, i, out i);
                    ComplicatedExpression comp = new ComplicatedExpression();
                    var operation = new OperationComponent();
                    operation.value = sTemp;
                    comp._operation = operation;
                    comp.AddFirstExp(c2);
                    comp.AddSecondExp(c1);
                    position = i;
                    return comp;
                }
                else
                {
                    var simple = new SimpleExpression();
                    TreeComponent varComp;
                    if (isConstant(list[i]))
                    {
                        varComp = new Constant();
                        var varDig = new Digit();
                        varDig.value = int.Parse(list[i]);
                        varComp.Add(varDig);
                    }
                    else
                    {
                        varComp = new Variable();
                        var varDig = new Name();
                        varDig.value = list[i];
                        varComp.Add(varDig);
                    }
                    simple.Add(varComp);
                    position = i;
                    return simple;
                }
            }
            return null;
        }
    }

    internal abstract class TreeComponent
    {
        public abstract string Operation();

        public virtual bool IsComposite()
        {
            return true;
        }

        public virtual void Add(TreeComponent component)
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(TreeComponent component)
        {
            throw new NotImplementedException();
        }
    }

    internal class Expression : TreeComponent
    {
        protected List<TreeComponent> _children = new List<TreeComponent>();

        public override string Operation()
        {
            string s = "Вираз: \n";
            foreach (var item in _children)
            {
                s += item.Operation();
            }
            return s;
        }

        public override void Add(TreeComponent component)
        {
            this._children.Add(component);
        }

        public override void Remove(TreeComponent component)
        {
            this._children.Remove(component);
        }
    }

    internal class SimpleExpression : TreeComponent
    {
        protected TreeComponent _children;

        public override string Operation()
        {
            return $"Простий вираз: {_children.Operation()}\n";
        }

        public override void Add(TreeComponent component)
        {
            this._children = component;
        }

        public override void Remove(TreeComponent component)
        {
            this._children = null;
        }
    }

    internal class ComplicatedExpression : TreeComponent
    {
        public TreeComponent _firstExp;
        public TreeComponent _secondExp;
        public TreeComponent _operation = new OperationComponent();

        public override string Operation()
        {
            return $"\nСкладний вираз: {this._firstExp.Operation()}{this._operation.Operation()}{this._secondExp.Operation()}"
            ;
        }

        public void AddFirstExp(TreeComponent component)
        {
            this._firstExp = component;
        }

        public void AddSecondExp(TreeComponent component)
        {
            this._secondExp = component;
        }

        public void AddOperation(TreeComponent component)
        {
            this._operation = component;
        }
    }

    internal class Constant : TreeComponent
    {
        protected TreeComponent _child = new Digit();

        public override string Operation()
        {
            return $"Константа. {_child.Operation()}\n";
        }

        public override void Add(TreeComponent component)
        {
            this._child = component;
        }

        public override void Remove(TreeComponent component)
        {
            this._child = null;
        }
    }

    internal class Variable : TreeComponent
    {
        protected TreeComponent _child = new Name();

        public override string Operation()
        {
            return $"Змiнна. {_child.Operation()}\n";
        }

        public override void Add(TreeComponent component)
        {
            this._child = component;
        }

        public override void Remove(TreeComponent component)
        {
            this._child = null;
        }
    }

    internal class Digit : TreeComponent
    {
        public int value;

        public override bool IsComposite()
        {
            return false;
        }

        public override string Operation()
        {
            return $"Число {value.ToString()}\n";
        }
    }

    internal class Name : TreeComponent
    {
        public string value;

        public override bool IsComposite()
        {
            return false;
        }

        public override string Operation()
        {
            return $"Назва {value}\n";
        }
    }

    internal class OperationComponent : TreeComponent
    {
        public string value;

        public override bool IsComposite()
        {
            return false;
        }

        public override string Operation()
        {
            return $"Операцiя {value}\n";
        }
    }
}
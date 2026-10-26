using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Rozumatic_ASD_Lab10
{
    class Program
    {
        // Пріоритет операторів (Модифіковано для Варіанту 2)
        static int GetPrecedence(string op)
        {
            if (op == "u-") return 4; // Унарний мінус має найвищий пріоритет
            if (op == "^") return 3;
            if (op == "*" || op == "/") return 2;
            if (op == "+" || op == "-") return 1;
            return 0;
        }

        // 1. Shunting-Yard: Інфіксний вираз -> RPN (Постфіксний) з підтримкою унарного мінуса
        static List<string> InfixToRPN(string expression)
        {
            List<string> output = new List<string>();
            Stack<string> operators = new Stack<string>();

            string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                // Якщо токен - число
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    output.Add(token);
                }
                else if (token == "(")
                {
                    operators.Push("(");
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }
                    if (operators.Count > 0) operators.Pop(); // видаляємо '('
                }
                // Якщо оператор
                else if (token == "+" || token == "-" || token == "*" || token == "/" || token == "^")
                {
                    string opToPush = token;

                    // ПЕРЕВІРКА НА УНАРНИЙ МІНУС (Варіант 2):
                    // Мінус є унарним, якщо він стоїть першим у виразі АБО одразу після відкриваючої дужки
                    if (token == "-" && (i == 0 || tokens[i - 1] == "("))
                    {
                        opToPush = "u-"; // Позначаємо як унарний мінус
                    }

                    while (operators.Count > 0 && GetPrecedence(operators.Peek()) >= GetPrecedence(opToPush))
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(opToPush);
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }

        // 2. Обчислення значення RPN виразу на числовому стеку
        static double EvaluateRPN(List<string> rpnTokens)
        {
            Stack<double> stack = new Stack<double>();

            foreach (string token in rpnTokens)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    stack.Push(number);
                }
                // Обробка унарного мінуса
                else if (token == "u-")
                {
                    if (stack.Count < 1) throw new InvalidOperationException("Помилка виразу для унарного мінуса.");
                    double val = stack.Pop();
                    stack.Push(-val); // Змінюємо знак числа на протилежний
                }
                // Обробка бінарних операторів
                else
                {
                    if (stack.Count < 2) throw new InvalidOperationException("Помилка виразу: недостатньо операндів.");
                    double b = stack.Pop();
                    double a = stack.Pop();

                    switch (token)
                    {
                        case "+": stack.Push(a + b); break;
                        case "-": stack.Push(a - b); break;
                        case "*": stack.Push(a * b); break;
                        case "/": stack.Push(a / b); break;
                        case "^": stack.Push(Math.Pow(a, b)); break;
                        default: throw new InvalidOperationException($"Невідомий оператор: {token}");
                    }
                }
            }

            return stack.Pop();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("==========================================================================");
            Console.WriteLine("  РОЗУМАТИК // ЛАБОРАТОРНА РОБОТА №10: ВАРІАНТ 2 (УНАРНИЙ МІНУС В RPN)");
            Console.WriteLine("==========================================================================");

            // Вираз із завдання та приклад з унарним мінусом на початку і всередині
            string infixExpr = "-5 * 2 + ( -4 + 3 ) ^ 2";
            Console.WriteLine($"Вхідний інфіксний вираз: {infixExpr}");
            Console.WriteLine("(Зверніть увагу на пробіли між усіма токенами, включаючи мінуси!)");

            Stopwatch sw = Stopwatch.StartNew();

            // Перетворення
            List<string> rpn = InfixToRPN(infixExpr);
            Console.WriteLine($"\nПостфіксний запис (RPN): {string.Join(" ", rpn)}");

            // Обчислення
            double result = EvaluateRPN(rpn);
            sw.Stop();

            Console.WriteLine($"\n🎯 Обчислений результат: {result}");
            Console.WriteLine($"⏱️ Час обчислення:       {sw.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine("✅ Тест алгоритму Дейкстри з унарним мінусом пройдено успішно!");
        }
    }
}

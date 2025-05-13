using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestConsoleCode
{
    public class BinTree
    {
        private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        // Tách biểu thức thành các token
        private string[] Tokenize(string expression)
        {
            List<string> tokens = new List<string>();
            int i = 0;
            while (i < expression.Length)
            {
                if (char.IsWhiteSpace(expression[i]))
                {
                    i++;
                }
                else if ("+-*/()".Contains(expression[i]))
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else if (char.IsDigit(expression[i]) || expression[i] == '.')
                {
                    int start = i;
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                        i++;
                    tokens.Add(expression.Substring(start, i - start));
                }
                else
                {
                    throw new ArgumentException($"Invalid character: {expression[i]}");
                }
            }
            return tokens.ToArray();
        }

        // Chuyển biểu thức từ infix sang postfix
        private string[] InfixToPostfix(string expression)
        {
            string[] tokens = Tokenize(expression);
            Stack<string> operators = new Stack<string>();
            List<string> output = new List<string>();

            foreach (string token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Any, _culture, out _))
                {
                    output.Add(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }
                    if (operators.Count == 0)
                        throw new ArgumentException("Mismatched parentheses");
                    operators.Pop(); // Loại bỏ dấu '('
                }
                else if (IsOperator(token))
                {
                    while (operators.Count > 0 && operators.Peek() != "(" &&
                           GetPrecedence(operators.Peek()) >= GetPrecedence(token))
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(token);
                }
                else
                {
                    throw new ArgumentException($"Invalid token: {token}");
                }
            }

            while (operators.Count > 0)
            {
                if (operators.Peek() == "(")
                    throw new ArgumentException("Mismatched parentheses");
                output.Add(operators.Pop());
            }

            return output.ToArray();
        }

        // Xây dựng cây từ biểu thức hậu tố
        public Node BuildTree(string[] postfix)
        {
            if (postfix == null || postfix.Length == 0)
                throw new ArgumentException("Invalid postfix expression");

            Stack<Node> stack = new Stack<Node>();

            foreach (string token in postfix)
            {
                if (IsOperator(token))
                {
                    if (stack.Count < 2)
                        throw new ArgumentException("Invalid postfix expression");
                    Node right = stack.Pop();
                    Node left = stack.Pop();
                    Node node = new Node(token)
                    {
                        Left = left,
                        Right = right
                    };
                    stack.Push(node);
                }
                else
                {
                    if (!double.TryParse(token, NumberStyles.Any, _culture, out _))
                        throw new ArgumentException($"Invalid number: {token}");
                    stack.Push(new Node(token));
                }
            }

            if (stack.Count != 1)
                throw new ArgumentException("Invalid postfix expression");

            return stack.Pop();
        }

        // Đánh giá giá trị của biểu thức
        public double Evaluate(Node node)
        {
            if (node == null)
                throw new ArgumentException("Invalid tree structure");

            if (node.Left == null && node.Right == null)
            {
                if (!double.TryParse(node.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    throw new ArgumentException($"Invalid number: {node.Value}");
                return value;
            }

            if (node.Left == null || node.Right == null)
                throw new ArgumentException("Invalid tree structure");

            double left = Evaluate(node.Left);
            double right = Evaluate(node.Right);

            switch (node.Value)
            {
                case "+": return left + right;
                case "-": return left - right;
                case "*": return left * right;
                case "/":
                    if (right == 0) throw new DivideByZeroException("Division by zero");
                    return left / right;
                default:
                    throw new InvalidOperationException($"Invalid operator: {node.Value}");
            }
        }

        //validate trước khi tính
        private void ValidateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be empty");

            int parentheses = 0;
            foreach (char c in expression)
            {
                if (c == '(') parentheses++;
                else if (c == ')') parentheses--;

                if (parentheses < 0)
                    throw new ArgumentException("Too many closing parentheses");
            }

            if (parentheses != 0)
                throw new ArgumentException("Mismatched parentheses");
        }

        // Cache biểu thức đã tính toán
        private Dictionary<string, double> _expressionCache = new Dictionary<string, double>();

        // Tính toán biểu thức từ chuỗi
        public double Calculate(string expression)
        {
            if (_expressionCache.TryGetValue(expression, out double cachedResult))
                return cachedResult;

            ValidateExpression(expression);
            var postfix = InfixToPostfix(expression);
            var root = BuildTree(postfix);
            double result = Evaluate(root);
            _expressionCache[expression] = result;
            return result;
        }

        // Kiểm tra toán tử
        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        // Độ ưu tiên toán tử
        private int GetPrecedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-": return 1;
                case "*":
                case "/": return 2;
                default: return 0;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    
    public class Evaluator
    {
       List<string> tokens = new List<string> { "5", "+", "7", "*", "3", "/", "3"};
       
        Stack<int> value = new Stack<int>();
        Stack<string> operand = new Stack<string>();
        private Lookup variableEvaluator;

      public void EvalTokens()
        {
            foreach(string token in tokens)
            {
                if (isDigit(token)){

                    int tokenDigit = int.Parse(token);
                    value.Push(tokenDigit);

                    if(operand.Count > 0 && (operand.Peek() == "*" || operand.Peek() == "/"))
                    {
                        int number = value.Pop();
                        string operand1 = operand.Pop();

                        int final = (operand1 == "*") ? number * tokenDigit : number / tokenDigit;
                        value.Push(final);
                    }
                    
                    else
                    {
                        value.Push(tokenDigit);
                    }
                
                }
                else if (isOperator(token) && token == "+" || token == "-") { 
                   
                    if(operand.Count > 0 && (operand.Peek() == "+" || operand.Peek() == "-"))
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();  
                        String operatorr = operand.Pop();

                        int final = (operatorr == "+") ? number1 + number2 : number2 - number1;

                        value.Push(final);
                    }
                    operand.Push(token);
                }
                
                else if(token == "(")
                {
                    operand.Push(token);
                }

                else if(token == ")")
                {
                    while (operand.Count>0 && (operand.Peek() == "+" || operand.Peek() == "-"))
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();
                        String operatorr = operand.Pop();

                        int final = (operatorr == "+") ? number1+number2 : number2 - number1;
                       
                        value.Push(final);
                    }
                
                    if(operand.Count > 0 && operand.Peek() == ")")
                    {
                        operand.Pop();
                    }
                
                    while(operand.Count > 0 && operand.Peek() == "*" || operand.Peek() == "/")
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();
                        String operatorr = operand.Pop();

                        int final = (operatorr == "*") ? number1 * number2 : number1 / number2;
                        value.Push(final);
                    }
                
                }

                else if (isOperator(token))
                {
                    operand.Push(token);
                }
            
                if(operand.Count == 1 && (operand.Peek() == "+" || operand.Peek() == "-"))
                {
                    if (value.Count == 2)
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();
                        String operatorr = operand.Pop();

                        int final = (operatorr == "+") ? number1 + number2 : number2 - number1;

                        value.Push(final);

                        Console.WriteLine("Final Result: " + final);
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Expression: There should be 2 values left.");
                    }

                }

                if(value.Count == 1)
                {
                    int final = value.Pop();
                    Console.WriteLine("Final Result: " + final);
                }
                else
                {
                    throw new InvalidOperationException("Invalid Expression: The value stack should contain one number.");
                }
           
            }
       
        }

        private bool isVariable(string token)
        {
            return Regex.IsMatch(token, "^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        private bool isDigit(string token)
        {
            return int.TryParse(token, out _);
        }

        private bool isOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }
        
        public void TokenizeExpression(string expression)
        {
            tokens = Sep(expression);
        }

        public delegate int Lookup(String variable_name);

        public Evaluator(Lookup variableEvaluator)
        {
            this.variableEvaluator = variableEvaluator;
        }

        public static int Evaluate(String expression,Lookup variableEvaluator)
        {
            List<string> tokens = Sep(expression);

            Evaluator evaluate = new Evaluator(variableEvaluator);
            evaluate.tokens = tokens;
            evaluate.EvalTokens();

            return evaluate.value.Pop();

        }


        public static List<string> Sep(string expression)
        {
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            List<string> tokens = new List<string>();
            foreach (string s in substrings)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    tokens.Add(s);
                }
                
            }
            return tokens;
        }
   
        public Stack<int> getValueS()
        {
            return value;
        }
    
    }
}

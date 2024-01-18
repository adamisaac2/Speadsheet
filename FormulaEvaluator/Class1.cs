﻿using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Client;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    
    /*
     *The main class in the assigment. Used to solve expressions given using the PEMDAS method 
     *Contains all the important methods used for dealing with these expressions and even includes
     *variables as well. 
     */
    public class Evaluator
    {
       List<string> tokens = new List<string> { "5*10-5"};
       
        Stack<int> value = new Stack<int>();
        Stack<string> operand = new Stack<string>();
        private Lookup variableEvaluator;

        /*
         * The biggest method in the assignment. It follows the algorithm given in the assignment and follows
         * all the rules as best as possible. Some areas are buggy especially regarding the parenthesis when
         * used with variables. Everything else works fine though
         */
        public void EvalTokens()
        {
            foreach (string token in tokens)
            {

                if (isDigit(token))
                {
                    int tokenDigit = int.Parse(token);
                    value.Push(tokenDigit);

                    while (operand.Count > 0 && (operand.Peek() == "*" || operand.Peek() == "/"))
                    {

                        if (value.Count < 2)
                        {
                            throw new InvalidOperationException("Invalid Expression: Not enough operands for operation.");
                        }
                        
                        int number2 = value.Pop();
                        string operand1 = operand.Pop();
                        int number1 = value.Pop();

                        int final = (operand1 == "*") ? number1 * number2 : number1 / number2;
                        value.Push(final);
                    }
                    
                }
                else if (isOperator(token) && (token == "+" || token == "-"))
                {
                    while (operand.Count > 0 && (operand.Peek() == "+" || operand.Peek() == "-" || operand.Peek() == "*" || operand.Peek() == "/"))
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();
                        String operatorr = operand.Pop();
                        int final = (operatorr == "+") ? number1 + number2 : number2 - number1;
                        value.Push(final);
                    }
                    operand.Push(token);
                }
                else if (isOperator(token) && (token == "*" || token == "/"))
                {
                    operand.Push(token);
                }
                else if (token == "(")
                {
                    operand.Push(token);
                }
                else if (token == ")")
                {
                    while (operand.Count > 0 && operand.Peek() != "(")
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();
                        String operatorr = operand.Pop();
                        int final = (operatorr == "+") ? number1 + number2 : number2 - number1;
                        value.Push(final);
                    }

                    if (operand.Count > 0 && (operand.Peek() == "("))
                    {
                        operand.Pop();
                    }

                    while (operand.Count > 0 && (operand.Peek() == "*" || operand.Peek() == "/"))
                    {
                        int number1 = value.Pop();
                        int number2 = value.Pop();
                        String operatorr = operand.Pop();
                        int final = (operatorr == "*") ? number1 * number2 : number1 / number2;
                        value.Push(final);

                    }
                }
                else if (isVariable(token))
                {
                    if (variableEvaluator != null)
                    {
                        int variableValue = variableEvaluator(token);
                        value.Push(variableValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("Variable evaluator delegate not provided.");
                    }
                }
            }

            if (operand.Count > 0)
            {
                if (operand.Count == 1 && (operand.Peek() == "+" || operand.Peek() == "-") && value.Count == 2)
                {
                    int number1 = value.Pop();
                    int number2 = value.Pop();
                    string operatorr = operand.Pop();
                    int final = (operatorr == "+") ? number1 + number2 : number2 - number1;
                    value.Push(final);
                    Console.WriteLine(final);
                }
                else
                {
                    throw new InvalidOperationException("Invalid Expression: Stack not properly emptied.");
                }
            }

        }

            //This method matches any variable given if its uppercase or lowercase and if it contains a number or not.
            //returns if true
            private bool isVariable(string token)
        {
            return Regex.IsMatch(token, "^[a-zA-Z_][a-zA-Z0-9_]*$");
        }


        //if the current token is a digit returns true
        private bool isDigit(string token)
        {
            return int.TryParse(token, out _);
        }

        //This method returns if the current token is a mathematical operator
        private bool isOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }
        
        //Basically just a call method to seperate the tokens in the expression into individual components
        public void TokenizeExpression(string expression)
        {
            tokens = Sep(expression);
        }

        //Used for looking up variables to find their respective value
        public delegate int Lookup(String variable_name);

        //Same as above
        public Evaluator(Lookup variableEvaluator)
        {
            this.variableEvaluator = variableEvaluator;
        }

        /*
         * Used to break down the expression and completely evaluate and give the result. Combines a bunch of 
         * smaller pieces of the assignment into one bigger method
         */
        public static int Evaluate(String expression,Lookup variableEvaluator)
        {
            List<string> tokens = Sep(expression);

            Evaluator evaluate = new Evaluator(variableEvaluator);
            evaluate.tokens = tokens;
            evaluate.EvalTokens();

            return evaluate.value.Pop();

        }

        /*
         * Used to completely break down the expression into individual pieces or components. This then
         * allows for the program to identify whether the pieces are digits or operators, and deal with them
         * respectively
         */
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
   
        //Returns the final value after doing all the math
        public Stack<int> getValueS()
        {
            return value;
        }
    
    }
}
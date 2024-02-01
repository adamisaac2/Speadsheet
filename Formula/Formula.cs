// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace SpreadsheetUtilities
{
  /// <summary>
  /// Represents formulas written in standard infix notation using standard precedence
  /// rules.  The allowed symbols are non-negative numbers written using double-precision 
  /// floating-point syntax (without unary preceeding '-' or '+'); 
  /// variables that consist of a letter or underscore followed by 
  /// zero or more letters, underscores, or digits; parentheses; and the four operator 
  /// symbols +, -, *, and /.  
  /// 
  /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
  /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
  /// and "x 23" consists of a variable "x" and a number "23".
  /// 
  /// Associated with every formula are two delegates:  a normalizer and a validator.  The
  /// normalizer is used to convert variables into a canonical form, and the validator is used
  /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
  /// that it consist of a letter or underscore followed by zero or more letters, underscores,
  /// or digits.)  Their use is described in detail in the constructor and method comments.
  /// </summary>
  public class Formula
  {
    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    /// 
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.  
    /// </summary>
    public Formula(String formula) :
        this(formula, s => s, s => true)
    {
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    /// 
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.  
    /// 
    /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
    /// throws a FormulaFormatException with an explanatory message. 
    /// 
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    /// 
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    /// 
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
   
    private List<string> tokens = new List<string>();
        private Func<string, string> normalizer;
    public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
            if (String.IsNullOrWhiteSpace(formula))
            {
                throw new FormulaFormatException("The formula cannot be empty or whitespace.");
            }

            int parenthesesBalance = 0;
            bool previousTokenWasOperatorOrOpeningParenthesis = true;

            foreach (var token in GetTokens(formula))
            {
                // Check for balanced parentheses and valid token order
                if (token == "(")
                {
                    parenthesesBalance++;
                    tokens.Add(token);
                }
                else if (token == ")")
                {
                    parenthesesBalance--;
                    tokens.Add(token);
                    if (parenthesesBalance < 0)
                    {
                        throw new FormulaFormatException("Unbalanced parentheses in the formula.");
                    }
                }

                if (IsOperator(token) || token == "(")
                {
                    if (previousTokenWasOperatorOrOpeningParenthesis && token != "(")
                    {
                        throw new FormulaFormatException("Two operators in a row or operator after opening parenthesis.");
                    }
                    previousTokenWasOperatorOrOpeningParenthesis = true;
                    if (IsOperator(token))
                    {
                        tokens.Add(token);
                    }
                }
                else if (token == ")")
                {
                    previousTokenWasOperatorOrOpeningParenthesis = false;
                }
                else // token is a number or variable
                {
                    if (!previousTokenWasOperatorOrOpeningParenthesis)
                    {
                        throw new FormulaFormatException("Missing operator between operands.");
                    }
                    previousTokenWasOperatorOrOpeningParenthesis = false;

                    // Normalize and validate variables
                    if (IsVariable(token))
                    {
                        var normalizedVariable = normalize(token);
                        if (!isValid(normalizedVariable))
                        {
                            throw new FormulaFormatException($"Invalid variable '{token}' in formula.");
                        }
                        tokens.Add(normalizedVariable);
                    }
                    else
                    {
                        tokens.Add(token);
                    }
                }
            }

            if (parenthesesBalance != 0)
            {
                throw new FormulaFormatException("Unbalanced parentheses in the formula.");
            }

            if (previousTokenWasOperatorOrOpeningParenthesis)
            {
                throw new FormulaFormatException("Formula ends with an operator.");
            }

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            {
                Stack<double> value = new Stack<double>();
                Stack<string> operators = new Stack<string>();

                foreach (string token in tokens)
                {

                    Console.WriteLine("Token: " + token);
                    Console.WriteLine("Value Stack: " + string.Join(", ", value));
                    Console.WriteLine("Operand Stack: " + string.Join(", ", operators));
                    Console.WriteLine("");



                    if (IsNumeric(token))
                    {
                        double tokenDigit;
                        double.TryParse(token, out tokenDigit);
                        value.Push(tokenDigit);

                        while (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                        {

                            if (value.Count < 2)
                            {
                                throw new ArgumentException("Invalid Expression: Not enough operands for operation.");
                            }

                            double number2 = value.Pop();
                            string operand1 = operators.Pop();
                            double number1 = value.Pop();

                            double final = (operand1 == "*") ? number1 * number2 : number1 / number2;
                            value.Push(final);
                        }

                    }
                    else if (IsOperator(token) && (token == "+" || token == "-"))
                    {
                        while (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-" || operators.Peek() == "*" || operators.Peek() == "/"))
                        {
                            double number1 = value.Pop();
                            double number2 = value.Pop();
                            String operatorr = operators.Pop();
                            double final = (operatorr == "+") ? number1 + number2 : number2 - number1;
                            value.Push(final);
                        }
                        operators.Push(token);
                    }
                    else if (IsOperator(token) && (token == "*" || token == "/"))
                    {
                        operators.Push(token);
                        continue;
                    }
                    else if (token == "(")
                    {
                        operators.Push(token);
                    }
                    else if (token == ")")
                    {
                        if (!operators.Contains("("))
                        {
                            throw new ArgumentException("Unmatched Parenthesis");
                        }

                        while (operators.Count > 0 && operators.Peek() != "(")
                        {
                            double number1 = value.Pop();
                            double number2 = value.Pop();
                            String operatorr = operators.Pop();
                            double final = 0;
                            switch (operatorr)
                            {
                                case "+":
                                    final = number2 + number1;
                                    break;
                                case "-":
                                    final = number2 - number1;
                                    break;
                                case "*":
                                    final = number2 * number1;
                                    break;
                                case "/":
                                    if (number1 == 0)
                                    {
                                        throw new ArgumentException("Cannot divide by zero.");
                                    }
                                    final = number2 / number1;
                                    break;
                                default:
                                    throw new ArgumentException("Unsupported operator: " + operatorr);
                            }
                            value.Push(final);
                        }
                        operators.Pop();
                    }

                    else if (IsVariable(token))
                    {
                        try
                        {

                            value.Push(lookup(token));
                        }
                        catch (ArgumentException)
                        {
                            return new FormulaError($"Undefined variable: {token}");
                        }
                    }


                    else if (operators.Count > 0 && (operators.Peek() == "("))
                    {
                        operators.Pop();
                    }

                    while (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                    {
                        double number1 = value.Pop();
                        double number2 = value.Pop();
                        String operatorr = operators.Pop();
                        double final = (operatorr == "*") ? number1 * number2 : number1 / number2;
                        value.Push(final);

                    }



                }

                Console.WriteLine("Before final evaluation:");
                Console.WriteLine("Value Stack: " + string.Join(", ", value));
                Console.WriteLine("Operand Stack: " + string.Join(", ", operators));



                while (operators.Count > 0)
                {
                    if (value.Count < 2)
                    {
                        throw new ArgumentException("Invalid Expression: Not enough operands for operation.");
                    }

                    double number2 = value.Pop();
                    double number1 = value.Pop();
                    string operatorr = operators.Pop();

                    if (operatorr == "/" && number2 == 0)
                    {
                        throw new ArgumentException("Division by zero is not allowed");
                    }

                    double final;
                    switch (operatorr)
                    {
                        case "+":
                            final = number1 + number2;
                            break;
                        case "-":
                            final = number1 - number2;
                            break;
                        case "*":
                            final = number1 * number2;
                            break;
                        case "/":
                            if (number1 == 0)
                            {
                                throw new ArgumentException("Cannot divide by zero.");
                            }
                            final = number2 / number1;
                            break;
                        default:
                            throw new ArgumentException("Invalid operator.");
                    }
                    value.Push(final);
                }

                if (value.Count == 1 && operators.Count == 0)
                {
                    return value.Pop();
                }
                else
                {
                    throw new ArgumentException();
                }

            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
    {
            if (tokens == null || normalizer == null) return Enumerable.Empty<string>();

            HashSet<string> normalizedVariables = new HashSet<string>();

        foreach(var token in tokens)
        {
                if (IsVariable(token))
                {
                    string normalizedVariable = normalizer(token);
                    normalizedVariables.Add(normalizedVariable);
                }
        }

        return normalizedVariables;

    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    /// 
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
            if (tokens == null)
            {
                return ""; // or some other appropriate response when tokens are null
            }

            StringBuilder sb = new StringBuilder();
            foreach (var token in tokens)
            {
                if (IsVariable(token))
                {
                    if (normalizer == null)
                    {
                        // Handle the case where normalizer is null
                        sb.Append(token); // Append the token as is, or handle differently
                    }
                    else
                    {
                        sb.Append(normalizer(token));
                    }
                }
                else
                {
                    sb.Append(token);
                }
            }

            return sb.ToString();

        }

    /// <summary>
    ///  <change> make object nullable </change>
    ///
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    /// 
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings 
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" 
    /// by C#'s standard conversion from string to double, then back to string. This 
    /// eliminates any inconsistencies due to limited floating point precision.
    /// Variable tokens are considered equal if their normalized forms are equal, as 
    /// defined by the provided normalizer.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///  
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Formula other = (Formula)obj;

            if (this.tokens == null || other.tokens == null)
            {
                return false; // One of the tokens lists is null
            }

            if (this.tokens.Count != other.tokens.Count)
            {
                return false;
            }

            for (int i = 0; i < this.tokens.Count; i++)
            {
                string token1 = this.tokens[i];
                string token2 = other.tokens[i];

                if (IsVariable(token1) && IsVariable(token2))
                {
                    if (this.normalizer == null || other.normalizer == null)
                    {
                        return false; // One of the normalizers is null
                    }

                    if (this.normalizer(token1) != other.normalizer(token2))
                    {
                        return false;
                    }
                }
                else if (token1 != token2)
                {
                    return false;
                }
            }

            return true;
        }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// 
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
      return f1.Equals(f2);
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
    ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
            return !(f1 == f2);
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
            HashCode hash = new HashCode();
            foreach(var token in tokens)
            {
                hash.Add(token);
            }
            return hash.ToHashCode();

    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
    /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
    /// match one of those patterns.  There are no empty tokens, and no token contains white space.
    /// </summary>
    public static IEnumerable<string> GetTokens(String formula)
    {
      // Patterns for individual tokens
      String lpPattern = @"\(";
      String rpPattern = @"\)";
      String opPattern = @"[\+\-*/]";
      String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
      String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
      String spacePattern = @"\s+";

      // Overall pattern
      String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                      lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

      // Enumerate matching tokens that don't consist solely of white space.
      foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
      {
        if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
        {
          yield return s;
        }
      }

    }

      

        private bool IsNumeric(string token)
        {
            return double.TryParse(token, out _);
        }

        private static bool IsVariable(string token)
        {
            // Adapted from the isVariable method in Evaluator class
            // Define the variable pattern
            string pattern = @"^[a-zA-Z_][a-zA-Z0-9_]*$";
            return Regex.IsMatch(token, pattern);
        }

        private static bool IsOperator(string token)
        {
            // Define the operator pattern
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

  }

  /// <summary>
  /// Used to report syntactic errors in the argument to the Formula constructor.
  /// </summary>
  public class FormulaFormatException : Exception
  {
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(String message)
        : base(message)
    {
    }
  }

  /// <summary>
  /// Used as a possible return value of the Formula.Evaluate method.
  /// </summary>
  public struct FormulaError
  {
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(String reason)
        : this()
    {
      Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; } 
    }

}




// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>

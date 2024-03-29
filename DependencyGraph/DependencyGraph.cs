﻿///<summary>
/// File: DependencyGraph.cs
/// Purpose: Contains the implementation of the DependencyGraph class.
/// Author: Adam Isaac
/// Created: January 20, 2024
/// Modified: - January 26, 2024 - Added all methods for submission
/// </summary>            

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private HashSet<Tuple<string, string>> dependencySet = new HashSet<Tuple<string,string>>();
       
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return dependencySet.Count; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return GetDependees(s).Count(); }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            foreach (Tuple<string, string> dependency in dependencySet)
            {
                if (dependency.Item1 == s)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            foreach(Tuple<string, string> dependency in dependencySet)
            {
                if (dependency.Item2 == s)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            foreach (Tuple<string, string> dependency in dependencySet)
            {
                if (dependency.Item1 == s)
                {
                    yield return dependency.Item2;
                }
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            foreach(Tuple<string, string> dependency in dependencySet)
            {
                if(dependency.Item2 == s)
                {
                    yield return dependency.Item1;
                }
            }
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            Tuple<string, string> dependency = Tuple.Create(s, t);

            if (!dependencySet.Contains(dependency))
            {
                dependencySet.Add(dependency);
            }
        
        
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            Tuple<string, string> remove = Tuple.Create(s, t);

            if (dependencySet.Contains(remove))
            {
                dependencySet.Remove(remove);
            }

        
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            dependencySet.RemoveWhere(pair => pair.Item1 == s);

            foreach(string t in newDependents)
            {
                Tuple<string, string> newDependency = Tuple.Create(s, t);
                dependencySet.Add(newDependency);
            }
        
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {

            //Exception handling
            if (newDependees == null)
            {
                throw new ArgumentNullException(nameof(newDependees));
            }
            
            dependencySet.RemoveWhere(pair => pair.Item2 == s);

            foreach(string t in newDependees)
            {
                Tuple<string, string> newDependency = Tuple.Create(t, s);
                dependencySet.Add(newDependency);
            }
           

        }
        public void Clear()
        {
            dependencySet.Clear();
        }


    }

}
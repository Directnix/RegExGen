﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegExGen
{
    [Serializable]
    public class Transition : IComparable<Transition>
    {
        public static readonly char EPSILON = 'ε';

        public string fromState;
        public char symbol;// edge
        public string toState;

        public int CompareTo(Transition other)
        {
            int fromCmp = fromState.CompareTo(other.fromState);//Vergelijk de 2 fromstates
            int symbolCmp = symbol.CompareTo(other.symbol); //Vergelijk de 2 symbolen van de overgang
            int toCmp = toState.CompareTo(other.toState);//Vergelijk de 2 to States

            return (fromCmp != 0 ? fromCmp : (symbolCmp != 0 ? symbolCmp : toCmp));
        }

        public Transition()
        {
            
        }

        // this constructor can be used to define loops:
        public Transition(string fromOrTo, char s)
        {
            this.fromState = fromOrTo;
            this.symbol = s;
            this.toState = fromOrTo;
        }

        public Transition(string from, string to)
        {
            this.fromState = from;
            this.symbol = Automata.EPSILON;
            this.toState = to;
        }


        public Transition(string from, char s, string to)
        {
            this.fromState = from;
            this.symbol = s;
            this.toState = to;
        }


        // overriding equals
        public bool equals(Object other)
        {
            if (other == null)
            {
                return false;
            }
            else if(other is Transition)
            {
                return this.fromState.Equals(((Transition)other).fromState) && this.toState.Equals(((Transition)other).toState) && this.symbol == (((Transition)other).symbol);
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "(" + fromState + ") --> " + symbol + " --> (" + toState + ")";
        }
    }
}

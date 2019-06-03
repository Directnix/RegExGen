﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegExGen
{
    class Transition : IComparable<Transition>   //<T extends Comparable> implements Comparable<Transition<T>>
    {
        public static readonly char EPSILON = '$';

        private string fromState;
        private char symbol;// edge
        private string toState;

        public int CompareTo(Transition other)
        {
            int fromCmp = fromState.CompareTo(other.fromState);//Vergelijk de 2 fromstates
            int symbolCmp = symbol.CompareTo(other.symbol); //Vergelijk de 2 symbolen van de overgang
            int toCmp = toState.CompareTo(other.toState);//Vergelijk de 2 to States

            return (fromCmp != 0 ? fromCmp : (symbolCmp != 0 ? symbolCmp : toCmp));
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
            this.symbol = EPSILON;
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

        public string getFromState()
        {
            return fromState;
        }

        public string getToState()
        {
            return toState;
        }

        public char getSymbol()
        {
            return symbol;
        }

        public String toString()
        {
            return "(" + this.getFromState() + ", " + this.getSymbol() + ")" + "-->" + this.getToState();
        }
    }
}
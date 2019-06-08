﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace RegExGen
{
    public class Automata
    {
        public static readonly string EMPTY = "#";

        // node - edge - node
        public SortedSet<Transition> transitions = new SortedSet<Transition>();

        public SortedSet<string> states {
            get {
                return new SortedSet<string>( 
                    transitions.SelectMany(tr => new[] { tr.toState, tr.fromState }));}
        }
        public SortedSet<string> startStates;
        public SortedSet<string> finalStates;
        // alphabet
        public SortedSet<char> symbols;

        public Automata()
        {
            startStates = new SortedSet<string>();
            finalStates = new SortedSet<string>();
            this.setAlphabet(new SortedSet<char>());
        }

        public Automata(Automata newAutomata)
        {
            transitions = newAutomata.transitions; 
            startStates = newAutomata.startStates;
            finalStates = newAutomata.finalStates;
            symbols = newAutomata.symbols;
            //states = newAutomata.states;
        }

        public Automata(char[] s)
        {
            startStates = new SortedSet<string>();
            finalStates = new SortedSet<string>();
            this.setAlphabet(new SortedSet<char>(s.ToList()));
        }

        public Automata(SortedSet<char> symbols)
        {
            startStates = new SortedSet<string>();
            finalStates = new SortedSet<string>();
            this.setAlphabet(symbols);
        }

        public void setAlphabet(char[] s)
        {
            this.setAlphabet(new SortedSet<char>(s.ToList()));
        }

        public void setAlphabet(SortedSet<char> symbols)
        {
            this.symbols = symbols;
        }

        public SortedSet<char> getAlphabet()
        {
            return symbols;
        }

        public void addTransition(Transition t)
        {
            transitions.Add(t);
            states.Add(t.fromState);
            states.Add(t.toState);
            symbols.Add(t.symbol);
        }

        public void defineAsStartState(string t)
        {
            // if already in states no problem because a Set will remove duplicates.
            startStates.Add(t);
        }

        public void defineAsFinalState(string t)
        {
            // if already in states no problem because a Set will remove duplicates.
            finalStates.Add(t);
        }

        public void printTransitions()
        {

            foreach(Transition t in transitions)
            {
                Debug.WriteLine(t.ToString());
            }
        }

        public SortedSet<string> getToStates(string from, char withSymbol)
        {
            return new SortedSet<string>(
            transitions.Where(transition => transition.symbol == withSymbol)
                .Select(transitionWithSymbol => transitionWithSymbol.toState)
            );
        }

        public bool isDFA()
        {
            bool isDFA = true;

            // elke node heeft elk symbool precies 1 keer als uitgaande pijl
            foreach (string from in states)
            {
                foreach (char symbol in symbols)
                {
                    isDFA = isDFA && getToStates(from, symbol).Count == 1;
                    if (!isDFA) return false;
                }
            }

            // geen epsilon overgangen
            isDFA = isDFA && transitions.Any(transitions => transitions.symbol != Transition.EPSILON);

            // geen meerdere start toestanden
            isDFA = isDFA && startStates.Count > 1; 
            return isDFA;
        }

        public Automata Inverse()
        {
            Automata returnAutomata = new Automata(this);
            returnAutomata.startStates = finalStates;
            returnAutomata.finalStates = startStates;
            returnAutomata.transitions = GetInverseTransitions();
            return returnAutomata;
        }

        public SortedSet<Transition> GetInverseTransitions()
        {
            SortedSet<Transition> inverseTransitions = new SortedSet<Transition>();
            foreach (Transition t in transitions)
            {
                inverseTransitions.Add(new Transition(t.toState, t.symbol, t.fromState));
            }
            return inverseTransitions;
        }

        //Returns the NOT of the automata.
        public Automata Not()
        {
            Automata returnAutomata = new Automata(this);
            returnAutomata.startStates = finalStates;
            returnAutomata.finalStates = startStates;
            return returnAutomata;
        }

        //Returns the AND of the automata.
        public Automata And(Automata other)
        {
            if ((getAlphabet().Equals(other.getAlphabet())))
            {
                throw new Exception("You cant AND these 2 Automata, because the alphabets are not the same.");
            }

            if (!isDFA() || !other.isDFA())
            {
                throw new Exception("One or both of the automata aren't DFA's");
            }

            //Creat Hulptabellen.
            string[] statesList = states.ToArray();
            char[] letterList = getAlphabet().ToArray();
            string[,] hulpTabel = new string[states.Count, getAlphabet().Count];
            
            //Hulptabel vullen.
            for (int letter = 0; letter < getAlphabet().Count; letter++)
            {
                for (int state = 0; state < states.Count; state++)
                {
                    hulpTabel[state, letter] = GetDestination(state, letter);
                }
            }

            print2dArray(hulpTabel);

            string[] statesListOther = other.states.ToArray();
            char[] letterListOther = other.getAlphabet().ToArray();
            string[,] hulpTabelOther = new string[other.states.Count,other.getAlphabet().Count];

            //Hulptabel vullen.
            for (int letter = 0; letter < other.getAlphabet().Count; letter++)
            {
                for (int state = 0; state < other.states.Count; state++)
                {
                    hulpTabelOther[state, letter] = other.GetDestination(state, letter);
                }
            }

            print2dArray(hulpTabelOther);



            var result = new SortedSet<Transition>();
            var currentStates = new SortedSet<Tuple<string, string>>();
            bool notDone = true;
            Automata returnAutomata = new Automata();

            returnAutomata.defineAsStartState(startStates.First() + "-" + other.startStates.First()); //Defineer de states die in zowel 1 als 2 start zijn.

            currentStates.Add(Tuple.Create(startStates.First(), other.startStates.First()));//Create the first mixed state (0,0)
            //Heb nu beide hulparrays,
            while (result.Count < transitions.Count * other.transitions.Count && notDone)//blijf doorgaan tot het maximum aantal states bereikt is.
            {
                var nextStates = new SortedSet<Tuple<string, string>>();
                foreach (var currentState in currentStates)//Alle huidige states             
                {
                    if (finalStates.Contains(currentState.Item1) && other.finalStates.Contains(currentState.Item2))//check of beide states in hun eigen automaat eindstates zijn.
                    {
                        returnAutomata.defineAsFinalState(currentState.Item1 + "-" + currentState.Item2);
                    }
                    foreach (char symbol in symbols)//ga alle sybolen langs voor de 2 states.
                    {
                        var nextState = 
                            new Tuple<string,string>(
                                FindDestinationBasedOnSymbolAndState(hulpTabel, statesList, letterList,currentState.Item1,symbol),
                                FindDestinationBasedOnSymbolAndState(hulpTabelOther, statesListOther, letterListOther, currentState.Item2, symbol)
                                );//De state waar het symbool naartoe gaat.
                        Debug.WriteLine("Found State: "+nextState.Item1 + "-" + nextState.Item2);
                        nextStates.Add(nextState);//Voeg de nieuwe state toe aan de lijst, zodat deze hierna behandeld kan worden.
                        result.Add(new Transition(currentState.Item1 + "-" + currentState.Item2, symbol, nextState.Item1 + "-" + nextState.Item2));
                        Debug.WriteLine("Found Transition: "+ currentState.Item1 + "-" + currentState.Item2 + " --> " +  symbol + " --> " + nextState.Item1 + "-" + nextState.Item2);
                    }
                }

                if (CompateSets(currentStates,nextStates))
                {
                    notDone = false;
                }
                currentStates = nextStates;
            }



            foreach (var t in result)
            {
                returnAutomata.addTransition(t);
            }

            return returnAutomata;
        }

        public bool CompateSets(SortedSet<Tuple<string,string>> s1, SortedSet<Tuple<string, string>> s2)
        {
            if (s1.Count != s2.Count)
            {
                return false;
            }

            for (int i = 0; i < s1.Count; i++)
            {
                if (s1.ToArray()[i].Item1 != s2.ToArray()[i].Item1 ||
                    s1.ToArray()[i].Item1 != s2.ToArray()[i].Item1)
                {
                    return false;
                }
            }


            return true;
        }

        public string FindDestinationBasedOnSymbolAndState(string[,] hulptable,string[] stateTable, char[] symbolTable,string fromState, char symbol)
        {
            if (fromState == EMPTY)
            {
                return EMPTY;
            }
            int indexOfState = -1;
            int indexOfSymbol = -1;

            int stateLenght = stateTable.GetLength(0);
            for (int i = 0; i < stateLenght; i++)
            {
                if (stateTable[i] == fromState)
                {
                    indexOfState = i;
                }
            }

            int symbolLenght = symbolTable.GetLength(0);
            for (int i = 0; i < symbolLenght; i++)
            {
                if (symbolTable[i] == symbol)
                {
                    indexOfSymbol = i;
                }
            }

            if (indexOfState >= 0 && indexOfSymbol >= 0)
                return hulptable[indexOfState, indexOfSymbol];
            throw new Exception("Cant find state");
        }

        public void print2dArray(string[,] arr)
        {
            //long[,] arr = new long[5, 4] { { 1, 2, 3, 4 }, { 1, 1, 1, 1 }, { 2, 2, 2, 2 }, { 3, 3, 3, 3 }, { 4, 4, 4, 4 } };

            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", arr[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        public string GetDestination(int state, int letter)
        {
            //Zoek de state op die bij het nummer hoort
            string previousState = transitions.First().fromState;
            int stateCounter = 0;
            string savedState = "#";
            foreach (Transition t in transitions)
            {
                if (!t.fromState.Equals(previousState))
                {
                    stateCounter++;
                }

                if (stateCounter == state)
                {
                    //Will enter this if once for every letter in the alphabet.
                    savedState = t.fromState;
                }
                previousState = t.fromState;
            }

            //Zoek de letter op die bij het nummer hoort.
            char previousLetter = transitions.First().symbol;
            int letterCounter = 0;
            char savedLetter = transitions.First().symbol;
            foreach (char t in symbols)
            {
                if (!t.Equals(previousLetter))
                {
                    letterCounter++;
                }

                if (letterCounter == letter)
                {
                    savedLetter = t;
                }

                previousLetter = t;
            }

            Debug.Write("Destination for: "+ savedState + " -> " + savedLetter + "-> ");
            var transition = transitions.Where(Transition => (Transition.fromState == savedState && Transition.symbol == savedLetter));
            string toState;

            if (transition.Count() > 0)
                toState = transition.First().toState;
            else
                toState = EMPTY;

            Debug.Write(toState+"\n");

            return toState;
        }

        //Returns the OR of the automata.
        public Automata Or(Automata other)
        {
            return this;
        }

    }
}

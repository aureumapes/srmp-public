using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRMultiplayer
{
    public class ConsoleInput
    {
        public string inputString = "";
        public event Action<string> OnInputText;

        public void ClearLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.BufferWidth));
            Console.CursorTop--;
            Console.CursorLeft = 0;
        }

        public void RedrawInputLine()
        {
            bool flag = Console.CursorLeft > 0;
            if (flag)
            {
                this.ClearLine();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            Console.Write(this.inputString);
        }

        internal void OnBackspace()
        {
            bool flag = this.inputString.Length <= 0;
            if (!flag)
            {
                this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
                this.RedrawInputLine();
            }
        }

        internal void OnEscape()
        {
            this.ClearLine();
            this.inputString = "";
            this.RedrawInputLine();
        }

        internal void OnEnter()
        {
            this.ClearLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("> " + this.inputString);
            string obj = this.inputString;
            this.inputString = "";
            bool flag = this.OnInputText != null;
            if (flag)
            {
                //on text inputed reset the search loc and cycle the search tree
                searchLoc = -1; //searchLoc set to -1 to always go to place 0 on first arrow
                AddToCommandTree(obj);

                this.OnInputText(obj);
            }
        }

        List<string> cmdTree = new List<string>();
        internal void AddToCommandTree(string cmdText)
        {
            cmdTree.Insert(0,cmdText);
         
            //if tree gets larger than 10 remove the 11th item
            if (cmdTree.Count > 10) { cmdTree.RemoveAt(10); };
        }


        //handle internal cycling of last 10 commands
        int searchLoc = -1;
        internal void GetCommand(int diff)
        {
            if (cmdTree.Count > 0)
            {
                searchLoc = searchLoc + diff;
                //prevent below 0 or over max position
                if (searchLoc > (cmdTree.Count - 1)) searchLoc = (cmdTree.Count - 1);
                if (searchLoc < 0) searchLoc = 0;

                //if a new location is found enter the search text in the input and redraw it.
                this.inputString = cmdTree[searchLoc];
                this.RedrawInputLine();
            }
            else
            {
                Console.WriteLine("cmdTree is empty");
            }
        }

        public void Update()
        {
            bool flag = !Console.KeyAvailable;
            if (!flag)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                bool flag2 = consoleKeyInfo.Key == ConsoleKey.Enter;
                if (flag2)
                {
                    this.OnEnter();
                }
                else
                {
                    bool flag3 = consoleKeyInfo.Key == ConsoleKey.Backspace;
                    if (flag3)
                    {
                        this.OnBackspace();
                    }
                    else
                    {
                        bool flag4 = consoleKeyInfo.Key == ConsoleKey.Escape;
                        if (flag4)
                        {
                            this.OnEscape();
                        }
                        else
                        {
                            bool flag5 = consoleKeyInfo.KeyChar > '\0';
                            if (flag5)
                            {
                                this.inputString += consoleKeyInfo.KeyChar.ToString();
                                this.RedrawInputLine();
                            }
                            else
                            {
                                //handle up pressed to get previous lines 
                                bool flag6 = consoleKeyInfo.Key == ConsoleKey.UpArrow;
                                if (flag6)
                                {
                                    this.GetCommand(1);
                                }
                                else
                                {
                                    //handle down pressed to get next lines 
                                    bool flag7 = consoleKeyInfo.Key == ConsoleKey.DownArrow;
                                    if (flag7)
                                    {
                                        this.GetCommand(-1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

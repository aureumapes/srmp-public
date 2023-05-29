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
                this.OnInputText(obj);
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
                        }
                    }
                }
            }
        }
    }
}

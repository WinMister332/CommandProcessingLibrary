﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WMCommandFramework
{
    public class CommandProcessor
    {
        public static CommandProcessor DEFAULT_INSTANCE = null;
        public bool Echo
        {
            get => CommandUtilities.Echo;
            set => CommandUtilities.Echo = value;
        }
        public AppName ApplicationName
        {
            get => CommandUtilities.ApplicationName;
            set => CommandUtilities.ApplicationName = value;
        }
        public bool Running = true;
        public ConsoleColor DefaultFGColor { get; set; } = ConsoleColor.White;
        public ConsoleColor DefaultBGColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor CurrentFGColor { get; set; } = Console.ForegroundColor;
        public ConsoleColor CurrentBGColor { get; set; } = Console.BackgroundColor;

        public TerminalMessage Message
        {
            get => CommandUtilities.Message;
            set
            {
                if (CommandUtilities.Message == null)
                {
                    value = TerminalMessage.EMPTY;
                    value.SetProcessor(this);
                    CommandUtilities.Message = value;
                }
                else
                {
                    value.SetProcessor(this);
                    CommandUtilities.Message = value;
                }
            }
        }

        private CommandInvoker invoker = null;

        public CommandProcessor()
        {
            DEFAULT_INSTANCE = this;
            invoker = new CommandInvoker();
            invoker.SetProcessor(this);
            Message.SetProcessor(this);
            Console.InputEncoding = Encoding.GetEncoding(65001);
            Console.OutputEncoding = Encoding.GetEncoding(65001);
        }

        public CommandProcessor(CommandInvoker invoker)
        {
            DEFAULT_INSTANCE = this;
            if (invoker == null)
            {
                invoker = new CommandInvoker();
                invoker.SetProcessor(this);
            }
            else
            {
                invoker.SetProcessor(this);
                this.invoker = invoker;
                if (this.invoker.GetProcessor() == null || (this.invoker.GetProcessor() != null && this.invoker.GetProcessor() != this))
                    this.invoker.SetProcessor(this);
            }
            Message.SetProcessor(this);
            Console.InputEncoding = Encoding.GetEncoding(65001);
            Console.OutputEncoding = Encoding.GetEncoding(65001);
        }

        public void Process(bool loop)
        {
            Running = true;
            //Double check to be sure "Message" is not null.
            if (Message == null)
            {
                Message = TerminalMessage.EMPTY;
                Message.SetProcessor(this);
            }
            if (loop)
            {
                while (Running)
                {
                    if (Echo)
                    {
                        //Write Echo and Read Commnads.
                        WriteMessage();
                        var input = Console.ReadLine();
                        if (!(input == null && input == ""))
                            //Forward to Invoker.
                            invoker.Invoke(input);
                    }
                    else
                    {
                        //Read Commands Only.
                        var input = Console.ReadLine();
                        if (!(input == null && input == ""))
                            //Forward to Invoker.
                            invoker.Invoke(input);
                    }
                }
            }
            else
            {
                if (!Running) return;
                if (Echo)
                {
                    //Write Echo and Read Commnads.
                    WriteMessage();
                    var input = Console.ReadLine();
                    if (!(input == null && input == ""))
                        //Forward to Invoker.
                        invoker.Invoke(input);
                }
                else
                {
                    //Read Commands Only.
                    var input = Console.ReadLine();
                    if (!(input == null && input == ""))
                        //Forward to Invoker.
                        invoker.Invoke(input);
                }
            }
        }

        public void WriteMessage()
        {
            if (Echo)
            {
                foreach (TerminalMessage.Message m in Message.GetMessage().ToArray())
                {
                    if (!(m == null))
                    {
                        Console.ForegroundColor = m.GetColor();
                        Console.Write(m.GetMessage());
                        Console.ForegroundColor = DefaultFGColor;
                    }
                }
                Console.Write("> ");
            }
        }

        public void ExitProcessor()
            => Running = false;
    }
}

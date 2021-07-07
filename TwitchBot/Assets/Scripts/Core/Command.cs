using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    public enum TypeCommand { Action, Text }

    public class BotCommand
    {
        public string Text;
        public Action<string[], Message> commandScript;
        public TypeCommand typeCommand;
        public bool modOnly;
        public bool subOnly;

        public BotCommand(string Text, bool modOnly = false, bool subOnly = false)
        {
            typeCommand = TypeCommand.Text;
            this.Text = Text;
            this.modOnly = modOnly;
            this.subOnly = subOnly;
        }
        public BotCommand(Action<string[], Message> CommandScript, bool modOnly = false, bool subOnly = false)
        {
            typeCommand = TypeCommand.Action;
            this.commandScript = CommandScript;
            this.modOnly = modOnly;
            this.subOnly = subOnly;
        }

        public void Invoke(string[] args, Message message)
        {
            if (!this.modOnly || (this.modOnly && message.isMod))
            {
                if (!this.subOnly || (this.subOnly && message.isSubscriber))
                {
                    commandScript.Invoke(args, message);
                }
            }
        }
        public void Invoke()
        {
            commandScript.Invoke(new string[] { }, new Message());
        }
    }
}

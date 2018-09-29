using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class Bot
    {
        const string server = "irc.chat.twitch.tv";
        const int port = 6667;

        public string username, password, channel;
        public List<string> Users = new List<string>();
        public Action<string, string> whenNewMessage;
        public Action<string> whenNewSystemMessage;
        public Action<string> whenNewChater;
        public Action whenStart;
        public Action whenDisconnect;
        public Dictionary<string, BotCommand> commands;

        bool open = false;
        StreamReader reader;
        StreamWriter writer;


        public Bot(string username, 
                    string password, 
                    string channel,
                    Dictionary<string, BotCommand> commands = null)
        {
            this.username = username;
            this.password = password;
            this.channel = channel;
            this.commands = commands;
        }

        public void Send(string message)
        {
            if (open == false)
                return;

            writer.WriteLine(message);
            writer.Flush();
        }
        public void SendMessage(string message)
        {
            if (open == false)
                return;

            Send(string.Format("PRIVMSG #{0} : {1}", channel, message));
            whenNewMessage(username, message);
        }
        public void Parse(string inputLine)
        {          
            if (inputLine.Contains("PING"))
            {
                Send("PONG :tmi.twitch.tv");
            }
            if (inputLine.Contains("PRIVMSG"))
            {
                // Mensaje en el chat
                string[] splitInput = inputLine.Split(new string[] { ";" }, StringSplitOptions.None);
                Dictionary<string, string> message = new Dictionary<string, string>();


                for (int i = 0; i < splitInput.Length; i++)
                {
                    string[] splitZone = splitInput[i].Split('=');
                    message.Add(splitZone[0], splitZone[1]);
                }

                string line = message["user-type"];
                line = line.Split(new string[] { "PRIVMSG" }, StringSplitOptions.None)[1];
                line = line.Split(new string[] { ":" }, StringSplitOptions.None)[1];

                string user = message["display-name"];
                whenNewMessage.Invoke(user, line);

                if (!Users.Contains(user))
                {
                    Users.Add(user);
                    whenNewChater.Invoke(user);
                }

                var commandInLine = line.Split(' ')[0];
                var args = line.Split(' ');
                args[0] = "";

                if (message["display-name"].ToLower() == channel.ToLower())
                    message["mod"] = "1";

                if (commands == null)
                    return;

                Process(commandInLine.ToLower(), message, args);

            }
            else
            {
                whenNewSystemMessage.Invoke(inputLine);
            }
            // DEBUG
            //Console.WriteLine(inputLine);

        }

        public void Process(string commandInLine, Dictionary<string, string> message, string[] args)
        {
            if (!commands.ContainsKey(commandInLine.ToLower()))
                return;

            var command = commands[commandInLine];

            if ((command.modOnly && message["mod"] == "1") || !command.modOnly)
            {
                if ((command.subOnly && message["subscriber"] == "1") || !command.subOnly)
                {
                    whenNewSystemMessage.Invoke("Ejecutando comando " + commandInLine);

                    if (command.typeCommand == TypeCommand.Action)
                    {
                        command.Invoke(args, message);
                    }
                    else if (command.typeCommand == TypeCommand.Text)
                    {
                        var text = command.Text.Replace("{username}", message["display-name"]);
                        SendMessage(text);
                    }
                }
            }
        }

        public void StartBot(int maxRetry)
        {
            var retry = false;
            var retryCount = 0;
            do
            {
                try
                {
                    using (var irc = new TcpClient(Bot.server, Bot.port))
                    using (var stream = irc.GetStream())
                    using (reader = new StreamReader(stream))
                    using (writer = new StreamWriter(stream))
                    {
                        open = true;

                        Send("CAP REQ :twitch.tv/tags twitch.tv/commands twitch.tv/membership");
                        Send(string.Format("PASS oauth:{0}", password));
                        Send(string.Format("NICK {0}", username));
                        Send(string.Format("JOIN #{0}", channel));

                        whenStart.Invoke();

                        while (true)
                        {
                            string inputLine;
                            while ((inputLine = reader.ReadLine()) != null)
                            {
                                Parse(inputLine);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // shows the exception, sleeps for a little while and then tries to establish a new connection to the IRC server
                    open = false;
                    whenDisconnect.Invoke();
                    whenNewSystemMessage.Invoke($"Error reconecting {retryCount}/{maxRetry}");
                    whenNewSystemMessage.Invoke($"{e.ToString()}");

                    //Thread.Sleep(5000);
                    retry = ++retryCount <= maxRetry;
                }
            } while (retry);
        }
    }
}

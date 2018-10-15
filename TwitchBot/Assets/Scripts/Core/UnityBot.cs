using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;

using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;

namespace TwitchBot
{
	public class UnityBot : MonoBehaviour 
	{
		const string server = "irc.chat.twitch.tv";
        const int port = 6667;
		public string username, password, channel;
		protected List<string> Users = new List<string>();
        protected Action<string, string> whenNewMessage;
        protected Action<string> whenNewSystemMessage;
        protected Action<string> whenNewChater;
		protected Action whenStart;
		protected Action whenDisconnect;
		protected Dictionary<string, BotCommand> commands;

		bool open = false;
        StreamReader reader;
        StreamWriter writer;


        public void Send(string message)
        {
            if (open == false)
                return;

            writer.WriteLine(message);
            writer.Flush();
        }
        public void SendMessageToChat(string message)
        {
            if (open == false)
                return;

            Send(string.Format("PRIVMSG #{0} : {1}", channel, message));
            //whenNewMessage(username, message);
        }

		public IEnumerator StartConnection(int maxRetry)
		{			
			yield return Ninja.JumpToUnity;
            var retryCount = 0;
			while(retryCount <= maxRetry)
			{
				Task gettingDataTask;
				yield return this.StartCoroutineAsync(GettingData(), out gettingDataTask); 
				if (gettingDataTask.State == TaskState.Error) 
				{    
					var e = gettingDataTask.Exception;
					open = false;
					whenDisconnect.Invoke();
					whenNewSystemMessage.Invoke($"Error reconecting {retryCount}/{maxRetry}");
					whenNewSystemMessage.Invoke($"{e.ToString()}");

					//Thread.Sleep(5000);
					retryCount++;
				}
			}
		}

		IEnumerator GettingData()
		{
			using (var irc = new TcpClient(server, port))
			using (var stream = irc.GetStream())
			using (reader = new StreamReader(stream))
			using (writer = new StreamWriter(stream))
			{
				open = true;

				if(password.Contains("oauth:"))
					password = password.Replace("oauth:", "");

				Send("CAP REQ :twitch.tv/tags twitch.tv/commands twitch.tv/membership");
				Send(string.Format("PASS oauth:{0}", password));
				Send(string.Format("NICK {0}", username));
				Send(string.Format("JOIN #{0}", channel));
				
				yield return Ninja.JumpToUnity;
				whenStart.Invoke();
				yield return Ninja.JumpBack;

				while (true)
				{
					string inputLine;
					while ((inputLine = reader.ReadLine()) != null)
					{
						yield return Ninja.JumpToUnity;
						yield return this.StartCoroutineAsync(Parse(inputLine));
						yield return Ninja.JumpBack;
						//; //Process
					}
				}
			}
		}

		IEnumerator Parse(string inputLine)
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

				yield return Ninja.JumpToUnity;
                whenNewMessage.Invoke(user, line);
				yield return Ninja.JumpBack;


                if (!Users.Contains(user))
                {
                    Users.Add(user);
					yield return Ninja.JumpToUnity;
                    whenNewChater.Invoke(user);
					yield return Ninja.JumpBack;
                }

                var commandInLine = line.Split(' ')[0];
                var args = line.Split(' ');
                args[0] = "";

                if (message["display-name"].ToLower() == channel.ToLower())
                    message["mod"] = "1";

                if (commands != null)
				{
					yield return Ninja.JumpToUnity;
					yield return this.StartCoroutineAsync(Process(commandInLine.ToLower(), message, args));
					yield return Ninja.JumpBack;
				}
            }
            else
            {
				yield return Ninja.JumpToUnity;
                whenNewSystemMessage.Invoke(inputLine);
				yield return Ninja.JumpBack;
            }
            // DEBUG
            //Console.WriteLine(inputLine);
        }
		IEnumerator Process(string commandInLine, Dictionary<string, string> message, string[] args)
        {
            if (commands.ContainsKey(commandInLine.ToLower()))
            {
				var command = commands[commandInLine];

				if ((command.modOnly && message["mod"] == "1") || !command.modOnly)
				{
					if ((command.subOnly && message["subscriber"] == "1") || !command.subOnly)
					{
						yield return Ninja.JumpToUnity;
						whenNewSystemMessage.Invoke("Ejecutando comando " + commandInLine);
						yield return Ninja.JumpBack;						

						if (command.typeCommand == TypeCommand.Action)
						{
							yield return Ninja.JumpToUnity;
							command.Invoke(args, message);
							yield return Ninja.JumpBack;
						}
						else if (command.typeCommand == TypeCommand.Text)
						{
							var text = command.Text.Replace("{username}", message["display-name"]);
							SendMessageToChat(text);
						}
					}
				}		
			}
        }
	}
}

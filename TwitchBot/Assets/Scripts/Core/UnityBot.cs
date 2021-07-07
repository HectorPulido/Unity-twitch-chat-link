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

namespace TwitchBot
{
    public class UnityBot : MonoBehaviour
    {
        const string server = "irc.chat.twitch.tv";
        const int port = 6667;
        public string username, oauth, channel;
        protected List<string> Users = new List<string>();
        protected Action<Message> whenNewMessage;
        protected Action<Message> whenSub;
        protected Action<Message> whenResub;
        protected Action<Message> whenRaid;
        protected Action<Message> whenSubfift;
        protected Action<Message> whenBits;
        protected Action<string> whenNewSystemMessage;
        protected Action<string> whenNewChater;
        protected Action whenStart;
        protected Action whenDisconnect;
        protected Dictionary<string, BotCommand> commands;
        protected Dictionary<string, BotCommand> customRewards;

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
            while (retryCount <= maxRetry)
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

                if (oauth.Contains("oauth:"))
                    oauth = oauth.Replace("oauth:", "");

                Send("CAP REQ :twitch.tv/tags twitch.tv/commands twitch.tv/membership");
                Send(string.Format("PASS oauth:{0}", oauth));
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
            else if (inputLine.Contains("PRIVMSG"))
            {
                // Mensaje en el chat
                var msg = Message.Format(inputLine, "PRIVMSG");

                // Bits
                if (msg.message_data.ContainsKey("bits") && whenBits != null)
                {
                    yield return Ninja.JumpToUnity;
                    whenBits.Invoke(msg);
                    yield return Ninja.JumpBack;
                }

                // Handle custom rewards 
                // you can get your custom reward id in
                // https://www.instafluff.tv/TwitchCustomRewardID/?channel=YOURTWITCHCHANNEL
                if (
                    msg.message_data.ContainsKey("custom-reward-id")
                    && customRewards != null
                    && customRewards.ContainsKey("custom-reward-id")
                    )
                {
                    yield return Ninja.JumpToUnity;
                    var args = msg.message.Split(' ');
                    customRewards[msg.message_data["custom-reward-id"]].Invoke(args, msg);
                    yield return Ninja.JumpBack;
                }

                string user = msg.username;

                // Send message callback
                yield return Ninja.JumpToUnity;
                whenNewMessage.Invoke(msg);
                yield return Ninja.JumpBack;

                // if user does't exist, create it
                if (!Users.Contains(user))
                {
                    Users.Add(user);
                    if (whenNewChater != null)
                    {
                        yield return Ninja.JumpToUnity;
                        whenNewChater.Invoke(user);
                        yield return Ninja.JumpBack;
                    }
                }

                if (commands != null)
                {
                    yield return Ninja.JumpToUnity;
                    yield return this.StartCoroutineAsync(Process(msg));
                    yield return Ninja.JumpBack;
                }
            }
            else if (inputLine.Contains("USERNOTICE"))
            {
                // Mensaje en el chat
                var msg = Message.Format(inputLine, "USERNOTICE");

                // Format msg id
                if (msg.message_data.ContainsKey("msg-id"))
                {
                    var msg_type = msg.message_data["msg-id"];

                    if (msg_type == "sub" && whenSub != null)
                    {
                        yield return Ninja.JumpToUnity;
                        whenSub.Invoke(msg);
                        yield return Ninja.JumpBack;
                    }
                    else if (msg_type == "resub" && whenResub != null)
                    {
                        yield return Ninja.JumpToUnity;
                        whenResub.Invoke(msg);
                        yield return Ninja.JumpBack;
                    }
                    else if (msg_type == "raid" && whenRaid != null)
                    {
                        yield return Ninja.JumpToUnity;
                        whenRaid.Invoke(msg);
                        yield return Ninja.JumpBack;
                    }
                    else if (msg_type == "subgift" && whenSubfift != null)
                    {
                        yield return Ninja.JumpToUnity;
                        whenSubfift.Invoke(msg);
                        yield return Ninja.JumpBack;
                    }
                }
            }
            else
            {
                yield return Ninja.JumpToUnity;
                whenNewSystemMessage.Invoke(inputLine);
                yield return Ninja.JumpBack;
            }
        }

        IEnumerator Process(Message msg)
        {
            var args = msg.message.Split(' ');

            if (commands.ContainsKey(args[0]))
            {
                var command = commands[args[0]];

                if (command.typeCommand == TypeCommand.Action)
                {
                    yield return Ninja.JumpToUnity;
                    command.Invoke(args, msg);
                    yield return Ninja.JumpToUnity;
                }
                else if (command.typeCommand == TypeCommand.Text)
                {
                    var text = command.Text.Replace("{username}", msg.username);
                    SendMessageToChat(text);
                }
            }
        }
    }
}

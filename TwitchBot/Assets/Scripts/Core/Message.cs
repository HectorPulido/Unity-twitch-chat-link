using System.Collections.Generic;
using System;
using UnityEngine;

namespace TwitchBot
{
    public struct Message
    {
        public string username;
        public string message;
        public string rawMessage;
        public bool isMod;
        public bool isSubscriber;
        public string bits;

        public Dictionary<string, string> message_data;

        public static Message Format(string inputLine, string splitString)
        {

            string[] splitInput = inputLine.Split(new string[] { ";" }, StringSplitOptions.None);
            Dictionary<string, string> message = new();
            string line = "";

            for (int i = 0; i < splitInput.Length; i++)
            {
                string[] splitZone = splitInput[i].Split('=');
                message.Add(splitZone[0], splitZone[1]);

                if(splitInput[i].Contains(splitString))
                {
                    line = splitZone[1];
                }
            }

            if (line == ""){
                line = message["user-type"];
            }
            Debug.Log(line);
            line = line.Split(new string[] { splitString }, StringSplitOptions.None)[1];

            if (line.Contains(":"))
            {
                line = line.Split(new string[] { ":" }, StringSplitOptions.None)[1];
            }

            return new Message
            {
                message = line,
                rawMessage = inputLine,
                message_data = message,
                isMod = message.ContainsKey("mod") && message["mod"] == "1",
                isSubscriber = message.ContainsKey("subscriber") && message["subscriber"] == "1",
                username = message.ContainsKey("display-name") ? message["display-name"].ToLower() : "",
                bits = message.ContainsKey("bits") ? message["bits"] : ""
            };
        }
    }
}
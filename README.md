# Unity Twitch Chat 
This is a handmade library to connect Unity with Twich Chat Api, easy to use and easy to understand

![Test twitch chat](/Images/Test.gif)<br/>

## WHY (MOTIVATION)
[![Banner](http://img.youtube.com/vi/wMvDKGSOsgA/0.jpg)](https://www.youtube.com/watch?v=wMvDKGSOsgA)<br/>
This tutorial was made for <b>Hector Pulido</b> for his youtube channel <br/>
https://www.youtube.com/c/HectorAndresPulidoPalmar <br/>
And his Twitch Channel<br/>
https://goo.gl/otWsda (Hector_Pulido_)<br/>

## Features
* Commands support
* Custom twitch events support
* Sub, resub, bits, raid, etc
* On init event, Disconnect event, Etc 
* Current chatter list

## TODO
* More Examples of use

## How to use
1. First of anything make sure you have a twitch acount 
2. You will need a OAUTH Key from Twitch https://twitchapps.com/tmi/
3. Just Code what do you need just like this
4. List of tags https://dev.twitch.tv/docs/irc/tags/

### Rotating cube
This example switch a cube rotation depending of the Twitch chat and the commands !stop and !continue
```C#
    using TwitchBot;
    using CielaSpike; //You have to import this two namespaces
    public class ConnectTest : UnityBot //You have to inherit from UnityBot
    {
        public Transform cube;
        public bool rotating;
        public int maxRetry;

        void Start()
        {

            commands = new Dictionary<string, BotCommand>();

            //Command bots has 2 Arguments, the first one is an array of arguments
            //The other one it's the message it self
            commands.Add("!stop", new BotCommand((string[] args, Message message) =>
            {
                Debug.Log($"{message.username} ha ejecutado el comando !stop");
                rotating = false;
            }));

            commands.Add("!continue", new BotCommand((string[] args, Message message) =>
            {
                Debug.Log($"{message.username} ha ejecutado el comando !continue");
                if (args.Length > 1)
                {
                    Debug.Log($"Y el primer comando es {args[1]}");
                }
                rotating = true;
            }));

            whenNewMessage += (Message message) =>
            {
                Debug.Log($"{message.username}: {message.message}");
            };

            whenNewSystemMessage += (string message) =>
            {
                Debug.Log($"System: {message}");
            };

            whenDisconnect += () =>
            {
                Debug.Log("Desconexion");
            };

            whenStart += () =>
            {
                Debug.Log("Conexion");
            };

            whenNewChater += (string username) =>
            {
                Debug.Log($"{username}, bienvenido al stream!");
                SendMessageToChat($"{username}, bienvenido al stream!");
            };

            whenSub += (Message message) =>
            {
                Debug.Log($"{message.username} Muchas gracias por esa subscripcion");
                SendMessageToChat($"{message.username} Muchas gracias por esa subscripcion");
            };

            // You have to initializate the bot with this command
            this.StartCoroutineAsync(StartConnection(maxRetry));
        }

        private void Update()
        {
            // Game logic
            if(rotating)
                cube.localEulerAngles += new Vector3(0, 0, 360) * Time.deltaTime;
        }
    }
```

## License
* This project uses [Thread Ninja](https://assetstore.unity.com/packages/tools/thread-ninja-multithread-coroutine-15717)
* This project uses [Twitch Api](https://dev.twitch.tv/docs/irc/#step-1-setup)
* Everything else is 100% handcrafted for me and MIT license


<div align="center">
<h3 align="center">Let's connect 😋</h3>
</div>
<p align="center">
<a href="https://www.linkedin.com/in/hector-pulido-17547369/" target="blank">
<img align="center" width="30px" alt="Hector's LinkedIn" src="https://www.vectorlogo.zone/logos/linkedin/linkedin-icon.svg"/></a> &nbsp; &nbsp;
<a href="https://twitter.com/Hector_Pulido_" target="blank">
<img align="center" width="30px" alt="Hector's Twitter" src="https://www.vectorlogo.zone/logos/twitter/twitter-official.svg"/></a> &nbsp; &nbsp;
<a href="https://www.twitch.tv/hector_pulido_" target="blank">
<img align="center" width="30px" alt="Hector's Twitch" src="https://www.vectorlogo.zone/logos/twitch/twitch-icon.svg"/></a> &nbsp; &nbsp;
<a href="https://www.youtube.com/channel/UCS_iMeH0P0nsIDPvBaJckOw" target="blank">
<img align="center" width="30px" alt="Hector's Youtube" src="https://www.vectorlogo.zone/logos/youtube/youtube-icon.svg"/></a> &nbsp; &nbsp;

</p>

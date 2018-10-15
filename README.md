# Unity Twitch Chat 
This is a handmade library to connect Unity with Twich Chat Api, easy to use and easy to understand

![Test twitch chat](/Images/Test.gif)<br/>

## WHY (MOTIVATION)
[![Banner](http://img.youtube.com/vi/wMvDKGSOsgA/0.jpg)](https://www.youtube.com/watch?v=wMvDKGSOsgA)<br/>
This tutorial was made for <b>Hector Pulido</b> for his youtube channel <br/>
https://www.youtube.com/c/HectorAndresPulidoPalmar <br/>
And his Twitch Channel<br/>
https://goo.gl/otWsda (Hector_Pulido_)<br/>

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
            commands.Add("!stop", new BotCommand((a,b) => { rotating = false; })); 
            //Command bots has 2 Arguments, the first one is an array of arguments, its basically a list of words after the command
            //The other one it's all the information tags, you can see all tag list above in #how to use
            //A usefull tag is "display-name", use it like this var username = b["display-name"].ToLower();
            commands.Add("!continue", new BotCommand((a, b) => { rotating = true; }));

            whenNewMessage += (username, message) => Debug.Log($"{username}: {message}");
            whenNewSystemMessage += (message) => Debug.Log($"System: {message}");
            whenDisconnect += () => Debug.Log("Desconexion");
            whenStart += () => Debug.Log("Conexion");
            whenNewChater += (username) => SendMessageToChat($"{username}, bienvenido al stream!");

            this.StartCoroutineAsync(StartConnection(maxRetry));
        }

        private void Update()
        {
            if(rotating)
                cube.localEulerAngles += new Vector3(0, 0, 360) * Time.deltaTime;
        }
    }
```

## License
* This project uses [Thread Ninja](https://assetstore.unity.com/packages/tools/thread-ninja-multithread-coroutine-15717)
* This project uses [Twitch Api](https://dev.twitch.tv/docs/irc/#step-1-setup)
* Everything else is 100% handcrafted for me and MIT license

## Support this on patreon
This project is free to use so please consider Support on Patreon<br/>
![Please consider support on patreon](/Images/Patreon.png)<br/>
https://www.patreon.com/HectorPulido

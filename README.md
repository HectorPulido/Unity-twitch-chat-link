# Unity Twitch Chat 
This is a handmade library to connect Unity with Twich Chat Api, easy to use and easy to understand

![Test twitch chat](/Images/Test.gif)<br/>

## TODO
* More Examples of use

## How to use
1. First of anything make sure you have a twitch acount 
2. You will need a OAUTH Key from Twitch https://twitchapps.com/tmi/
3. Just Code what do you need just like this

### Rotating cube
This example switch a cube rotation depending of the Twitch chat and the commands !stop and !continue
```C#
    public string channel;
    public string oauth;
    public string username;

    public Transform cube;
    public bool rotating;

    Bot bot;

    private void Update()
    {
        if(rotating)
            cube.localEulerAngles += new Vector3(0, 0, 360) * Time.deltaTime;
    }

    private void Start()
    {
        this.StartCoroutineAsync(StartBot());
    }

    IEnumerator StartBot()
    {
        yield return Ninja.JumpBack;

        Dictionary<string, BotCommand> commands = new Dictionary<string, BotCommand>();
        commands.Add("!stop", new BotCommand((a,b) => { rotating = false; }));
        commands.Add("!continue", new BotCommand((a, b) => { rotating = true; }));

        bot = new Bot(username,
                       oauth,
                       channel,
                       commands);

        bot.whenNewMessage += (username, message) => Debug.Log($"{username}: {message}");
        bot.whenNewSystemMessage += (message) => Debug.Log($"System: {message}");
        bot.whenDisconnect += () => Debug.Log("Desconexion");
        bot.whenStart += () => Debug.Log("Conexion");
        bot.whenNewChater += (username) => bot.SendMessage($"{username}, bienvenido al stream!");

        bot.StartBot(99);
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

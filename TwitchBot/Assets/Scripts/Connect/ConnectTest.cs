using CielaSpike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchBot;
using System.Threading;

public class ConnectTest : MonoBehaviour {

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





}

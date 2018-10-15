using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchBot;
using CielaSpike;

public class ConnectTest : UnityBot {
    public Transform cube;
    public bool rotating;
    public int maxRetry;

    void Start()
    {
        commands = new Dictionary<string, BotCommand>();
        commands.Add("!stop", new BotCommand((a,b) => { rotating = false; }));
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

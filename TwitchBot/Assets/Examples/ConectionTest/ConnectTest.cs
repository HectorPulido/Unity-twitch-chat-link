using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchBot;
using CielaSpike;

public class ConnectTest : UnityBot
{
    public Transform cube;
    public bool rotating;
    public int maxRetry;

    void Start()
    {
        commands = new Dictionary<string, BotCommand>();

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

        this.StartCoroutineAsync(StartConnection(maxRetry));
    }

    private void Update()
    {
        if (rotating)
            cube.localEulerAngles += new Vector3(0, 0, 360) * Time.deltaTime;
    }
}

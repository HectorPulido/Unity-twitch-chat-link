using CielaSpike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchBot;
using System.Threading;

public class ConectionSpawner : UnityBot 
{
    public GameObject spawnPrefab;
    public int maxRetry;

    Dictionary<string, GameObject> usersGameObject = new Dictionary<string, GameObject>(); 

    void Start()
    {
        commands = new Dictionary<string, BotCommand>();
        commands.Add("!up", new BotCommand((a,b) => 
        {
            var username = b["display-name"].ToLower();
            if(usersGameObject.ContainsKey(username))
            {
                usersGameObject[username].transform.position += Vector3.up;
            }
        }));

        whenNewMessage += (username, message) => Debug.Log($"{username}: {message}");
        whenNewSystemMessage += (message) => {};//Debug.Log($"System: {message}");
        whenDisconnect += () => Debug.Log("Desconexion");
        whenStart += () => Debug.Log("Conexion");

        whenNewChater += (username) => 
		{	           	
            var a = Instantiate(spawnPrefab);
            a.name = username;
            usersGameObject.Add(username.ToLower(), a);
		};

        this.StartCoroutineAsync(StartConnection(maxRetry));
    }
}

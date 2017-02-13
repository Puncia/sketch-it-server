
# - Tipo di invio messaggi

momentaneamente socket non sicuri (senza codifica), invio di dati in formato json e ricezione sempre in formato json.
i messaggi non devono avere un limite di lunghezza, puoi decidere di usare un terminatore di messaggio
nel caso sia necessario per capire la fine del messaggio e impedire di avere concatenazione di mesaggi (come ci era capitato).
(in quel caso devi dirmi tu che terminatore hai usato).

## - Formato dei messaggi

il formao json è il seguente

```
{
	command: 'Context/Action',
	parameters:
	{
		ParameterName: { ... },
		ParameterName: { ... },
		ParameterName: { ... },
		...
	}
}
```

sia per l'invio che per la ricezione.

## - Tipi di comandi

Per ora ti faccio fare questi comandi:

### - Login

Request:

```
{
	command: 'Authentication/Login',
	parameters:
	{
		Username: '...',
		Password: '...'
	}
}
```

Response:

```
{
	command: 'Authentication/Login',
	parameters:
	{
		Response: true
	}
}
```

nel caso di fallimento nel login il parametro response deve contenere una stringa con l'errore.

### - Register

Request:

```
{
	command: 'Authentication/Register',
	parameters:
	{
		Username: '...',
		Password: '...',
		Email: '...',
		Language: '...'
	}
}
```

Response:

```
{
	command: 'Authentication/Register',
	parameters:
	{
		Response: true
	}
}
```

nel caso di fallimento nel login il parametro response deve contenere una stringa con l'errore.

### - Create room

Request:

```
{
	command: 'Lobby/CreateRoom',
	parameters:
	{
		RoomName: '...',
		Password: '...', (può essere null)
		Description: '...', (può essere vuoto)
		EndlessMode: false,
		EnableHints: false,
		EnableAway: false,
		MaxPlayers: 3,
		MaxRounds: 7,
		Language: '...'
	}
}
```

Response:

```
{
	command: 'Lobby/CreateRoom',
	parameters:
	{
		Response: true
	}
}
```

nel caso di fallimento nel login il parametro response deve contenere una stringa con l'errore.


### - Lista delle stanze

Il server ad ogni cambiamento della lista delle stanze, al login e ogni tot di tempo (metti 5 secondi),
deve inviare al client la lista delle stanze aggiornata seguendo questo formato:


Response:

```
{
	command: 'Lobby/RoomList',
	parameters:
	{
		Rooms:
		[
			{ 
				name: '...', 
				maxUsers: 5, 
				currentRound: 4, 
				maxRounds: 7, 
				language: '...', 
				users: [ { name: '...', drawing: false, score: 12 }, ... ], 
				description: '...', 
				cration: '15/03/2017 16:32' 
			},

			...
		]
	}
}
```

### - Lobby chat

La chat funziona in modo che il server non invia conferma dei messaggi inviati,
ma ad ogni messaggio inviato da qualsiasi utente, il server invia il nuovo mesaggio al client.

#### - Invio dei messaggi:

Request:

```
{
	command: 'Lobby/SendMessage',
	parameters:
	{
		Content: '...'
	}
}
```

No response.


#### - Ricezione dei messaggi:

No request.

Response:

```
{
	command: 'Lobby/ReceiveMesage',
	parameters:
	{
		Content: '...',
		User: '...',
		System: false
	}
}
```

Se il parametro system è true User contiene una stringa vuota;
Il parametro system è dedicato all'invio dei messaggi di sistema, che verranno
mostrati con un altro stile.
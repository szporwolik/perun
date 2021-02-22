#ifndef PERUN_CONNECTION_H
#define PERUN_CONNECTION_H

#include "winsock.h"

#include <string>
#include <queue>
#include <mutex>
#include <fstream>

#include "library.h"

enum enumConnectionState {
	DISCONNECTED,
	CONNECTED,
	FAILED,
};

class socketConnection {
public:
	socketConnection();
	~socketConnection();

	void socketDisconnect();
	void socketCreateConnection(std::string* host, int* port);
	void socketSendData(std::string* payload);
	
	int getFlagReconnected();
	int getFlagConnected();

private:
	SOCKET tcpSocket;
	std::string* tcpHost;
	int tcpPort;

	int flagReconnected = 0;
	volatile enumConnectionState flagConnectionState = DISCONNECTED;

	std::queue<std::string*> dataBuffer;
	std::deque<std::string*> sendQueue;

	std::mutex mutexLock;

	void socketReconnect();
	void socketConnect();
};


#endif
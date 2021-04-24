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
};

class SocketWrapper {
public:
    SocketWrapper();
	~SocketWrapper();

	void disconnect();
	void createConnection(std::string* host, const int* port);
	void enqueueForSending(std::string* payload);
	
	int getAndResetReconnected();
	int getFlagConnected();

private:
	SOCKET tcpSocket;
	std::string* tcpHost;
	int tcpPort;

	int flagReconnected = 0;
	volatile enumConnectionState connectionState = DISCONNECTED;

	std::queue<std::string*> dataBuffer;
	std::deque<std::string*> sendQueue;

	std::mutex mutexLock;

	void reconnect();
	void tcpConnect();
};


#endif
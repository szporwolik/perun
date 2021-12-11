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
    SocketWrapper(std::string logPath,
                  std::string host,
                  const int port);

    ~SocketWrapper();

	void disconnect();
	void createConnection();
    void startNewRecording();
	void enqueueForSending(std::string* payload);
	
	int getAndResetReconnected();
	int getFlagConnected();

private:
	SOCKET tcpSocket;
    const std::string path;
	const std::string tcpHost = "localhost";
	const int tcpPort = 0;

	int flagReconnected = 0;
	volatile enumConnectionState connectionState = DISCONNECTED;
    std::atomic<long long> sentCounter;
    std::atomic<boolean> shouldRun = true;

	std::deque<std::string*> dataBuffer;
	std::deque<std::string*> sendQueue;

	std::mutex mutexLock;
    std::ofstream * outputFile = nullptr;

	void reconnect();
	void tcpConnect();
};


#endif
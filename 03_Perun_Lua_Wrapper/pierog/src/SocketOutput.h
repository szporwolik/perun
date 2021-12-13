#ifndef PARENT_SOCKETOUTPUT_H
#define PARENT_SOCKETOUTPUT_H

#include "winsock.h"
#include <string>
#include <atomic>

enum enumConnectionState {
    DISCONNECTED,
    CONNECTED,
    NEVER_CONNECTED
};

class SocketOutput {
public:
    SocketOutput(std::string host,
                 int port);
    virtual ~SocketOutput();

    int write(std::string* payload);

    bool isConnected();
private:
    SOCKET tcpSocket = INVALID_SOCKET;
    SOCKADDR_IN* address = nullptr;

    const std::string tcpHost = "localhost";
    const int tcpPort = 0;

    volatile enumConnectionState connectionState = NEVER_CONNECTED;

    bool _connect();
//    void reconnect();
    void disconnect();
};


#endif //PARENT_SOCKETOUTPUT_H

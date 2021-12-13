#include "SocketOutput.h"

SocketOutput::SocketOutput(std::string host,
                           const int port): tcpHost(std::move(host)), tcpPort(port) {

    address = new SOCKADDR_IN;
    address->sin_family = AF_INET;
    address->sin_port = htons(u_short(this->tcpPort));
    address->sin_addr.s_addr = *((unsigned long*)gethostbyname(this->tcpHost.c_str())->h_addr);
}

SocketOutput::~SocketOutput() = default;

int SocketOutput::write(std::string *payload) {

    if(!(isConnected() || _connect())) {
        return 0;
    }

    int bytesSent = send(tcpSocket, payload->c_str(), payload->length(), 0);
    if(bytesSent > 0) {
        return bytesSent;
    } else {
        switch (WSAGetLastError()) {
            case WSAECONNRESET:     // Connection reset
            case WSAECONNABORTED:   // Connection aborted
            case WSAESHUTDOWN:      // Connection closed
                disconnect();
        }
        return 0;
    }
}

bool SocketOutput::isConnected() {
    return connectionState == CONNECTED;
}

bool SocketOutput::_connect() {
    if(tcpSocket == INVALID_SOCKET) {
        tcpSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    }
    if(connectionState != CONNECTED) {
        if (connect(tcpSocket, (sockaddr *) address, sizeof(SOCKADDR_IN)) == 0) {
            connectionState = CONNECTED;
            return true;
        }
    }

    return false;
}

void SocketOutput::disconnect() {
    closesocket(tcpSocket);
    tcpSocket = INVALID_SOCKET;
    connectionState = DISCONNECTED;
}
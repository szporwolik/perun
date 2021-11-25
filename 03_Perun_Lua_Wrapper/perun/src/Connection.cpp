#include "Connection.h"

SocketWrapper::SocketWrapper(std::string name): outputFile(name, std::ofstream::app) {
	tcpSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
}

SocketWrapper::~SocketWrapper() {
}

int SocketWrapper::getAndResetReconnected() {
    int result = this->flagReconnected;

    this->flagReconnected = 0; 

    return result;
}

int SocketWrapper::getFlagConnected() {
    return this->connectionState;
}

void SocketWrapper::tcpConnect() {
    // Create socket address object from TCP port and host
    SOCKADDR_IN socketAddress;
    socketAddress.sin_family = AF_INET;
    socketAddress.sin_port = htons(u_short(this->tcpPort));
    socketAddress.sin_addr.s_addr = *((unsigned long*)gethostbyname(this->tcpHost->c_str())->h_addr);

    if (connect(tcpSocket, (sockaddr*)&socketAddress, sizeof(SOCKADDR_IN)) == 0) {
        this->connectionState = CONNECTED;
        this->flagReconnected = 1;
    }
}

void SocketWrapper::createConnection(std::string* host, const int* port) {
    // TCP connection - ConnectTo
	this->tcpHost = host;  
	this->tcpPort = *port;

    tcpConnect();

    // Create new thread
    std::thread thread_object([this]() {
        // TCP sending loop
        bool nothingToSend = false;
        while (true) {
            if (connectionState == CONNECTED && mutexLock.try_lock()) {
                if (sendQueue.empty()) {
                    nothingToSend = true;
                } else {
                    // Payload in queue
                    auto payload = sendQueue.front();

                    outputFile.write(payload->c_str(), payload->length());
                    outputFile.flush();
                    int bytesSent = send(tcpSocket, payload->c_str(), payload->length(), 0);

                    if (bytesSent == payload->length()) {
                        // All payload was sent
                        sendQueue.pop_front();
                        delete payload;
                    } else {
                        // Remaining paylad
                        if (bytesSent > 0) {
                            // Send remaining bytes
                            auto shortened = payload->substr(bytesSent, payload->length() - bytesSent);
                            sendQueue.pop_front();
                            sendQueue.push_front(&shortened);
                            delete payload;
                        } else {
                            // Payload was not sent - handle error
                            switch (WSAGetLastError()) {
                                // Connection was reset
                                case WSAECONNRESET:
                                // Connection aborted
                                case WSAECONNABORTED:
                                // Connection was closed
                                case WSAESHUTDOWN:
                                    connectionState = DISCONNECTED;
                                    reconnect();
                            }
                        }
                    }
                }
                mutexLock.unlock();
            } else {
                // Not connected
                reconnect();
            }
            // sleep longer if nothing to send
            if (nothingToSend) { Sleep(100); } else { Sleep(10); }
        }
        });
    thread_object.detach(); // Detach TCP thread from main thread
}

void SocketWrapper::reconnect() {
    if (connectionState == DISCONNECTED) {
        disconnect();
        tcpSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP); // Reset socket
    }

    tcpConnect();
}

void SocketWrapper::disconnect() {
    // TCP connection - Disconnect
	closesocket(tcpSocket);
    connectionState = DISCONNECTED;
}

void SocketWrapper::enqueueForSending(std::string* payload) {
    if (mutexLock.try_lock()) {
        while (!dataBuffer.empty()) {
            // Shift buffer to queue
            sendQueue.push_back(dataBuffer.front());
            dataBuffer.pop();
        }
        sendQueue.push_back(payload);  
        mutexLock.unlock(); 
    }
    else {
        dataBuffer.push(payload);
    }
}
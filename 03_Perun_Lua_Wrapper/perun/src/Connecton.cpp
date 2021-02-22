#include "Connection.h"

socketConnection::socketConnection() {
    // Constructor
	tcpSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
}

socketConnection::~socketConnection() {
    // Destrcutor - do nothing
}

int socketConnection::getFlagReconnected() {
    // Return the reconnection flag and reset
    int flagReconnected = this->flagReconnected;

    this->flagReconnected = 0;  // Reset the flag

    return flagReconnected;
}

int socketConnection::getFlagConnected() {
    // Return the connection flag
    int flagConnected = this->flagConnectionState;

    return flagConnected;
}

void socketConnection::socketConnect() {
    // Create socket adress object  from TCP port and host
    SOCKADDR_IN socketAddress;
    socketAddress.sin_family = AF_INET;
    socketAddress.sin_port = htons(u_short(this->tcpPort));
    socketAddress.sin_addr.s_addr = *((unsigned long*)gethostbyname(this->tcpHost->c_str())->h_addr);

    // Connect to socket
    if (connect(tcpSocket, (sockaddr*)&socketAddress, sizeof(SOCKADDR_IN)) == 0) {
        this->flagConnectionState = CONNECTED;
        this->flagReconnected = 1;
    }
}

void socketConnection::socketCreateConnection(std::string* host, int* port) {
    // TCP connection - ConnectTo
	this->tcpHost = host;   // Set target host
	this->tcpPort = *port;  // Set target port

    // Connect to socket
    socketConnect();

    // Create new thread
    std::thread thread_object([this]() {
        // TCP sending loop
        bool isQueueEmpty = false;  // Helper

        while (true) {
            // Endless loop (will run after main dll thread is active)
            if (flagConnectionState == CONNECTED && mutexLock.try_lock()) {
                if (sendQueue.empty()) {
                    // Empty queue
                    isQueueEmpty = true;
                } else {
                    // Payload in queue
                    auto payload = sendQueue.front();
                    int length = payload->length();
                    int bytesSent = send(tcpSocket, payload->c_str(), payload->length(), 0);

                    if (bytesSent == payload->length()) {
                        // All payload was sent
                        sendQueue.pop_front();
                        delete payload;
                    } else {
                        // Remaining paylad
                        if (bytesSent > 0) {
                            // Send remaining bytes
                            auto shortened = payload->substr(bytesSent, length - bytesSent);
                            sendQueue.pop_front();
                            sendQueue.push_front(&shortened);
                            delete payload;
                        } else {
                            // Payload was not sent - handle error
                            switch (WSAGetLastError()) {
                                case WSAECONNRESET:
                                    // Connection was reset
                                    flagConnectionState = DISCONNECTED; // Set flag
                                    socketReconnect();    // Try to reconnect
                                    break;
                                case WSAECONNABORTED:
                                    // Connection aborted
                                    flagConnectionState = DISCONNECTED; // Set flag
                                    socketReconnect();    // Try to reconnect
                                    break;
                                case WSAESHUTDOWN:
                                    // Connection was closed
                                    flagConnectionState = DISCONNECTED; // Set flag
                                    socketReconnect();    // Try to reconnect
                                    break;
                            }
                        }
                    }
                }
                mutexLock.unlock();
            } else {
                // Not connected - try to reconnect
                socketReconnect();
            }
            // Add delay
            if (isQueueEmpty) { Sleep(100); } else { Sleep(10); }   // Delay depending if there is something in Queue or not
        }
        });
    thread_object.detach(); // Detach TCP thread from main thread
}

void socketConnection::socketReconnect() {
    // TCP connection - Reconnect
    if (flagConnectionState == DISCONNECTED) {
        socketDisconnect();   // Disconnect
        tcpSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP); // Reset socket
    }

    // Connect to socket
    socketConnect();
}

void socketConnection::socketDisconnect() {
    // TCP connection - Disconnect
	closesocket(tcpSocket); // Close socket
    flagConnectionState = DISCONNECTED; // Set flag
}

void socketConnection::socketSendData(std::string* payload) {
    // Send data
    if (mutexLock.try_lock()) {
        // Add to queue (set mutex lock)
        while (!dataBuffer.empty()) {
            // Shift buffer to queue
            sendQueue.push_back(dataBuffer.front());
            dataBuffer.pop();
        }
        sendQueue.push_back(payload);   // Shift queue
        mutexLock.unlock(); // Unlock sending thread
    }
    else {
        // Add to buffer - queue is busy
        dataBuffer.push(payload);
    }
}
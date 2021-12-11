#include "Connection.h"

#include <utility>
#include <filesystem>

SocketWrapper::SocketWrapper(std::string logPath,
                             std::string host,
                             const int port): path(std::move(logPath)), tcpHost(std::move(host)), tcpPort(port){
	this->tcpSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

    startNewRecording();
}

SocketWrapper::~SocketWrapper() {
}

void SocketWrapper::tcpConnect() {
    // Create socket address object from TCP port and host
    SOCKADDR_IN socketAddress;
    socketAddress.sin_family = AF_INET;
    socketAddress.sin_port = htons(u_short(this->tcpPort));
    socketAddress.sin_addr.s_addr = *((unsigned long*)gethostbyname(this->tcpHost.c_str())->h_addr);

    if (connect(tcpSocket, (sockaddr*)&socketAddress, sizeof(SOCKADDR_IN)) == 0) {
        this->connectionState = CONNECTED;
        this->flagReconnected = 1;
    }
}

void SocketWrapper::startNewRecording() {
    if(outputFile != nullptr && outputFile->is_open() && outputFile->good()) {
        outputFile->flush();
        outputFile->close();
        delete outputFile;
    }

    auto const now = std::chrono::system_clock::now();
    auto const gmt = std::chrono::locate_zone("Etc/GMT");
    auto const filename = std::format("pierog.{:%FT_%H%M%S}.log", std::chrono::zoned_time{gmt, floor<std::chrono::milliseconds>(now)});

    std::filesystem::path dir(this->path);
    std::filesystem::path file(filename);
    auto fullPath = (dir / file).string();

    outputFile = new std::ofstream(fullPath, std::ofstream::app | std::ios::out);

    // clean the network buffer if nothing was ever sent
    if(sentCounter > 0) {
        mutexLock.lock();
        dataBuffer.clear();
        sendQueue.clear();
        mutexLock.unlock();
    }
}

void SocketWrapper::createConnection() {
    // TCP connection - ConnectTo

    tcpConnect();

    std::thread thread_object([this]() {

        bool nothingToSend = false;
        while (shouldRun) {
            if (connectionState == CONNECTED && mutexLock.try_lock()) {
                if (sendQueue.empty()) {
                    nothingToSend = true;
                } else {
                    // Payload in queue
                    auto payload = sendQueue.front();
                    int bytesSent = send(tcpSocket, payload->c_str(), payload->length(), 0);

                    if (bytesSent == payload->length()) {
                        // All payload was sent
                        sendQueue.pop_front();
                        delete payload;
                        sentCounter += bytesSent;
                    } else {
                        // Remaining paylad
                        if (bytesSent > 0) {
                            // Send remaining bytes
                            auto shortened = payload->substr(bytesSent, payload->length() - bytesSent);
                            sendQueue.pop_front();
                            sendQueue.push_front(&shortened);
                            delete payload;
                            sentCounter += bytesSent;
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
    if(outputFile == nullptr) {
        startNewRecording();
    }

    if(outputFile != nullptr) {
        outputFile->write(payload->c_str(), payload->length());
        if(payload->length() > 50) {
            outputFile->flush();
        }
    }
    if (mutexLock.try_lock()) {
        while (!dataBuffer.empty()) {
            // Shift buffer to queue
            sendQueue.push_back(dataBuffer.front());
            dataBuffer.pop_front();
        }
        sendQueue.push_back(payload);  
        mutexLock.unlock(); 
    }
    else {
        dataBuffer.push_back(payload);
    }
}

int SocketWrapper::getAndResetReconnected() {
    int result = this->flagReconnected;

    this->flagReconnected = 0;

    return result;
}

int SocketWrapper::getFlagConnected() {
    return this->connectionState;
}
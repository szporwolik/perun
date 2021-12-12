#ifndef PARENT_DATADISTRIBUTOR_H
#define PARENT_DATADISTRIBUTOR_H

#include <string>
#include <queue>
#include "SocketOutput.h"
#include "RotatingFileOutput.h"

class DataDistributor {
public:
    DataDistributor(std::string logPath,
                    std::string host,
                    int port);

    virtual ~DataDistributor();

    void enqueueForSending(std::string* payload);
    void markNewRecording();

    void start();
    void stop();

    int isConnected();

private:
    std::atomic<boolean> shouldRun = true;
    std::atomic<boolean> running = false;
    std::atomic<boolean> everSentViaSocket = false;
    std::deque<std::string*> dataBuffer;
    std::deque<std::string*> sendQueue;
    std::mutex lock;

    std::chrono::time_point<std::chrono::system_clock> lastSent = std::chrono::system_clock::now();

    SocketOutput* socketOutput;
    RotatingFileOutput* fileOutput;
};


#endif //PARENT_DATADISTRIBUTOR_H

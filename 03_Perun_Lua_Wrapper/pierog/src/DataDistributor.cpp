#include "DataDistributor.h"

#include <utility>

DataDistributor::DataDistributor(std::string logPath,
                                 std::string host,
                                 int port) {

    fileOutput = new RotatingFileOutput(std::move(logPath));
    socketOutput = new SocketOutput(std::move(host), port);
}

DataDistributor::~DataDistributor() = default;

void DataDistributor::start() {
    if(running) {
        return;
    }

    std::thread thread_object([this]() {
        bool anythingToSend = true;
        long KEEP_ALIVE = 1000;
        auto now = std::chrono::system_clock::now();

        while(shouldRun) {
            if(lock.try_lock()) {
                if(sendQueue.empty()) {
                    auto delay = std::chrono::duration_cast<std::chrono::milliseconds>(now - lastSent);
                    if(delay.count() > KEEP_ALIVE) {
                        sendQueue.push_front(new std::string(" "));
                    } else {
                        anythingToSend = false;
                    }
                } else {
                    auto payload = sendQueue.front();
                    int sent = socketOutput->write(payload);

                    if(sent > 0) {
                        everSentViaSocket = true;
                        sendQueue.pop_front();
                        if(sent == payload->length()) {
                            delete payload;
                        } else {
                            auto shortened = payload->substr(sent, payload->length() - sent);
                            sendQueue.push_front(&shortened);
                            delete payload;
                        }
                        lastSent = std::chrono::system_clock::now();
                    } else {
                        // failed to send
                    }
                }

                lock.unlock();
            }

            if(anythingToSend) {
                Sleep(10);
            } else {
                Sleep(100);
            }
        }
    });
    thread_object.detach();

    shouldRun = true;
    running = true;
}

void DataDistributor::stop() {
    shouldRun = false;
}

void DataDistributor::enqueueForSending(std::string *payload) {
    fileOutput->write(payload);
    if (lock.try_lock()) {
        while (!dataBuffer.empty()) {
            // Shift buffer to queue
            sendQueue.push_back(dataBuffer.front());
            dataBuffer.pop_front();
        }
        sendQueue.push_back(payload);
        lock.unlock();
    }
    else {
        dataBuffer.push_back(payload);
    }
}

void DataDistributor::markNewRecording() {
    fileOutput->markNewRecording();
    if(!everSentViaSocket) {
        lock.lock();
        sendQueue.clear();
        dataBuffer.clear();
        lock.unlock();
    }
}

int DataDistributor::isConnected() {
    return socketOutput->isConnected();
}
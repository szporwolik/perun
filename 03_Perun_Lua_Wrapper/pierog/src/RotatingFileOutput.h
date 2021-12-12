#ifndef PARENT_ROTATINGFILEOUTPUT_H
#define PARENT_ROTATINGFILEOUTPUT_H

#include <mutex>
#include <fstream>
#include <iostream>
#include <filesystem>

class RotatingFileOutput {
public:
    RotatingFileOutput(std::string outputPath);
    virtual ~RotatingFileOutput();

    void markNewRecording();
    void write(std::string* payload);

private:
    const std::string path;

    std::atomic<long long> bytesWritten;
    std::ofstream * outputFile = nullptr;

    std::string generateFileName();
};


#endif //PARENT_ROTATINGFILEOUTPUT_H

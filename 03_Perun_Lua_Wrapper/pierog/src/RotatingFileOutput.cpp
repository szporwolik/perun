#include "RotatingFileOutput.h"

RotatingFileOutput::~RotatingFileOutput() = default;

RotatingFileOutput::RotatingFileOutput(std::string outputPath): path(std::move(outputPath)) {

}

void RotatingFileOutput::markNewRecording() {
    if(outputFile != nullptr && outputFile->good()) {
        outputFile->flush();
        outputFile->close();
        delete outputFile;
        outputFile = nullptr;
    }
}

void RotatingFileOutput::write(std::string *payload) {
    if (outputFile == nullptr) {
        std::string fileName = generateFileName();
        outputFile = new std::ofstream(fileName, std::ofstream::app | std::ios::out);
    }

    outputFile->write(payload->c_str(), payload->length());
    bytesWritten += (long) payload->length();
    if (payload->length() > 50) {
        outputFile->flush();
    }
}

std::string RotatingFileOutput::generateFileName() {
    auto const now = std::chrono::system_clock::now();
    auto const gmt = std::chrono::locate_zone("Etc/GMT");
    auto const filename = std::format("pierog.{:%FT_%H%M%S}.log",
                                      std::chrono::zoned_time{gmt, floor<std::chrono::milliseconds>(now)});

    std::filesystem::path dir(this->path);
    std::filesystem::path file(filename);
    return (dir / file).string();
}


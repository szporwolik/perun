#include "library.h"
#include <format>
#include <filesystem>

static SocketWrapper * tcpConnection = nullptr;

static std::string *startingDelimiter = nullptr;
static std::string *endingDelimiter = nullptr;
static std::string lastObservedHash = std::string("");

/* this method exists for comments */
static int valuesToReturn(int input) {
    return input;
}

static int appStartHook(lua_State* luaState) {
    // Starting the app - prepare
    if(tcpConnection == nullptr) {

        int i = 1;
        const std::string path = std::string(lua_tolstring(luaState, i++, 0));
        const std::string host = std::string(lua_tolstring(luaState, i++, 0));
        const int port = (int) lua_tointeger(luaState, i++);

        tcpConnection = new SocketWrapper(path, host, port);
        tcpConnection->createConnection();
    }

    lua_pushinteger(luaState, 1);       // First return value: confirmation that app was started

    return valuesToReturn(1);
}

static int appEndHook(lua_State* luaState) {
    // Closing the app - clean up

    if(tcpConnection != nullptr) {
        tcpConnection->disconnect();
    }

    return valuesToReturn(0);
}

static int setDelimiters(lua_State* luaState) {
    startingDelimiter = new std::string(lua_tolstring(luaState, 1, 0));
    endingDelimiter = new std::string(lua_tolstring(luaState, 2, 0));

    return valuesToReturn(0);
}

static int markMissionStart(lua_State* luaState) {

    std::string missionHash = std::string(lua_tolstring(luaState, 1, 0));

    if(tcpConnection != nullptr && missionHash ==lastObservedHash) {
        tcpConnection->startNewRecording();
        lastObservedHash = missionHash;
    }

    return valuesToReturn(0);
}

static int tcpSend(lua_State* luaState) {
    // Send frame over TCP socket 

    if(tcpConnection != nullptr) {
        if(startingDelimiter != nullptr) {
            tcpConnection->enqueueForSending(new std::string(*startingDelimiter));
        }
        tcpConnection->enqueueForSending(new std::string(lua_tolstring(luaState, 1, 0)));
        if(endingDelimiter != nullptr) {
            tcpConnection->enqueueForSending(new std::string(*endingDelimiter));
        }

        lua_pushinteger(luaState,
                        tcpConnection->getFlagConnected());     // First return value: information if there is TCP connection
        lua_pushinteger(luaState,
                        tcpConnection->getAndResetReconnected());   // Second return value: information if there was recent reconnection to TCP server
    } else {
        lua_pushinteger(luaState, 0);
        lua_pushinteger(luaState, 0);
    }

    return valuesToReturn(2);
}

extern "C" int __declspec(dllexport) luaopen_pierog(lua_State * L) {
    static const luaL_Reg Map[] = {
            {"StartOfApp", appStartHook},               // Called at the begining of the session
            {"EndOfApp",   appEndHook },                // Called at the end of the session out of LuaExportStop
            {"tcpSend",    tcpSend},          // Called to send data from lua over TCP
            {"delimiters",   setDelimiters },
            {"markMissionStart", markMissionStart },
            { NULL, NULL }
    };

    // Register the list of functions for lua
    luaL_register(L, "pierog", Map);

    return 1;
}
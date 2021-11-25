#include "library.h"

static SocketWrapper * tcpConnection = nullptr;

/* this method exists for comments */
static int valuesToReturn(int input) {
    return input;
}

static int appStartHook(lua_State* luaState) {
    // Starting the app - prepare
    if(tcpConnection == nullptr) {
        auto nowMillis = std::chrono::time_point_cast<std::chrono::milliseconds>(std::chrono::system_clock::now());
        auto millis = nowMillis.time_since_epoch().count();

        char buffer[50];
        sprintf_s(buffer, "c:/temp/perun.%llu.log", millis);

        tcpConnection = new SocketWrapper(std::string(buffer));
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

static int tcpConnect(lua_State* luaState) {
    // Connect to the TCP socket

    if(tcpConnection != nullptr) {
        auto *host = new std::string(lua_tolstring(luaState, 1, 0));
        const int *port = new int(lua_tointeger(luaState, 2));

        tcpConnection->createConnection(host, port);
    }

    return valuesToReturn(0);
}

static int tcpSend(lua_State* luaState) {
    // Send frame over TCP socket 

    if(tcpConnection != nullptr) {
        tcpConnection->enqueueForSending(new std::string(lua_tolstring(luaState, 1, 0)));

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

extern "C" int __declspec(dllexport) luaopen_perun(lua_State * L) {
    static const luaL_Reg Map[] = {
            {"StartOfApp", appStartHook},           // Called at the begining of the session
            {"EndOfApp",   appEndHook },              // Called at the end of the session out of LuaExportStop
            {"tcpSend",    tcpSend},          // Called to send data from lua over TCP
            {"tcpConnect", tcpConnect},         // Create connection
            { NULL, NULL }
    };

    // Register the list of functions for lua
    luaL_register(L, "perun", Map);       

    return 1;
}
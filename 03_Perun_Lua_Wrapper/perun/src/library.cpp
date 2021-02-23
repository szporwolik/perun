#include "library.h"

static SocketWrapper tcpConnection;

static int appStartHook(lua_State* luaState) {
    // Starting the app - prepare

    lua_pushinteger(luaState, 1);       // First return value: confirmation that app was started

    return (1);                         // Set return to number of arguments returned
}

static int appEndHook(lua_State* luaState) {
    // Closing the app - clean up

    tcpConnection.disconnect();

    return (0);                         // No return values
}

static int tcpConnect(lua_State* luaState) {
    // Connect to the TCP socket

    auto *host = new std::string(lua_tolstring(luaState, 1, 0));
    const int *port = new int(lua_tointeger(luaState, 2));
    tcpConnection.createConnection(host, port);

    return (0);                         // No return values
}

static int tcpSend(lua_State* luaState) {
    // Send frame over TCP socket 

    tcpConnection.enqueueForSending(new std::string(lua_tolstring(luaState, 1, 0)));

    lua_pushinteger(luaState, tcpConnection.getFlagConnected());     // First return value: information if there is TCP connection
    lua_pushinteger(luaState, tcpConnection.getAndResetReconnected());   // Secound return value: information if there was recent reconnection to TCP server

    return (2);                         // Set return to number of arguments returned
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
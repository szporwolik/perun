#include "library.h"

static socketConnection tcpConnection;

static int appStart(lua_State* luaState) {
    // Starting the app - prepare

    // No return values
    return (0);
}

static int appEnd(lua_State* luaState) {
    // Closing the app - clean up

    // Close the connection
    tcpConnection.socketDisconnect();

    // No return values
    return (0);
}

static int tcpConnect(lua_State* luaState) {
    // Connect to the TCP socket

    // Connect
    tcpConnection.socketCreateConnection(new std::string(lua_tolstring(luaState, 1, 0)), new int(lua_tointeger(luaState, 2)));

    // No return values
    return (0);
}

static int tcpSend(lua_State* luaState) {
    // Send frame over TCP socket

    // Send over the socket
    tcpConnection.socketSendData(new std::string(lua_tolstring(luaState, 1, 0)));

    // Return values
    lua_pushinteger(luaState, tcpConnection.getFlagConnected());     // First return value: information if there is TCP connection
    lua_pushinteger(luaState, tcpConnection.getFlagReconnected());   // Secound return value: information if there was recent reconnection to TCP server

    // Set return to number of arguments returned
    return (2);
}

extern "C" int __declspec(dllexport) luaopen_main(lua_State * L) {
    static const luaL_Reg Map[] = {
            {"StartOfApp", appStart},         // Called at the begining of the session
            {"EndOfApp", appEnd },            // Called at the end of the session out of LuaExportStop
            {"tcpSendFrame", tcpSend},     // Called to send data from lua over TCP
            {"tcpConnect", tcpConnect},         // Create connection
            { NULL, NULL }
    };

    // Register the list of functions for lua
    luaL_register(L, "perun", Map);       

    return 1;
}
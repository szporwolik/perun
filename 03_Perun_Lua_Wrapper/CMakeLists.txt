# CMakeList.txt : CMake project for perun_dll, include source and define
# project specific logic here.
#
cmake_minimum_required(VERSION 3.17)

project ("parent")

set(CMAKE_CXX_STANDARD 20)
set_property(GLOBAL PROPERTY USE_FOLDERS ON)

if (CMAKE_CXX_COMPILER_VERSION VERSION_GREATER_EQUAL 19.12.25835)
    set(CMAKE_CXX20_STANDARD_COMPILE_OPTION "-std:c++latest")
    set(CMAKE_CXX20_EXTENSION_COMPILE_OPTION "-std:c++latest")
endif()

add_subdirectory(lua-5.1.5)
add_subdirectory(pierog)
add_subdirectory(experimental)
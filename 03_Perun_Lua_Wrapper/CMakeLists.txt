# CMakeList.txt : CMake project for perun_dll, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

project ("perun")

set(CMAKE_CXX_STANDARD 20)
set_property(GLOBAL PROPERTY USE_FOLDERS ON)

add_subdirectory(lua-5.1.5)
add_subdirectory(perun)
cmake_minimum_required(VERSION 3.17)
project(experimental)

set(CMAKE_CXX_STANDARD 20)

set (EXPERIMENTAL_SOURCES
        "src/main.cpp"
        )

add_executable(experimental ${EXPERIMENTAL_SOURCES})

include_directories(${pierog_SOURCE_DIR})

target_link_libraries(
        experimental
        ws2_32
        pierog
)

target_include_directories (pierog PUBLIC pierog)
#set_target_properties(main PROPERTIES OUTPUT_NAME "pierog")
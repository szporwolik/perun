cmake_minimum_required(VERSION 3.17)
project(experimental)

set(CMAKE_CXX_STANDARD 20)

set (EXPERIMENTAL_SOURCES
        "src/main.cpp"
        )

add_executable(experimental ${EXPERIMENTAL_SOURCES})

target_link_libraries(
        lua-5.1.5
        ws2_32
)

target_include_directories (pierog PUBLIC)
#set_target_properties(main PROPERTIES OUTPUT_NAME "perun")
cmake_minimum_required(VERSION 3.16)

# Beginner-friendly project setup
project(SportsClubCLI LANGUAGES CXX)

# Use modern C++
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Main CLI application
add_executable(sports_club_cli
    Project1/main.cpp
    Project1/auth.cpp
    Project1/club.cpp
    Project1/coach.cpp
    Project1/event.cpp
    Project1/member.cpp
    Project1/team.cpp
)

# Make headers under Project1 easy to include
target_include_directories(sports_club_cli PRIVATE Project1)

# Optional strict warnings for GCC/Clang
if (CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
    target_compile_options(sports_club_cli PRIVATE -Wall -Wextra -Wpedantic)
endif()

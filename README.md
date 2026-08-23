# Software Engineering Portfolio

This repository is my consolidated software engineering portfolio. Each project is preserved in the `projects/` directory with its source files and Git history.

## Sport Club Management (CLI)

**Tech:** C++, STL, GoogleTest, CSV I/O, OOP, CMake, Visual Studio

- Designed and built a C++ command-line application to manage athletes, coaches, teams, and events with role-based access control for Athlete, Coach, and Administrator accounts.
- Implemented a template-based CSV persistence layer of approximately 200 lines to automatically load and save entities at application startup and shutdown.
- Wrote GoogleTest unit tests and achieved more than 90% coverage across authentication and core workflows, including edge-case file I/O.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/sports-club-cli)

## Penguin Chess (CLI Game)

**Tech:** C, `malloc`/`free`, File I/O, ANSI escape codes

- Co-defined the game rules and modular architecture across the user interface, core logic, and persistence modules, and produced design documentation.
- Implemented dynamic memory management for board and player structures and ensured there were no memory leaks across repeated sessions.
- Built save and load functionality using file I/O to persist player nicknames and scores, enabling session restoration.
- Implemented ANSI escape-code rendering for colored tiles and pieces to improve the terminal user experience.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/penguin-game)

## Numerical Methods

**Tech:** MATLAB, Fourier series, signal filtering, ODE solving (`ode45`, Gear2, RK4)

- Processed 3D gait data by filtering noisy coordinate signals and reconstructing lower-limb joint angles.
- Compared Fourier-series reconstruction strategies for impulsive inputs and analysed approximation error.
- Extended the Lotka-Volterra model with quadratic density terms; solved the resulting ODEs with `ode45`, Gear2, and RK4 and compared accuracy against runtime.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/numerical-methods)

## SmartCare Solutions

**Tech:** Java, Spring Boot, Thymeleaf, RESTful APIs, JPA, MySQL, MongoDB

- Built a healthcare management application as a backend-focused Coursera course project using Spring Boot.
- Implemented features for managing patients, doctors, appointments, and prescriptions.
- Used JPA with MySQL and MongoDB for data persistence and retrieval.
- Integrated application modules and tested core workflows in a layered backend architecture.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/smartcare-solutions)

## Cinema Manager

**Tech:** C#, ASP.NET MVC, Entity Framework, SQLite, Bootstrap

- Built a cinema ticket management web application with user registration, authentication, and profile management.
- Implemented screening administration, seat selection, and reservation workflows.
- Used Entity Framework migrations with SQLite to model and persist users, cinemas, screenings, and reservations.
- Added server-side validation and role-specific management functionality.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/cinema-manager)

## Mini Cat Gallery

**Tech:** Vue 3, TypeScript, Vite, CSS Grid, TheCatAPI

- Built a responsive Vue image gallery that displays bundled images and retrieves new cat images from an external API.
- Implemented modal image previews, loading feedback, and gallery refresh functionality.
- Structured the frontend with reusable Vue components and a dedicated API service written in TypeScript.
- Used CSS Grid to support responsive layouts across different screen sizes.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/mini-cat-gallery)

## Library Management

**Tech:** Java, OOP

- Built a small Java application to model books and basic library-management operations.
- Applied object-oriented programming concepts by separating book data from library-management logic.
- Used the project to practise Java classes, methods, collections, and program flow.

[View project](https://github.com/daizhongtian/Software-Engineering-Portfolio/tree/main/projects/library-management)



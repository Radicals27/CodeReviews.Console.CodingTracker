# Coding Tracker

The Coding Tracker is a lightweight terminal-based application designed to help developers record and manage their coding sessions. The application stores session data, including the date, start time, and end time, in a SQLite database. With features like session recording, manual entry, and data visualization, the Coding Tracker is perfect for anyone who wants to track their coding productivity over time.

---

## Features

### 1. **Record Coding Sessions in Real-Time**

- Start recording your session with a single command.
- Automatically records the current date and time as the session start time.
- Stop the session to record the end time and calculate the total session duration.

### 2. **Manual Session Entry**

- Enter session details manually, including:
  - **Date:** Specify the date in `dd-MM-yyyy` format.
  - **Start Time:** Provide a valid 4-digit 24-hour format time (e.g., `0630` for 6:30 AM).
  - **End Time:** Provide the end time in the same format.

### 3. **View All Sessions**

- List all recorded sessions, displaying:
  - **ID:** Unique identifier for the session.
  - **Date:** The day the session occurred.
  - **Start Time and End Time:** Times in `HH:mm` format (e.g., `06:30`, `12:00`).
  - **Duration:** Total duration of the session in minutes.

### 4. **Update Existing Sessions**

- Modify any session by specifying its ID.
- Change the date, start time, or end time.

### 5. **Delete Sessions**

- Remove any session by specifying its ID.
- Automatically warns if the session does not exist.

### 6. **Data Storage**

- Uses SQLite for persistent data storage.
- Dapper ORM handles database interactions, ensuring efficient and secure operations.

---

## Usage

### Start the Application

Run the application using your terminal or command prompt:

```bash
dotnet run
```

---

## Database Structure

The application uses a SQLite database with the following table schema:

| Column      | Type    | Description                         |
| ----------- | ------- | ----------------------------------- |
| `Id`        | INTEGER | Unique identifier (Primary Key).    |
| `Date`      | TEXT    | Date of the session (`dd-MM-yyyy`). |
| `StartTime` | INTEGER | Start time (`HHmm`).                |
| `EndTime`   | INTEGER | End time (`HHmm`).                  |

## Built With

- C# and .NET: Core programming language and framework.
- SQLite: Lightweight database for session storage.
- Dapper ORM: Simplifies database operations.

## Future Features

Data Export: Export session data to CSV or JSON.
Analytics: Visualize coding trends with graphs.
Tags: Categorize sessions by project or technology.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Contact

For questions or support, please reach out via email at cclose27@gmail.com.

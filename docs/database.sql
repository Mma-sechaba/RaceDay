/* ===================================================== 
RACE DAY DATABASE 
===================================================== */
---this where the main database is created for the whole project---

USE master; 
GO

/* Create the database if it does not already exist */

IF DB_ID('RaceDayDB') IS NULL

BEGIN 
CREATE DATABASE RaceDayDB; 
END;
GO

USE RaceDayDB;
GO

USE RaceDayDB;
DROP TABLE IF EXISTS RESULT;
DROP TABLE IF EXISTS REGISTRATION;
DROP TABLE IF EXISTS WEATHER;
DROP TABLE IF EXISTS CATEGORY;
DROP TABLE IF EXISTS EVENT;
DROP TABLE IF EXISTS PARTICIPANT;
DROP TABLE IF EXISTS EVENT_ORGANISER;
GO

/* ===================================================== 1.
EVENT ORGANISER TABLE 
===================================================== */
---the event organiser table is now created. and attributes are included---
   

CREATE TABLE EVENT_ORGANISER 
(
OrganiserId INT IDENTITY(1,1) PRIMARY KEY,
FirstName VARCHAR(50) NOT NULL,
LastName VARCHAR(50) NOT NULL,
Email VARCHAR(100) NOT NULL UNIQUE,
Password VARCHAR(255) NOT NULL,
Phone VARCHAR(20),
CreatedAt DATETIME DEFAULT GETDATE()
);
GO


/* ===================================================== 
2. PARTICIPANT TABLE
===================================================== */
---the participant table is now created and the attributes are included---
   

CREATE TABLE PARTICIPANT ( ParticipantId INT IDENTITY(1,1) PRIMARY KEY,
FirstName VARCHAR(50) NOT NULL,
LastName VARCHAR(50) NOT NULL,
Email VARCHAR(100) NOT NULL UNIQUE,
Password VARCHAR(255) NOT NULL,
DateOfBirth DATE NOT NULL,
Phone VARCHAR(20),
CreatedAt DATETIME DEFAULT GETDATE()
);
GO

/* ===================================================== 
3. EVENT TABLE
===================================================== */
---the event table is now created and attributes are included---

   

CREATE TABLE EVENT
(
EventId INT IDENTITY(1,1) PRIMARY KEY,
OrganiserId INT NOT NULL,
EventName VARCHAR(100) NOT NULL,
Description VARCHAR(500),
EventDate DATE NOT NULL,
Location VARCHAR(150) NOT NULL,
Distance DECIMAL(6,2) NOT NULL,
EventType VARCHAR(20) NOT NULL,
Status VARCHAR(20) DEFAULT 'Available',

   FOREIGN KEY (OrganiserId)
          REFERENCES EVENT_ORGANISER(OrganiserId)
          
);
GO


/* ===================================================== 
4. CATEGORY TABLE
===================================================== */
   ---the category table is now added and attributes also included---

CREATE TABLE CATEGORY 
(
CategoryId INT IDENTITY(1,1) PRIMARY KEY,
EventId INT NOT NULL,
CategoryName VARCHAR(100) NOT NULL,
MinimumAge INT NOT NULL,
MaximumAge INT NOT NULL,
Distance DECIMAL(6,2) NOT NULL,

    FOREIGN KEY (EventId) 
            REFERENCES EVENT(EventId)
            
);
GO


/* ===================================================== 
5. WEATHER TABLE
===================================================== */
---the weather table is added and the attributes are also included---

   

CREATE TABLE WEATHER
(
WeatherId INT IDENTITY(1,1) PRIMARY KEY,
EventId INT NOT NULL,
Temperature DECIMAL(5,2),
WeatherCondition VARCHAR(100),
WindSpeed DECIMAL(5,2),
WeatherDate DATE NOT NULL,
      FOREIGN KEY (EventId)  
             REFERENCES EVENT(EventId)
             
);
GO

/* ===================================================== 
6. REGISTRATION TABLE
===================================================== */
   ---the registration table is added and the attributes are also included---


CREATE TABLE REGISTRATION
(
RegistrationId INT IDENTITY(1,1) PRIMARY KEY,
ParticipantId INT NOT NULL,
EventId INT NOT NULL,
CategoryId INT NOT NULL,
RegistrationDate DATETIME DEFAULT GETDATE(),
Status VARCHAR(20) DEFAULT 'Pending',
   
    FOREIGN KEY (ParticipantId) 
        REFERENCES PARTICIPANT(ParticipantId),
        
    FOREIGN KEY (EventId)
        REFERENCES EVENT(EventId),
        
    FOREIGN KEY (CategoryId)
        REFERENCES CATEGORY(CategoryId),
        
    UNIQUE (ParticipantId, EventId)
); 
GO

/* ===================================================== 
7. RESULT TABLE
===================================================== */
---the results table is now added and the attributes are also included---

   

CREATE TABLE RESULT
(

ResultId INT IDENTITY(1,1) PRIMARY KEY,
RegistrationId INT NOT NULL,
FinishTime TIME NOT NULL,
Position INT NOT NULL,

     FOREIGN KEY (RegistrationId) 
          REFERENCES REGISTRATION(RegistrationId)
);
GO


/* ===================================================== 
SAMPLE DATA
===================================================== */

/* Event organiser */
INSERT INTO EVENT_ORGANISER 
(
FirstName,
LastName,
Email,
Password,
Phone
)
VALUES
(
'Sarah',
'Mokoena',
'sarah@email.com',
'Password123',
'0821234567' 
);
GO

/* Participant */
INSERT INTO PARTICIPANT
(
FirstName,
LastName,
Email,
Password,
DateOfBirth,
Phone
)
VALUES
(
'John',
'Smith',
'john@email.com',
'Password123', '1998-05-14',
'0837654321'
);
GO

/* Event */
INSERT INTO EVENT
(
OrganiserId,
EventName,
Description,
EventDate,
Location,
Distance,
EventType,
Status 
)
VALUES 
(
1,
'Cape Town City Run',
'A running event for all participants.',
'2026-10-18',
'Cape Town Stadium',
10.00,
'Run',
'Available'
);
GO

/* Category */
INSERT INTO CATEGORY
(
EventId,
CategoryName,
MinimumAge,
MaximumAge,
Distance
)
VALUES
(
1,
'Senior',
18,
39,
10.00
);
GO

/* Weather */
INSERT INTO WEATHER
(
EventId,
Temperature,
WeatherCondition,
WindSpeed,
WeatherDate
)
VALUES
(
1,
22.50,
'Sunny',
10.00,
'2026-10-18'
);
GO

/* Registration */
INSERT INTO REGISTRATION
(
ParticipantId,
EventId,
CategoryId,
Status
)
VALUES 
(
1,
1,
1,
'Confirmed'
);
GO

/* Result */
INSERT INTO RESULT
(
RegistrationId,
FinishTime,
Position 
)
VALUES
(
1,
'01:42:35',
1 
);
GO

/* ===================================================== 
TEST QUERIES
===================================================== */
--- this is to give out the test queries----

SELECT * FROM EVENT_ORGANISER;
SELECT * FROM PARTICIPANT;
SELECT * FROM EVENT;
SELECT * FROM CATEGORY;
SELECT * FROM WEATHER;
SELECT * FROM REGISTRATION;
SELECT * FROM RESULT;
GO









































































































































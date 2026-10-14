# PROG2B-PART-1
RaceDay System – Part 1

Project Overview

RaceDay is a web-based event management system designed to manage running, walking, and cycling events. The system allows event organisers to manage events and related information, while participants can register for events and view their event information.

This repository contains the Part 1 – System Planning and Database work for the Programming 2B (PROG6212) Portfolio of Evidence.

Part 1 Objectives

Part 1 focuses on planning the RaceDay system before the API is developed. It includes:

Designing the Entity Relationship Diagram (ERD).

Identifying entities, attributes, primary keys and foreign keys.

Defining database relationships and cardinality.

Planning the RESTful API endpoints.

Defining HTTP methods, routes, roles, request bodies and expected responses.

Creating the SQL Server database script.

Adding keys, relationships and database constraints.

Adding realistic sample data.

Maintaining the project using GitHub.

Using GitHub Actions to validate the required Part 1 repository structure.

User Roles

Organiser

The Organiser manages the RaceDay events and event-related information.

Main responsibilities include:

Creating events.

Updating events.

Deleting events.

Managing event categories.

Viewing event registrations.

Managing participant results.

Participant

The Participant takes part in RaceDay events.

Main responsibilities include:

Registering for an account.

Browsing available events.

Registering/enrolling for events.

Viewing their registrations.

Viewing their results.

Managing their profile information.

Main RaceDay Resources

The Part 1 design is organised around these main resources:

Event Organiser

Category

Participant

Weather

Event

Registration

Result

Database

The planned database is:

RaceDayDB

Main tables:

Table

Purpose

EVENT_ORGANISER

Stores event organiser information

PARTICIPANT

Stores participant information

EVENT

Stores RaceDay event information

CATEGORY

Stores event categories

WEATHER

Stores weather information

REGISTRATION

Stores participant event registrations

RESULT

Stores participant results

Main Relationships

EVENT_ORGANISER 1 ────────< EVENT

EVENT 1 ──────────────────< CATEGORY

EVENT 1 ──────────────────< WEATHER

PARTICIPANT 1 ────────────< REGISTRATION

EVENT 1 ──────────────────< REGISTRATION

CATEGORY 1 ───────────────< REGISTRATION

REGISTRATION 1 ─────────── 0..1 RESULT

The ERD and SQL script must remain consistent with one another.

API Endpoint Plan

The API endpoint plan documents the RESTful routes planned for the RaceDay system.

The six main resource groups are:

/api/organisers
/api/categories
/api/participants
/api/weather
/api/events
/api/registrations

The plan also accounts for result functionality and the required authentication/profile functionality.

Each endpoint in the plan records:

HTTP Method

Route

Description

Role Required

Request Body

Expected Response

The endpoint plan is stored in:

/docs/API-Endpoint-Plan.md

Part 1 Repository Structure

RaceDay/
│
├── docs/
│   ├── ERD.pdf
│   ├── API-Endpoint-Plan.md
│   └── database.sql
│
├── .github/
│   └── workflows/
│       └── part1-validation.yml
│
└── README.md


SQL Database Script

The SQL script is designed for SQL Server / SQL Server Management Studio (SSMS).

It

Create the RaceDay database.

Create all tables represented in the ERD.

Define primary keys.

Define foreign keys.

Apply required constraints.

Insert realistic sample data.

Run successfully on a clean SQL Server instance.

The script is stored in:

/docs/database.sql

GitHub and CI/CD

GitHub is used for version control and to store all Part 1 documentation.

A GitHub Actions workflow is included to validate that the required Part 1 files and folders are present.

The workflow checks for:

/docs

ERD file

API Endpoint Plan

SQL database script

README.md

CI/CD Status

![CI Status](docs/ci-status.png)
<img width="1914" height="799" alt="image" src="https://github.com/user-attachments/assets/586f4495-747d-4020-b716-bea817009f95" />



YouTube video link 

added to this README.

Part 1 Video

The Part 1 video demonstrate and explain:

The planning documents.

ERD design decisions.

Database relationships.

API endpoint plan and endpoint choices.

The SQL database script.

Running the SQL script in SSMS.

YouTube link:
https://youtu.be/3arvoiBoRyE



Part 1
│
├── ERD
│     └── Database design
│
├── API Endpoint Plan
│     └── API specification
│
└── SQL Script
      └── SQL Server database
    
Technologies

SQL Server

SQL Server Management Studio (SSMS)

GitHub

GitHub Actions


Author is Mmasechab Kgolane Nkadimeng



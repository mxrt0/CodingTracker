# Coding Tracker

Console-based CRUD app to track coding hours. Developed with C# and SQLite.

## Given requirements

* When the app starts, it should create a SQLite DB if one is not present and insert a table for logging coding sessions.

* User needs to be able to insert, delete, update and view coding sessions.

* All exceptions and errors should be handled so as to never crash the application.

* The application should only be terminated when the user enters 0.

* DB interaction through Dapper ORM

* Reporting Capabilities

## Features

* SQLite DB connection to store and read coding sessions.

* If no database exists, or the correct table does not exist, they will be created on program start.

* Console-based UI using Spectre.Console navigable with key presses.

* CRUD DB functionality

  * From the Main Menu users can Create Sessions, View Sessions, Update Session Details and Delete Sessions by ID.
  * Time and Dates inputted are checked to make sure they are in the correct and realistic format.

* JSON Coding Goal Storage
  * From the Main Menu users can Set Goals, View Goal Progress, Contribute toward a Goal and Update Goals by ID.
  * Time and Dates inputted are checked to make sure they are in the correct and realistic format.
  * Users are presented with necessary coding timto reach goal and average coding time to reach goal in time.
  * Goals are stored in a goals.json file.

## Challenges

* Extracting methods to shorten code and avoid confusion and repetition (DRY principle).
* Managing to deal with all of the functionality and checking if it works as expected.
* Calculating and correctly implementing all timespan-related functions.

## Lessons Learned
* Following DRY Principle is of paramount importance.
* Testing every function upon initial implementation

## Areas To Improve
* Navigating the UI when I have such a high number of methods.
* Extracting methods efficiently without breaking flow.

## Resources
* CSharpAcademy YT video on the topic(only for base DB CRUD)
* Dapper Tutorial
* TimeSpan docs to help with conversions and calculations

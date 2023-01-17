# University.Server
Provides backend functionality for the University Administration App

## Timetable Generation Algorithm

Requires:
	- Location: Is required to ensure that a given location is not used by multiple modules at the same time
	- Module: Is required to know the required time and available professors of a module
	- Semester: Is required to know the time window and all available modules
	- User: Is required to ensure that the user is not expected at multiple modules at the same time
	- Course: Is required to determine the required modules of a group of users

Steps:
	1. Create a table with all available modules of the semester
	2. Mark modules that cant be simultainously because they are both required modules of a course
		- Modules x Modules Matrix, where each module will have a boolean of true and false for each module
		|		   | Module A | Module B | Module C |
		| Module A |		  |			 |			|
		| Module B |		  |			 |			|
		| Module C |		  |			 |			|
	3. Mark which professor can work in which module
		- Professors x Modules Matrix, where each professor will have a boolean of true or false for each module
		|			  | Module A | Module B | Module C |
		| Professor A |			 |			|		   |
		| Professor B |			 |			|		   |
	4. Mark which professor can work in which time window
		- Professors x Time Window Matrix, where each professor will have a boolean of true or false for each time window
		|			  | 8:00 - 11:30 | 13:00 - 16:00 | 17:00 - 19:30 |
		| Professor A |				 |				 |				 |
		| Professor B |				 |				 |				 |
	5. Allocate the professors to the modules, starting with the module with the least available professors
		- Always preferably use the professors with the least possible modules
	6. Count expected required seats amount for module
	7. Mark which locations a module can use based on requirements of module
		- Module x Location Matrix, where each module will have a boolean of true or false for each location
		|		   | Location A | Location B | Location C | Location D |
		| Module A |			|			 |			  |			   |
		| Module B |			|			 |			  |			   |
		| Module C |			|			 |			  |			   |
	8. Combine all previously created matrix to calculate time table

## Routes

Base Route: '{BASE_URL}/api/v1'

### Location

Route: '{BASE_ROUTE}/locations'
Endpoints:
	- Location Create
	- Location Delete
	- Location Update
	- Location Retrieve
		- All
		- By Id

### Module

Route: '{BASE_ROUTE}/modules'
Endpoints:
	- Module Create
	- Module Delete
	- Module Update
		- PUT for complete update
		- PATCH to add/remove Professors
	- Module Retrieve
		- All
		- By Id
		- By Filter

### Semester

Route: '{BASE_ROUTE}/semesters'
Endpoints:
	- Semester Create
	- Semester Delete
	- Semester Update
	- Semester Retrieve
		- All
		- By Id
		- By Filter

### User

Route '{BASE_ROUTE}/users'
Endpoints:
	- User Create
	- User Delete
	- User Update
	- User Retrieve
		- All
		- By Id
		- By Filter

### Course

Route '{BASE_ROUTE}/courses'
Endpoints:
	- Course Create
	- Course Delete
	- Course Update
	- Course Retrieve
		- All
		- By Id
		- By Filter
# University

## Setup
1. Open in Visual Studio
2. Install dependencies
3. Add secrets.json

```json
{
  "Jwt": {
    "Key": "Secret Key",
    "Issuer": "The Server",
    "Audience": "The Client"
  },
  "ConnectionString": "Your Azure Cosmos DB Connection String"
}
```

## Internal
How the data is viewed in the system.
### Entities
| Entity | Description |
| --- | --- |
| `Course` | Stores Users and assigns Compulsory Modules to these. |
| `Location` | Defined with Coordinates and the amount of people it can hold. |
| `Module` | General Module that can be assigned to users or added to a semester. |
| `Semester` | A semester with the modules available in that semester. |
| `User` | Account for all types of users. Including students, professors and administrators. |

### Class Diagram
![Class Diagram](Diagram.png)

## External
How the data is exposed to the user.

### Resources
| Resource | Description |
| --- | --- |
| `CourseResource` | Displays informations of a course, including students and modules |
| `LocationResource` | Displays informations of a location, including coordinates. |
| `ModuleResource` | Displays informations of a module, including professors. |
| `SemesterResource` | Displays information of a semester, including all modules and lectures. |
| `UserResource` | Displays informations of a user, including authorization and assignments. |
| `ExtendedLectureResource` | Displays informations of a lecture, including module name and professor. |

### Class Diagram
![Class Diagram](Resource_Diagram.png)
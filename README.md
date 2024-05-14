# TamagotchiAPI

An API that allows a user to create and care for a virtual pet, akin to a Tamagotchi Virtual Pet.

**Link to front-end:** https://github.com/jbadge/TamagotchiFrontEnd

**Link to static implementation of project:** https://tamagotchi-reloaded-jb.netlify.app/

## How It's Made:

**Tech used:** ASP.NET CORE, C#, Entity Framework, PostgreSQL

This RESTful backend is built using C#. Entity Framework is used to run SQL queries to a PostgreSQL database.

## Optimizations

I added a PUT method in order to make on-the-fly changes to pets for diagnostic purposes. I also have (currently commented out) code in order to run on external devices within a LAN, both in the frontend and backend code. Not only did this help with testing, as I did not deploy the backend codebase, but also allowed my 7 year old to have her own database, resulting in a happy 7 year old.

## Lessons Learned:

How to implement a full-stack application by building a backend API. After all was said and done, I was fascinated by the simplicity of the HTTP requests. Seeing it all come together after the frontend was built, with the SQL queries to the database resulting in a fun, interactive pet database, was very satisfying.

## Endpoints

Please note, the API uses PUT instead of PATCH for updating.

### Request methods

| Method   | Description                                                                     |
| -------- | ------------------------------------------------------------------------------- |
| `GET`    | Used to retrieve a single pet or all pets in the database.                      |
| `POST`   | Used when creating a new pet, or pet action: playtimes, feedings, or scoldings. |
| `PUT`    | Used to update pet (replaces all fields with new data), e.g. when pet dies      |
| `DELETE` | Used to delete a pet by id.                                                     |

### Examples

During pet creation, Birthday defaults to the current DateTime, Hunger Level defaults to 0 and Happiness Level defaults to 0.

| Method   | URL                     | Description                                                                          |
| -------- | ----------------------- | ------------------------------------------------------------------------------------ |
| `GET`    | `/api/Pets `            | Retrieve all posts.                                                                  |
| `POST`   | `/api/Pets`             | Create a new pet.                                                                    |
| `GET`    | `/api/Pets/5`           | Retrieve pet #id                                                                     |
| `PUT`    | `/api/Pets/5`           | Update pet #5 to dead                                                                |
| `DELETE` | `/api/Pets/5`           | Delete pet #5.                                                                       |
| `POST`   | `/api/Pets/5/Playtimes` | Add a playtime to pet #5, adding 5 to Happiness and 3 to Hunger levels.              |
| `POST`   | `/api/Pets/5/Feedings`  | Add a feeding to pet #5, subtracting 5 from Hunger and adding 3 to Happiness levels. |
| `POST`   | `/api/Pets/5/Scoldings` | Add a scolding to post #5, subtracting 5 from Happiness level.                       |

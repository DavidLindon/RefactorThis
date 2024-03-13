Issues that I observed
1. Handling the data access within the Product and ProductOption classes is a violation of SOLID principles
2. Not using parameterised SQL querys leaves the project vulnerable to SQL injection
3. The SQL connection is not wrapped in a using statement which would be best practice, the connections are not disposed at all
4. No data transfer classes
5. No unit tests!
6. When deleting a product, the foreach loop executes Delete on each of the product's options; each Delete method on the option opens a new database connection and executes an SQL statement to delete that specific option id, rather than deleting all where the productID = xx. I consider this a code smell.
7. The NewConnection method in the Helper class returns a connection without calling  Con.Open(), which means this snippet is then called many many time through the code.
8. Normally I would use an int for the primary key of id and let the db auto increment. I decided to stick with guids as this could be considered a breaking change.

Approach
The original solution was .net framework, I took the liberty to re-create as a .net core solution.
I decided to use Entity Framework to abstract the database. I then tested this by using the In Memory database driver (I wasn't able to open the .mdf file for some reason)
I have create a data transfer object for the Product class, returning the class in it's entirity could lead to returning more properties than required and worse still, security concerns.
As I'm unaware what properties the front end needs, I've simply included all properties from the Product class.

Logging & Error Handling
In Program.cs I have registered an error handler 'app.ConfigureExceptionHandler(app.Logger)' which will inspect all exceptions and when the client is requesting a JSON response,
it will return a JSON formatted error message. Additionally, I'm injecting an ILogger instance to log all exceptions.

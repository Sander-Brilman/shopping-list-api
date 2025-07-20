# shopping-list-api

The backend api for my shopping list application.

For historical reasons this also contains a web UI written in Blazor, allthough all active development on the UI has shifted to the [newer svelte UI](https://github.com/Sander-Brilman/shopping-list-ui-svelte)


## how to run

1. pull the repo
2. make sure you have the dotnet 9 SDK installed
3. navigate to the api project folder
4. Add your sql server connection string to either the *user secrets* using your IDE or alternatively the `appsettings.Development.json` file:
```json
{
    ...
    "ConnectionStrings": {
        "ShoppingList": "YOUR_SQL_SERVER_DATABASE_CONNECTION_STRING"
    }
}
```
5. run the `dotnet ef database update` command to create the tables within the database 
6. run the `dotnet run` command to start the api
7. navigate to `http://localhost:5023/swagger/index.html` to start using the api and viewing the possible endpoints

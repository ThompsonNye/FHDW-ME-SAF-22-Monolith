# Vehicle Gas Consumption Server

The backend for
the [Vehicle Gas Consumption App V2](https://git.nuyken.dev/thomas.nuyken/vehicle-gas-consumption-app-v2).

## Configuration options

| Option                            | Type   | Description                                                                                                                                                                                                                                                                                         | Required                                            | Default value                                |
|-----------------------------------|--------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|----------------------------------------------|
| CONNECTION_STRING                 | string | The connection string to the database to use.                                                                                                                                                                                                                                                       | If DATABASE_TYPE is not null, yes                   | null                                         |
| DATABASE__AUTOMIGRATE             | bool   | Whether or not to automatically apply the available EF migrations on startup.                                                                                                                                                                                                                       | No                                                  | false                                        |
| DATABASE__TYPE                    | string | What database type is used. Allowed values: null, mysql, mariadb. If null, an in-memory database is used. In case of mysql or mariadb, an appropriate database has to be set up (e.g. via another docker container in the docker-compose) and the connection string provided via CONNECTION_STRING. | No                                                  | null                                         |
| SWAGGER__DESCRIPTION              | string | The description in the swagger document.                                                                                                                                                                                                                                                            | No                                                  | Backend for the Vehicle Gas Consumption App. |
| SWAGGER__TITLE                    | string | The title of the swagger document.                                                                                                                                                                                                                                                                  | No                                                  | Vehicle Cas Consumption                      |
| SWAGGER__PUBLISHSWAGGERUI         | bool   | Whether to publish the Swagger UI besides the swagger.json file                                                                                                                                                                                                                                     | No                                                  | false                                        |

___Note the double underscore as separator___ _(not in CONNECTION_STRING)____!___

## Logging

## Migrations

### Working with migrations

- Add-Migration
- Remove-Migration
- Update-Database
- --> link to migrations in Microsoft docs

### Add new migrations

When adding a new migration, the connection string and the db type have to be set, e.g. ___temporarily___ set them
in `appsetting.json`:

```json
{
    "ConnectionStrings": {
        "Default": "<connection string>"
    },
    "Database": {
        "Type": "<db type, e.g. 'mariadb'"
    }
}
```

### Auto apply on startup

If the configuration option `DATABASE__AUTOMIGRATE` is set to `true`, the available migrations are applied to the
database during startup.

## Docker

```yaml
version: '3.8'

networks:
    vegasco:

volumes:
    vegasco-data:
    vegasco-secure-data:

services: 
    vegasco-db:
        container_name: vegasco-db
        image: mariadb:10.5.11-focal
        environment:
            MYSQL_DATABASE: vegasco
            MYSQL_USER: vegasco
            MYSQL_PASSWORD: <password>
            MYSQL_ROOT_PASSWORD: <password>
        networks:
            - vegasco
        ports:
            - 127.0.0.1:3306:3306
        volumes:
            - vegasco-data:/var/lib/mysql
        healthcheck:
            test: ["CMD", "mysqladmin" ,"ping", "-h", "localhost"]
            interval: 5s
    
    vegasco-app:
        container_name: vegasco-app
        image: vegasco-server
        environment: 
            CONNECTION_STRING: <connection string>
            DATABASE__AUTOMIGRATE: "true"
            DATABASE__TYPE: mariadb
            SWAGGER__TITLE: "Fancy title"
            SWAGGER__DESCRIPTION: "An even fancier description"
        volumes:
            - vegasco-secure-data:/root/.aspnet/DataProtection-Keys
        networks:
            - vegasco
        ports: 
            - 127.0.0.1:8080:80
        depends_on:
            vegasco-db:
                condition: service_healthy
```
# FHDW-ME-SAF-22-Monolith

Monolithische Anwendung zur Studienarbeit in SAF im Sommersemester '22 an der FHDW Mettmann.

## Vehicle Gas Consumption Server (Vegasco)

### Configuration options

| Option                            | Type   | Description                                                                                                                                                                                                                                                                                                                              | Required                                            | Default value                                |
|-----------------------------------|--------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|----------------------------------------------|
| CONNECTION_STRING                 | string | The connection string to the database to use.                                                                                                                                                                                                                                                                                            | If DATABASE_TYPE is not null, yes                   | null                                         |
| DATABASE__AUTOMIGRATE             | bool   | Whether or not to automatically apply the available EF migrations on startup.                                                                                                                                                                                                                                                            | No                                                  | false                                        |
| DATABASE__TYPE                    | string | What database type is used. Allowed values: null, mysql, mariadb, postgres, postgresql. If null, an in-memory database is used. In case of one of the other allowed values, an appropriate database has to be set up (e.g. via another docker container in the docker-compose) and the connection string provided via CONNECTION_STRING. | No                                                  | null                                         |
| SWAGGER__DESCRIPTION              | string | The description in the swagger document.                                                                                                                                                                                                                                                                                                 | No                                                  | Backend for the Vehicle Gas Consumption App. |
| SWAGGER__TITLE                    | string | The title of the swagger document.                                                                                                                                                                                                                                                                                                       | No                                                  | Vehicle Cas Consumption                      |
| SWAGGER__PUBLISHSWAGGERUI         | bool   | Whether to publish the Swagger UI besides the swagger.json file                                                                                                                                                                                                                                                                          | No                                                  | false                                        |

___Note the double underscore as separator___ _(not in CONNECTION_STRING)____!___

### Logging

### Migrations

#### Working with migrations

- Add-Migration
- Remove-Migration
- Update-Database
- --> link to migrations in Microsoft docs

#### Add new migrations

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

#### Auto apply on startup

If the configuration option `DATABASE__AUTOMIGRATE` is set to `true`, the available migrations are applied to the
database during startup.

### Docker

With MariaDB:

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
    image: mariadb
    environment:
      MYSQL_DATABASE: vegasco
      MYSQL_USER: vegasco
      MYSQL_PASSWORD: <password>
      MYSQL_ROOT_PASSWORD: <password>
    networks:
      - vegasco
    ports:
      - "127.0.0.1:3306:3306"
    volumes:
      - vegasco-data:/var/lib/mysql
    healthcheck:
      test: [ "CMD", "mysqladmin" ,"ping", "-h", "localhost" ]
      interval: 5s

  vegasco-app:
    container_name: vegasco-app
    image: vegasco-monolith:test
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
      - "127.0.0.1:8080:80"
    depends_on:
      vegasco-db:
        condition: service_healthy
```

With PostgreSQL:

```yaml
version: '3.8'

services:
  app:
    container_name: vegasco-app-dev
    image: vegasco-monolith:test
    restart: unless-stopped
    environment:
      CONNECTION_STRING: "Host=db;Port=5432;Database=postgres;Username=postgres;Password=password"
      DATABASE__AUTOMIGRATE: "true"
      DATABASE__TYPE: "postgres"
      SWAGGER__PUBLISHSWAGGERUI: "true"
    ports:
      - "127.0.0.1:8080:80"
    depends_on:
      - db

  db:
    container_name: vegasco-db-dev
    image: postgres
    restart: unless-stopped
    environment:
      POSTGRES_PASSWORD: password
    ports:
      - "127.0.0.1:5432:5432"
```

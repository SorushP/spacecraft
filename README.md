Use these commands to run the project in dev mode (port `5159`, Swagger is enabled):
```sh
# cd <PROJECT_ROOT>

# When it stop, the DBMS container will be removed.
docker run --rm -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=38GR3r8073ho40HR84h" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Wait about 10 seconds...

dotnet run --project Weather --environment Development
```

Use this command to run the project in production mode (port `8080`, Swagger is not enabled):
```sh
# cd <PROJECT_ROOT>

docker-compose up
```

Access to the API:
```sh
curl -vv http://localhost:<PORT>/Weather
```
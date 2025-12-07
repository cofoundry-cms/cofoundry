# Local Development Environment

Samples are configured to use local databases provisioned via a single [docker compose file](docker-compose.yml). The first time the compose file is run a backup of each sample database is restored to the SqlServer instance running in docker. Each sample project is already configured to point to the correct database.

## Getting Started

1. First, ensure you have docker and docker compose installed e.g. via [Docker Desktop](https://www.docker.com/products/docker-desktop/), [Rancher Desktop](https://rancherdesktop.io/) or similar.
2. Next, open a terminal in the local-dev folder containing the docker-compose.yml file and run it with `docker compose up`
3. On your first run it may take a while to download the SQLServer image and run the backups. When it's finished you will see the message `Db initialization complete`. Subsequent runs should take less than 10s.
4. To stop the containers running press `CTRL+C`, or use `docker compose stop` if running in detached mode via `docker compose up -d`.

Databases will persist as long as the container has not been removed. To reset the databases you can use `docker compose down` to remove the container images. To rebuild containers after a change use `docker compose --build`.

For more information see the [official docker compose docs](https://docs.docker.com/compose/gettingstarted/).

## Using your own SqlServer Instance

If you prefer to use your own local SQLServer instance you can manually restore the bacpac files in the [db/backups](db/backups) directory. You will also need to configure the connection strings in the sample proejcts to point to your SQLServer instance.


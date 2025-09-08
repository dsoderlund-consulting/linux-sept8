# Run postgres, mount its data in .pg/data
docker run \
    --name postgres \
    -e POSTGRES_USER=username \
    -e POSTGRES_PASSWORD=password \
    -e POSTGRES_DB=postgres \
    -e PGDATA=/var/lib/postgresql/data/pgdata \
    -p 5432:5432 \
    -v ./.pg/data:/var/lib/postgresql/data:z \
    postgres


# stop and remove postgres container
docker stop postgres
docker rm postgres
# This is linux-sept8

This repository was created with the template 
`demo-fullstack-template`.

To get started with development, remove or replace `backend/ShoppingListApi`. Recreate the required database migrations: `dotnet ef migrations add InitialCreate`. Remove or replace shoppinglistapi from the following files: 
 - /frontend/src/App.tsx
 - /frontend/src/api/shoppingListApi.ts
 - /frontend/src/components/ShoppingList.tsx

> THE OWNER OF THIS REPOSITORY SHOULD REPLACE THIS README WITH THE README OF YOUR PROJECT, AND REMOVE THE EXAMPLE ONCE THEY HAVE HAD A CHANCE TO SEE HOW IT ALL FITS TOGETHER.

## About

frontend is vite with typescript running on bun, with a dockerfile build and tilt support.

backend is dotnet 8.0 with mvc with a dockerfile and postgres support.

It deploys to kubernetes outside of development using the `crossplane.dsoderlund.consulting/FullstackApp` abstraction.



## To test out locally

First configure backend to use a postgres database, it is preconfigured with a connection string that would match the docker/podman postgres cluster created with `backend/pgup.sh`

Install the dependencies, set environment variables, and run.

### backend

in /backend run

``` sh
dotnet restore
dotnet build
dotnet run
```

### frontend

``` sh
bun install
echo "VITE_API_BASE_URL=http://localhost:5159" > .env
bun run dev
```

## To test out with kubernetes

`ctlptl apply -f kind-cluster.yaml && tilt up`

Press space when promted by tilt to see the deployment in your browser, and when viewing the resource groups for frontend/backend there should be links to reach them.

Frontend: http://localhost:8081/

Backend api docs: http://localhost:8080/swagger/

## Deployment

By default the application once the images are pushed into the registry is deployed with a composite resource defintion:

``` yaml
apiVersion: crossplane.dsoderlund.consulting/v1
kind: FullstackApp
metadata:
  name: linux-sept8
  namespace: default
spec:
  backend:
    image: docker.io/dsoderlund/linux-sept8-backend:0.0.1
    replicas: 1
    containerPort: 8080
    databaseConnectionValues: true
    extraEnvironmentVariables:
      - name: ASPNETCORE_ENVIRONMENT
        value: Production
  frontend:
    image: docker.io/dsoderlund/linux-sept8-frontend:0.0.1
    replicas: 1
    containerPort: 8081
  ingress:
    hostname: linux-sept8.sam.dsoderlund.consulting
  database:
    replicas: 1
```
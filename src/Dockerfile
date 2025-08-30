# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

COPY --link ./ /source

WORKDIR /source/ShoppingListApi

# Copy source code and publish app
RUN dotnet restore
RUN dotnet publish -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS shopping-list-api
EXPOSE 8080
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./ShoppingListApi"]
